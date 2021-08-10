using System;
using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.ViewModels {
  public abstract class TaskViewModel : ViewModelBase, IDisposable {
    private double _progress;

    private string _state = "";

    protected TaskViewModel(ServerEntry server) {
      Server = server;

      ReactiveCommand.CreateFromTask(async () => {
        ClientId = await Hubs.NotificationHub<double, HubMethods>(Server, HubMethods.Progress, progress => {
          Progress = progress;
          return Task.CompletedTask;
        });

        await Hubs.NotificationHub<string, HubMethods>(Server, HubMethods.Message, message => {
          State = message;
          return Task.CompletedTask;
        });

        await Hubs.NotificationHub<double, string, HubMethods>(Server, HubMethods.ProgressMessage,
          (progress, message) => {
            Progress = progress;
            State = message;
            return Task.CompletedTask;
          });
      });
    }

    public double Progress {
      get => _progress;
      set => this.RaiseAndSetIfChanged(ref _progress, value);
    }

    public string State {
      get => _state;
      set => this.RaiseAndSetIfChanged(ref _state, value);
    }

    public Task Worker { get; protected set; } = new(() => { });

    protected CancellationTokenSource Canceller { get; set; } = new();

    protected ServerEntry Server { get; }

    protected string ClientId { get; private set; } = "";

    public void Dispose() {
      Worker.Dispose();
      Canceller.Dispose();
    }

    public abstract void Run();

    public void Cancel() {
      Canceller.Cancel();
    }
  }
}
