using System.Collections.Generic;

namespace ACContentSynchronizer.Models {
  public class CarInfo {
    public string Name { get; set; } = "";

    public string Brand { get; set; } = "";

    public string Description { get; set; } = "";

    public List<string> Tags { get; set; } = new();

    public string Class { get; set; } = "";

    public Specs Specs { get; set; } = new();

    public List<List<double>> TorqueCurve { get; set; } = new();

    public List<List<double>> PowerCurve { get; set; } = new();

    public string Country { get; set; } = "";

    public string Author { get; set; } = "";

    public string Version { get; set; } = "";

    public string Url { get; set; } = "";

    public string LocalPath { get; set; } = "";
  }
}
