using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace ACContentSynchronizer.Models {
  public class AvailableContent {
    public List<EntryInfo> Cars { get; set; } = new();
    public EntryInfo? Track { get; set; }

    public void Pack(string session) {
      var contentFolder = Path.Combine(session, Constants.ContentFolder);
      var contentArchive = Path.Combine(session, Constants.ContentArchive);
      var carsFolder = Path.Combine(contentFolder, Constants.CarsFolder);
      var tracksFolder = Path.Combine(contentFolder, Constants.TracksFolder);

      DirectoryUtils.DeleteIfExists(contentFolder, true);
      Directory.CreateDirectory(carsFolder);
      Directory.CreateDirectory(tracksFolder);
      FileUtils.DeleteIfExists(contentArchive);

      foreach (var car in Cars) {
        var carPath = Path.Combine(carsFolder, car.Name);
        DirectoryUtils.Copy(car.Path, carPath, true);
      }

      if (Track != null) {
        var trackPath = Path.Combine(tracksFolder, Track.Name);
        DirectoryUtils.Copy(Track.Path, trackPath, true);
      }

      ZipFile.CreateFromDirectory(contentFolder, contentArchive, CompressionLevel.Optimal, false);
    }
  }
}
