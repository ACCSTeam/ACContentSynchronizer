using System.Collections.Generic;

namespace ACContentSynchronizer.Models {
  public class UploadManifest : Manifest {
    public Dictionary<string, Dictionary<string, string>> ServerConfig { get; set; } = new();
  }
}
