using System.Collections.Generic;

namespace ACContentSynchronizer.Models {
  public class EntryInfo {
    private string? _name;

    public EntryInfo() {
    }

    public EntryInfo(string path) {
      Path = path;
    }

    public EntryInfo(string path, string name, List<string> variations) {
      Path = path;
      Name = name;
      Variations = variations;
    }

    public string Name {
      get => _name ?? DirectoryUtils.Name(Path);
      set => _name = value;
    }

    public string Path { get; set; } = "";
    public List<string> Variations { get; set; } = new();
    public string? SelectedVariation { get; set; }
  }
}
