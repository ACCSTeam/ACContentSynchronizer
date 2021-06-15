using System.Threading.Tasks;
using ACContentSynchronizer.Client.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.ClientGui.Windows;
using Avalonia.Controls;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views {
  public class MainViewModel : ViewModelBase {
    private bool _initRun;

    private string _path = "";

    private string _playerName = "Player";

    private long _steamId;

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

    public bool InitRun {
      get => _initRun;
      set => this.RaiseAndSetIfChanged(ref _initRun, value);
    }

    public string Path {
      get => _path;
      set => this.RaiseAndSetIfChanged(ref _path, value);
    }

    public string PlayerName {
      get => _playerName;
      set => this.RaiseAndSetIfChanged(ref _playerName, value);
    }

    public long SteamId {
      get => _steamId;
      set => this.RaiseAndSetIfChanged(ref _steamId, value);
    }

    public async Task GetPath() {
      var path = await new OpenFolderDialog().ShowAsync(MainWindow.Instance);
      if (!string.IsNullOrEmpty(path)) {
        Path = path;
      }
    }

    public async Task SaveAndContinue() {
      var settings = Settings.Instance();

      settings.GamePath = Path;
      settings.PlayerName = PlayerName;
      settings.SteamId = SteamId;

      await settings.SaveAsync();
      InitRun = false;
    }

    public void AddNewConnection() {
      InitRun = true;
    }
  }
}
