using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Tasks;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.ClientGui.Views;
using ACContentSynchronizer.Models;
using Avalonia.Collections;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class RaceViewModel : ViewModelBase {
    private AvaloniaList<ContentEntry> _cars = new();

    private ContentEntry? _selectedCar;

    private ContentEntry _track = new();

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
    }

    private Task Booking(ContentEntry value) {
      var server = Views.Server.GetServer;
      using var kunosClient = new KunosClient(server.Ip, server.HttpPort);
      return kunosClient.Booking(value.DirectoryName, value.Variation ?? "", "fEst", "EE",
        Settings.Instance.SteamId.ToString(), server.Password);
    }

    public async Task Join() {
      var car = SelectedCar;
      await ValidateContent();
      var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      var cfgPath = Path.Combine(documents, "Assetto Corsa", "cfg");
      var iniProvider = new IniProvider(cfgPath);
      var config = iniProvider.GetConfig("race.ini");
      var server = Views.Server.GetServer;

      if (car != null && !string.IsNullOrEmpty(car.Variation)) {
        var remote = config["REMOTE"];
        remote["ACTIVE"] = "1";
        remote["SERVER_IP"] = server.Ip;
        remote["SERVER_PORT"] = "9600";
        remote["SERVER_NAME"] = server.Name;
        remote["SERVER_HTTP_PORT"] = server.HttpPort;
        remote["REQUESTED_CAR"] = car.DirectoryName;
        remote["GUID"] = Settings.Instance.SteamId.ToString();
        remote["__CM_EXTENDED"] = "1";
        remote["NAME"] = Settings.Instance.PlayerName;
        remote["PASSWORD"] = server.Password;
        config["REMOTE"] = remote;

        var race = config["RACE"];
        race["TRACK"] = Track.DirectoryName;
        race["CONFIG_TRACK"] = Track.Variation ?? "";
        config["RACE"] = race;

        await iniProvider.SaveConfig("race.ini", config);
        var gamePath = Path.Combine(Settings.Instance.GamePath, "acs.exe");
        var process = new Process {
          StartInfo = {
            FileName = gamePath,
            WorkingDirectory = Settings.Instance.GamePath,
          },
        };

        process.Start();

        ReactiveCommand.CreateFromTask(() => Booking(car)).Execute();
      }
    }

    public async Task Refresh() {
      var server = Views.Server.GetServer;
      var selectedCar = SelectedCar?.DirectoryName;

      Cars = new();
      Track = new();

      var kunosClient = new KunosClient(server.Ip, server.HttpPort);
      var cars = await kunosClient.GetCars(Settings.Instance.SteamId);

      if (cars != null) {
        Cars = new(cars.Cars.GroupBy(x => x.Model)
          .Select(x => new ContentEntry {
            DirectoryName = x.Key,
            Name = ContentUtils.GetCarName(x.Key, Settings.Instance.GamePath) ?? x.Key,
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

        var trackNames = ContentUtils.GetTrackName(name, Settings.Instance.GamePath);
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
      var validationTask = new ValidationTask(Views.Server.GetServer);
      StatusBar.Instance.AddTask(validationTask);
      await validationTask.Worker;

      await Refresh();
    }

    public async Task UpdateCars() {
      var server = Views.Server.GetServer;
      var kunosClient = new KunosClient(server.Ip, server.HttpPort);
      var cars = await kunosClient.GetCars(Settings.Instance.SteamId);
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
