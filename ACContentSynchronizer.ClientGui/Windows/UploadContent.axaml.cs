using ACContentSynchronizer.ClientGui.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Windows {
  public class UploadContentWindow : Window {
    private readonly UploadContentViewModel _vm;

    public UploadContentWindow() {
      DataContext = _vm = new();

      InitializeComponent();
#if DEBUG
      this.AttachDevTools();
#endif
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public static void Open(Window parent, ServerEntry server) {
      var wnd = new UploadContentWindow();
      wnd._vm.Server = server;
      wnd.ShowDialog(parent);
    }

    private void Close(object? sender, RoutedEventArgs e) {
      Close();
    }
  }
}
