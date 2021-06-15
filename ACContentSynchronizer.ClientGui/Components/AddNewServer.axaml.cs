using ACContentSynchronizer.ClientGui.Models;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components {
  public class AddNewServer : Modal {
    private readonly AddNewServerViewModel _vm;

    public AddNewServer() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
