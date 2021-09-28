using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Components;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Services;
using ACContentSynchronizer.ClientGui.Tasks;
using ACContentSynchronizer.Models;
using Avalonia.Collections;
using ReactiveUI;
using Splat;

namespace ACContentSynchronizer.ClientGui.ViewModels {
  public class RaceViewModel : ViewModelBase, IDisposable {
    private ApplicationViewModel _application;
    private ContentService _contentService;
    private ServerEntryViewModel _server;
    private SettingsViewModel _settings;
    private readonly HubService _hubService;

    private AvaloniaList<ContentEntry> _cars = new();

    private ContentEntry? _selectedCar;

    private ContentEntry _track = new();

    public RaceViewModel(ServerEntryViewModel serverEntry,
                         HubService hubService) {
      _application = Locator.Current.GetService<ApplicationViewModel>();
      _contentService = Locator.Current.GetService<ContentService>();
      _settings = _application.Settings;
      _server = serverEntry;
      _hubService = hubService;

      Task.Run(Refresh);
    }

    public AvaloniaList<ContentEntry> Cars {
      get => _cars;
      set => this.RaiseAndSetIfChanged(ref _cars, value);
    }

    public ContentEntry? SelectedCar {
      get => _selectedCar;
      set => this.RaiseAndSetIfChanged(ref _selectedCar, value);
    }

    public ContentEntry Track {
      get => _track;
      set => this.RaiseAndSetIfChanged(ref _track, value);
    }

    public void Dispose() {
      _selectedCar?.Dispose();
      _track.Dispose();
      foreach (var car in _cars) {
        car.Dispose();
      }
    }

    private Task Booking(ContentEntry value) {
      using var kunosClient = new KunosClient(_server.Ip, _server.KunosPort);
      return kunosClient.Booking(value.DirectoryName, value.Variation ?? "", "fEst", "EE",
        _settings.SteamId, _server.Password);
    }

    public async Task Join() {
      var car = SelectedCar;
      await ValidateContent();
      var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      var cfgPath = Path.Combine(documents, "Assetto Corsa", "cfg");
      var iniProvider = new IniProvider(cfgPath);
      var config = iniProvider.GetConfig("race");

      if (car != null && !string.IsNullOrEmpty(car.Variation)) {
        var remote = config["REMOTE"];
        remote["ACTIVE"] = "1";
        remote["SERVER_IP"] = _server.Ip;
        remote["SERVER_PORT"] = "9600";
        remote["SERVER_NAME"] = _server.Name;
        remote["SERVER_HTTP_PORT"] = _server.KunosPort;
        remote["REQUESTED_CAR"] = car.DirectoryName;
        remote["GUID"] = _settings.SteamId;
        remote["__CM_EXTENDED"] = "1";
        remote["NAME"] = _settings.PlayerName;
        remote["PASSWORD"] = _server.Password;
        config["REMOTE"] = remote;

        var race = config["RACE"];
        race["TRACK"] = Track.DirectoryName;
        race["CONFIG_TRACK"] = Track.Variation ?? "";
        config["RACE"] = race;

        await iniProvider.SaveConfig("race", config);
        var gamePath = Path.Combine(_settings.GamePath, "acs.exe");
        var process = new Process {
          StartInfo = {
            FileName = gamePath,
            WorkingDirectory = _settings.GamePath,
          },
        };

        process.Start();

        ReactiveCommand.CreateFromTask(() => Booking(car)).Execute();
      }
    }

    public async Task Refresh() {
      var selectedCar = SelectedCar?.DirectoryName;

      Cars = new();
      Track = new();

      var kunosClient = new KunosClient(_server.Ip, _server.KunosPort);
      var cars = await kunosClient.GetCars(_settings.SteamId);

      if (cars != null) {
        Cars = new(cars.Cars.GroupBy(x => x.Model)
          .Select(x => new ContentEntry {
            DirectoryName = x.Key,
            Name = _contentService.GetCarName(x.Key, _settings.GamePath) ?? x.Key,
            Preview = Preview.GetCarPreview(x.Key, x.First().Skin),
            Variations = new(x.Select(s => new EntryVariation {
              Variation = s.Skin,
              IsConnected = s.IsConnected,
            })),
          }));

        SelectedCar = !string.IsNullOrEmpty(selectedCar)
          ? Cars.FirstOrDefault(x => x.DirectoryName == selectedCar)
          : Cars.FirstOrDefault();
      }

      var info = await kunosClient.GetServerInfo();
      if (info != null) {
        var index = info.Track.IndexOf('-');
        var (name, variation) = index == -1
          ? (info.Track, string.Empty)
          : (info.Track[..index], info.Track[(index + 1)..]);

        var trackNames = _contentService.GetTrackName(name, _settings.GamePath);
        Track = new() {
          DirectoryName = name,
          Variations = new() {
            new() {
              Variation = variation,
            },
          },
          Name = (string.IsNullOrEmpty(variation)
              ? trackNames.FirstOrDefault()?.Name
              : trackNames.FirstOrDefault(x => x.Variation == variation)?.Name
            ) ?? name,
          Preview = Preview.GetTrackPreview(name),
        };

        await UpdateCars();
      }
    }

    public async Task ValidateContent() {
      var validationTask = new ValidationTask(_server, _hubService);
      _application.AddTask(validationTask);
      await validationTask.Worker;
      await Refresh();
    }

    public async Task UpdateCars() {
      var kunosClient = new KunosClient(_server.Ip, _server.KunosPort);
      var cars = await kunosClient.GetCars(_settings.SteamId);
      if (cars != null) {
        var grouped = cars.Cars.GroupBy(x => x.Model);

        foreach (var update in grouped) {
          var car = Cars.FirstOrDefault(x => x.DirectoryName == update.Key);

          if (car == null) {
            continue;
          }

          car.Variations = new(update.Select(x => new EntryVariation {
            Variation = x.Skin,
            IsConnected = x.IsConnected,
          }));
        }
      }
    }
  }
}
