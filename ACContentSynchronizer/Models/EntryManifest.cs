namespace ACContentSynchronizer.Models {
  public class EntryManifest : EntryInfo {
    public EntryManifest() : base() {
    }

    public EntryManifest(string path, long size) : base(path) {
      Size = size;
    }

    public long Size { get; set; }
  }
}
