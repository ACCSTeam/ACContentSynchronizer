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
  }
}
