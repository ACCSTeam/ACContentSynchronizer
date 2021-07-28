using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using ACContentSynchronizer.Extensions;
using ACContentSynchronizer.Models;
using Newtonsoft.Json;

namespace ACContentSynchronizer {
  public static class ContentUtils {
    public static readonly JsonSerializerOptions JsonSerializerOptions = new() {
      PropertyNameCaseInsensitive = true,
    };

    public static Manifest GetManifest(string gamePath, string[] carsName, string trackName) {
      var path = Path.Combine(gamePath, Constants.ContentFolder);
      var pathsOfCars = Directory.GetDirectories(Path.Combine(path, Constants.CarsFolder));
      var pathsOfTracks = Directory.GetDirectories(Path.Combine(path, Constants.TracksFolder));
      var trackPath = pathsOfTracks.FirstOrDefault(x => DirectoryUtils.Name(x) == trackName);

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

      var cars = manifest.Cars.DistinctBy(x => x.Name)
        .Where(entry => EntryNeedUpdate(entry, carsPath));

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
        .FirstOrDefault(dir => DirectoryUtils.Name(dir) == entry.Name);

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
          .Contains(DirectoryUtils.Name(dir)));

      var trackPath = Directory.GetDirectories(Path.Combine(path, Constants.TracksFolder))
        .FirstOrDefault(dir => manifest.Track?.Name == DirectoryUtils.Name(dir));

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
          .Select(DirectoryUtils.Name)
          .ToArray(),
        Directory.GetDirectories(Path
            .Combine(path, Constants.TracksFolder))
          .Select(DirectoryUtils.Name)
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

      using var content = ZipFile.Open(archive, ZipArchiveMode.Read);
      foreach (var entry in content.Entries) {
        var path = Path.Combine(downloads, entry.FullName.Replace(entry.Name, string.Empty));
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
          path = path.Replace('\\', Path.DirectorySeparatorChar);
        }

        DirectoryUtils.CreateIfNotExists(path);
        var destinationFileName = Path.Combine(path, entry.Name);

        using Stream fs = new FileStream(destinationFileName, FileMode.CreateNew, FileAccess.Write, FileShare.None,
          0x1000, false);
        using Stream es = entry.Open();
        es.CopyTo(fs);
        fs.Close();
        es.Close();
        fs.Dispose();
        es.Dispose();
      }

