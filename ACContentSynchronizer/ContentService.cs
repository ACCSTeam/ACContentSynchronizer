using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ACContentSynchronizer.Extensions;
using ACContentSynchronizer.Models;
using Newtonsoft.Json;

namespace ACContentSynchronizer {
  public class ContentService {
    public Manifest GetManifest(string gamePath, string[] carsName, string trackName) {
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

    public Manifest CompareContent(string gamePath, Manifest manifest) {
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

    private bool EntryNeedUpdate(EntryManifest? entry, string carsPath) {
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

    public AvailableContent PrepareContent(string gamePath, Manifest manifest) {
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

    public void UnpackContent(string connectionId) {
      var downloads = Path.Combine(connectionId, Constants.DownloadsPath);
      var archive = Path.Combine(connectionId, Constants.ContentArchive);
      DirectoryUtils.DeleteIfExists(downloads);

      if (!File.Exists(archive)) {
        return;
      }

      DirectoryUtils.DeleteIfExists(downloads);
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

    public void ExecuteCommand(string command) {
      Task.Run(async () => {
        ProcessStartInfo processInfo = new("cmd.exe", $"/k \"{command}\"") {
          CreateNoWindow = true,
          UseShellExecute = false,
          RedirectStandardError = true,
          RedirectStandardOutput = true,
          WorkingDirectory = Directory.GetCurrentDirectory(),
        };

        var process = Process.Start(processInfo);
        await Task.Delay(TimeSpan.FromSeconds(15));
        process?.Close();
      });
    }

    public void ApplyContent(string gamePath, string connectionId) {
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

      DirectoryUtils.DeleteIfExists(Path.Combine(connectionId, Constants.DownloadsPath));
    }

    private void MoveContent(string entry, string contentPath) {
      DirectoryUtils.DeleteIfExists(contentPath);
      DirectoryUtils.Move(entry, contentPath);
    }

    private IEnumerable<string> GetDownloadedEntries(string entryType, string connectionId) {
      var path = Path.Combine(connectionId, Constants.DownloadsPath, entryType);
      return Directory.Exists(path)
        ? Directory.GetDirectories(path).ToList()
        : new();
    }

    private (string CarsPath, string TracksPath) GetContentHierarchy(string path) {
      var contentPath = Path.Combine(path, Constants.ContentFolder);
      var contentCarsPath = Path.Combine(contentPath, Constants.CarsFolder);
      var contentTracksPath = Path.Combine(contentPath, Constants.TracksFolder);

      DirectoryUtils.CreateIfNotExists(contentPath);
      DirectoryUtils.CreateIfNotExists(contentCarsPath);
      DirectoryUtils.CreateIfNotExists(contentTracksPath);

      return (contentCarsPath, contentTracksPath);
    }

    public string? GetCarName(string entry, string gamePath) {
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
      return car?.Name;
    }

    public List<string> GetCarSkins(string entry, string gamePath) {
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

    public string? GetCarDirectory(string entry, string gamePath) {
      var carsFolder = Path.Combine(gamePath, Constants.ContentFolder, Constants.CarsFolder);
      if (!Directory.Exists(carsFolder)) {
        return entry;
      }
      var directories = Directory.GetDirectories(carsFolder);
      return directories.FirstOrDefault(x => DirectoryUtils.Name(x) == entry);
    }

    public List<TrackName> GetTrackName(string entry, string gamePath) {
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

    public List<string> GetTrackDirectories(string entry, string gamePath) {
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

    private List<string> GetFirst(string trackDirectory, string ui, string data) {
      var uiPath = Path.Combine(trackDirectory, ui);
      var dataPath = Path.Combine(uiPath, data);
      return File.Exists(dataPath)
        ? new() { uiPath }
        : GetVariations(uiPath);
    }

    private List<string> GetVariations(string uiPath) {
      try {
        return Directory.GetDirectories(uiPath).ToList();
      } catch {
        return new();
      }
    }

    public IEnumerable<string> GetWeathers(string gamePath) {
      var path = Path.Combine(gamePath, Constants.ContentFolder, Constants.WeatherFolder);
      if (!Directory.Exists(path)) {
        return Array.Empty<string>();
      }

      var weathers = Directory.GetDirectories(path)
        .Select(x => {
          var iniProvider = new IniProvider(x);
          var config = iniProvider.GetConfig(Constants.WeatherFolder);
          return config.V("LAUNCHER", "NAME", DirectoryUtils.Name(x));
          ;
        });

      return weathers;
    }

    public string ContentSize(long length) {
      string[] sizes = { "B", "KB", "MB", "GB", "TB" };
      var order = 0;
      while (length >= 1024 && order < sizes.Length - 1) {
        order++;
        length /= 1024;
      }

      return $"{length:0.##} {sizes[order]}";
    }
  }
}
