using System;
using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia.Collections;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerConditionsViewModel : ViewModelBase {
    private bool _dynamicTrack;

    private short _laps = 10;

    private short _randomness = 2;

    private ServerWeather? _selectedWeather;

    private short _startValue = 95;

    private TimeSpan _time = TimeSpan.FromHours(12);

    private double _timeMultiplier = 1;

    private short _transferred = 90;

    public TimeSpan Time {
      get => _time;
      set => this.RaiseAndSetIfChanged(ref _time, value);
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

    public short Laps {
      get => _laps;
      set => this.RaiseAndSetIfChanged(ref _laps, value);
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

    public short TimeMinimum => 8;
    public short TimeMaximum => 18;
    public double TickFrequency => 0.0166666666667;

    public short TimeMultiplierMinimum => 0;
    public short TimeMultiplierMaximum => 60;
    public double TickMultiplierFrequency => 0.1;

    public short LapsMinimum => 1;
    public short LapsMaximum => 81;

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
  }
}