      content.Dispose();
      FileUtils.DeleteIfExists(archive);
    }

    public static void ExecuteCommand(string command) {
      Task.Run(() => {
       ProcessStartInfo processInfo = new("cmd.exe", $"/k \"{command}\"") {
          CreateNoWindow = true,
          UseShellExecute = false,
          RedirectStandardError = true,
          RedirectStandardOutput = true,
          WorkingDirectory = Directory.GetCurrentDirectory(),
        };

        var process = Process.Start(processInfo);

        var output = process?.StandardOutput.ReadToEnd();
        var error = process?.StandardError.ReadToEnd();

        var exitCode = process?.ExitCode;

        Console.WriteLine("output>>" + (string.IsNullOrEmpty(output) ? "(none)" : output));
        Console.WriteLine("error>>" + (string.IsNullOrEmpty(error) ? "(none)" : error));
        Console.WriteLine("ExitCode: " + exitCode, "ExecuteCommand");
        process?.Close();
      });
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
        var carName = DirectoryUtils.Name(car);
        if (string.IsNullOrEmpty(carName)) {
          continue;
        }

        var contentCarPath = Path.Combine(content.CarsPath, carName);
        if (Directory.Exists(car)) {
          MoveContent(car, contentCarPath);
        }
      }

      if (!string.IsNullOrEmpty(downloadedTrack)) {
        var trackName = DirectoryUtils.Name(downloadedTrack);
        var contentTrackPath = Path.Combine(content.TracksPath, trackName);

        if (Directory.Exists(downloadedTrack)) {
          MoveContent(downloadedTrack, contentTrackPath);
        }
      }

      DirectoryUtils.DeleteIfExists(Path.Combine(connectionId, Constants.DownloadsPath), true);
    }

    private static void MoveContent(string entry, string contentPath) {
      DirectoryUtils.DeleteIfExists(contentPath, true);
      DirectoryUtils.Move(entry, contentPath);
    }

    private static IEnumerable<string> GetDownloadedEntries(string entryType, string connectionId) {
      var path = Path.Combine(connectionId, Constants.DownloadsPath, entryType);
      return Directory.Exists(path)
        ? Directory.GetDirectories(path).ToList()
        : new();
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

    public static string GetCarName(string entry, string gamePath) {
      var carDirectory = GetCarDirectory(entry, gamePath);
      if (string.IsNullOrEmpty(carDirectory)) {
        return entry;
      }

      var carDataPath = Path.Combine(carDirectory, Constants.UiFolder, Constants.UiCar);
      if (!File.Exists(carDataPath)) {
        return entry;
      }

      var json = File.ReadAllText(carDataPath);
      var car = JsonConvert.DeserializeObject<CarInfo>(json);
      return car.Name;
    }

    public static List<string> GetCarSkins(string entry, string gamePath) {
      var carDirectory = GetCarDirectory(entry, gamePath);
      if (string.IsNullOrEmpty(carDirectory)
          || !Directory.Exists(carDirectory)) {
        return new();
      }

      var skinsDirectory = Path.Combine(carDirectory, "skins");
      if (!Directory.Exists(skinsDirectory)) {
        return new();
      }

      return Directory.GetDirectories(skinsDirectory)
        .Select(DirectoryUtils.Name)
        .ToList();
    }

    public static string? GetCarDirectory(string entry, string gamePath) {
      var carsFolder = Path.Combine(gamePath, Constants.ContentFolder, Constants.CarsFolder);
      var directories = Directory.GetDirectories(carsFolder);
      return directories.FirstOrDefault(x => DirectoryUtils.Name(x) == entry);
    }

    public static List<TrackName> GetTrackName(string entry, string gamePath) {
      var data = "ui_track.json";
      var variations = GetTrackDirectories(entry, gamePath);

      if (!variations.Any()) {
        return new();
      }

      return variations.Select(x => {
        var variation = Path.Combine(x, data);
        var json = File.ReadAllText(variation);
        var baseName = DirectoryUtils.Name(x);
        if (baseName == Constants.UiFolder) {
          baseName = string.Empty;
        }

        var name = TrackInfo.FromJson(json)?.Name ?? baseName;

        return new TrackName {
          Name = name,
          Variation = baseName,
        };
      }).ToList();
    }

    public static List<string> GetTrackDirectories(string entry, string gamePath) {
      var trackName = entry.Split('-');

      if (trackName.Length > 0) {
        var tracksFolder = Path.Combine(gamePath, Constants.ContentFolder, Constants.TracksFolder);
        var trackDirectory = Path.Combine(tracksFolder, trackName[0]);
        if (Directory.Exists(trackDirectory)) {
          return trackName.Length > 1
            ? new() { Path.Combine(trackDirectory, Constants.UiFolder, trackName[1], Constants.UiTrack) }
            : GetFirst(trackDirectory, Constants.UiFolder, Constants.UiTrack);
        }
      }

      return new();
    }

    private static List<string> GetFirst(string trackDirectory, string ui, string data) {
      var uiPath = Path.Combine(trackDirectory, ui);
      var dataPath = Path.Combine(uiPath, data);
      return File.Exists(dataPath)
        ? new() { uiPath }
        : GetVariations(uiPath);
    }

    private static List<string> GetVariations(string uiPath) {
      try {
        return Directory.GetDirectories(uiPath).ToList();
      } catch {
        return new();
      }
    }
  }
}
