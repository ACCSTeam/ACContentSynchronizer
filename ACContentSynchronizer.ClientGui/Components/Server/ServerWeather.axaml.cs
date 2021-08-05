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

    public Dictionary<string, Dictionary<string, string>> ToConfig(
      Dictionary<string, Dictionary<string, string>> source, int index) {
      source.Add($"WEATHER_{index}", new() {
        ["GRAPHICS"] = _vm.Graphics,
        ["BASE_TEMPERATURE_AMBIENT"] = _vm.AmbientTemperature.ToString(CultureInfo.InvariantCulture),
        ["BASE_TEMPERATURE_ROAD"] = _vm.RoadTemperature.ToString(CultureInfo.InvariantCulture),
        ["VARIATION_AMBIENT"] = _vm.AmbientVariation.ToString(CultureInfo.InvariantCulture),
        ["VARIATION_ROAD"] = _vm.RoadVariation.ToString(CultureInfo.InvariantCulture),
      });

      return source;
    }
  }
}
