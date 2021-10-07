using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Extensions;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Services;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.Models;
using ReactiveUI;
using Splat;

namespace ACContentSynchronizer.ClientGui.Modals {
  public class AddNewServerViewModel : ModalViewModel, IDisposable {
    private readonly ApplicationViewModel _application;
    private readonly DataService _dataService;

    public AddNewServerViewModel() {
      _application = Locator.Current.GetService<ApplicationViewModel>();
      _dataService = new(Server);

      Server.SubscribeValue(x => x.Http,
        _ => ReactiveCommand.CreateFromTask(GetServerPresets).Execute());

      Server.SubscribeValue(x => x.ServerPreset,
        x => x.Password,
        _ => ReactiveCommand.CreateFromTask(GetServerProps).Execute());
    }

    public ServerEntryViewModel Server { get; set; } = new();
    private List<ServerPreset> _serverPresets = new();

    public List<ServerPreset> ServerPresets {
      get => _serverPresets;
      set => this.RaiseAndSetIfChanged(ref _serverPresets, value);
    }

    private ServerPreset? _selectedServerPreset;

    public ServerPreset? SelectedServerPreset {
      get => _selectedServerPreset;
      set {
        this.RaiseAndSetIfChanged(ref _selectedServerPreset, value);

        if (value != null) {
          Server.ServerPreset = value.Preset;
        }
      }
    }

    private bool _canSave;

    public bool CanSave {
      get => _canSave;
      set => this.RaiseAndSetIfChanged(ref _canSave, value);
    }

    private async Task GetServerPresets() {
      ServerPresets = await _dataService.GetAllowedServers();
    }

    private async Task GetServerProps() {
      if (SelectedServerPreset == null
          || string.IsNullOrEmpty(Server.Password)) {
        return;
      }

      try {
        var serverProps = await _dataService.GetServerProps();
        Server.Name = serverProps.Name;
        Server.KunosPort = serverProps.HttpPort;
        CanSave = true;
      } catch {
        Server.Name = "";
        Server.KunosPort = "";
        CanSave = false;
        throw;
      }
    }

    public void Save() {
      _application.Servers.Add(Server);

      Close();
    }

    public void Dispose() {
      _dataService?.Dispose();
    }
  }
}
