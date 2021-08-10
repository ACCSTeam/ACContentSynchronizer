using ACContentSynchronizer.ClientGui.Components.Server;
using ACContentSynchronizer.ClientGui.Models;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views {
  public class Server : UserControl {
    private static Server? _instance;
    private readonly ServerViewModel _vm;

    public Server() {
      _instance ??= this;
      DataContext = _vm = new();
      InitializeComponent();
    }

    public static Server Instance => _instance ??= new();

    public ServerEntry GetServer => _vm.ServerEntry;

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void SetServer(ServerEntry serverEntry) {
      _vm.ServerEntry = serverEntry;
      Race.Instance.Refresh();
      ServerSettings.Instance.Load(serverEntry);
    }
  }
}
