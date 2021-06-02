namespace ACContentSynchronizer.Models {
  public class EntryManifest : EntryInfo {
    public EntryManifest(string path) : base(path) {
    }

    public long Size => DirectoryUtils.Size(Path);
  }
}
