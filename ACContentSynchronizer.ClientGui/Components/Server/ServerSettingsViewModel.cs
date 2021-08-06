using System.Collections.Generic;
using ACContentSynchronizer.ClientGui.ViewModels;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerSettingsViewModel : ViewModelBase {
    public void Save() {
      var serverConfig = new Dictionary<string, Dictionary<string, object>> {
        ["SERVER"] = new() {
        },
        ["FTP"] = new() {
        },
        ["PRACTICE"] = new() {
        },
        ["QUALIFY"] = new() {
        },
        ["RACE"] = new() {
        },
        ["DYNAMIC_TRACK"] = new() {
        },
        ["DATA"] = new() {
        },
      };

      serverConfig = ServerMain.Instance.ToConfig(serverConfig);
      serverConfig = ServerConditions.Instance.ToConfig(serverConfig);
      serverConfig = ServerRules.Instance.ToConfig(serverConfig);
    }
  }
}
