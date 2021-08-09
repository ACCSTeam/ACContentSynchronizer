using System;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.ViewModels {
  public class EntryViewModel : ViewModelBase, IDisposable {
    private string _name = "";

    private string _path = "";

    private Bitmap? _preview;

    private string? _selectedVariation;

    private AvaloniaList<string> _variations = new();

    public EntryViewModel() {
    }

    public EntryViewModel(string path) {
      Path = path;
    }

    public EntryViewModel(string path, string? variation) {
      Path = path;
      SelectedVariation = variation;
    }

    public EntryViewModel(string path, string name, string? variation, Bitmap? preview) {
      Path = path;
      Name = name;
      SelectedVariation = variation;
      Preview = preview;
    }

    public EntryViewModel(string path, string name, AvaloniaList<string> variations, Bitmap? preview) {
      Path = path;
      Name = name;
      Variations = variations;
      SelectedVariation = variations.FirstOrDefault();
      Preview = preview;
    }

    public EntryViewModel(string path, string name, AvaloniaList<string> variations, string? selectedVariation) {
      Path = path;
      Name = name;
      Variations = variations;
      SelectedVariation = selectedVariation;
    }

    public string Name {
      get => _name;
      set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public string Path {
      get => _path;
      set => this.RaiseAndSetIfChanged(ref _path, value);
    }

    public AvaloniaList<string> Variations {
      get => _variations;
      set => this.RaiseAndSetIfChanged(ref _variations, value);
    }

    public string? SelectedVariation {
      get => _selectedVariation;
      set => this.RaiseAndSetIfChanged(ref _selectedVariation, value);
    }

    public Bitmap? Preview {
      get => _preview;
      set => this.RaiseAndSetIfChanged(ref _preview, value);
    }

    public void Dispose() {
      _preview?.Dispose();
    }

    public EntryViewModel Clone() {
      return new(Path, Name, Variations, SelectedVariation);
    }
  }
}
