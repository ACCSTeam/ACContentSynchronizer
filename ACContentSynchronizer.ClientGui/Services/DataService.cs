using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Extensions;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.Extensions;
using ACContentSynchronizer.Models;
using Splat;

namespace ACContentSynchronizer.ClientGui.Services {
  public class DataService : IDisposable {
    public delegate void CompleteEvent();

    public delegate void ProgressEvent(double progress);

    private HttpClient _client;
    private readonly ContentService _contentService;

    private string _session = "";

    public DataService(ServerEntryViewModel serverEntry) {
      _contentService = Locator.Current.GetService<ContentService>();
      _client = CreateClient(serverEntry);

      serverEntry.SubscribeValue(model => model.Http, http => {
        if (string.IsNullOrEmpty(http)
            || _client?.BaseAddress?.ToString() == http) {
          return;
        }

        _client = CreateClient(serverEntry);
      });

      serverEntry.SubscribeValue(model => model.Password,
        password => _client.AddHeader(DefaultHeaders.AccessPassword, password));

      serverEntry.SubscribeValue(model => model.ClientId,
        clientId => _client.AddHeader(DefaultHeaders.WSClientId, clientId));

      serverEntry.SubscribeValue(model => model.ServerPreset,
        serverPreset => _client.AddHeader(DefaultHeaders.ServerPreset, serverPreset));
    }

    private HttpClient CreateClient(ServerEntryViewModel serverEntry) {
      if (string.IsNullOrEmpty(serverEntry.Http)) {
        return new();
      }

      var client = new HttpClient {
        BaseAddress = new(serverEntry.Http),
        Timeout = Timeout.InfiniteTimeSpan,
      };

      client.AddHeader(DefaultHeaders.AccessPassword, serverEntry.Password);
      client.AddHeader(DefaultHeaders.WSClientId, serverEntry.ClientId);
      client.AddHeader(DefaultHeaders.ServerPreset, serverEntry.ServerPreset);

      return client;
    }

    private HttpClient Client {
      get {
        if (_client == null) {
          throw new();
        }

        return _client;
      }
    }

    public void Dispose() {
      _client?.Dispose();
    }

    public event ProgressEvent? OnProgress;
    public event CompleteEvent? OnComplete;

    public Task<Manifest?> DownloadManifest() {
      return Client.GetJson<Manifest>(Routes.GetManifest);
    }

    public Manifest CompareContent(string gamePath, Manifest manifest) {
      return _contentService.CompareContent(gamePath, manifest);
    }

    public async Task PrepareContent(Manifest manifest) {
      _session = await Client.PostString(Routes.PrepareContent, manifest);
    }

    public Task PackContent() {
      return Client.GetAsync($"{Routes.PackContent}?session={_session}");
    }

    public Task CancelPack() {
      return Client.GetAsync($"{Routes.CancelPack}?session={_session}");
    }

    public void DownloadContent() {
      var archive = Path.Combine(_session, Constants.ContentArchive);
      var server = $"{Client.BaseAddress}{Routes.DownloadContent}?session={_session}";
      using var client = new WebClient();
      foreach (var (header, value) in _client.DefaultRequestHeaders) {
        client.Headers[header] = value.FirstOrDefault();
      }
      FileUtils.DeleteIfExists(archive);
      DirectoryUtils.CreateIfNotExists(_session);

      try {
        client.DownloadProgressChanged += (_, e) => OnProgress?.Invoke(e.ProgressPercentage);
      } catch (OperationCanceledException) {
        OnComplete?.Invoke();
      }
      client.DownloadFileCompleted += (_, _) => OnComplete?.Invoke();
      client.DownloadFileAsync(new(server), archive);
    }

    public void SaveData() {
      _contentService.UnpackContent(_session);
    }

    public void Apply(string gamePath) {
      _contentService.ApplyContent(gamePath, _session);
    }

    public Task<Manifest?> GetUpdateManifest(Manifest manifest) {
      return Client.PostJson<Manifest, Manifest>(Routes.GetUpdateManifest, manifest);
    }

    public async Task UpdateContent() {
      var contentArchive = Path.Combine(Constants.Client, Constants.ContentArchive);
      await using var stream = File.OpenRead(contentArchive);
      await Client.PostAsync(Routes.UpdateContent, new StreamContent(stream));
    }

    public async Task RefreshServer(UploadManifest manifest) {
      await Client.PostAsJsonAsync(Routes.RefreshServer, manifest);
    }

    public Task<ServerProps?> GetServerProps() {
      return Client.GetJson<ServerProps>(Routes.GetServerProps);
    }

    public Task<List<ServerPreset>?> GetAllowedServers() {
      return Client.GetJson<List<ServerPreset>>(Routes.GetAllowedServers);
    }

    public Task<IniFile?> GetServerConfig() {
      return Client.GetJson<IniFile?>(Routes.GetServerConfig);
    }

    public Task<IniFile?> GetEntryList() {
      return Client.GetJson<IniFile?>(Routes.GetEntryConfig);
    }
  }
}
