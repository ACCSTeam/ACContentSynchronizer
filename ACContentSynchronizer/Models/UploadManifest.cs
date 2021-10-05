using System.Collections.Generic;

namespace ACContentSynchronizer.Models {
  public class UploadManifest : Manifest {
    public IniFile ServerConfig { get; set; } = new();
  }
}
