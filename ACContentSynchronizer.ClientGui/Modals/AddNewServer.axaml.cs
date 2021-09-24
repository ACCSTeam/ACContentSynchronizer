using ACContentSynchronizer.ClientGui.Models;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Modals {
  public class AddNewServer : Modal {
    public AddNewServer() {
      DataContext = new AddNewServerViewModel {
        ControlInstance = this,
      };
      InitializeComponent();
    }

    public AddNewServer(AddNewServerViewModel vm) {
      vm.ControlInstance = this;
      DataContext = vm;
      InitializeComponent();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
