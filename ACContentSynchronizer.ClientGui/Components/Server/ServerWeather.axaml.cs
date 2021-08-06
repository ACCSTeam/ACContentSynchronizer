using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerWeather : UserControl {
    private readonly ServerWeatherViewModel _vm;

    public ServerWeather() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public Dictionary<string, Dictionary<string, object>> ToConfig(
      Dictionary<string, Dictionary<string, object>> source, int index) {
      source.Add($"WEATHER_{index}", new() {
        ["GRAPHICS"] = _vm.Graphics,
        ["BASE_TEMPERATURE_AMBIENT"] = _vm.AmbientTemperature,
        ["BASE_TEMPERATURE_ROAD"] = _vm.RoadTemperature,
        ["VARIATION_AMBIENT"] = _vm.AmbientVariation,
        ["VARIATION_ROAD"] = _vm.RoadVariation,
      });

      return source;
    }
  }
}
