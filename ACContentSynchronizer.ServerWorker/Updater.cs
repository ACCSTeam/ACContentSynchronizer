using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.ServerShared;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ACContentSynchronizer.ServerWorker {
  public class Worker : IDisposable {
    [Import(typeof(IServer))]
    public IServer? Server { get; set; }

    public void Dispose() {
      Server?.Dispose();
    }
  }

  public class Updater : BackgroundService {
    private const string ArchiveName = "ACContentSynchronizer.ServerWorker.zip";
    private long _latestRelease;

    private readonly ILogger<Updater> _logger;
    private Task? _serverTask;
    private IServer? _server;
    private readonly HttpClient _github;

    public Updater(ILogger<Updater> logger) {
      _logger = logger;
      _github = new() {
        BaseAddress = new("https://api.github.com"),
        DefaultRequestHeaders = {
          { "Accept", "application/vnd.github.v3+json" },
          { "User-Agent", "ACContentSynchronizer" },
        },
      };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
      while (true) {
        // var releases = await _github.GetJson<List<GithubRepository>>("/repos/the1fest/ACContentSynchronizer/releases");
        // var latestRelease = releases.FirstOrDefault(x => x.Id == releases.Max(r => r.Id));
        // var serverUrl = latestRelease?.Assets
        //   .FirstOrDefault(x => x.Name == ArchiveName)?.BrowserDownloadUrl;
        //
        // if (!string.IsNullOrEmpty(serverUrl) && latestRelease?.Id > _latestRelease) {
        //   var applyTask = new Task(() => { });
        //   var downloadClient = new WebClient();
        //   downloadClient.DownloadFileCompleted += (_, _) => applyTask.Start();
        //   downloadClient.DownloadFileAsync(new(serverUrl), ArchiveName);
        //   await applyTask;
        //
        //   ZipFile.ExtractToDirectory(ArchiveName, Constants.DownloadsPath);
        //
        //   _latestRelease = latestRelease.Id;
        // }
        StartServer(stoppingToken);
        await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
      }
    }

    private void StartServer(CancellationToken stoppingToken) {
      Task.Run(async () => {
        if (_serverTask != null) {
          _server?.Stop();
          await _serverTask;
        }

        _serverTask = Task.Run(() => {
          _server = GetServer();
          _server?.EntryPoint();
        }, stoppingToken);
      }, stoppingToken);
    }

    private IServer? GetServer() {
      try {
        var asm = Assembly.LoadFrom("server/ACContentSynchronizer.Server.dll");
        using var catalog = new AssemblyCatalog(asm);
        using var worker = new Worker();
        using var container = new CompositionContainer(catalog);
        container.ComposeParts(worker);

        return worker.Server ?? null;
      } catch (Exception e) {
        _logger.LogInformation(e.Message);
        return null;
      }
    }
  }
}
