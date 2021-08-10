using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerConditions : UserControl {
    private static ServerConditions? _instance;
    private readonly ServerConditionsViewModel _vm;

    public ServerConditions() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    public static ServerConditions Instance => _instance ??= new();
    public static ServerConditionsViewModel ViewModel => Instance._vm;

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void Load(IniFile serverConfig) {
      throw new NotImplementedException();
    }

    public IniFile Save(IniFile source) {
      source["SERVER"]["SUN_ANGLE"] = GetSunAngle();
      source["SERVER"]["TIME_OF_DAY_MULT"] = _vm.TimeMultiplier;
      if (_vm.DynamicTrack) {
        source.Add("DYNAMIC_TRACK", new() {
          ["SESSION_START"] = _vm.StartValue,
          ["RANDOMNESS"] = _vm.Randomness,
          ["SESSION_TRANSFER"] = _vm.Transferred,
          ["LAP_GAIN"] = _vm.Laps,
        });
      }

      for (var i = 0; i < _vm.Weathers.Count; i++) {
        var weather = _vm.Weathers[i].ViewModel;

        source.Add($"WEATHER_{i}", new() {
          ["GRAPHICS"] = weather.Graphics,
          ["BASE_TEMPERATURE_AMBIENT"] = weather.AmbientTemperature,
          ["BASE_TEMPERATURE_ROAD"] = weather.RoadTemperature,
          ["VARIATION_AMBIENT"] = weather.AmbientVariation,
          ["VARIATION_ROAD"] = weather.RoadVariation,
          ["WIND_BASE_SPEED_MIN"] = weather.WindMin,
          ["WIND_BASE_SPEED_MAX"] = weather.WindMax,
          ["WIND_BASE_DIRECTION"] = weather.WindDirection,
          ["WIND_VARIATION_DIRECTION"] = weather.WindDirectionVariation,
        });
      }

      return source;
    }

    private double GetSunAngle() {
      const int middle = 13;
      const double angleInMin = 3.7;
      const double angleInHour = 16;

      var hours = _vm.Time.Hours;
      var hoursPassed = hours - middle;
      var angle = hoursPassed * angleInHour;
      angle += _vm.Time.Minutes / angleInMin;

      return Math.Round(angle);
    }
  }
}
