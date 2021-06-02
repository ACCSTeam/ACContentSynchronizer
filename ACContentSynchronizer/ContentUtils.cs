using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ACContentSynchronizer.Models;

namespace ACContentSynchronizer {
  public static class ContentUtils {
    public static async Task<AvailableContent> GetContent(string gamePath, string trackName) {
      var path = Path.Combine(gamePath, Constants.ContentFolder);
      var pathsOfCars = Directory.GetDirectories(Path.Combine(path, Constants.CarsFolder));
      var pathsOfTracks = Directory.GetDirectories(Path.Combine(path, Constants.TracksFolder));
      var availableContent = new AvailableContent();

      var options = new JsonSerializerOptions {
        PropertyNameCaseInsensitive = true,
      };

      foreach (var carPath in pathsOfCars) {
        var carInfoPath = Path.Combine(carPath, "ui", "ui_car.json");

        if (!File.Exists(carInfoPath)) {
          continue;
        }

        var json = await File.ReadAllTextAsync(carInfoPath);
        var carInfo = JsonSerializer.Deserialize<CarInfo>(json, options);

        if (carInfo == null) {
          continue;
        }

        carInfo.LocalPath = carPath;
        availableContent.Cars.Add(carInfo);
      }

      var trackPath = pathsOfTracks.FirstOrDefault(track => track.Contains(trackName));

      if (!string.IsNullOrEmpty(trackPath)) {
        var trackInfoPath = Path.Combine(trackPath, "ui", "ui_track.json");
        var trackInfo = new TrackInfo { Name = trackPath.Split('/').LastOrDefault() ?? "" };

        if (File.Exists(trackInfoPath)) {
          var json = await File.ReadAllTextAsync(trackInfoPath);
          trackInfo = JsonSerializer.Deserialize<TrackInfo>(json, options);
        }

        if (trackInfo != null) {
          trackInfo.LocalPath = trackPath;
          availableContent.Track = trackInfo;
        }
      }

      return availableContent;
    }
  }
}
