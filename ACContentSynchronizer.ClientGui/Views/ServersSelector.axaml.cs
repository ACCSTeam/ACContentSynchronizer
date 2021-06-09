using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views {
  public class ServersSelector : UserControl {
    private readonly ServersSelectorViewModel _vm;

    public ServersSelector() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    private void SelectServer(object? sender, RoutedEventArgs e) {
      var server = _vm.SelectedServer;
    }
  }
}
