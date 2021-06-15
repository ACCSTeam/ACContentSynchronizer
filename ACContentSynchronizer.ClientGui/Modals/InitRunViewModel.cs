using System.Threading.Tasks;
using ACContentSynchronizer.Client.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.ClientGui.Windows;
using Avalonia.Collections;
using Avalonia.Controls;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Modals {
  public class InitRunViewModel : ViewModelBase {
    public InitRunViewModel() {
      var settings = Settings.Instance;
      Path = settings.GamePath;
      PlayerName = settings.PlayerName;
      SteamId = settings.SteamId;
    }

    private string _path = "";

    private string _playerName = "Player";

    private long _steamId;

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

    public AvaloniaList<long> Profiles { get; set; } = new() {
      76561198142095501,
    };

    public async Task GetPath() {
      var path = await new OpenFolderDialog().ShowAsync(MainWindow.Instance);
      if (!string.IsNullOrEmpty(path)) {
        Path = path;
      }
    }

    public async Task SaveAndContinue() {
      var settings = Settings.Instance;

      settings.GamePath = Path;
      settings.PlayerName = PlayerName;
      settings.SteamId = SteamId;

      await settings.SaveAsync();
    }
  }
}
