using System;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.Extensions;
using Avalonia.VisualTree;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerSettingsViewModel : ViewModelBase {
    private IniFile? _serverConfig;
    private IniFile? _entryList;

    public async Task Load(ServerEntry server) {
      var dataReceiver = new DataReceiver(server.Http);

      _serverConfig = await dataReceiver.GetServerConfig() ?? new();
      _entryList = await dataReceiver.GetEntryList() ?? new();

      ServerMain.Instance.Load(_serverConfig, _entryList);
      ServerRules.Instance.Load(_serverConfig);
      // ServerConditions.Instance.Load(_serverConfig);
      // ServerSessions.Instance.Load(_serverConfig);
    }

    public void Save() {
      var serverConfig = new IniFile {
        ["SERVER"] = new(),
      };
      serverConfig = ServerMain.Instance.Save(serverConfig);
      serverConfig = ServerConditions.Instance.Save(serverConfig);
      serverConfig = ServerRules.Instance.Save(serverConfig);
      serverConfig = ServerSessions.Instance.Save(serverConfig);
      serverConfig = SaveServerConfig(serverConfig);

      _serverConfig = serverConfig;
      _entryList = ServerMain.Instance.SaveEntries();
    }

    private IniFile SaveServerConfig(IniFile source) {
      source["SERVER"]["SLEEP_TIME"] = _serverConfig?.V("SERVER", "SLEEP_TIME", 1) ?? 1;
      source["SERVER"]["SEND_BUFFER_SIZE"] = _serverConfig?.V("SERVER", "SEND_BUFFER_SIZE", 0) ?? 0;
      source["SERVER"]["RECV_BUFFER_SIZE"] = _serverConfig?.V("SERVER", "RECV_BUFFER_SIZE", 0) ?? 0;
      source["SERVER"]["BLACKLIST_MODE"] = _serverConfig?.V("SERVER", "BLACKLIST_MODE", 0) ?? 0;
      source["SERVER"]["UDP_PLUGIN_LOCAL_PORT"] = _serverConfig?.V("SERVER", "UDP_PLUGIN_LOCAL_PORT", 0) ?? 0;
      source["SERVER"]["UDP_PLUGIN_ADDRESS"] = _serverConfig?.V("SERVER", "UDP_PLUGIN_ADDRESS", "");
      source["SERVER"]["AUTH_PLUGIN_ADDRESS"] = _serverConfig?.V("SERVER", "AUTH_PLUGIN_ADDRESS", "");
      source["SERVER"]["LEGAL_TYRES"] = _serverConfig?.V("SERVER", "LEGAL_TYRES", "");

      source["DATA"] = new() {
        ["DESCRIPTION"] = _serverConfig?.V("DATA", "DESCRIPTION", ""),
        ["EXSERVEREXE"] = _serverConfig?.V("DATA", "EXSERVEREXE", ""),
        ["EXSERVERBAT"] = _serverConfig?.V("DATA", "EXSERVERBAT", ""),
        ["EXSERVERHIDEWIN"] = _serverConfig?.V("DATA", "EXSERVERHIDEWIN", 0) ?? 0,
        ["WEBLINK"] = _serverConfig?.V("DATA", "WEBLINK", ""),
        ["WELCOME_PATH"] = _serverConfig?.V("DATA", "WELCOME_PATH", ""),
      };

      return source;
    }
  }
}
