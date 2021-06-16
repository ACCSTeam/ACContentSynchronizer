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
  public class DataReceiver : IDisposable{
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

    public async Task<string> PrepareContent(Manifest manifest, string clientId) {
      var result = await Client.PostAsJsonAsync($"prepareContent?client={clientId}", manifest);
      var session = await result.Content.ReadAsStringAsync();
      return session;
    }

    public void DownloadContent(string session, string clientId) {
      var archive = Path.Combine(Constants.Client, Constants.ContentArchive);
      var server = $"{Client.BaseAddress}downloadContent?session={session}&client={clientId}";
      using var client = new WebClient();
      FileUtils.DeleteIfExists(archive);
      DirectoryUtils.CreateIfNotExists(Constants.Client);

      client.DownloadProgressChanged += (_, e) => OnProgress?.Invoke(e.ProgressPercentage);
      client.DownloadFileCompleted += (_, _) => OnComplete?.Invoke();
      client.DownloadFileAsync(new(server), archive);
    }

    public void SaveData() {
      ContentUtils.UnpackContent(Constants.Client);
    }

    public void Apply(string gamePath) {
      ContentUtils.ApplyContent(gamePath, Constants.Client);
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

    public Task<string> GetServerInfo() {
      return Client.GetStringAsync("getServerInfo");
    }

    public void Dispose()
    {
      Client.Dispose();
    }
  }
}
