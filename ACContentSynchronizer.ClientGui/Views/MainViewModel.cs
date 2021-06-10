using System.Threading.Tasks;
using ACContentSynchronizer.Client.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.ClientGui.Windows;
using Avalonia.Controls;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views {
  public class MainViewModel : ViewModelBase {
    public MainViewModel() {
      var settings = Settings.Instance();
      if (!string.IsNullOrEmpty(settings.GamePath)
          && !string.IsNullOrEmpty(settings.PlayerName)
          && settings.SteamId != 0) {
        return;
      }

      InitRun = true;
      Path = settings.GamePath;
      PlayerName = settings.PlayerName;
      SteamId = settings.SteamId;
    }

    private bool _initRun;

    public bool InitRun {
      get => _initRun;
      set => this.RaiseAndSetIfChanged(ref _initRun, value);
    }

    private string _path = "";

    public string Path {
      get => _path;
      set => this.RaiseAndSetIfChanged(ref _path, value);
    }

    private string _playerName = "Player";

    public string PlayerName {
      get => _playerName;
      set => this.RaiseAndSetIfChanged(ref _playerName, value);
    }

    private long _steamId;

    public long SteamId {
      get => _steamId;
      set => this.RaiseAndSetIfChanged(ref _steamId, value);
    }

    public async Task GetPath() {
      Path = await new OpenFolderDialog().ShowAsync(MainWindow.Instance);
    }

    public async Task SaveAndContinue() {
      var settings = Settings.Instance();

      settings.GamePath = Path;
      settings.PlayerName = PlayerName;
      settings.SteamId = SteamId;

      await settings.SaveAsync();
    }
  }
}
