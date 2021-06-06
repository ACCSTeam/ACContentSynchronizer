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

    public DataReceiver(string serverAddress) {
      if (string.IsNullOrEmpty(serverAddress)) {
        throw new NullReferenceException(nameof(serverAddress));
      }

      _client = new HttpClient {
        BaseAddress = new Uri(serverAddress),
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

    public async Task<Manifest?> GetUpdateManifest(Manifest manifest) {
      var response = await _client.PostAsJsonAsync("getUpdateManifest", manifest);
      var json = await response.Content.ReadAsStringAsync();
      return JsonSerializer.Deserialize<Manifest>(json, ContentUtils.JsonSerializerOptions);
    }

    public async Task UpdateContent(string adminPassword, string gamePath, Manifest comparedManifest,UpdateManifest updateManifest) {
      var content = ContentUtils.PrepareContent(gamePath, comparedManifest);
      var session = "client";
      content.Pack(session);
      var contentArchive = Path.Combine(session, Constants.ContentArchive);
      var data = await File.ReadAllBytesAsync(contentArchive);
      updateManifest.Content = data;
      await _client.PostAsJsonAsync($"updateContent?adminPassword={adminPassword}", updateManifest);
    }
  }
}
