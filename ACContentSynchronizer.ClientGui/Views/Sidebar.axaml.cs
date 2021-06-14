using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views {
  public class Sidebar : UserControl {
    private readonly SideBarViewModel _vm;
    private static Sidebar? _instance;

    public static Sidebar Instance => _instance ??= new();

    public Sidebar() {
      DataContext = _vm = new();

      InitializeComponent();

      _instance = this;
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void Toggle() {
      _vm.Size = _vm.IsMinimized ? 300 : 60;
      _vm.IsMinimized = !_vm.IsMinimized;
    }
  }
}
