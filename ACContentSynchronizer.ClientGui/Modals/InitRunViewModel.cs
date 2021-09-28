using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.ClientGui.Windows;
using Avalonia.Collections;
using Avalonia.Controls;
using ReactiveUI;
using Splat;

namespace ACContentSynchronizer.ClientGui.Modals {
  public class InitRunViewModel : ModalViewModel {
    private readonly ApplicationViewModel _application;

    private string _path = "";

    private string _playerName = "accsPlayer";

    private SteamProfile? _profile;

    private AvaloniaList<SteamProfile> _profiles = new();

    public InitRunViewModel() {
      _application = Locator.Current.GetService<ApplicationViewModel>();

      Path = _application.Settings.GamePath;
      PlayerName = _application.Settings.PlayerName;
      Profiles = new(SteamIdHelper.FindUsers());
      Profile = Profiles.FirstOrDefault(x => x.SteamId == _application.Settings.SteamId);
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
      _application.Settings.GamePath = Path;
      _application.Settings.PlayerName = PlayerName;
      _application.Settings.SteamId = Profile?.SteamId ?? "";

      await _application.SaveAsync();
      Close();
    }
  }
}
