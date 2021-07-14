using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.Models;
using Avalonia.Collections;
using Avalonia.Threading;
using DynamicData;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Views {
  public class UploadViewModel : ViewModelBase {
    private ReadOnlyObservableCollection<EntryInfo> _cars;

    private ReadOnlyObservableCollection<EntryInfo> _tracks;

    private string _carSearch = "";

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

    private SourceList<EntryInfo> AvailableCars { get; set; } = new();

    public ReadOnlyObservableCollection<EntryInfo> Cars {
      get => _cars;
      set => this.RaiseAndSetIfChanged(ref _cars, value);
    }

    private SourceList<EntryInfo> AvailableTracks { get; set; } = new();

    public ReadOnlyObservableCollection<EntryInfo> Tracks {
      get => _tracks;
      set => this.RaiseAndSetIfChanged(ref _tracks, value);
    }

    public AvaloniaList<EntryInfo> SelectedCars { get; set; } = new();

    public EntryInfo? SelectedTrack { get; set; }

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
          .Select(x => new EntryInfo(x,
            ContentUtils.GetCarName(x, settings.GamePath),
            ContentUtils.GetCarSkins(x, settings.GamePath))));
      }

      if (!Directory.Exists(tracksDirectory)) {
        return Task.CompletedTask;
      }

      var tracks = Directory.GetDirectories(tracksDirectory)
        .SelectMany(x => ContentUtils.GetTrackName(x, settings.GamePath)
          .Select(v => new EntryInfo(v, v, new() { v })));

      AvailableTracks.AddRange(tracks);

      return Task.CompletedTask;
    }

    private void Add(EntryInfo entry) {
      SelectedCars.Add(entry);
    }

    private void Remove(EntryInfo entry) {
      SelectedCars.Remove(entry);
    }

    private Task Upload() {
      return Task.CompletedTask;
    }

    private Func<EntryInfo, bool> BuildFilter(string searchText) {
      if (string.IsNullOrEmpty(searchText)) {
        return _ => true;
      }

      return t => t.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }
  }
}
