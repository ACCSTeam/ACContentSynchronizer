namespace ACContentSynchronizer.ClientGui.Models {
  public class ServerEntry {
    public string Ip { get; set; } = "";
    public string Http => $"http://{Ip}";

    private string? _name;

    public string Name {
      get => _name ?? Ip;
      set => _name = value;
    }
  }
}
