using ACContentSynchronizer.ClientGui.Windows;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui {
  public class App : Application {
    public override void Initialize() {
      AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {
      if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
        desktop.MainWindow = MainWindow.Instance;
      }

      base.OnFrameworkInitializationCompleted();
    }
  }
}
