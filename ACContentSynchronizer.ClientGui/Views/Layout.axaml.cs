using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Windows;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Input;
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

    public void Attach(Window window) {
      this.FindControl<CaptionButtons>("CaptionButtons").Attach(window);
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void SelectServer(ServerEntry serverEntry) {
      _vm.ServerSelected = true;
      Server.Instance.SetServer(serverEntry);
    }

    private void DragWindow(object? sender, PointerPressedEventArgs e) {
      MainWindow.Instance.BeginMoveDrag(e);
    }
  }
}
