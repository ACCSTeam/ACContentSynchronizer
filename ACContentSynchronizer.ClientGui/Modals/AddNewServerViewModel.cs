using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Extensions;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Services;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.Models;
using ReactiveUI;
using Splat;

namespace ACContentSynchronizer.ClientGui.Modals {
  public class AddNewServerViewModel : ModalViewModel {
    private readonly ApplicationViewModel _application;
    private readonly DataService _dataService;

    public AddNewServerViewModel() {
      _application = Locator.Current.GetService<ApplicationViewModel>();
      _dataService = new(Server);

      Server.SubscribeValue(x => x.Http,
        _ => ReactiveCommand.CreateFromTask(GetServerPresets).Execute());

      Server.SubscribeValue(x => x.ServerPreset,
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

    private async Task GetServerPresets() {
      try {
        ServerPresets = await _dataService.GetAllowedServers() ?? new();
      } catch (Exception e) {
        if (e is HttpRequestException { StatusCode: null }) {
          Toast.Open(Localization.ConnectionEstablishedError);
        }
        ServerPresets = new();
      }
    }

    private async Task GetServerProps() {
      try {
        var serverProps = await _dataService.GetServerProps();
        if (serverProps != null) {
          Server.Name = serverProps.Name;
          Server.KunosPort = serverProps.HttpPort;
        }
      } catch (Exception e) {
        if (e is HttpRequestException re) {
          switch (re.StatusCode) {
            case HttpStatusCode.Forbidden:
              Toast.Open(Localization.WrongPassword);
              break;
            default:
              Toast.Open(Localization.ConnectionEstablishedError);
              break;
          }
        }
        Server.Name = "";
        Server.KunosPort = "";
      }
    }

    public void Save() {
      _application.Servers.Add(Server);
      Close();
    }
  }
}
