using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ACContentSynchronizer.Client;
using ACContentSynchronizer.Client.Models;
using ACContentSynchronizer.Models;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Views {
  public class Server : UserControl {
    private static Server? _instance;
    private readonly ServerViewModel _vm;

    public Server() {
      DataContext = _vm = new();
      InitializeComponent();
      _instance = this;
    }

    public static Server Instance => _instance ??= new();

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void SetServer(ServerEntry serverEntry) {
      _vm.ServerEntry = serverEntry;
      Task.Run(async () => {
        using var dataReceiver = new DataReceiver(serverEntry.Http);
        var info = await dataReceiver.GetServerInfo();
        if (info != null) {
          _vm.Cars = info.Cars.Select(x => new Entry {
            DirectoryName = x,
            Name = GetCarName(x),
          }).ToList();

          _vm.Track = new() {
            DirectoryName = info.Track,
            Name = GetTrackName(info.Track),
          };
        }
      });
    }

    private string GetCarName(string entry) {
      var settings = Settings.Instance;
      var carsFolder = Path.Combine(settings.GamePath, Constants.ContentFolder, Constants.CarsFolder);
      var carFolder = Directory.GetDirectories(carsFolder).FirstOrDefault(x => x == entry);
      if (string.IsNullOrEmpty(carFolder)) {
        return entry;
      }

      var carDataPath = Path.Combine(carFolder, "ui", "ui_car.json");
      if (!File.Exists(carDataPath)) {
        return entry;
      }

      var json = File.ReadAllText(carDataPath);
      var car = JsonSerializer.Deserialize<CarInfo>(json, ContentUtils.JsonSerializerOptions);
      return car != null
        ? car.Name
        : entry;
    }

    private string GetTrackName(string entry) {
      var settings = Settings.Instance;
      var tracksFolder = Path.Combine(settings.GamePath, Constants.ContentFolder, Constants.TracksFolder);
      var directories = Directory.GetDirectories(tracksFolder, entry, SearchOption.AllDirectories);
      if (directories.Any()) {
        var trackDataPath = Path.Combine(directories[0], "ui", "ui_track.json");
        if (!File.Exists(trackDataPath)) {
          return entry;
        }

        var json = File.ReadAllText(trackDataPath);
        var car = JsonSerializer.Deserialize<CarInfo>(json, ContentUtils.JsonSerializerOptions);
        return car != null
          ? car.Name
          : entry;
      }

      return entry;
    }
  }
}
