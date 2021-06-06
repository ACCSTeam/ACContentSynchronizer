using System.Collections.Generic;

namespace ACContentSynchronizer.Models {
  public class Manifest {
    public List<EntryManifest> Cars { get; set; } = new();
    public EntryManifest? Track { get; set; }
  }
}
