using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia.Collections;
using Avalonia.Threading;
using DynamicData;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class ServerMainViewModel : ViewModelBase, IDisposable {
    private string _adminPassword = $"accsAdm{new Random().Next(100, 1000)}";

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

    public ServerMainViewModel() {
      ReactiveCommand.CreateFromTask(Load).Execute();
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

    public string AdminPassword {
      get => _adminPassword;
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
      set {
        this.RaiseAndSetIfChanged(ref _packetSize, value);
        this.RaisePropertyChanged(nameof(PacketSizeLabel));
      }
    }

    public short Threads {
      get => _threads;
      set {
        this.RaiseAndSetIfChanged(ref _threads, value);
        this.RaisePropertyChanged(nameof(ThreadsLabel));
      }
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

    public string PacketSizeLabel => $"Packet size: {_packetSize} hz";

    public string ThreadsLabel => $"Threads: {_threads}";

    public string CarsCount => $"{SelectedCars.Count}/100";

    private Task Load() {
      var settings = Settings.Instance;
      var carsDirectory = Path.Combine(settings.GamePath, Constants.ContentFolder, Constants.CarsFolder);
      var tracksDirectory = Path.Combine(settings.GamePath, Constants.ContentFolder, Constants.TracksFolder);

      if (Directory.Exists(carsDirectory)) {
        AvailableCars.AddRange(Directory.GetDirectories(carsDirectory)
          .Select(x => new EntryViewModel(x,
            ContentUtils.GetCarName(DirectoryUtils.Name(x), settings.GamePath) ?? DirectoryUtils.Name(x),
            new AvaloniaList<string>(ContentUtils
              .GetCarSkins(DirectoryUtils.Name(x), settings.GamePath)),
            Preview.GetCarPreview("", ""))));
      }

      if (!Directory.Exists(tracksDirectory)) {
        return Task.CompletedTask;
      }

      var tracks = Directory.GetDirectories(tracksDirectory)
        .SelectMany(x => ContentUtils.GetTrackName(x, settings.GamePath)
          .Select(v => new EntryViewModel(x, v.Name, v.Variation,
            Preview.GetTrackPreview(x))));

      AvailableTracks.AddRange(tracks);

      return Task.CompletedTask;
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

    public void Dispose() {
      AvailableCars.Dispose();
      AvailableTracks.Dispose();
      _selectedTrack?.Dispose();
    }
  }
}
