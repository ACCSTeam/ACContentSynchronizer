using System.IO;
using System.Linq;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.Models;
using Avalonia.Collections;

namespace ACContentSynchronizer.ClientGui.Modals {
  public class UploadViewModel : ViewModelBase {
    public UploadViewModel() {
      var settings = Settings.Instance;
      var carsDirectory = Path.Combine(settings.GamePath, Constants.ContentFolder, Constants.CarsFolder);
      var tracksDirectory = Path.Combine(settings.GamePath, Constants.ContentFolder, Constants.TracksFolder);

      if (Directory.Exists(carsDirectory)) {
        Cars.AddRange(Directory.GetDirectories(carsDirectory)
          .Select(x => new EntryInfo(x,
            ContentUtils.GetCarName(x, settings.GamePath),
            ContentUtils.GetCarSkins(x, settings.GamePath))));
      }

      if (Directory.Exists(tracksDirectory)) {
        var tracks = Directory.GetDirectories(tracksDirectory)
          .SelectMany(x => ContentUtils.GetTrackName(x, settings.GamePath)
            .Select(v => new EntryInfo(v, v, new() { v })));

        Tracks.AddRange(tracks);
      }
    }

    public AvaloniaList<EntryInfo> Cars { get; set; } = new();

    public AvaloniaList<EntryInfo> Tracks { get; set; } = new();

    public AvaloniaList<EntryInfo> SelectedCars { get; set; } = new();

    public EntryInfo? SelectedTrack { get; set; }
  }
}
