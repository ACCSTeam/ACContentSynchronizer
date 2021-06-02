using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ACContentSynchronizer.Models;

namespace ACContentSynchronizer {
  public static class ContentUtils {
    public static readonly JsonSerializerOptions JsonSerializerOptions = new() {
      PropertyNameCaseInsensitive = true,
    };

    public static Manifest GetManifest(string gamePath, string trackName) {
      var path = Path.Combine(gamePath, Constants.ContentFolder);
      var pathsOfCars = Directory.GetDirectories(Path.Combine(path, Constants.CarsFolder));
      var pathsOfTracks = Directory.GetDirectories(Path.Combine(path, Constants.TracksFolder));
      var manifest = new Manifest();

      foreach (var carPath in pathsOfCars) {
        manifest.Cars.Add(new EntryManifest(carPath));
      }

      var trackPath = pathsOfTracks.FirstOrDefault(track => track.Contains(trackName));

      if (!string.IsNullOrEmpty(trackPath)) {
        manifest.Tracks.Add(new EntryManifest(trackPath));
      }

      return manifest;
    }

    public static AvailableContent GetContent(string gamePath, string trackName, string[] updatableContent) {
      var availableContent = new AvailableContent();
      var path = Path.Combine(gamePath, Constants.ContentFolder);

      var pathsOfCars = Directory.GetDirectories(Path
          .Combine(path, Constants.CarsFolder))
        .Where(updatableContent.Contains);

      var pathsOfTracks = Directory.GetDirectories(Path
          .Combine(path, Constants.TracksFolder))
        .Where(updatableContent.Contains);

      foreach (var carPath in pathsOfCars) {
        availableContent.Cars.Add(new EntryInfo(carPath));
      }

      var trackPath = pathsOfTracks.FirstOrDefault(track => track.Contains(trackName));

      if (!string.IsNullOrEmpty(trackPath)) {
        availableContent.Track = new EntryInfo(trackPath);
      }

      return availableContent;
    }
  }
}
