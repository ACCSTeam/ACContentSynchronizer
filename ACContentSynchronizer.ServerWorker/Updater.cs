using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.Extensions;
using ACContentSynchronizer.ServerWorker.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ACContentSynchronizer.ServerWorker {
  public class Updater : BackgroundService {
    private const string ServerName = "ACContentSynchronizer.Server";
    private readonly string _archiveName = $"{ServerName}.zip";
    private readonly string _executableName = $"{ServerName}.exe";

    private readonly HttpClient _github;
    private readonly ILogger<Updater> _logger;
    private long _latestRelease;

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
        try {
          var releases = await _github
            .GetJson<GithubRepository>("/repos/the1fest/ACContentSynchronizer/releases/latest");

          var serverUrl = releases?.Assets
            .FirstOrDefault(x => x.Name == _archiveName)?.BrowserDownloadUrl;

          if (!string.IsNullOrEmpty(serverUrl) && releases?.Id > _latestRelease) {
            _logger.LogInformation($"Has new release {DateTime.Now}");

            var downloadClient = new WebClient();
            downloadClient.DownloadFileAsync(new(serverUrl), _archiveName);

            downloadClient.DownloadFileCompleted += (_, _) => {
              _logger.LogInformation($"Downloaded {DateTime.Now}");

              DirectoryUtils.DeleteIfExists(Constants.DownloadsPath);
              ZipFile.ExtractToDirectory(_archiveName, Constants.DownloadsPath);
              FileUtils.DeleteIfExists(_archiveName);

              StopServer();

              var files = Directory.GetFiles(Constants.DownloadsPath);
              var serverPath = Path.Combine(Directory.GetCurrentDirectory(), ServerName);

              DirectoryUtils.CreateIfNotExists(serverPath);

              foreach (var file in files) {
                var destinationPath = Path.Combine(serverPath, DirectoryUtils.Name(file));
                FileUtils.GrantAccess(() => {
                  File.Move(file, destinationPath, true);
                  return Task.CompletedTask;
                }, TimeSpan.FromMinutes(1));
              }

              DirectoryUtils.DeleteIfExists(Constants.DownloadsPath);
              StartServer();

              _latestRelease = releases.Id;
              _logger.LogInformation($"Done {DateTime.Now}");
            };
          }
        } catch (Exception e) {
          _logger.LogError(e.Message);
        }
        await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
      }
    }

    private void StartServer() {
      try {
        var path = Path.Combine(Directory.GetCurrentDirectory(),
          $"{ServerName}/{_executableName}");

        new ContentService().ExecuteCommand(path);

        _logger.LogInformation($"Server started {DateTime.Now}");
      } catch (Exception e) {
        _logger.LogInformation(e.Message);
      }
    }

    private void StopServer() {
      var runningProcess = Process.GetProcesses().FirstOrDefault(x => x.ProcessName == ServerName);

      if (runningProcess != null) {
        runningProcess.Kill();

        _logger.LogInformation($"Server stopped  {DateTime.Now}");
      } else {
        _logger.LogInformation($"Server not in running state {DateTime.Now}");
      }
    }
  }
}
