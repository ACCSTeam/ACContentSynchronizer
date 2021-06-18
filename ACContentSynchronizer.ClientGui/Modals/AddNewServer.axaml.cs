using ACContentSynchronizer.ClientGui.Models;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Modals {
  public class AddNewServer : Modal {
    private readonly AddNewServerViewModel _vm;

    public AddNewServer() {
      DataContext = _vm = new() {
        Instance = this,
      };
      InitializeComponent();
    }
    public AddNewServer(AddNewServerViewModel vm) {
      DataContext = _vm = vm;
      _vm.Instance = this;
        InitializeComponent();
    }



    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
