using System.IO;

namespace ACContentSynchronizer.Models {
  public class EntryManifest : EntryInfo {
    public long Size => DirectoryUtils.Size(Path);

    public EntryManifest(string path) : base(path) {
    }
  }
}
