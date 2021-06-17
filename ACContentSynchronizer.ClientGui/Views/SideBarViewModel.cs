using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.Client.Models;
using ACContentSynchronizer.ClientGui.Modals;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia.Collections;
using Material.Icons;
using Material.Icons.Avalonia;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views {
  public class SideBarViewModel : ViewModelBase {
    private bool _isMinimized;

    private int _labelWidth;

    private object _newContent = "Add new";

    private ServerEntry _selectedServer = new();

    private object _settingsContent = "Settings";

    private int _size = 300;

    public SideBarViewModel() {
      var settings = Settings.Instance;

      Servers = new(settings.Servers);
      IsMinimized = settings.SidebarMinimized;
    }

    public int Size {
      get => _size;
      set => this.RaiseAndSetIfChanged(ref _size, value);
    }

    public int LabelWidth {
      get => _labelWidth;
      set => this.RaiseAndSetIfChanged(ref _labelWidth, value);
    }

    public bool IsMinimized {
      get => _isMinimized;
      set {
        LabelWidth = value ? 20 : 140;
        Size = value ? 60 : 300;
        NewContent = value ? new MaterialIcon { Kind = MaterialIconKind.Plus } : "Add new";
        SettingsContent = value ? new MaterialIcon { Kind = MaterialIconKind.Cog } : "Settings";
        this.RaiseAndSetIfChanged(ref _isMinimized, value);

        var settings = Settings.Instance;
        settings.SidebarMinimized = value;
        settings.Save();
      }
    }

    public object NewContent {
      get => _newContent;
      set => this.RaiseAndSetIfChanged(ref _newContent, value);
    }

    public object SettingsContent {
      get => _settingsContent;
      set => this.RaiseAndSetIfChanged(ref _settingsContent, value);
    }

    public AvaloniaList<ServerEntry> Servers { get; set; }

    public ServerEntry SelectedServer {
      get => _selectedServer;
      set {
        Layout.Instance.SelectServer(value);
        this.RaiseAndSetIfChanged(ref _selectedServer, value);
      }
    }

    public void Toggle() {
      IsMinimized = !IsMinimized;
    }

    public async Task OpenAddServerDialog() {
      await Modal.Open<AddNewServer>();
      await SaveServersState();
    }

    public Task Remove(ServerEntry server) {
      Servers.Remove(server);
      return SaveServersState();
    }

    private Task SaveServersState() {
      var settings = Settings.Instance;
      settings.Servers = Servers.ToList();
      return settings.SaveAsync();
    }
  }
}
