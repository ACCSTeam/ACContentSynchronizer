using System.Collections.Generic;

namespace ACContentSynchronizer.Models {
  public class CarsUpdate {
    public string Name { get; set; } = "";
    public List<CarSkin> Skins { get; set; } = new();
  }

  public class CarSkin {
    public string Skin { get; set; } = "";
    public bool IsConnected { get; set; }
  }
}
