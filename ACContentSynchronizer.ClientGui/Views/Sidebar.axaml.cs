using System.Threading.Tasks;
using ACContentSynchronizer.Client.Models;
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
      _vm.Servers.Add(server);
    }
  }
}
