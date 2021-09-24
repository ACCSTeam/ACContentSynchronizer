using System;
using System.Collections.Generic;

namespace ACContentSynchronizer.ClientGui.Models {
  public class Settings {
    public string GamePath { get; set; } = "";

    public string PlayerName { get; set; } = "";

    public string SteamId { get; set; } = "";

    public bool SidebarMinimized { get; set; }
    public List<ServerEntry> Servers { get; set; } = new();
  }

  public class ServerEntry {
    public string Name { get; set; } = "";
    public string Ip { get; set; } = "";
    public string Port { get; set; } = "";
    public string KunosPort { get; set; } = "";
    public DateTime DateTime { get; set; }
    public string Password { get; set; } = "";
  }
}
