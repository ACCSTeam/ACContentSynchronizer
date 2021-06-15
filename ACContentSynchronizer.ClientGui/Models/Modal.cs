using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Windows;
using Avalonia;
using Avalonia.Controls;

namespace ACContentSynchronizer.ClientGui.Models {
  public class Modal : Window {
    protected Modal() {
#if DEBUG
      this.AttachDevTools();
#endif
    }

    public static async Task Open<T>() where T : Window, new() {
      var modal = new T();
      await modal.ShowDialog(MainWindow.Instance);
    }
  }
}
