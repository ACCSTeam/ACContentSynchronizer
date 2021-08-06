using System.Collections.Generic;
using System.Linq;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerWeatherViewModel : ViewModelBase {
    private double _ambientTemperature = 18;

    private double _ambientVariation = 1;

    private string _graphics = "";

    private double _roadTemperature = 24;

    private double _roadVariation = 0.5;

    private string? _selectedWeather;

    private double _wind;

    private double _windDirection;

    private double _windDirectionVariation;

    public ServerWeatherViewModel() {
      AllowedWeathers = ContentUtils.GetWeathers(Settings.Instance.GamePath);
      SelectedWeather = AllowedWeathers.FirstOrDefault();
    }

    public string? SelectedWeather {
      get => _selectedWeather;
      set => this.RaiseAndSetIfChanged(ref _selectedWeather, value);
    }

    public string Graphics {
      get => _graphics;
      set {
        this.RaiseAndSetIfChanged(ref _graphics, value);
        this.RaisePropertyChanged(nameof(GraphicsLabel));
      }
    }

    public double AmbientTemperature {
      get => _ambientTemperature;
      set {
        this.RaiseAndSetIfChanged(ref _ambientTemperature, value);
        this.RaisePropertyChanged(nameof(AmbientTemperatureLabel));
      }
    }

    public double RoadTemperature {
      get => _roadTemperature;
      set {
        this.RaiseAndSetIfChanged(ref _roadTemperature, value);
        this.RaisePropertyChanged(nameof(RoadTemperatureLabel));
      }
    }

    public double AmbientVariation {
      get => _ambientVariation;
      set {
        this.RaiseAndSetIfChanged(ref _ambientVariation, value);
        this.RaisePropertyChanged(nameof(AmbientVariationLabel));
      }
    }

    public double RoadVariation {
      get => _roadVariation;
      set {
        this.RaiseAndSetIfChanged(ref _roadVariation, value);
        this.RaisePropertyChanged(nameof(RoadVariationLabel));
      }
    }

    public double Wind {
      get => _wind;
      set {
        this.RaiseAndSetIfChanged(ref _wind, value);
        this.RaisePropertyChanged(nameof(WindLabel));
      }
    }

    public string WindLabel => $"Wind: {Wind} km/h";

    public double WindDirection {
      get => _windDirection;
      set {
        this.RaiseAndSetIfChanged(ref _windDirection, value);
        this.RaisePropertyChanged(nameof(WindDirectionLabel));
      }
    }

    public string WindDirectionLabel => $"Wind direction: {WindDirection}";

    public double WindDirectionVariation {
      get => _windDirectionVariation;
      set {
        this.RaiseAndSetIfChanged(ref _windDirectionVariation, value);
        this.RaisePropertyChanged(nameof(WindDirectionVariationLabel));
      }
    }

    public string WindDirectionVariationLabel => $"Wind direction variation: {WindDirectionVariation}";

    public IEnumerable<string> AllowedWeathers { get; set; }

    public string RoadVariationLabel => $"Road variation: {RoadVariation}";
    public string AmbientVariationLabel => $"Temperature variation: {AmbientVariation}";
    public string RoadTemperatureLabel => $"Road temperature: {RoadTemperature}";
    public string AmbientTemperatureLabel => $"Temperature: {AmbientTemperature}";
    public string GraphicsLabel => $"Weather pattern: {Graphics}";
  }
}
