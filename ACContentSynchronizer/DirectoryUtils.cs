using System.IO;
using System.Linq;

namespace ACContentSynchronizer {
  public static class DirectoryUtils {
    public static string Name(string path) {
      return new DirectoryInfo(path).Name;
    }

    public static void CreateIfNotExists(string path) {
      if (!Directory.Exists(path)) {
        Directory.CreateDirectory(path);
      }
    }

    public static void DeleteIfExists(string path, bool recursive = false) {
      if (Directory.Exists(path)) {
        Directory.Delete(path, recursive);
      }
    }

    public static void Copy(string sourceDirName, string destDirName, bool copySubDirs) {
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
          Copy(subDir.FullName, tempPath, copySubDirs);
        }
      }
    }

    public static void Move(string sourceDirName, string destDirName) {
      if (Path.GetPathRoot(sourceDirName) != Path.GetPathRoot(destDirName)) {
        Copy(sourceDirName, destDirName, true);
        DeleteIfExists(sourceDirName, true);
      } else {
        Directory.Move(sourceDirName, destDirName);
      }
    }

    public static long Size(string dirName) {
      var dirInfo = new DirectoryInfo(dirName);

      return dirInfo.GetFiles("*", SearchOption.AllDirectories)
        .Sum(fi => fi.Length);
    }
  }
}
