using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.Extensions;
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

    public event ProgressEvent? OnProgress;
    public event CompleteEvent? OnComplete;

    public Task<Manifest> DownloadManifest() {
      return Client.GetJson<Manifest>("getManifest");
    }

    public Manifest CompareContent(string gamePath, Manifest manifest) {
      return ContentUtils.CompareContent(gamePath, manifest);
    }

    public Task<string> PrepareContent(Manifest manifest) {
      return Client.PostString("prepareContent", manifest);
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

    public Task<Manifest> GetUpdateManifest(Manifest manifest) {
      return Client.PostJson<Manifest>("getUpdateManifest", manifest);
    }

    public async Task RefreshServer(string adminPassword, Manifest manifest) {
      await Client.PostAsJsonAsync($"refreshServer?adminPassword={adminPassword}", manifest);
    }

    public Task<Dictionary<string, Dictionary<string, string>>> GetServerInfo() {
      return Client.GetJson<Dictionary<string, Dictionary<string, string>>>("getServerInfo");
    }

    public Task<ServerProps> GetServerProps() {
      return Client.GetJson<ServerProps>("getServerProps");
    }

    public Task<List<CarsUpdate>> GetCarsUpdate(long steamId) {
      return Client.GetJson<List<CarsUpdate>>($"getCarsUpdate?steamId={steamId}");
    }
  }
}
