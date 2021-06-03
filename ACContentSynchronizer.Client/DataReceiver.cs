using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.Models;

namespace ACContentSynchronizer.Client {
  public class DataReceiver {
    public delegate void ProgressEvent(double progress);

    private readonly HttpClient _client;
    private Manifest _manifest = new();

    public DataReceiver(string serverAddress) {
      if (string.IsNullOrEmpty(serverAddress)) {
        throw new NullReferenceException(nameof(serverAddress));
      }

      _client = new HttpClient {
        BaseAddress = new Uri(serverAddress),
        Timeout = Timeout.InfiniteTimeSpan,
      };
    }

    public event ProgressEvent OnDownload;

    public async Task DownloadManifest() {
      var json = await _client.GetStringAsync("getManifest");
      _manifest = JsonSerializer.Deserialize<Manifest>(json, ContentUtils.JsonSerializerOptions);
    }

    public List<string> CompareContent(string gamePath) {
      FileUtils.DeleteIfExists(Constants.ContentArchive);

      var content = ContentUtils.GetContentHierarchy(gamePath);

      var cars = _manifest.Cars.Where(entry => EntryNeedUpdate(entry, content.CarsPath));
      var tracks = _manifest.Tracks.Where(entry => EntryNeedUpdate(entry, content.TracksPath));
      var updatableEntries = cars.Union(tracks).Select(entry => entry.Name);

      return updatableEntries.ToList();
    }

    public async Task DownloadData(List<string> updatableEntries) {
      OnDownload?.Invoke(-1);
      var content = JsonContent.Create(updatableEntries);
      var response = await _client.PostAsync("getContent", content);

      await FileUtils.CreateIfNotExists(Constants.ContentArchive);

      await using var responseStream = await response.Content.ReadAsStreamAsync();
      var totalBytes = response.Content.Headers.ContentLength ?? 0;

      var totalBytesRead = 0L;
      var readCount = 0L;
      var buffer = new byte[8192];
      var isMoreToRead = true;
      double progress = -1;

      await using var fileStream = new FileStream(Constants.ContentArchive,
        FileMode.Create,
        FileAccess.Write,
        FileShare.None,
        8192,
        true);

      do {
        var bytesRead = await responseStream.ReadAsync(buffer.AsMemory(0, buffer.Length));
        if (bytesRead == 0) {
          isMoreToRead = false;
          progress = Progress(totalBytesRead, totalBytes, progress);
          continue;
        }

        await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));

        totalBytesRead += bytesRead;
        readCount += 1;

        if (readCount % 100 == 0) {
          progress = Progress(totalBytesRead, totalBytes, progress);
        }
      } while (isMoreToRead);
    }

    private double Progress(long totalBytesRead, long totalBytes, double progress) {
      var nextStep = Math.Round((double) totalBytesRead / totalBytes * 100);

      if (!(nextStep > progress)) {
        return progress;
      }

      progress = nextStep;
      OnDownload?.Invoke(progress);

      return progress;
    }

    public void SaveData() {
      ContentUtils.UnpackContent();
    }

    public void Apply(string gamePath) {
      ContentUtils.ApplyContent(gamePath);
    }

    public async Task SetContent(string adminPassword, string gamePath, List<string> updateEntries) {
      var content = ContentUtils.GetContent(gamePath, updateEntries);
      await _client.PostAsync($"setContent?adminPassword={adminPassword}",
        new ByteArrayContent(await content.Pack()));
    }

    private bool EntryNeedUpdate(EntryManifest entry, string carsPath) {
      var localEntry = Directory.GetDirectories(carsPath)
        .FirstOrDefault(dir => new DirectoryInfo(dir).Name == entry.Name);

      if (string.IsNullOrEmpty(localEntry)) {
        return true;
      }

      var needUpdate = entry.Size != DirectoryUtils.Size(localEntry);
      return needUpdate;
    }
  }
}
