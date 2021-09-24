using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Splat;

namespace ACContentSynchronizer.ClientGui.Views {
  public class Main : UserControl {
    public Main() {
      DataContext = Locator.Current.GetService<ApplicationViewModel>();
      InitializeComponent();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
