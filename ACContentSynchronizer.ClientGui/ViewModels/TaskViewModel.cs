using System;
using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Services;
using ReactiveUI;
using Splat;

namespace ACContentSynchronizer.ClientGui.ViewModels {
  public abstract class TaskViewModel : ViewModelBase, IDisposable {
    protected readonly ApplicationViewModel Application;
    protected readonly ContentService ContentService;
    protected readonly DataService DataService;

    private HubService? _hubService;

    private string _message = "";
    private double _progress;

    protected TaskViewModel(ServerEntryViewModel server) {
      var locator = Locator.Current;
      Server = server;
      DataService = new(server);
      Application = locator.GetService<ApplicationViewModel>();
      ContentService = locator.GetService<ContentService>();
    }

    public double Progress {
      get => _progress;
      set => this.RaiseAndSetIfChanged(ref _progress, value);
    }

    public string Message {
      get => _message;
      set => this.RaiseAndSetIfChanged(ref _message, value);
    }

    public Task Worker { get; protected set; } = new(() => { });

    protected CancellationTokenSource Canceller { get; set; } = new();

    private ServerEntryViewModel Server { get; }

    public void Dispose() {
      Worker.Dispose();
      Canceller.Dispose();
      _hubService?.Dispose();
      DataService.Dispose();
    }

    public async Task Initialize() {
      _hubService = new(Server);
      await _hubService.NotificationHub<double, HubMethods>(HubMethods.Progress,
        progress => {
          Progress = progress;
          return Task.CompletedTask;
        });

      await _hubService.NotificationHub<string, HubMethods>(HubMethods.Message,
        message => {
          Message = message;
          return Task.CompletedTask;
        });

      await _hubService.NotificationHub<double, string, HubMethods>(HubMethods.ProgressMessage,
        (progress, message) => {
          Progress = progress;
          Message = message;
          return Task.CompletedTask;
        });
    }

    public abstract void Run();

    public void Cancel() {
      Canceller.Cancel();
    }
  }
}
