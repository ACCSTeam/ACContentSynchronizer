using System;
using ACContentSynchronizer.ClientGui.Tasks;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.ClientGui.Views;
using Avalonia.Collections;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Components.Server {
  public class UploadViewModel : ViewModelBase, IDisposable {
    private string _carSearch = "";

    private EntryViewModel? _selectedTrack;

    private string _trackSearch = "";

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

    public void Dispose() {
      _selectedTrack?.Dispose();
    }

    public void Add(EntryViewModel entry) {
      SelectedCars.Add(entry.Clone());
    }

    public void Remove(EntryViewModel entry) {
      SelectedCars.Remove(entry);
    }

    public void Upload() {
      StatusBar.Instance.AddTask(new UploadTask(Views.Server.GetServer, this));
    }
  }
}
