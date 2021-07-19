using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Windows;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

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

    private void RestoreWindow(object? sender, RoutedEventArgs e) {
      MainWindow.Instance.WindowState = MainWindow.Instance.WindowState == WindowState.Maximized
        ? WindowState.Normal
        : WindowState.Maximized;
    }

    private void DragWindow(object? sender, PointerEventArgs e) {
      if (sender is not IVisual visual) {
        return;
      }

      var pointerPoint = e.GetCurrentPoint(visual);
      if (!pointerPoint.Properties.IsLeftButtonPressed) {
        return;
      }

      if (e.Source != null) {
        MainWindow.Instance.BeginMoveDrag(new(e.Source, e.Pointer,
          visual, new(),
          e.Timestamp, pointerPoint.Properties,
          KeyModifiers.None));
      }
    }
  }
}
