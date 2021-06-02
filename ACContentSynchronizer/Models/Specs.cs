using System.Text.Json.Serialization;

namespace ACContentSynchronizer.Models {
  public class Specs {
    public string Bhp { get; set; } = "";

    public string Torque { get; set; } = "";

    public string Weight { get; set; } = "";

    [JsonPropertyName("topspeed")]
    public string TopSpeed { get; set; } = "";

    public string Acceleration { get; set; } = "";

    [JsonPropertyName("pwratio")]
    public string PwRatio { get; set; } = "";
  }
}
