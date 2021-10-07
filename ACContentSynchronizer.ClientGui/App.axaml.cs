using System;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Windows;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui {
  public class App : Application {
    public override void Initialize() {
      AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {
      if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
        desktop.MainWindow = MainWindow.Instance;
      }

      AppDomain.CurrentDomain.UnhandledException += (sender, args) => {
        Console.WriteLine(args.ExceptionObject);
      };

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
