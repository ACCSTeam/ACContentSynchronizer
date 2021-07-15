using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace ACContentSynchronizer.Models {
  public class AvailableContent {
    public delegate void ProgressEvent(double progress, string entry);

    private CancellationTokenSource Canceller { get; } = new();
    public List<EntryInfo> Cars { get; set; } = new();
    public EntryInfo? Track { get; set; }
    public string Session { get; set; } = "";

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
      } catch (Exception) {
        DirectoryUtils.DeleteIfExists(session, true);
      } finally {
        GC.Collect();
      }
    }

    public void AbortPacking() {
      Canceller.Cancel();
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
        try {
          Canceller.Token.ThrowIfCancellationRequested();
          var entry = archive.CreateEntry(Path.Combine(type, file.FullName.Replace(path + Path.DirectorySeparatorChar, "")));
          var entryStream = entry.Open();

          await GrantAccess(async () => {
            await using var fileStream = File.Open(file.FullName, FileMode.Open);
            await fileStream.CopyToAsync(entryStream);
          }, TimeSpan.FromMinutes(1));
        } catch (Exception e) {
          Console.WriteLine(e);
          throw;
        }
      }
    }

    private async Task GrantAccess(Func<Task> action, TimeSpan timeout) {
      var time = Stopwatch.StartNew();
      while (time.ElapsedMilliseconds < timeout.TotalMilliseconds) {
        try {
          await action();
          return;
        } catch (IOException e) {
          if (e.HResult != -2147024864) {
            throw;
          }
        }
      }
      throw new("Failed perform action within allotted time.");
    }
  }
}
