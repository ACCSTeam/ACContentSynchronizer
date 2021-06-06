using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Windows {
  public class DownloadContentWindow : Window {
    private readonly DownloadContentWindowViewModel _vm;

    public DownloadContentWindow() {
      DataContext = _vm = new DownloadContentWindowViewModel();

      InitializeComponent();
#if DEBUG
      this.AttachDevTools();
#endif
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public static void Open(Window parent, string server) {
      var wnd = new DownloadContentWindow();
      var unused = wnd._vm.GetDataFromServer(server);
      wnd.ShowDialog(parent);
    }

    private void Close(object? sender, RoutedEventArgs e) {
      Close();
    }
  }
}
