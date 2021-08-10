using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerWeather : UserControl {
    private readonly ServerWeatherViewModel _vm;
    public ServerWeatherViewModel ViewModel => _vm;

    public ServerWeather() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
