using System.Collections.Generic;
using System.Linq;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerWeatherViewModel : ViewModelBase {
    private double _ambientTemperature;

    private double _ambientVariation;

    private string _graphics = "";

    private double _roadTemperature;

    private double _roadVariation;

    private string? _selectedWeather;

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

    public IEnumerable<string> AllowedWeathers { get; set; }

    public string RoadVariationLabel => $"Road variation: {RoadVariation}";
    public string AmbientVariationLabel => $"Temperature variation: {AmbientVariation}";
    public string RoadTemperatureLabel => $"Road temperature: {RoadTemperature}";
    public string AmbientTemperatureLabel => $"Temperature: {AmbientTemperature}";
    public string GraphicsLabel => $"Weather pattern: {Graphics}";
  }
}
