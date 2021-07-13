using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.Models;

namespace ACContentSynchronizer.ClientGui {
  public class DataReceiver : IDisposable {
    public delegate void CompleteEvent();

    public delegate void PackProgressEvent(double progress, string entry);

    public delegate void ProgressEvent(double progress);

    public readonly HttpClient Client;

    public DataReceiver(string serverAddress) {
      if (string.IsNullOrEmpty(serverAddress)) {
        throw new NullReferenceException(nameof(serverAddress));
      }

      Client = new() {
        BaseAddress = new(serverAddress),
        Timeout = Timeout.InfiniteTimeSpan,
      };
    }

    public void Dispose() {
      Client.Dispose();
    }

    public event PackProgressEvent? OnPack;
    public event ProgressEvent? OnProgress;
    public event CompleteEvent? OnComplete;

    public async Task<Manifest?> DownloadManifest() {
      var json = await Client.GetStringAsync("getManifest");
      return JsonSerializer.Deserialize<Manifest>(json, ContentUtils.JsonSerializerOptions);
    }

    public Manifest CompareContent(string gamePath, Manifest manifest) {
      return ContentUtils.CompareContent(gamePath, manifest);
    }

    public async Task<string> PrepareContent(Manifest manifest) {
      var result = await Client.PostAsJsonAsync("prepareContent", manifest);
      var session = await result.Content.ReadAsStringAsync();
      return session;
    }

    public Task PackContent(string session, string clientId) {
      return Client.GetAsync($"packContent?session={session}&client={clientId}");
    }

    public Task CancelPreparing(string session) {
      return Client.GetAsync($"cancelPack?session={session}");
    }

    public void DownloadContent(string session, string clientId) {
      var archive = Path.Combine(session, Constants.ContentArchive);
      var server = $"{Client.BaseAddress}downloadContent?session={session}&client={clientId}";
      using var client = new WebClient();
      FileUtils.DeleteIfExists(archive);
      DirectoryUtils.CreateIfNotExists(session);

      client.DownloadProgressChanged += (_, e) => OnProgress?.Invoke(e.ProgressPercentage);
      client.DownloadFileCompleted += (_, _) => OnComplete?.Invoke();
      client.DownloadFileAsync(new(server), archive);
    }

    public void SaveData(string session) {
      ContentUtils.UnpackContent(session);
    }

    public void Apply(string gamePath, string session) {
      ContentUtils.ApplyContent(gamePath, session);
    }

    public async Task<Manifest?> GetUpdateManifest(Manifest manifest) {
      var response = await Client.PostAsJsonAsync("getUpdateManifest", manifest);
      var json = await response.Content.ReadAsStringAsync();
      return JsonSerializer.Deserialize<Manifest>(json, ContentUtils.JsonSerializerOptions);
    }

    private void OnPackHandler(double progress, string entry) {
      OnPack?.Invoke(progress, entry);
    }

    public async Task RefreshServer(string adminPassword, Manifest manifest) {
      await Client.PostAsJsonAsync($"refreshServer?adminPassword={adminPassword}", manifest);
    }

    public async Task<ServerInfo?> GetServerInfo() {
      var json = await Client.GetStringAsync("getServerInfo");
      return JsonSerializer.Deserialize<ServerInfo>(json, ContentUtils.JsonSerializerOptions);
    }

    public Task<string> GetServerName() {
      return Client.GetStringAsync("getServerName");
    }
  }
}
