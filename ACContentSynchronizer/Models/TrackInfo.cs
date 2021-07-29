using System.Collections.Generic;
using Newtonsoft.Json;

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

    [JsonProperty("pitboxes")]
    public string PitBoxes { get; set; } = "";

    public string Run { get; set; } = "";

    public string Author { get; set; } = "";

    public string Version { get; set; } = "";

    public string Url { get; set; } = "";

    public string LocalPath { get; set; } = "";

    public static TrackInfo? FromJson(string json) {
      try {
        return JsonConvert.DeserializeObject<TrackInfo>(json);
      } catch {
        return new() {
          Name = "error",
        };
      }
    }
  }
}
