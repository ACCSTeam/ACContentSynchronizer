using ACContentSynchronizer.ClientGui.Components;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views.ServerViews {
  public class ServerWeather : DisposableControl {
    public ServerWeather() {
      DataContext = new ServerWeatherViewModel();
      InitializeComponent();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
