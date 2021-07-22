using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ACContentSynchronizer.Models {
  public class ServerInfo {
    public string Ip { get; set; } = "";

    public short Port { get; set; }

    [JsonPropertyName("cport")]
    public short CPort { get; set; }

    public string Name { get; set; } = "";

    public short Clients { get; set; }

    [JsonPropertyName("maxclients")]
    public short MaxClients { get; set; }

    public string Track { get; set; } = "";

    public List<string> Cars { get; set; } = new();

    [JsonPropertyName("timeofday")]
    public long TimeOfDay { get; set; }

    public long Session { get; set; }

    [JsonPropertyName("sessiontypes")]
    public List<long> SessionTypes { get; set; } = new();

    public List<long> Durations { get; set; } = new();

    [JsonPropertyName("timeleft")]
    public long TimeLeft { get; set; }

    public List<string> Country { get; set; } = new();

    public bool Pass { get; set; }

    public long Timestamp { get; set; }

    public object Json { get; set; } = new();

    public bool L { get; set; }

    public bool Pickup { get; set; }

    [JsonPropertyName("tport")]
    public short TPort { get; set; }

    public bool Timed { get; set; }

    public bool Extra { get; set; }

    public bool Pit { get; set; }

    public long Inverted { get; set; }
  }
}
