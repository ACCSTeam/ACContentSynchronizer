using ACContentSynchronizer.ClientGui.Windows;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Models {
  public class Modal : Window {
    public Modal() {
#if DEBUG
      this.AttachDevTools();
#endif
    }

    public static void Open<T>() where T : Window, new() {
      var modal = new T();
      modal.ShowDialog(MainWindow.Instance);
    }
  }
}
