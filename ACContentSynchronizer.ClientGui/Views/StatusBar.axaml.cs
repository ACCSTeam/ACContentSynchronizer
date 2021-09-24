using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views {
  public class StatusBar : UserControl {
    private readonly StatusBarViewModel _vm;

    public StatusBar() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    private void OpenPopup(object? sender, RoutedEventArgs e) {
      this.FindControl<Popup>("StatusPopup").IsOpen = true;
    }
  }
}
