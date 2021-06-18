using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.Client.Models;
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

      _instance = this;
    }

    public static Sidebar Instance => _instance ??= new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public Task Open() {
      return _vm.OpenAddServerDialog();
    }

    public void Add(ServerEntry server) {
      var serverEntry = _vm.Servers.FirstOrDefault(x => x.DateTime == server.DateTime);
      if (serverEntry != null) {
        serverEntry.Ip = server.Ip;
        serverEntry.Password = server.Password;
      } else {
        _vm.Servers.Add(server);
      }
    }
  }
}
