using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Components;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.Extensions;
using Avalonia.Collections;
using Avalonia.Threading;
using DynamicData;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views.ServerViews {
  public partial class ServerSettingsViewModel {
    private string? _adminPassword;

    private ReadOnlyObservableCollection<EntryViewModel> _cars;

    private string _carSearch = "";

    private short _httpPort = 8081;

    private short _packetSize = 18;

    private string _password = "";

    private bool _publicServer = true;

    private AvaloniaList<EntryViewModel> _selectedCars = new();

    private EntryViewModel? _selectedTrack;

    private string _serverName = "ACCS Server";

    private short _tcpPort = 9600;

    private short _threads = 2;

    private ReadOnlyObservableCollection<EntryViewModel> _tracks;

    private string _trackSearch = "";

    private short _udpPort = 9600;

    private string _welcomeMessage = "";

    public EntryViewModel? SelectedTrack {
      get => _selectedTrack;
      set => this.RaiseAndSetIfChanged(ref _selectedTrack, value);
    }

    public AvaloniaList<EntryViewModel> SelectedCars {
      get => _selectedCars;
      set => this.RaiseAndSetIfChanged(ref _selectedCars, value);
    }

    public string ServerName {
      get => _serverName;
      set => this.RaiseAndSetIfChanged(ref _serverName, value);
    }

    public string Password {
      get => _password;
      set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public string WelcomeMessage {
      get => _welcomeMessage;
      set => this.RaiseAndSetIfChanged(ref _welcomeMessage, value);
    }

    public string AdminPassword {
      get => _adminPassword ??= $"accsAdm{GeneratePassword()}";
      set => this.RaiseAndSetIfChanged(ref _adminPassword, value);
    }

    public bool PublicServer {
      get => _publicServer;
      set => this.RaiseAndSetIfChanged(ref _publicServer, value);
    }

    public short TcpPort {
      get => _tcpPort;
      set => this.RaiseAndSetIfChanged(ref _tcpPort, value);
    }

    public short UdpPort {
      get => _udpPort;
      set => this.RaiseAndSetIfChanged(ref _udpPort, value);
    }

    public short HttpPort {
      get => _httpPort;
      set => this.RaiseAndSetIfChanged(ref _httpPort, value);
    }

    public short PacketSize {
      get => _packetSize;
      set => this.RaiseAndSetIfChanged(ref _packetSize, value);
    }

    public short Threads {
      get => _threads;
      set => this.RaiseAndSetIfChanged(ref _threads, value);
    }

    public string CarSearch {
      get => _carSearch;
      set => this.RaiseAndSetIfChanged(ref _carSearch, value);
    }

    public string TrackSearch {
      get => _trackSearch;
      set => this.RaiseAndSetIfChanged(ref _trackSearch, value);
    }

    private SourceList<EntryViewModel> AvailableCars { get; } = new();

    public ReadOnlyObservableCollection<EntryViewModel> Cars {
      get => _cars;
      set => this.RaiseAndSetIfChanged(ref _cars, value);
    }

    private SourceList<EntryViewModel> AvailableTracks { get; } = new();

    public ReadOnlyObservableCollection<EntryViewModel> Tracks {
      get => _tracks;
      set => this.RaiseAndSetIfChanged(ref _tracks, value);
    }

    public string CarsCount => $"{SelectedCars.Count}/100";

    public void InitMainView() {
      LoadContent();
      const int throttle = 500;

      var carFilter = this.WhenAnyValue(vm => vm.CarSearch)
        .Throttle(TimeSpan.FromMilliseconds(throttle))
        .Select(BuildFilter);

      var trackFilter = this.WhenAnyValue(vm => vm.TrackSearch)
        .Throttle(TimeSpan.FromMilliseconds(throttle))
        .Select(BuildFilter);

      AvailableTracks.Connect()
        .Filter(trackFilter)
        .ObserveOn(AvaloniaScheduler.Instance)
        .Bind(out _tracks)
        .Subscribe();

      AvailableCars.Connect()
        .Filter(carFilter)
        .ObserveOn(AvaloniaScheduler.Instance)
        .Bind(out _cars)
        .Subscribe();
    }

    private static string GeneratePassword() {
      string[] randomChars = {
        "ABCDEFGHJKLMNOPQRSTUVWXYZ", // uppercase
        "abcdefghijkmnopqrstuvwxyz", // lowercase
        "0123456789", // digits
      };

      var rand = new Random(Environment.TickCount);
      var chars = new List<char>();

      chars.Insert(rand.Next(0, chars.Count),
        randomChars[0][rand.Next(0, randomChars[0].Length)]);

      chars.Insert(rand.Next(0, chars.Count),
        randomChars[1][rand.Next(0, randomChars[1].Length)]);

      chars.Insert(rand.Next(0, chars.Count),
        randomChars[2][rand.Next(0, randomChars[2].Length)]);

      const int length = 5;
      for (var i = chars.Count;
        i < length
        || chars.Distinct().Count() < length;
        i++) {
        var rcs = randomChars[rand.Next(0, randomChars.Length)];
        chars.Insert(rand.Next(0, chars.Count),
          rcs[rand.Next(0, rcs.Length)]);
      }

      return new(chars.ToArray());
    }

    private Task LoadContent() {
      var carsDirectory = Path.Combine(_settings.GamePath, Constants.ContentFolder, Constants.CarsFolder);
      var tracksDirectory = Path.Combine(_settings.GamePath, Constants.ContentFolder, Constants.TracksFolder);

      if (Directory.Exists(carsDirectory)) {
          AvailableCars.AddRange(Directory.GetDirectories(carsDirectory)
            .Select(x => new EntryViewModel(x,
              _settings.GamePath,
              _contentService.GetCarName,
              _contentService.GetCarSkins,
              Preview.GetCarPreview)));
      }

      if (!Directory.Exists(tracksDirectory)) {
        return Task.CompletedTask;
      }

      var tracks = Directory.GetDirectories(tracksDirectory)
        .SelectMany(x => _contentService.GetTrackName(x, _settings.GamePath)
          .Select(v => new EntryViewModel(x, v.Name, v.Variation,
            Preview.GetTrackPreview)));

      AvailableTracks.AddRange(tracks);
      return Task.CompletedTask;
    }

    public IniFile SaveMain(IniFile source) {
      var cars = string.Join(';', SelectedCars
        .Select(x => x.EntryName)
        .Distinct());

      source["SERVER"]["NAME"] = ServerName;
      source["SERVER"]["CARS"] = cars;
      source["SERVER"]["CONFIG_TRACK"] = SelectedTrack?.SelectedVariation;
      source["SERVER"]["TRACK"] = SelectedTrack?.EntryName;
      source["SERVER"]["PASSWORD"] = Password;
      source["SERVER"]["ADMIN_PASSWORD"] = AdminPassword;
      source["SERVER"]["UDP_PORT"] = UdpPort;
      source["SERVER"]["TCP_PORT"] = TcpPort;
      source["SERVER"]["HTTP_PORT"] = HttpPort;
      source["SERVER"]["CLIENT_SEND_INTERVAL_HZ"] = PacketSize;
      source["SERVER"]["REGISTER_TO_LOBBY"] = PublicServer.ToInt();
      source["SERVER"]["MAX_CLIENTS"] = SelectedCars.Count;
      source["SERVER"]["NUM_THREADS"] = Threads;
      source["SERVER"]["WELCOME_MESSAGE"] = WelcomeMessage;

      return source;
    }

    public IniFile SaveEntries() {
      var entryList = new IniFile();
      for (var i = 0; i < SelectedCars.Count; i++) {
        var car = SelectedCars[i];
        entryList.Add($"CAR_{i}", new() {
          ["MODEL"] = car.EntryName,
          ["SKIN"] = car.SelectedVariation,
          ["SPECTATOR_MODE"] = 0,
          ["DRIVERNAME"] = "",
          ["TEAM"] = "",
          ["GUID"] = "",
          ["BALLAST"] = 0,
          ["RESTRICTOR"] = 0,
        });
      }

      return entryList;
    }

    public void LoadMain(IniFile serverConfig, IniFile entryList) {
      ServerName = serverConfig.V("SERVER", "NAME", ServerName);
      Password = serverConfig.V("SERVER", "PASSWORD", Password);
      AdminPassword = serverConfig.V("SERVER", "ADMIN_PASSWORD", AdminPassword);
      PublicServer = serverConfig.V("SERVER", "REGISTER_TO_LOBBY", PublicServer);
      UdpPort = serverConfig.V("SERVER", "UDP_PORT", UdpPort);
      TcpPort = serverConfig.V("SERVER", "TCP_PORT", TcpPort);
      HttpPort = serverConfig.V("SERVER", "HTTP_PORT", HttpPort);
      PacketSize = serverConfig.V("SERVER", "CLIENT_SEND_INTERVAL_HZ", PacketSize);
      Threads = serverConfig.V("SERVER", "NUM_THREADS", Threads);
      WelcomeMessage = serverConfig.V("SERVER", "WELCOME_MESSAGE", WelcomeMessage);

      SelectedCars = new();

      foreach (var (_, car) in entryList.Source) {
        var name = car.V("MODEL", "");
        if (string.IsNullOrEmpty(name)) {
          continue;
        }

        var skin = car.V("SKIN", "");
        if (string.IsNullOrEmpty(skin)) {
          continue;
        }

        SelectedCars.Add(new() {
          Name = _contentService.GetCarName(name, _settings.GamePath) ?? name,
          Preview = Preview.GetCarPreview(name, skin),
          Path = _contentService.GetCarDirectory(name, _settings.GamePath) ?? name,
          Variations = new() {
            skin,
          },
          SelectedVariation = skin,
        });
      }

      var track = serverConfig.V("SERVER", "TRACK", string.Empty);

      if (string.IsNullOrEmpty(track)) {
        return;
      }

      var trackVariation = serverConfig.V("SERVER", "CONFIG_TRACK", string.Empty);
      var trackNames = _contentService.GetTrackName(track, _settings.GamePath);

      SelectedTrack = new() {
        Path = track,
        Name = (string.IsNullOrEmpty(trackVariation)
            ? trackNames.FirstOrDefault()?.Name
            : trackNames.FirstOrDefault(x => x.Variation == trackVariation)?.Name
          ) ?? track,
        Preview = Preview.GetTrackPreview(track),
        Variations = new() { trackVariation },
        SelectedVariation = trackVariation,
      };
    }

    public void Add(EntryViewModel entry) {
      SelectedCars.Add(entry.Clone());
    }

    public void Remove(EntryViewModel entry) {
      SelectedCars.Remove(entry);
    }

    private Func<EntryViewModel, bool> BuildFilter(string searchText) {
      if (string.IsNullOrEmpty(searchText)) {
        return _ => true;
      }

      return t => t.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }
  }
}
