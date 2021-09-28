using ACContentSynchronizer.ClientGui.Models;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Modals {
  public class InitRun : Modal {

    public InitRun() {
      InitializeComponent();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
