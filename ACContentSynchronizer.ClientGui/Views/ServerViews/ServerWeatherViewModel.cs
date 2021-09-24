using System.Collections.Generic;
using System.Linq;
using ACContentSynchronizer.ClientGui.ViewModels;
using ReactiveUI;
using Splat;

namespace ACContentSynchronizer.ClientGui.Views.ServerViews {
  public class ServerWeatherViewModel : ViewModelBase {
    private readonly ApplicationViewModel _application;
    private readonly ContentService _contentService;

    private double _ambientTemperature = 18;

    private double _ambientVariation = 1;

    private string _graphics = "";

    private double _roadTemperature = 24;

    private double _roadVariation = 0.5;

    private string? _selectedWeather;

    private short _windDirection;

    private short _windDirectionVariation;

    private double _windMax;

    private double _windMin;

    public ServerWeatherViewModel() {
      _contentService = Locator.Current.GetService<ContentService>();
      _application = Locator.Current.GetService<ApplicationViewModel>();
      AllowedWeathers = _contentService.GetWeathers(_application.Settings.GamePath);
      SelectedWeather = AllowedWeathers.FirstOrDefault();
    }

    public string? SelectedWeather {
      get => _selectedWeather;
      set => this.RaiseAndSetIfChanged(ref _selectedWeather, value);
    }

    public string Graphics {
      get => _graphics;
      set => this.RaiseAndSetIfChanged(ref _graphics, value);
    }

    public double AmbientTemperature {
      get => _ambientTemperature;
      set => this.RaiseAndSetIfChanged(ref _ambientTemperature, value);
    }

    public double RoadTemperature {
      get => _roadTemperature;
      set => this.RaiseAndSetIfChanged(ref _roadTemperature, value);
    }

    public double AmbientVariation {
      get => _ambientVariation;
      set => this.RaiseAndSetIfChanged(ref _ambientVariation, value);
    }

    public double RoadVariation {
      get => _roadVariation;
      set => this.RaiseAndSetIfChanged(ref _roadVariation, value);
    }

    public double WindMin {
      get => _windMin;
      set => this.RaiseAndSetIfChanged(ref _windMin, value);
    }

    public double WindMax {
      get => _windMax;
      set => this.RaiseAndSetIfChanged(ref _windMax, value);
    }

    public short WindDirection {
      get => _windDirection;
      set => this.RaiseAndSetIfChanged(ref _windDirection, value);
    }

    public short WindDirectionVariation {
      get => _windDirectionVariation;
      set => this.RaiseAndSetIfChanged(ref _windDirectionVariation, value);
    }

    public IEnumerable<string> AllowedWeathers { get; set; }

    public static short WindDirectionMin => 0;
    public static short WindDirectionMax => 359;
    public static short WindSpeedMin => 0;
    public static short WindSpeedMax => 40;
    public static short TemperatureMin => 0;
    public static short TemperatureMax => 36;
    public static short WindDirectionVariationMin => 0;
    public static short WindDirectionVariationMax => 90;
  }
}
