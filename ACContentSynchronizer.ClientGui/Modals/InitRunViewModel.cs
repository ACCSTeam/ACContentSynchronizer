using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.ClientGui.Windows;
using Avalonia.Collections;
using Avalonia.Controls;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Modals {
  public class InitRunViewModel : ModalViewModel<InitRun> {
    private string _path = "";

    private string _playerName = "Player";

    private SteamProfile? _profile;

    private AvaloniaList<SteamProfile> _profiles = new();

    public InitRunViewModel(InitRun instance) {
      Instance = instance;
      var settings = Settings.Instance;
      Path = settings.GamePath;
      PlayerName = settings.PlayerName;
      Profiles = new(SteamIdHelper.FindUsers());
      Profile = Profiles.FirstOrDefault(x => x.SteamId == settings.SteamId);
    }

    public string Path {
      get => _path;
      set => this.RaiseAndSetIfChanged(ref _path, value);
    }

    public string PlayerName {
      get => _playerName;
      set => this.RaiseAndSetIfChanged(ref _playerName, value);
    }

    public SteamProfile? Profile {
      get => _profile;
      set => this.RaiseAndSetIfChanged(ref _profile, value);
    }

    public AvaloniaList<SteamProfile> Profiles {
      get => _profiles;
      set => this.RaiseAndSetIfChanged(ref _profiles, value);
    }

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
      settings.SteamId = Profile?.SteamId ?? default;

      await settings.SaveAsync();
      Close();
    }
  }
}
