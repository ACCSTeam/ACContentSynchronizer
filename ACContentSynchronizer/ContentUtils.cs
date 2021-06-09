using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
      var trackPath = pathsOfTracks.FirstOrDefault(x => new DirectoryInfo(x).Name == trackName);

      var manifest = new Manifest {
        Cars = pathsOfCars.Where(carPath => carsName.Contains(new DirectoryInfo(carPath).Name))
          .Select(carPath => new EntryManifest(carPath, DirectoryUtils.Size(carPath)))
          .ToList(),
      };

      if (!string.IsNullOrEmpty(trackPath)) {
        manifest.Track = new(trackPath, DirectoryUtils.Size(trackPath));
      }

      return manifest;
    }

    public static Manifest CompareContent(string gamePath, Manifest manifest) {
      FileUtils.DeleteIfExists(Constants.ContentArchive);

      var (carsPath, tracksPath) = GetContentHierarchy(gamePath);

      var cars = manifest.Cars.Where(entry => EntryNeedUpdate(entry, carsPath));
      var tracksNeedUpdate = EntryNeedUpdate(manifest.Track, tracksPath);
      var comparedManifest = new Manifest {
        Cars = cars.ToList(),
      };

      if (tracksNeedUpdate) {
        comparedManifest.Track = manifest.Track;
      }

      return comparedManifest;
    }

    private static bool EntryNeedUpdate(EntryManifest? entry, string carsPath) {
      if (entry == null) {
        return false;
      }

      var localEntry = Directory.GetDirectories(carsPath)
        .FirstOrDefault(dir => new DirectoryInfo(dir).Name == entry.Name);

      if (string.IsNullOrEmpty(localEntry)) {
        return true;
      }

      var needUpdate = entry.Size != DirectoryUtils.Size(localEntry);
      return needUpdate;
    }

    public static AvailableContent PrepareContent(string gamePath, Manifest manifest) {
      var availableContent = new AvailableContent();
      var path = Path.Combine(gamePath, Constants.ContentFolder);

      var pathsOfCars = Directory.GetDirectories(Path.Combine(path, Constants.CarsFolder))
        .Where(dir => manifest.Cars
          .Select(x => x.Name)
          .Contains(new DirectoryInfo(dir).Name));

      var trackPath = Directory.GetDirectories(Path.Combine(path, Constants.TracksFolder))
        .FirstOrDefault(dir => manifest.Track?.Name == new DirectoryInfo(dir).Name);

      foreach (var carPath in pathsOfCars) {
        availableContent.Cars.Add(new(carPath));
      }

      if (!string.IsNullOrEmpty(trackPath)) {
        availableContent.Track = new(trackPath);
      }

      return availableContent;
    }

    public static (string[] cars, string[] tracks) GetContentNames(string gamePath) {
      var path = Path.Combine(gamePath, Constants.ContentFolder);

      return (
        Directory.GetDirectories(Path
            .Combine(path, Constants.CarsFolder))
          .Select(dir => new DirectoryInfo(dir).Name)
          .ToArray(),
        Directory.GetDirectories(Path
            .Combine(path, Constants.TracksFolder))
          .Select(dir => new DirectoryInfo(dir).Name)
          .ToArray()
      );
    }

    public static void UnpackContent(string connectionId) {
      var downloads = Path.Combine(connectionId, Constants.DownloadsPath);
      var archive = Path.Combine(connectionId, Constants.ContentArchive);
      DirectoryUtils.DeleteIfExists(downloads, true);

      if (!File.Exists(archive)) {
        return;
      }

      DirectoryUtils.DeleteIfExists(downloads, true);
      Directory.CreateDirectory(downloads);
      ZipFile.ExtractToDirectory(archive, downloads);
      File.Delete(archive);
    }

    public static void ApplyContent(string gamePath, string connectionId) {
      if (!Directory.Exists(gamePath)) {
        return;
      }

      var downloadedCars = GetDownloadedEntries(Constants.CarsFolder, connectionId);
      var downloadedTrack = GetDownloadedEntries(Constants.TracksFolder, connectionId)
        .FirstOrDefault();

      var content = GetContentHierarchy(gamePath);

      foreach (var car in downloadedCars) {
        var carName = new DirectoryInfo(car).Name;
        if (string.IsNullOrEmpty(carName)) {
          continue;
        }

        var contentCarPath = Path.Combine(content.CarsPath, carName);
        MoveContent(car, contentCarPath);
      }

      if (!string.IsNullOrEmpty(downloadedTrack)) {
        var trackName = new DirectoryInfo(downloadedTrack).Name;
        var contentTrackPath = Path.Combine(content.TracksPath, trackName);

        MoveContent(downloadedTrack, contentTrackPath);
      }

      DirectoryUtils.DeleteIfExists(Constants.DownloadsPath, true);
    }

    private static void MoveContent(string entry, string contentPath) {
      DirectoryUtils.DeleteIfExists(contentPath, true);
      DirectoryUtils.Move(entry, contentPath);
    }

    private static IEnumerable<string> GetDownloadedEntries(string entryType, string connectionId) {
      return Directory.GetDirectories(Path.Combine(connectionId, Constants.DownloadsPath, entryType)).ToList();
    }

    private static (string CarsPath, string TracksPath) GetContentHierarchy(string path) {
      var contentPath = Path.Combine(path, Constants.ContentFolder);
      var contentCarsPath = Path.Combine(contentPath, Constants.CarsFolder);
      var contentTracksPath = Path.Combine(contentPath, Constants.TracksFolder);

      DirectoryUtils.CreateIfNotExists(contentPath);
      DirectoryUtils.CreateIfNotExists(contentCarsPath);
      DirectoryUtils.CreateIfNotExists(contentTracksPath);

      return (contentCarsPath, contentTracksPath);
    }
  }
}
