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

      if (Directory.Exists(zipsPath)) {
        Directory.Delete(zipsPath, true);
      }

      Directory.CreateDirectory(zipsPath);
      Directory.CreateDirectory(Path.Combine(zipsPath, Constants.CarsFolder));
      Directory.CreateDirectory(Path.Combine(zipsPath, Constants.TracksFolder));

      if (File.Exists(Constants.DataFile)) {
        File.Delete(Constants.DataFile);
      }

      foreach (var car in Cars) {
        var carPath = Path.Combine(zipsPath, Constants.CarsFolder, NormalizeName(car.Name));
        DirectoryCopy(car.LocalPath, carPath, true);
      }

      if (Track != null) {
        var trackPath = Path.Combine(zipsPath, Constants.TracksFolder, NormalizeName(Track.Name));
        DirectoryCopy(Track.LocalPath, trackPath, true);
      }

      ZipFile.CreateFromDirectory(zipsPath, Constants.DataFile, CompressionLevel.Optimal, false);

      return File.ReadAllBytesAsync(Constants.DataFile);
    }

    private string NormalizeName(string name) {
      return name.Replace(' ', '_');
    }

    private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs) {
      DirectoryInfo dir = new(sourceDirName);

      if (!dir.Exists) {
        throw new DirectoryNotFoundException(
          "Source directory does not exist or could not be found: "
          + sourceDirName);
      }

      DirectoryInfo[] dirs = dir.GetDirectories();

      Directory.CreateDirectory(destDirName);

      FileInfo[] files = dir.GetFiles();

      foreach (FileInfo file in files) {
        string tempPath = Path.Combine(destDirName, file.Name);
        file.CopyTo(tempPath, false);
      }

      if (copySubDirs) {
        foreach (DirectoryInfo subDir in dirs) {
          string tempPath = Path.Combine(destDirName, subDir.Name);
          DirectoryCopy(subDir.FullName, tempPath, copySubDirs);
        }
      }
    }
  }
}
