using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Windows {
  public class MainWindow : Window {
    private static MainWindow? _instance;

    public MainWindow() {
      InitializeComponent();
#if DEBUG
      this.AttachDevTools();
#endif

      _instance ??= this;
    }

    public static MainWindow Instance => _instance ??= new MainWindow();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);

      if (File.Exists(Constants.SettingPath)) {
        return;
      }

      SettingsWindow.Open(this);
    }
  }
}
