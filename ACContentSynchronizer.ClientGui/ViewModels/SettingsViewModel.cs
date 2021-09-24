using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.ViewModels {
  public class SettingsViewModel : ViewModelBase {
    private string _gamePath = "";

    private string _playerName = "";

    private bool _sidebarMinimized;

    private string _steamId = "";

    public string GamePath {
      get => _gamePath;
      set => this.RaiseAndSetIfChanged(ref _gamePath, value);
    }

    public string PlayerName {
      get => _playerName;
      set => this.RaiseAndSetIfChanged(ref _playerName, value);
    }

    public string SteamId {
      get => _steamId;
      set => this.RaiseAndSetIfChanged(ref _steamId, value);
    }

    public bool SidebarMinimized {
      get => _sidebarMinimized;
      set => this.RaiseAndSetIfChanged(ref _sidebarMinimized, value);
    }
  }
}
