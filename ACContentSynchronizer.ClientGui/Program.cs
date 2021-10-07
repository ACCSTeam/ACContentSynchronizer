using System;
using Avalonia;
using Avalonia.ReactiveUI;
using Splat;

namespace ACContentSynchronizer.ClientGui {
  internal class Program {
    public static void Main(string[] args) {
      BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() {
      Startup.Register(Locator.CurrentMutable);
      return AppBuilder.Configure<App>()
        .UsePlatformDetect()
        .With(new Win32PlatformOptions { OverlayPopups = true })
        .With(new AvaloniaNativePlatformOptions { OverlayPopups = true })
        .LogToTrace()
        .UseReactiveUI();
    }
  }
}
