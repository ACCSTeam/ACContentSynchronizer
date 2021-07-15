using System.Collections.Generic;
using System.Linq;

namespace ACContentSynchronizer.Models {
  public class EntryInfo {
    private string? _name;

    public EntryInfo() {
    }

    public EntryInfo(string path) {
      Path = path;
    }

    public EntryInfo(string path, string? variation) {
      Path = path;
      SelectedVariation = variation;
    }

    public EntryInfo(string path, string name, List<string> variations) {
      Path = path;
      Name = name;
      Variations = variations;
      SelectedVariation = variations.FirstOrDefault();
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
