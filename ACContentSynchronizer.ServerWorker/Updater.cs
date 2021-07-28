using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.Server;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ACContentSynchronizer.ServerWorker {
  public class Worker {
    [Import(typeof(IServer))]
    public IServer? Server { get; set; }
  }

  public class Updater : BackgroundService {
    private readonly ILogger<Updater> _logger;
    private Task? _serverTask;
    private IServer? _server;

    public Updater(ILogger<Updater> logger) {
      _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
      StartServer(stoppingToken);
      while (!stoppingToken.IsCancellationRequested) {
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        await Task.Delay(1000, stoppingToken);
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
      var asm = Assembly.LoadFrom("bin/Debug/net5.0/ACContentSynchronizer.Server.dll");
      var catalog = new AssemblyCatalog(asm);
      var worker = new Worker();
      var container = new CompositionContainer(catalog);
      container.ComposeParts(worker);

      return worker.Server ?? null;
    }
  }
}
