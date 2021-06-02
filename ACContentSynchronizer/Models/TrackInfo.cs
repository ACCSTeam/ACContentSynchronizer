using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ACContentSynchronizer.Models {
  public class TrackInfo {
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public List<string> Tags { get; set; } = new();

    public List<string> Geotags { get; set; } = new();

    public string Country { get; set; } = "";

    public string City { get; set; } = "";

    public string Length { get; set; } = "";

    public string Width { get; set; } = "";

    [JsonPropertyName("pitboxes")]
    public string PitBoxes { get; set; } = "";

    public string Run { get; set; } = "";

    public string Author { get; set; } = "";

    public string Version { get; set; } = "";

    public string Url { get; set; } = "";

    public string LocalPath { get; set; } = "";
  }
}
