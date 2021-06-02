using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ACContentSynchronizer.Models;

namespace ACContentSynchronizer.Client {
  public class DataReceiver {
    private const string DownloadsPath = "downloads";
    private Manifest _manifest = new();
    private readonly HttpClient _client;

    public DataReceiver(string serverAddress) {
      if (string.IsNullOrEmpty(serverAddress)) {
        throw new NullReferenceException(nameof(serverAddress));
      }

      _client = new HttpClient {
        BaseAddress = new Uri(serverAddress),
      };
    }

    public async Task DownloadManifest() {
      var json = await _client.GetStringAsync("getManifest");
      _manifest = JsonSerializer.Deserialize<Manifest>(json, ContentUtils.JsonSerializerOptions);
    }

    public List<string> CompareContent(string gamePath) {
      if (File.Exists(Constants.ContentArchive)) {
        File.Delete(Constants.ContentArchive);
      }

      var content = CheckContentHierarchy(gamePath);

      var cars = _manifest.Cars.Where(entry => EntryNeedUpdate(entry, content.CarsPath));
      var tracks = _manifest.Tracks.Where(entry => EntryNeedUpdate(entry, content.TracksPath));
      var updatableEntries = cars.Union(tracks).Select(entry => entry.Path);

      return updatableEntries.ToList();
    }

    public async Task DownloadData(List<string> updatableEntries) {
      var content = JsonContent.Create(updatableEntries);
      var response = await _client.PostAsync("getContent", content);
      var data = await response.Content.ReadAsByteArrayAsync();

      if (!File.Exists(Constants.ContentArchive)) {
        await File.Create(Constants.ContentArchive).DisposeAsync();
      }

      await File.WriteAllBytesAsync(Constants.ContentArchive, data);
    }

    public void SaveData() {
      if (!File.Exists(Constants.ContentArchive)) {
        return;
      }

      if (Directory.Exists(DownloadsPath)) {
        Directory.Delete(DownloadsPath, true);
      }

      Directory.CreateDirectory(DownloadsPath);
      ZipFile.ExtractToDirectory(Constants.ContentArchive, DownloadsPath);
      File.Delete(Constants.ContentArchive);
    }

    public void Apply(string gamePath) {
      if (!Directory.Exists(gamePath)) {
        return;
      }

      var downloadedCars = Directory.GetDirectories(Path.Combine(DownloadsPath, Constants.CarsFolder));
      var downloadedTracks = Directory.GetDirectories(Path.Combine(DownloadsPath, Constants.TracksFolder));

      var content = CheckContentHierarchy(gamePath);

      foreach (var car in downloadedCars) {
        var carName = new DirectoryInfo(car).Name;

        if (string.IsNullOrEmpty(carName)) {
          continue;
        }

        var contentCarPath = Path.Combine(content.CarsPath, carName);

        DirectoryUtils.DeleteIfExists(contentCarPath, true);
        Directory.Move(car, contentCarPath);
      }

      foreach (var track in downloadedTracks) {
        var trackName = new DirectoryInfo(track).Name;

        if (string.IsNullOrEmpty(trackName)) {
          continue;
        }

        var contentTrackPath = Path.Combine(content.TracksPath, trackName);

        DirectoryUtils.DeleteIfExists(contentTrackPath, true);
        Directory.Move(track, contentTrackPath);
      }

      DirectoryUtils.DeleteIfExists(DownloadsPath, true);
    }

    private (string Path,
      string CarsPath,
      string TracksPath) CheckContentHierarchy(string path) {
      var contentPath = Path.Combine(path, Constants.ContentFolder);
      var contentCarsPath = Path.Combine(contentPath, Constants.CarsFolder);
      var contentTracksPath = Path.Combine(contentPath, Constants.TracksFolder);

      DirectoryUtils.CreateIfNotExists(contentPath);
      DirectoryUtils.CreateIfNotExists(contentCarsPath);
      DirectoryUtils.CreateIfNotExists(contentTracksPath);

      return (contentPath, contentCarsPath, contentTracksPath);
    }

    private bool EntryNeedUpdate(EntryManifest entry, string carsPath) {
      var localEntry = Directory.GetDirectories(carsPath)
        .FirstOrDefault(dir => new DirectoryInfo(dir).Name == entry.Name);

      if (string.IsNullOrEmpty(localEntry)) {
        return true;
      }

      var needUpdate = entry.Size != DirectoryUtils.Size(localEntry);
      return needUpdate;
    }
  }
}
