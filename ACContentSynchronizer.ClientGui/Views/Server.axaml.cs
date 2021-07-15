using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;

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

    public ServerEntry GetServer => _vm.ServerEntry;

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }

    public void SetServer(ServerEntry serverEntry) {
      _vm.ServerEntry = serverEntry;
      Task.Run(async () => {
        using var dataReceiver = new DataReceiver(serverEntry.Http);
        var info = await dataReceiver.GetServerInfo();
        if (info != null) {
          _vm.Cars = new(info.Cars.Select(x => new ContentEntry {
            DirectoryName = x,
            Name = ContentUtils.GetCarName(x, Settings.Instance.GamePath),
            Preview = GetCarPreview(x),
          }));

          var trackName = ContentUtils.GetTrackName(info.Track, Settings.Instance.GamePath)
            .FirstOrDefault();

          _vm.Track = new() {
            DirectoryName = info.Track,
            Name = trackName?.Name ?? info.Track,
            Preview = GetTrackPreview(info.Track),
          };
        }
      });
    }

    private Bitmap? GetCarPreview(string entry) {
      var carDirectory = ContentUtils.GetCarDirectory(entry, Settings.Instance.GamePath);
      if (string.IsNullOrEmpty(carDirectory)) {
        return null;
      }

      var carSkinsDirectory = Path.Combine(carDirectory, "skins");
      if (!Directory.Exists(carSkinsDirectory)) {
        return null;
      }

      var skins = Directory.GetDirectories(carSkinsDirectory);
      var rnd = new Random();
      var skin = skins[rnd.Next(0, skins.Length)];
      return new(Path.Combine(skin, "preview.jpg"));
    }

    private Bitmap? GetTrackPreview(string entry) {
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
  }
}
