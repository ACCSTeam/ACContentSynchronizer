using System;
using System.Collections.Generic;
using System.IO;
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
    public delegate void CompleteEvent();

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
    public event CompleteEvent OnComplete;

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

    public async Task<string> PrepareContent(List<string> updatableEntries) {
      var result = await _client.PostAsync("prepareContent", JsonContent.Create(updatableEntries));
      var session = await result.Content.ReadAsStringAsync();
      return session;
    }

    public void DownloadContent(string session) {
      var server = $"{_client.BaseAddress}downloadContent?session={session}";
      using var client = new WebClient();

      OnDownload?.Invoke(-1);

      client.DownloadProgressChanged += (_, e) => OnDownload?.Invoke(e.ProgressPercentage);
      client.DownloadFileCompleted += (_, _) => OnComplete?.Invoke();
      client.DownloadFileAsync(new Uri(server), Constants.ContentArchive);
    }

    public Task RemoveSession(string session) {
      return _client.GetAsync($"removeSession?session={session}");
    }

    public void SaveData() {
      ContentUtils.UnpackContent();
    }

    public void Apply(string gamePath) {
      ContentUtils.ApplyContent(gamePath);
    }

    public async Task SetContent(string adminPassword, string gamePath, List<string> updateEntries) {
      var content = ContentUtils.PrepareContent(gamePath, updateEntries);
      content.Pack("client");
      await _client.PostAsync($"setContent?adminPassword={adminPassword}",
        new ByteArrayContent(await File.ReadAllBytesAsync(Constants.ContentArchive)));
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
