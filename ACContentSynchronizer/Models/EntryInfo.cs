using System.IO;

namespace ACContentSynchronizer.Models {
  public class EntryInfo {
    public EntryInfo(string path) {
      Path = path;
    }

    public string Name => new DirectoryInfo(Path).Name;
    public string Path { get; set; }
  }
}
