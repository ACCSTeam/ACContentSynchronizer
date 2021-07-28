using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.Extensions;
using ACContentSynchronizer.ServerWorker.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ACContentSynchronizer.ServerWorker {
  public class CustomAssemblyLoadContext : AssemblyLoadContext {
    public CustomAssemblyLoadContext() : base(true) {
    }

    protected override Assembly? Load(AssemblyName assemblyName) {
      return null;
    }
  }

  public class Updater : BackgroundService {
    private const string ServerName = "ACContentSynchronizer.Server";
    private readonly string _archiveName = $"{ServerName}.zip";
    private readonly string _executableName = $"{ServerName}.exe";
    private long _latestRelease;

    private readonly ILogger<Updater> _logger;
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
      while (!stoppingToken.IsCancellationRequested) {
        var releases = await _github.GetJson<GithubRepository>("/repos/the1fest/ACContentSynchronizer/releases/latest");
        var serverUrl = releases.Assets
          .FirstOrDefault(x => x.Name == _archiveName)?.BrowserDownloadUrl;

        if (!string.IsNullOrEmpty(serverUrl) && releases.Id > _latestRelease) {
          var applyTask = new Task(() => { });
          var downloadClient = new WebClient();
          downloadClient.DownloadFileCompleted += (_, _) => applyTask.Start();
          downloadClient.DownloadFileAsync(new(serverUrl), _archiveName);
          await applyTask;

          ZipFile.ExtractToDirectory(_archiveName, Constants.DownloadsPath);
          StopServer();

          StartServer();
          _latestRelease = releases.Id;
        }
        await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
      }
    }

    private void StartServer() {
      try {
        var path = Path.Combine(Directory.GetCurrentDirectory(),
          $"{ServerName}/{_executableName}");
        ContentUtils.ExecuteCommand(path);
      } catch (Exception e) {
        _logger.LogInformation(e.Message);
      }
    }

    private void StopServer() {
      var runningProcess = Process.GetProcesses().FirstOrDefault(x => x.ProcessName == _executableName);
      runningProcess?.Kill();
    }
  }
}
