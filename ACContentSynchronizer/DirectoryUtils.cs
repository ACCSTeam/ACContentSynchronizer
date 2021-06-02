using System.IO;

namespace ACContentSynchronizer {
  public static class DirectoryUtils {
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

    public static long Size(string dirName) {
      long size = 0;
      var dirInfo = new DirectoryInfo(dirName);

      foreach (FileInfo fi in dirInfo.GetFiles("*", SearchOption.AllDirectories))
      {
        size += fi.Length;
      }

      return size;
    }
  }
}
