using ACContentSynchronizer.ClientGui.Models;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views {
  public class Layout : UserControl {
    private static Layout? _instance;
    private readonly LayoutViewModel _vm;

    public Layout() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    public static Layout Instance => _instance ??= new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void SelectServer(ServerEntry serverEntry) {
      _vm.ServerSelected = true;
      Server.Instance.SetServer(serverEntry);
    }
  }
}
