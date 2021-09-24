using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.Extensions;
using ACContentSynchronizer.Models;
using ReactiveUI;
using Splat;

namespace ACContentSynchronizer.ClientGui.Services {
  public class DataService : IDisposable {
    public delegate void CompleteEvent();

    public delegate void ProgressEvent(double progress);

    private readonly HttpClient _client;
    private readonly ContentService _contentService;

    private string _session = "";

    public DataService(ServerEntryViewModel serverEntry) {
      _contentService = Locator.Current.GetService<ContentService>();

      if (string.IsNullOrEmpty(serverEntry.Http)) {
        throw new NullReferenceException(nameof(serverEntry.Http));
      }

      _client = new() {
        BaseAddress = new(serverEntry.Http),
        Timeout = Timeout.InfiniteTimeSpan,
      };

      _client.DefaultRequestHeaders.Add(DefaultHeaders.Password, serverEntry.Password);
      serverEntry.WhenAnyValue(model => model.ClientId).Subscribe(clientId => {
          _client.DefaultRequestHeaders.Remove(DefaultHeaders.ClientId);
          _client.DefaultRequestHeaders.Add(DefaultHeaders.ClientId, clientId);
        }
      );
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
      var server = $"{Client.BaseAddress}/{Routes.DownloadContent}?session={_session}";
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
      return Client.PostJson<Manifest>(Routes.GetUpdateManifest, manifest);
    }

    public async Task UpdateContent() {
      var contentArchive = Path.Combine(Constants.Client, Constants.ContentArchive);
      await using var stream = File.OpenRead(contentArchive);
      await Client.PostAsync(Routes.UpdateContent, new StreamContent(stream));
    }

    public async Task RefreshServer(Manifest manifest) {
      await Client.PostAsJsonAsync(Routes.RefreshServer, manifest);
    }

    public Task<ServerProps?> GetServerProps() {
      return Client.GetJson<ServerProps>(Routes.GetServerProps);
    }

    public async Task<IniFile?> GetServerConfig() {
      var dictionary = await Client.GetJson<Dictionary<string, Dictionary<string, string>>?>(Routes.GetServerConfig);
      return dictionary == null
        ? new()
        : IniProvider.DictionaryToIniFile(dictionary);
    }

    public async Task<IniFile?> GetEntryList() {
      var dictionary = await Client.GetJson<Dictionary<string, Dictionary<string, string>>?>(Routes.GetEntryConfig);
      return dictionary == null
        ? new()
        : IniProvider.DictionaryToIniFile(dictionary);
    }
  }
}
