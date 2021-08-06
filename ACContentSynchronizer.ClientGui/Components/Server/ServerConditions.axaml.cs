using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerConditions : UserControl {
    private readonly ServerConditionsViewModel _vm;
    private static ServerConditions? _instance;

    public ServerConditions() {
      DataContext = _vm = new();
      InitializeComponent();
    }

    public static ServerConditions Instance => _instance ??= new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public Dictionary<string, Dictionary<string, object>> ToConfig(
      Dictionary<string, Dictionary<string, object>> source) {
      source["SERVER"]["SUN_ANGLE"] = GetSunAngle();

      if (_vm.DynamicTrack) {
        source["DYNAMIC_TRACK"]["SESSION_START"] = _vm.StartValue;
        source["DYNAMIC_TRACK"]["RANDOMNESS"] = _vm.Randomness;
        source["DYNAMIC_TRACK"]["SESSION_TRANSFER"] = _vm.Transferred;
        source["DYNAMIC_TRACK"]["LAP_GAIN"] = _vm.Laps;
      } else {
        source["DYNAMIC_TRACK"]["SESSION_START"] = "0";
        source["DYNAMIC_TRACK"]["RANDOMNESS"] = "0";
        source["DYNAMIC_TRACK"]["SESSION_TRANSFER"] = "0";
        source["DYNAMIC_TRACK"]["LAP_GAIN"] = "0";
      }

      for (var i = 0; i < _vm.Weathers.Count; i++) {
        source = _vm.Weathers[i].ToConfig(source, i);
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
