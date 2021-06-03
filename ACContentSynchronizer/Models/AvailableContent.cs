using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace ACContentSynchronizer.Models {
  public class AvailableContent {
    public List<EntryInfo> Cars { get; set; } = new();
    public EntryInfo? Track { get; set; }

    public Task<byte[]> Pack() {
      DirectoryUtils.DeleteIfExists(Constants.ContentFolder, true);
      Directory.CreateDirectory(Constants.ContentFolder);
      Directory.CreateDirectory(Path.Combine(Constants.ContentFolder, Constants.CarsFolder));
      Directory.CreateDirectory(Path.Combine(Constants.ContentFolder, Constants.TracksFolder));
      FileUtils.DeleteIfExists(Constants.ContentArchive);

      foreach (var car in Cars) {
        var carPath = Path.Combine(Constants.ContentFolder, Constants.CarsFolder, car.Name);
        DirectoryUtils.Copy(car.Path, carPath, true);
      }

      if (Track != null) {
        var trackPath = Path.Combine(Constants.ContentFolder, Constants.TracksFolder, Track.Name);
        DirectoryUtils.Copy(Track.Path, trackPath, true);
      }

      ZipFile.CreateFromDirectory(Constants.ContentFolder, Constants.ContentArchive, CompressionLevel.Optimal, false);

      return File.ReadAllBytesAsync(Constants.ContentArchive);
    }
  }
}
