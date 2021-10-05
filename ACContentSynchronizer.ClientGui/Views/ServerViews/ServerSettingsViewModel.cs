using System;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Services;
using ACContentSynchronizer.ClientGui.Tasks;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.Models;
using ReactiveUI;
using Splat;

namespace ACContentSynchronizer.ClientGui.Views.ServerViews {
  public partial class ServerSettingsViewModel : ViewModelBase, IDisposable {
    private readonly ContentService _contentService;
    private readonly ApplicationViewModel _application;
    private readonly HubService _hubService;
    private readonly SettingsViewModel _settings;
    private readonly ServerEntryViewModel _server;
    private IniFile? _entryList;
    private IniFile? _serverConfig;

    public ServerSettingsViewModel(ServerEntryViewModel serverEntry,
                                   HubService hubService) {
      _application = Locator.Current.GetService<ApplicationViewModel>();
      _settings = _application.Settings;
      _contentService = Locator.Current.GetService<ContentService>();
      _server = serverEntry;
      _hubService = hubService;
      _tracks = new(new());
      _cars = new(new());

      NewServerName = $"{_server.Name}Clone";
      NewServerIp = _server.Ip;
      NewServerPort = _server.Port;
      NewServerPassword = $"accsAdm{GeneratePassword()}";

      Load(serverEntry);
    }

    private string _newServerName = "";

    public string NewServerName {
      get => _newServerName;
      set => this.RaiseAndSetIfChanged(ref _newServerName, value);
    }

    private string _newServerIp = "";

    public string NewServerIp {
      get => _newServerIp;
      set => this.RaiseAndSetIfChanged(ref _newServerIp, value);
    }

    private string _newServerPort = "";

    public string NewServerPort {
      get => _newServerPort;
      set => this.RaiseAndSetIfChanged(ref _newServerPort, value);
    }

    private string _newTcpPort = "";

    public string NewTcpPort {
      get => _newTcpPort;
      set => this.RaiseAndSetIfChanged(ref _newTcpPort, value);
    }

    private string _newUdpPort = "";

    public string NewUdpPort {
      get => _newUdpPort;
      set => this.RaiseAndSetIfChanged(ref _newUdpPort, value);
    }

    private string _newHttpPort = "";

    public string NewHttpPort {
      get => _newHttpPort;
      set => this.RaiseAndSetIfChanged(ref _newHttpPort, value);
    }

    private string _newServerPassword = "";

    public string NewServerPassword {
      get => _newServerPassword;
      set => this.RaiseAndSetIfChanged(ref _newServerPassword, value);
    }

    public void Dispose() {
      AvailableCars.Dispose();
      AvailableTracks.Dispose();
      _selectedTrack?.Dispose();
      _selectedWeather?.Dispose();
    }

    public void Load(ServerEntryViewModel server) {
      InitMainView();
      ReactiveCommand.CreateFromTask(async () => {
        var dataService = new DataService(server);
        _serverConfig = await dataService.GetServerConfig() ?? new();
        _entryList = await dataService.GetEntryList() ?? new();

        LoadMain(_serverConfig, _entryList);
        LoadRules(_serverConfig);
      }).Execute();
    }

    public async Task Clone() {
      var newServer = new ServerEntryViewModel {
        Name = NewServerName,
        Ip = NewServerIp,
        Port = NewServerPort,
        KunosPort = NewHttpPort,
        Password = NewServerPassword,
        DateTime = DateTime.Now,
        ServerPreset = NewServerName,
      };

      _application.Servers.Add(newServer);
      _application.SelectedServer = newServer;

      var manifest = CreateManifest();
      manifest.ServerConfig["SERVER"]["NAME"] = NewServerName;
      manifest.ServerConfig["SERVER"]["ADMIN_PASSWORD"] = NewServerPassword;
      manifest.ServerConfig["SERVER"]["TCP_PORT"] = NewTcpPort;
      manifest.ServerConfig["SERVER"]["UDP_PORT"] = NewUdpPort;
      manifest.ServerConfig["SERVER"]["HTTP_PORT"] = NewHttpPort;
      var cloneTask = new ServerConfigurationTask(newServer, _hubService) {
        Manifest = manifest,
      };

      _application.AddTask(cloneTask);
      await cloneTask.Worker;
    }

    public async Task Save() {
      var manifest = CreateManifest();
      var uploadTask = new ServerConfigurationTask(_server, _hubService) {
        Manifest = manifest,
      };

      _application.AddTask(uploadTask);
      await uploadTask.Worker;
    }

    private UploadManifest CreateManifest() {
      var serverConfig = new IniFile {
        ["SERVER"] = new(),
      };
      serverConfig = SaveMain(serverConfig);
      serverConfig = SaveConditions(serverConfig);
      serverConfig = SaveRules(serverConfig);
      serverConfig = SaveSession(serverConfig);
      serverConfig = SaveServerConfig(serverConfig);

      _serverConfig = serverConfig;

      var manifest = new UploadManifest {
        Cars = _selectedCars
          .Select(car => new EntryManifest(car.Path,
            DirectoryUtils.Size(car.Path)))
          .ToList(),
        ServerConfig = _serverConfig,
      };

      if (_selectedTrack != null) {
        manifest.Track = new(_selectedTrack.Path, DirectoryUtils.Size(_selectedTrack.Path));
      }

      return manifest;
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
