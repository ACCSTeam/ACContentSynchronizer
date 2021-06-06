namespace ACContentSynchronizer.Models {
  public class UpdateManifest {
    public Manifest Manifest { get; set; } = new();
    public byte[] Content { get; set; } = new byte[0];
  }
}
