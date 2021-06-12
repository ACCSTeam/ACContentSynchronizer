using System;
using System.IO;
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

    public delegate void PackProgressEvent(double progress, string entry);

    public delegate void ProgressEvent(double progress);

    private readonly HttpClient _client;

    public DataReceiver(string serverAddress) {
      if (string.IsNullOrEmpty(serverAddress)) {
        throw new NullReferenceException(nameof(serverAddress));
      }

      _client = new() {
        BaseAddress = new(serverAddress),
        Timeout = Timeout.InfiniteTimeSpan,
      };
    }

    public event PackProgressEvent? OnPack;
    public event ProgressEvent? OnProgress;
    public event CompleteEvent? OnComplete;

    public async Task<Manifest?> DownloadManifest() {
      var json = await _client.GetStringAsync("getManifest");
      return JsonSerializer.Deserialize<Manifest>(json, ContentUtils.JsonSerializerOptions);
    }

    public Manifest CompareContent(string gamePath, Manifest manifest) {
      return ContentUtils.CompareContent(gamePath, manifest);
    }

    public async Task<string> PrepareContent(Manifest manifest, string clientId) {
      var result = await _client.PostAsJsonAsync($"prepareContent?client={clientId}", manifest);
      var session = await result.Content.ReadAsStringAsync();
      return session;
    }

    public void DownloadContent(string session) {
      var archive = Path.Combine(Constants.Client, Constants.ContentArchive);
      var server = $"{_client.BaseAddress}downloadContent?session={session}";
      using var client = new WebClient();
      FileUtils.DeleteIfExists(archive);
      DirectoryUtils.CreateIfNotExists(Constants.Client);

      client.DownloadProgressChanged += (_, e) => OnProgress?.Invoke(e.ProgressPercentage);
      client.DownloadFileCompleted += (_, _) => OnComplete?.Invoke();
      // 81920
      client.DownloadFileAsync(new(server), archive);
    }

    public void SaveData() {
      ContentUtils.UnpackContent(Constants.Client);
    }

    public void Apply(string gamePath) {
      ContentUtils.ApplyContent(gamePath, Constants.Client);
    }

    public async Task<Manifest?> GetUpdateManifest(Manifest manifest) {
      var response = await _client.PostAsJsonAsync("getUpdateManifest", manifest);
      var json = await response.Content.ReadAsStringAsync();
      return JsonSerializer.Deserialize<Manifest>(json, ContentUtils.JsonSerializerOptions);
    }

    public async Task UpdateContent(string adminPassword, string gamePath, Manifest comparedManifest) {
      var content = ContentUtils.PrepareContent(gamePath, comparedManifest);
      content.OnProgress += OnPackHandler;
      await content.Pack(Constants.Client);
      var contentArchive = Path.Combine(Constants.Client, Constants.ContentArchive);

      // 81920
      await using var stream = File.OpenRead(contentArchive);
      await _client.PostAsync($"updateContent?adminPassword={adminPassword}", new StreamContent(stream));

      content.OnProgress -= OnPackHandler;
    }

    private void OnPackHandler(double progress, string entry) {
      OnPack?.Invoke(progress, entry);
    }

    public async Task RefreshServer(string adminPassword, Manifest manifest) {
      await _client.PostAsJsonAsync($"refreshServer?adminPassword={adminPassword}", manifest);
    }
  }
}
