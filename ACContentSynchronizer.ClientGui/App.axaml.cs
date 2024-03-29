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

      // var settings = Settings.Instance;
      // if (string.IsNullOrEmpty(settings.GamePath)
      //     || string.IsNullOrEmpty(settings.PlayerName)
      //     || settings.SteamId == default) {
      //   Modal.Open<InitRun>().ConfigureAwait(false);
      // }

      base.OnFrameworkInitializationCompleted();
    }
  }
}
