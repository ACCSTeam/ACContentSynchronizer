using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Tasks;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.Models;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views {
  public class ServerViewModel : ViewModelBase, IDisposable {
    private AvaloniaList<ContentEntry> _cars = new();

    private ContentEntry? _selectedCar;

    private ServerEntry _serverEntry = new();

    private ContentEntry _track = new();

    public ServerEntry ServerEntry {
      get => _serverEntry;
      set => this.RaiseAndSetIfChanged(ref _serverEntry, value);
    }

    public AvaloniaList<ContentEntry> Cars {
      get => _cars;
      set => this.RaiseAndSetIfChanged(ref _cars, value);
    }

    public ContentEntry? SelectedCar {
      get {
        if (_selectedCar != null) {
          return _selectedCar;
        }

        Application.Current.TryFindResource("CarPlaceholder", out var placeholder);

        if (placeholder is Image { Source: Bitmap bitmap }) {
          return new() {
            Preview = bitmap,
          };
        }

        return null;
      }
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
      using var kunosClient = new KunosClient(ServerEntry.Ip, ServerEntry.HttpPort);
      return kunosClient.Booking(value.DirectoryName, value.Variation ?? "", "fEst", "EE",
        Settings.Instance.SteamId.ToString(), ServerEntry.Password);
    }

    public async Task Join() {
      var car = SelectedCar;
      await ValidateContent();
      var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      var cfgPath = Path.Combine(documents, "Assetto Corsa", "cfg");
      var iniProvider = new IniProvider(cfgPath);
      var config = iniProvider.GetConfig("race.ini");

      if (car != null && !string.IsNullOrEmpty(car.Variation)) {
        var remote = config["REMOTE"];
        remote["ACTIVE"] = "1";
        remote["SERVER_IP"] = ServerEntry.Ip;
        remote["SERVER_PORT"] = "9600";
        remote["SERVER_NAME"] = ServerEntry.Name;
        remote["SERVER_HTTP_PORT"] = ServerEntry.HttpPort;
        remote["REQUESTED_CAR"] = car.DirectoryName;
        remote["GUID"] = Settings.Instance.SteamId.ToString();
        remote["__CM_EXTENDED"] = "1";
        remote["NAME"] = Settings.Instance.PlayerName;
        remote["PASSWORD"] = ServerEntry.Password;
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
      var selectedCar = SelectedCar?.DirectoryName;
      Cars = new();
      Track = new();

      var kunosClient = new KunosClient(ServerEntry.Ip, ServerEntry.HttpPort);
      var cars = await kunosClient.GetCars(Settings.Instance.SteamId);

      if (cars != null) {
        Cars = new(cars.Cars.GroupBy(x => x.Model)
          .Select(x => new ContentEntry {
            DirectoryName = x.Key,
            Name = ContentUtils.GetCarName(x.Key, Settings.Instance.GamePath) ?? x.Key,
            Preview = GetCarPreview(x.Key, x.First().Skin),
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
          Preview = GetTrackPreview(name),
        };

        await UpdateCars();
      }
    }

    public async Task ValidateContent() {
      var validationTask = new ValidationTask(ServerEntry);
      StatusBar.Instance.AddTask(validationTask);
      await validationTask.Worker;

      await Refresh();
    }

    private static Bitmap? GetCarPreview(string entry, string? skinName = null) {
      var carDirectory = ContentUtils.GetCarDirectory(entry, Settings.Instance.GamePath);
      if (string.IsNullOrEmpty(carDirectory)) {
        return null;
      }
      var carSkinsDirectory = Path.Combine(carDirectory, "skins");
      if (!Directory.Exists(carSkinsDirectory)) {
        return null;
      }

      if (!string.IsNullOrEmpty(skinName)) {
        return new(Path.Combine(carSkinsDirectory, skinName, "preview.jpg"));
      }

      var skins = Directory.GetDirectories(carSkinsDirectory);
      var rnd = new Random();
      var skin = skins[rnd.Next(0, skins.Length)];
      return new
        (Path.Combine(skin, "preview.jpg"));
    }

    private static Bitmap? GetTrackPreview(string entry) {
      var trackDirectory = ContentUtils.GetTrackDirectories(entry, Settings.Instance.GamePath)
        .FirstOrDefault();
      if (string.IsNullOrEmpty(trackDirectory)) {
        return null;
      }
      var trackPreview = Path.Combine(trackDirectory, "preview.png");
      if (!File.Exists(trackPreview)) {
        return null;
      }

      return new(trackPreview);
    }

    public async Task UpdateCars() {
      var kunosClient = new KunosClient(ServerEntry.Ip, ServerEntry.HttpPort);
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
