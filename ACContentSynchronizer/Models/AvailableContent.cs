using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace ACContentSynchronizer.Models {
  public class AvailableContent {
    public delegate void ProgressEvent(double progress, string entry);

    public List<EntryInfo> Cars { get; set; } = new();
    public EntryInfo? Track { get; set; }

    public event ProgressEvent? OnProgress;

    public async Task Pack(string session) {
      try {
        var contentArchive = Path.Combine(session, Constants.ContentArchive);

        DirectoryUtils.DeleteIfExists(session, true);
        Directory.CreateDirectory(session);
        FileUtils.DeleteIfExists(contentArchive);
        await FileUtils.CreateIfNotExistsAsync(contentArchive);
        var entriesPassed = 0;

        foreach (var car in Cars) {
          await AddToArchive(contentArchive, car.Path, Constants.CarsFolder);
          entriesPassed = CalculateProgress(entriesPassed, car.Name);
        }

        if (Track != null) {
          await AddToArchive(contentArchive, Track.Path, Constants.TracksFolder);
          CalculateProgress(entriesPassed, Track.Name);
        }
      } finally {
        GC.Collect();
      }
    }

    private int CalculateProgress(int entriesPassed, string entry) {
      var entriesCount = Cars.Count + (Track != null ? 1 : 0);
      entriesPassed++;
      var progress = (double) entriesPassed / entriesCount * 100;
      OnProgress?.Invoke(progress, entry);
      return entriesPassed;
    }

    private async Task AddToArchive(string contentArchive, string path, string type) {
      await using FileStream zipFile = File.Open(contentArchive, FileMode.Open);
      using var archive = new ZipArchive(zipFile, ZipArchiveMode.Update);

      var root = new DirectoryInfo(path);
      var p = root.Parent?.FullName ?? "";
      var directories = root.GetDirectories("*.*", SearchOption.AllDirectories);

      foreach (var directory in directories) {
        await AddFromDirectory(archive, directory, p, type);
      }

      await AddFromDirectory(archive, root, p, type);
    }

    private async Task AddFromDirectory(ZipArchive archive, DirectoryInfo directory, string path, string type) {
      var files = directory.GetFiles();
      foreach (var file in files) {
        var entry = archive.CreateEntry(Path.Combine(type, file.FullName.Replace(path + @"\", "")));
        var entryStream = entry.Open();
        await using var fileStream = File.Open(file.FullName, FileMode.Open);
        await fileStream.CopyToAsync(entryStream);
      }
    }
  }
}
