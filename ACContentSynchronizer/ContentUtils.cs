using System;
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

      return new Manifest {
        Cars = pathsOfCars.Where(carPath => carsName.Contains(new DirectoryInfo(carPath).Name))
          .Select(carPath => new EntryManifest(carPath, DirectoryUtils.Size(carPath)))
          .ToList(),
        Tracks = pathsOfTracks.Where(trackPath => new DirectoryInfo(trackPath).Name == trackName)
          .Select(trackPath => new EntryManifest(trackPath, DirectoryUtils.Size(trackPath)))
          .ToList(),
      };
    }

    public static AvailableContent PrepareContent(string gamePath, List<string> updatableContent) {
      var availableContent = new AvailableContent();
      var path = Path.Combine(gamePath, Constants.ContentFolder);

      var pathsOfCars = Directory.GetDirectories(Path
          .Combine(path, Constants.CarsFolder))
        .Where(dir => updatableContent.Contains(new DirectoryInfo(dir).Name));

      var trackPath = Directory.GetDirectories(Path
          .Combine(path, Constants.TracksFolder))
        .FirstOrDefault(dir => updatableContent.Contains(new DirectoryInfo(dir).Name));

      foreach (var carPath in pathsOfCars) {
        availableContent.Cars.Add(new EntryInfo(carPath));
      }

      if (!string.IsNullOrEmpty(trackPath)) {
        availableContent.Track = new EntryInfo(trackPath);
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

    public static void UnpackContent() {
      if (!File.Exists(Constants.ContentArchive)) {
        return;
      }

      DirectoryUtils.DeleteIfExists(Constants.DownloadsPath, true);
      Directory.CreateDirectory(Constants.DownloadsPath);
      ZipFile.ExtractToDirectory(Constants.ContentArchive, Constants.DownloadsPath);
      File.Delete(Constants.ContentArchive);
    }

    public static (List<string> cars, string? track) ApplyContent(string gamePath) {
      if (!Directory.Exists(gamePath)) {
        return (new(), string.Empty);
      }

      var downloadedCars = GetDownloadedEntries(Constants.CarsFolder);
      var downloadedTrack = GetDownloadedEntries(Constants.TracksFolder).FirstOrDefault();

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

      return (downloadedCars
          .Select(dir => new DirectoryInfo(dir).Name)
          .ToList(),
        !string.IsNullOrEmpty(downloadedTrack)
          ? new DirectoryInfo(downloadedTrack).Name
          : null);
    }

    private static void MoveContent(string entry, string contentPath) {
      DirectoryUtils.DeleteIfExists(contentPath, true);
      if (Path.GetPathRoot(entry) == Path.GetPathRoot(contentPath)) {
        Directory.Move(entry, contentPath);
      } else {
        DirectoryUtils.Copy(entry, contentPath, true);
        DirectoryUtils.DeleteIfExists(entry, true);
      }
    }

    private static List<string> GetDownloadedEntries(string entryType) {
      return Directory.GetDirectories(Path.Combine(Constants.DownloadsPath, entryType)).ToList();
    }

    public static (string Path,
      string CarsPath,
      string TracksPath) GetContentHierarchy(string path) {
      var contentPath = Path.Combine(path, Constants.ContentFolder);
      var contentCarsPath = Path.Combine(contentPath, Constants.CarsFolder);
      var contentTracksPath = Path.Combine(contentPath, Constants.TracksFolder);

      DirectoryUtils.CreateIfNotExists(contentPath);
      DirectoryUtils.CreateIfNotExists(contentCarsPath);
      DirectoryUtils.CreateIfNotExists(contentTracksPath);

      return (contentPath, contentCarsPath, contentTracksPath);
    }
  }
}
