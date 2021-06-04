using ACContentSynchronizer.ClientGui.Windows;
using Avalonia;
using Avalonia.Controls;
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
  }
}

