using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Services;
using ACContentSynchronizer.ClientGui.ViewModels;
using ReactiveUI;
using Splat;

namespace ACContentSynchronizer.ClientGui.Views.ServerViews {
  public partial class ServerSettingsViewModel : ViewModelBase, IDisposable {
    private readonly ContentService _contentService;
    private readonly SettingsViewModel _settings;
    private IniFile? _entryList;
    private IniFile? _serverConfig;

    public ServerSettingsViewModel(ServerEntryViewModel serverEntry) {
      _settings = Locator.Current.GetService<ApplicationViewModel>().Settings;
      _contentService = Locator.Current.GetService<ContentService>();

      ReactiveCommand.CreateFromTask(() => Load(serverEntry)).Execute();
    }

    public void Dispose() {
      AvailableCars.Dispose();
      AvailableTracks.Dispose();
      _selectedTrack?.Dispose();
    }

    public async Task Load(ServerEntryViewModel server) {
      InitMainView();
      var dataService = new DataService(server);
      _serverConfig = await dataService.GetServerConfig() ?? new();
      _entryList = await dataService.GetEntryList() ?? new();

      LoadMain(_serverConfig, _entryList);
      LoadRules(_serverConfig);
    }

    public void Save() {
      var serverConfig = new IniFile {
        ["SERVER"] = new(),
      };
      serverConfig = SaveMain(serverConfig);
      serverConfig = SaveConditions(serverConfig);
      serverConfig = SaveRules(serverConfig);
      serverConfig = SaveSession(serverConfig);
      serverConfig = SaveServerConfig(serverConfig);

      _serverConfig = serverConfig;
      _entryList = SaveEntries();
    }

    private IniFile SaveServerConfig(IniFile source) {
      source["SERVER"]["SLEEP_TIME"] = _serverConfig?.V("SERVER", "SLEEP_TIME", 1);
      source["SERVER"]["SEND_BUFFER_SIZE"] = _serverConfig?.V("SERVER", "SEND_BUFFER_SIZE", 0);
      source["SERVER"]["RECV_BUFFER_SIZE"] = _serverConfig?.V("SERVER", "RECV_BUFFER_SIZE", 0);
      source["SERVER"]["BLACKLIST_MODE"] = _serverConfig?.V("SERVER", "BLACKLIST_MODE", 0);
      source["SERVER"]["UDP_PLUGIN_LOCAL_PORT"] = _serverConfig?.V("SERVER", "UDP_PLUGIN_LOCAL_PORT", 0);
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
