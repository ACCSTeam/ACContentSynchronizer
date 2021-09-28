using ACContentSynchronizer.ClientGui.Components;
using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Splat;

namespace ACContentSynchronizer.ClientGui.Views {
  public class Main : DisposableControl {
    public Main() {
      DataContext = Locator.Current.GetService<ApplicationViewModel>();
      InitializeComponent();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
