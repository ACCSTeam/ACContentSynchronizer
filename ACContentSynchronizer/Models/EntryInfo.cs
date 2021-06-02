using System.Linq;

namespace ACContentSynchronizer.Models {
  public class EntryInfo {
    public EntryInfo(string path) {
      Path = path;
    }

    public string Name => Path.Split('/').LastOrDefault() ?? "";
    public string Path { get; set; }
  }
}
