using System.Collections.Generic;

namespace ACContentSynchronizer.Models {
  public class ServerState {
    public List<Car> Cars { get; set; } = new();
  }

  public class Car {
    public string Model { get; set; } = "";
    public string Skin { get; set; } = "";
    public string DriverName { get; set; } = "";
    public string DriverTeam { get; set; } = "";
    public string DriverNation { get; set; } = "";
    public bool IsConnected { get; set; }
    public bool IsRequestedGuid { get; set; }
    public bool IsEntryList { get; set; }
  }
}
