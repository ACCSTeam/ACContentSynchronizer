using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Modals;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia.Collections;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views {
  public class SideBarViewModel : ViewModelBase {
    private bool _isMinimized;

    private ServerEntry? _selectedServer;

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

    public bool IsMinimized {
      get => _isMinimized;
      private set {
        this.RaiseAndSetIfChanged(ref _isMinimized, value);

        var settings = Settings.Instance;
        Size = value
          ? 60
          : 300;
        settings.SidebarMinimized = value;
        settings.Save();
      }
    }

    public AvaloniaList<ServerEntry> Servers { get; set; }

    public ServerEntry? SelectedServer {
      get => _selectedServer;
      set {
        if (value != null) {
          Layout.Instance.SelectServer(value);
        }

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

    public async Task EditServerDialog(ServerEntry server) {
      await Modal.Open<AddNewServer, AddNewServerViewModel>(new() {
        Ip = server.Ip,
        Port = server.Port,
        Password = server.Password,
        DateTime = server.DateTime,
      });
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

    public async Task OpenSettings() {
      await Modal.Open<InitRun>();
    }
  }
}
