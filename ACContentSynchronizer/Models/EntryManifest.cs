namespace ACContentSynchronizer.Models {
  public class EntryManifest : EntryInfo {
    public EntryManifest() {
    }

    public EntryManifest(string path, long size) : base(path) {
      Size = size;
    }

    public EntryManifest(string path, long size, string? variation) : base(path, variation) {
      Size = size;
    }

    public long Size { get; set; }
  }
}
