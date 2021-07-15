using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Tasks;
using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia.Collections;
using Avalonia.Threading;
using DynamicData;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views {
  public class UploadViewModel : ViewModelBase {
    private ReadOnlyObservableCollection<EntryViewModel> _cars;

    private string _carSearch = "";

    private EntryViewModel? _selectedTrack;

    private ReadOnlyObservableCollection<EntryViewModel> _tracks;

    private string _trackSearch = "";

    public UploadViewModel() {
      ReactiveCommand.CreateFromTask(Load).Execute();

      var carFilter = this.WhenAnyValue(vm => vm.CarSearch)
        .Throttle(TimeSpan.FromMilliseconds(100))
        .Select(BuildFilter);

      var trackFilter = this.WhenAnyValue(vm => vm.TrackSearch)
        .Throttle(TimeSpan.FromMilliseconds(100))
        .Select(BuildFilter);

      AvailableCars.Connect()
        .Filter(carFilter)
        .ObserveOn(AvaloniaScheduler.Instance)
        .Bind(out _cars)
        .Subscribe();

      AvailableTracks.Connect()
        .Filter(trackFilter)
        .ObserveOn(AvaloniaScheduler.Instance)
        .Bind(out _tracks)
        .Subscribe();
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

    public AvaloniaList<EntryViewModel> SelectedCars { get; set; } = new();

    public EntryViewModel? SelectedTrack {
      get => _selectedTrack;
      set => this.RaiseAndSetIfChanged(ref _selectedTrack, value);
    }

    public string CarSearch {
      get => _carSearch;
      set => this.RaiseAndSetIfChanged(ref _carSearch, value);
    }

    public string TrackSearch {
      get => _trackSearch;
      set => this.RaiseAndSetIfChanged(ref _trackSearch, value);
    }

    private Task Load() {
      var settings = Settings.Instance;
      var carsDirectory = Path.Combine(settings.GamePath, Constants.ContentFolder, Constants.CarsFolder);
      var tracksDirectory = Path.Combine(settings.GamePath, Constants.ContentFolder, Constants.TracksFolder);

      if (Directory.Exists(carsDirectory)) {
        AvailableCars.AddRange(Directory.GetDirectories(carsDirectory)
          .Select(x => new EntryViewModel(x,
            ContentUtils.GetCarName(DirectoryUtils.Name(x), settings.GamePath),
            new AvaloniaList<string>(ContentUtils
              .GetCarSkins(DirectoryUtils.Name(x), settings.GamePath)))));
      }

      if (!Directory.Exists(tracksDirectory)) {
        return Task.CompletedTask;
      }

      var tracks = Directory.GetDirectories(tracksDirectory)
        .SelectMany(x => ContentUtils.GetTrackName(x, settings.GamePath)
          .Select(v => new EntryViewModel(x, v.Name, v.BaseName)));

      AvailableTracks.AddRange(tracks);

      return Task.CompletedTask;
    }

    private void Add(EntryViewModel entry) {
      SelectedCars.Add(entry.Clone());
    }

    private void Remove(EntryViewModel entry) {
      SelectedCars.Remove(entry);
    }

    private void Upload() {
      StatusBar.Instance.AddTask(new UploadTask(Server.Instance.GetServer, this));
    }

    private Func<EntryViewModel, bool> BuildFilter(string searchText) {
      if (string.IsNullOrEmpty(searchText)) {
        return _ => true;
      }

      return t => t.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }
  }
}
