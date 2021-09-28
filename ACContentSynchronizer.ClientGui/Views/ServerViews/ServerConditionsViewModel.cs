using System;
using Avalonia.Collections;
using ReactiveUI;
using Splat;

namespace ACContentSynchronizer.ClientGui.Views.ServerViews {
  public partial class ServerSettingsViewModel {
    private bool _dynamicTrack;

    private short _lapsConditions = 10;

    private short _randomness = 2;

    private ServerWeather? _selectedWeather;

    private short _startValue = 95;

    private TimeSpan _timeConditions = TimeSpan.FromHours(12);

    private double _timeMultiplier = 1;

    private short _transferred = 90;

    public TimeSpan TimeConditions {
      get => _timeConditions;
      set => this.RaiseAndSetIfChanged(ref _timeConditions, value);
    }

    public double TimeMultiplier {
      get => _timeMultiplier;
      set => this.RaiseAndSetIfChanged(ref _timeMultiplier, value);
    }

    public bool DynamicTrack {
      get => _dynamicTrack;
      set => this.RaiseAndSetIfChanged(ref _dynamicTrack, value);
    }

    public short StartValue {
      get => _startValue;
      set => this.RaiseAndSetIfChanged(ref _startValue, value);
    }

    public short Transferred {
      get => _transferred;
      set => this.RaiseAndSetIfChanged(ref _transferred, value);
    }

    public short Randomness {
      get => _randomness;
      set => this.RaiseAndSetIfChanged(ref _randomness, value);
    }

    public short LapsConditions {
      get => _lapsConditions;
      set => this.RaiseAndSetIfChanged(ref _lapsConditions, value);
    }

    public ServerWeather? SelectedWeather {
      get => _selectedWeather;
      set {
        this.RaiseAndSetIfChanged(ref _selectedWeather, value);
        this.RaisePropertyChanged(nameof(CanRemoveWeather));
      }
    }

    public AvaloniaList<ServerWeather> Weathers { get; set; } = new() {
      new(),
    };

    public static TimeSpan TimeMinimum => TimeSpan.FromHours(8);
    public static TimeSpan TimeMaximum => TimeSpan.FromHours(18);

    public static short TimeMultiplierMinimum => 0;
    public static short TimeMultiplierMaximum => 60;
    public static double TickMultiplierFrequency => 0.1;

    public static short LapsMinimum => 1;
    public static short LapsMaximum => 81;

    public bool CanRemoveWeather => SelectedWeather != null && Weathers.Count > 1;

    public void AddWeather() {
      Weathers.Add(new());
      this.RaisePropertyChanged(nameof(CanRemoveWeather));
    }

    public void RemoveWeather() {
      if (SelectedWeather == null) {
        return;
      }

      Weathers.Remove(SelectedWeather);
      this.RaisePropertyChanged(nameof(CanRemoveWeather));
    }

    private double GetSunAngle() {
      const int middle = 13;
      const double angleInMin = 3.7;
      const double angleInHour = 16;

      var hours = TimeConditions.Hours;
      var hoursPassed = hours - middle;
      var angle = hoursPassed * angleInHour;
      angle += TimeConditions.Minutes / angleInMin;

      return Math.Round(angle);
    }

    public IniFile SaveConditions(IniFile source) {
      source["SERVER"]["SUN_ANGLE"] = GetSunAngle();
      source["SERVER"]["TIME_OF_DAY_MULT"] = TimeMultiplier;
      if (DynamicTrack) {
        source.Add("DYNAMIC_TRACK", new() {
          ["SESSION_START"] = StartValue,
          ["RANDOMNESS"] = Randomness,
          ["SESSION_TRANSFER"] = Transferred,
          ["LAP_GAIN"] = LapsConditions,
        });
      }

      for (var i = 0; i < Weathers.Count; i++) {
        if (Weathers[i].DataContext is ServerWeatherViewModel weather) {
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
      }

      return source;
    }
  }
}
