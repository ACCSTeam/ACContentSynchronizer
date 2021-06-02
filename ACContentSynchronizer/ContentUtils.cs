using System.IO;
using System.Linq;
using System.Text.Json;
using ACContentSynchronizer.Models;

namespace ACContentSynchronizer {
  public static class ContentUtils {
    public static readonly JsonSerializerOptions JsonSerializerOptions = new() {
      PropertyNameCaseInsensitive = true,
    };

    public static Manifest GetManifest(string gamePath, string[] carsName, string trackName) {
      var path = Path.Combine(gamePath, Constants.ContentFolder);
      var pathsOfCars = Directory.GetDirectories(Path.Combine(path, Constants.CarsFolder));
      var pathsOfTracks = Directory.GetDirectories(Path.Combine(path, Constants.TracksFolder));

      return new Manifest {
        Cars = pathsOfCars.Where(carPath => carsName.Contains(new DirectoryInfo(carPath).Name))
          .Select(carPath => new EntryManifest(carPath))
          .ToList(),
        Tracks = pathsOfTracks.Where(trackPath => new DirectoryInfo(trackPath).Name == trackName)
          .Select(trackPath => new EntryManifest(trackPath))
          .ToList(),
      };
    }

    public static AvailableContent GetContent(string gamePath, string[] updatableContent) {
      var availableContent = new AvailableContent();
      var path = Path.Combine(gamePath, Constants.ContentFolder);

      var pathsOfCars = Directory.GetDirectories(Path
          .Combine(path, Constants.CarsFolder))
        .Where(updatableContent.Contains);

      var trackPath = Directory.GetDirectories(Path
          .Combine(path, Constants.TracksFolder))
        .FirstOrDefault(updatableContent.Contains);

      foreach (var carPath in pathsOfCars) {
        availableContent.Cars.Add(new EntryInfo(carPath));
      }

      if (!string.IsNullOrEmpty(trackPath)) {
        availableContent.Track = new EntryInfo(trackPath);
      }

      return availableContent;
    }
  }
}
