﻿using System.Runtime.Serialization;

namespace ACContentSynchronizer.Client.Models {
  public class ServerEntry {
    private string? _name;

    public ServerEntry() {
    }

    public ServerEntry(string ip) {
      Ip = ip;
    }

    public string Ip { get; set; } = "localhost";

    [IgnoreDataMember]
    public string Http => $"http://{Ip}";

    public string Name {
      get => _name ?? Ip;
      set => _name = value;
    }

    public string Password { get; set; } = "";
  }
}