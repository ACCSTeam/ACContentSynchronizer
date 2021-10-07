using System;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Services;
using ACContentSynchronizer.ClientGui.Views.ServerViews;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.ViewModels {
  public class ServerViewModel : ViewModelBase, IDisposable {
    private readonly HubService _hubService;

    public ServerViewModel(ServerEntryViewModel server) {
      _hubService = new(server);
      ReactiveCommand.CreateFromTask(async () => {
        Race = new(new(server, _hubService));

        var dataService = new DataService(server);
        HasPrivileges = await dataService.HasPrivileges();
        if (HasPrivileges) {
          ServerSettings = new(new(server, _hubService));
        }
        dataService.Dispose();
      }).Execute();
    }

    private Race? Race { get; set; }

    private ServerSettings? _serverSettings;

    public ServerSettings? ServerSettings {
      get => _serverSettings;
      set => this.RaiseAndSetIfChanged(ref _serverSettings, value);
    }

    private bool _hasPrivileges;

    public bool HasPrivileges {
      get => _hasPrivileges;
      set => this.RaiseAndSetIfChanged(ref _hasPrivileges, value);
    }

    public void Dispose() {
      _hubService.Dispose();
      Race?.Dispose();
      ServerSettings?.Dispose();
    }
  }
}
