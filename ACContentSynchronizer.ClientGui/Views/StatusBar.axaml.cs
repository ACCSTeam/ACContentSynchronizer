using ACContentSynchronizer.ClientGui.Components;
using ACContentSynchronizer.ClientGui.Extensions;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views {
  public class StatusBar : DisposableControl {
    private readonly StatusBarViewModel _vm;

    public StatusBar() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    private void OpenStatusPopup(object? sender, RoutedEventArgs e) {
      this.OpenPopup("StatusPopup");
    }
  }
}
