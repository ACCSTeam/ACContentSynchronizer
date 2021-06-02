using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.Models;

namespace ACContentSynchronizer.Client {
  public class DataReceiver {
    public delegate void ProgressEvent(string progress);

    private const string DownloadsPath = "downloads";
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
      if (File.Exists(Constants.ContentArchive)) {
        File.Delete(Constants.ContentArchive);
      }

      var content = CheckContentHierarchy(gamePath);

      var cars = _manifest.Cars.Where(entry => EntryNeedUpdate(entry, content.CarsPath));
      var tracks = _manifest.Tracks.Where(entry => EntryNeedUpdate(entry, content.TracksPath));
      var updatableEntries = cars.Union(tracks).Select(entry => entry.Path);

      return updatableEntries.ToList();
    }

    public async Task DownloadData(List<string> updatableEntries) {
      OnDownload?.Invoke("Preparing...");
      var content = JsonContent.Create(updatableEntries);
      var response = await _client.PostAsync("getContent", content);

      if (!File.Exists(Constants.ContentArchive)) {
        await File.Create(Constants.ContentArchive).DisposeAsync();
      }

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
      OnDownload?.Invoke($"{progress}%");

      return progress;
    }

    public void SaveData() {
      if (!File.Exists(Constants.ContentArchive)) {
        return;
      }

      if (Directory.Exists(DownloadsPath)) {
        Directory.Delete(DownloadsPath, true);
      }

      Directory.CreateDirectory(DownloadsPath);
      ZipFile.ExtractToDirectory(Constants.ContentArchive, DownloadsPath);
      File.Delete(Constants.ContentArchive);
    }

    public void Apply(string gamePath) {
      if (!Directory.Exists(gamePath)) {
        return;
      }

      var downloadedCars = Directory.GetDirectories(Path.Combine(DownloadsPath, Constants.CarsFolder));
      var downloadedTracks = Directory.GetDirectories(Path.Combine(DownloadsPath, Constants.TracksFolder));

      var content = CheckContentHierarchy(gamePath);

      foreach (var car in downloadedCars) {
        var carName = new DirectoryInfo(car).Name;

        if (string.IsNullOrEmpty(carName)) {
          continue;
        }

        var contentCarPath = Path.Combine(content.CarsPath, carName);

        DirectoryUtils.DeleteIfExists(contentCarPath, true);
        Directory.Move(car, contentCarPath);
      }

      foreach (var track in downloadedTracks) {
        var trackName = new DirectoryInfo(track).Name;

        if (string.IsNullOrEmpty(trackName)) {
          continue;
        }

        var contentTrackPath = Path.Combine(content.TracksPath, trackName);

        DirectoryUtils.DeleteIfExists(contentTrackPath, true);
        Directory.Move(track, contentTrackPath);
      }

      DirectoryUtils.DeleteIfExists(DownloadsPath, true);
    }

    private (string Path,
      string CarsPath,
      string TracksPath) CheckContentHierarchy(string path) {
      var contentPath = Path.Combine(path, Constants.ContentFolder);
      var contentCarsPath = Path.Combine(contentPath, Constants.CarsFolder);
      var contentTracksPath = Path.Combine(contentPath, Constants.TracksFolder);

      DirectoryUtils.CreateIfNotExists(contentPath);
      DirectoryUtils.CreateIfNotExists(contentCarsPath);
      DirectoryUtils.CreateIfNotExists(contentTracksPath);

      return (contentPath, contentCarsPath, contentTracksPath);
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
