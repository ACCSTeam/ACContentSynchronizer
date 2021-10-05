namespace ACContentSynchronizer.Models {
  public class ServerPreset {
    public string Name { get; set; } = "";
    public string Preset { get; set; } = "";

    public string BindingValue => $"{Name} ({Preset})";
  }
}
