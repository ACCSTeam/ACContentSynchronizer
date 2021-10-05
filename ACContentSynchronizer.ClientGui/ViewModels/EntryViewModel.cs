using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public EntryViewModel(string path,
                          string name,
                          string? variation,
                          Func<string, Bitmap?> getPreview) {
      Task.Run(() => {
        Path = path;
        Name = name;
        SelectedVariation = variation;

        var preview = getPreview(path);
        Preview = preview;
      });
    }

    public EntryViewModel(string path,
                          string gamePath,
                          Func<string, string, string?> getName,
                          Func<string, string, List<string>> getSkins,
                          Func<string, string?, Bitmap?> getPreview) {
      Task.Run(() => {
        Path = path;
        var directName = DirectoryUtils.Name(path);
        var name = getName(directName, gamePath) ?? directName;
        Name = name;

        var skins = getSkins(directName, gamePath);
        Variations = new(skins);
        SelectedVariation = skins.FirstOrDefault();

        var preview = getPreview("", "");
        Preview = preview;
      });
    }

    private EntryViewModel(string path, string name, AvaloniaList<string> variations, string? selectedVariation) {
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

    public string EntryName => DirectoryUtils.Name(Path);

    public void Dispose() {
      _preview?.Dispose();
    }

    public EntryViewModel Clone() {
      return new(Path, Name, Variations, SelectedVariation);
    }
  }
}
