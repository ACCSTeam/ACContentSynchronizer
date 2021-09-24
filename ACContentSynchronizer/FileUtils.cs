using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ACContentSynchronizer {
  public static class FileUtils {
    public static void DeleteIfExists(string path) {
      if (File.Exists(path)) {
        File.Delete(path);
      }
    }

    public static async Task CreateIfNotExistsAsync(string path) {
      if (!File.Exists(path)) {
        await File.Create(path).DisposeAsync();
      }
    }

    public static void CreateIfNotExists(string path) {
      if (!File.Exists(path)) {
        File.Create(path).Dispose();
      }
    }

    public static async Task GrantAccess(Func<Task> action, TimeSpan timeout) {
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
