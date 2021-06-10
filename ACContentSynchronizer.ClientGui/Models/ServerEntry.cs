namespace ACContentSynchronizer.ClientGui.Models {
  public class ServerEntry {
    private string? _name;
    public string Ip { get; set; } = "";
    public string Http => $"http://{Ip}";

    public string Name {
      get => _name ?? Ip;
      set => _name = value;
    }
  }
}
