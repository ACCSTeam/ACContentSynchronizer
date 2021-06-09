using ACContentSynchronizer.Client.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Windows {
  public class SettingsWindow : Window {
    private readonly SettingsWindowViewModel _vm;

    public SettingsWindow() {
      DataContext = _vm = new();

      InitializeComponent();
#if DEBUG
      this.AttachDevTools();
#endif
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public static void Open(Window parent) {
      var wnd = new SettingsWindow();
      wnd.ShowDialog<Settings>(parent);
    }

    private void SelectPath(object? sender, RoutedEventArgs e) {
      var unused = _vm.SelectPath(this);
    }

    private void Apply(object? sender, RoutedEventArgs e) {
      var settings = Settings.Instance();

      settings.GamePath = _vm.Path;
      settings.Save();

      Close();
    }
  }
}
