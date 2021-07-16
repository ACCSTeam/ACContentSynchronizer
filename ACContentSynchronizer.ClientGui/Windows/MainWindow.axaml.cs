using System.Runtime.InteropServices;
using ACContentSynchronizer.ClientGui.Views;
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
    }

    public static MainWindow Instance => _instance ??= new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
      Layout.Instance.Attach(this);

      TransparencyLevelHint = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
        ? WindowTransparencyLevel.Transparent
        : WindowTransparencyLevel.AcrylicBlur;
    }
  }
}
