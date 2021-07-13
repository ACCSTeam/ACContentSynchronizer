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

    private string _search = "";

    public UploadViewModel() {
      ReactiveCommand.CreateFromTask(Load).Execute();

      var filter = this.WhenAnyValue(vm => vm.Search)
        .Throttle(TimeSpan.FromMilliseconds(100))
        .Select(BuildFilter);

      AvailableCars.Connect()
        .Filter(filter)
        .ObserveOn(AvaloniaScheduler.Instance)
        .Bind(out _cars)
        .Subscribe();
    }

    private SourceList<EntryInfo> AvailableCars { get; set; } = new();

    public ReadOnlyObservableCollection<EntryInfo> Cars {
      get => _cars;
      set => this.RaiseAndSetIfChanged(ref _cars, value);
    }

    public AvaloniaList<EntryInfo> Tracks { get; set; } = new();

    public AvaloniaList<EntryInfo> SelectedCars { get; set; } = new();

    public EntryInfo? SelectedTrack { get; set; }

    public string Search {
      get => _search;
      set => this.RaiseAndSetIfChanged(ref _search, value);
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

      Tracks.AddRange(tracks);

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
