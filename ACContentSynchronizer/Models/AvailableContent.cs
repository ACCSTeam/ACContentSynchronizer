using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace ACContentSynchronizer.Models {
  public class AvailableContent {
    public List<CarInfo> Cars { get; set; } = new();
    public TrackInfo? Track { get; set; }

    public Task<byte[]> Pack() {
      const string zipsPath = "archives";
      const string dataPath = "data";

      if (Directory.Exists(zipsPath)) {
        Directory.Delete(zipsPath, true);
      }

      Directory.CreateDirectory(zipsPath);
      Directory.CreateDirectory(Path.Combine(zipsPath, Constants.CarsFolder));
      Directory.CreateDirectory(Path.Combine(zipsPath, Constants.TracksFolder));

      if (File.Exists(dataPath)) {
        File.Delete(dataPath);
      }

      foreach (var car in Cars) {
        var zipPath = Path.Combine(zipsPath, Constants.CarsFolder, car.Name.Replace(' ', '_'));
        ZipFile.CreateFromDirectory(car.LocalPath, zipPath,
          CompressionLevel.Optimal, false);
      }

      if (Track != null) {
        var zipPath = Path.Combine(zipsPath, Constants.TracksFolder, Track.Name.Replace(' ', '_'));
        ZipFile.CreateFromDirectory(Track.LocalPath, zipPath,
          CompressionLevel.Optimal, false);
      }

      ZipFile.CreateFromDirectory(zipsPath, dataPath, CompressionLevel.Optimal, false);

      return File.ReadAllBytesAsync(dataPath);
    }
  }
}
