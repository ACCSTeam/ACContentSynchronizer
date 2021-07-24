using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views {
  public class Sidebar : UserControl {
    private static Sidebar? _instance;
    private readonly SideBarViewModel _vm;

    public Sidebar() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    public static Sidebar Instance => _instance ??= new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public Task Open() {
      return _vm.OpenAddServerDialog();
    }

    public void Save(ServerEntry server) {
      var serverEntry = _vm.Servers.FirstOrDefault(x => x.DateTime == server.DateTime);
      if (serverEntry != null) {
        serverEntry.Ip = server.Ip;
        serverEntry.Port = server.Port;
        serverEntry.Password = server.Password;
        serverEntry.Name = server.Name;
      } else {
        _vm.Servers.Add(server);
      }
    }

    public void UnsetServer() {
      _vm.SelectedServer = null;
    }
  }
}
