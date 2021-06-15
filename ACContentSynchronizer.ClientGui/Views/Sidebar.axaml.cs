using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views {
  public class Sidebar : UserControl {
    private static Sidebar? _instance;
    public readonly SideBarViewModel Vm;

    public Sidebar() {
      DataContext = Vm = new();

      InitializeComponent();

      _instance = this;
    }

    public static Sidebar Instance => _instance ??= new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
