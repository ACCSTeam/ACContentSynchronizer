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

    public event ProgressEvent? OnDownload;
    public event CompleteEvent? OnComplete;

    public async Task<Manifest?> DownloadManifest() {
      var json = await _client.GetStringAsync("getManifest");
      return JsonSerializer.Deserialize<Manifest>(json, ContentUtils.JsonSerializerOptions);
    }

    public Manifest CompareContent(string gamePath, Manifest manifest) {
      return ContentUtils.CompareContent(gamePath, manifest);
    }

    public async Task<string> PrepareContent(Manifest manifest) {
      var result = await _client.PostAsJsonAsync("prepareContent", manifest);
      var session = await result.Content.ReadAsStringAsync();
      return session;
    }

    public void DownloadContent(string session) {
      var server = $"{_client.BaseAddress}downloadContent?session={session}";
      using var client = new WebClient();

      OnDownload?.Invoke(-1);

      client.DownloadProgressChanged += (_, e) => OnDownload?.Invoke(e.ProgressPercentage);
      client.DownloadFileCompleted += (_, _) => OnComplete?.Invoke();
      client.DownloadFileAsync(new(server), Constants.ContentArchive);
    }

    public Task RemoveSession(string session) {
      return _client.GetAsync($"removeSession?session={session}");
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
      content.Pack(Constants.Client);
      var contentArchive = Path.Combine(Constants.Client, Constants.ContentArchive);
      await using var stream = File.OpenRead(contentArchive);
      await _client.SendAsync(new(HttpMethod.Post, $"updateContent?adminPassword={adminPassword}") {
        Content = new StreamContent(stream),
      });
    }

    public async Task RefreshServer(Manifest manifest) {
    }
  }
}
