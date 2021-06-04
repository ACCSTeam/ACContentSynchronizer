using System.IO;
using System.Threading.Tasks;
using ACContentSynchronizer.Client.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Windows {
  public partial class MainWindow : Window {
    private static MainWindow? _instance;

    public static MainWindow Instance => _instance ??= new MainWindow();

    public  MainWindow() {
      InitializeComponent();
#if DEBUG
      this.AttachDevTools();
#endif

      _instance ??= this;
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);

      if (File.Exists(Constants.SettingPath)) {
        return;
      }

      SettingsWindow.Open(this);
    }
  }
}
