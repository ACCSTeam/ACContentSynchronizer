using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using ACContentSynchronizer.ClientGui.Modals;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Views;
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
      var singletons = Assembly
        .GetExecutingAssembly()
        .GetTypes()
        .Where(x => x.GetProperties()
          .Any(p => p.Name == "Instance"));
      var instances = singletons.Select(x => x.GetProperty("Instance")?.GetValue(null, null))
        .ToList();

      if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
        desktop.MainWindow = MainWindow.Instance;
      }

      var settings = Settings.Instance;
      if (string.IsNullOrEmpty(settings.GamePath)
          || string.IsNullOrEmpty(settings.PlayerName)
          || settings.SteamId == default) {
        Modal.Open<InitRun>().ConfigureAwait(false);
      }

      Layout.Instance.SetTheme(settings.Theme);
       // .Select(x=>((dynamic?)x)?.Instance);

      base.OnFrameworkInitializationCompleted();
    }
  }
}
