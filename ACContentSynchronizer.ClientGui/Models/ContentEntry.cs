using System;
using System.Linq;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.Models;
using Avalonia.Collections;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Models {
  public class ContentEntry : ViewModelBase, IDisposable {
    private string _directoryName = "";

    private string _name = "";

    private Bitmap? _preview;

    private AvaloniaList<EntryVariation> _variations = new();

    public string? Variation => _variations.FirstOrDefault(x => !x.IsConnected)?.Variation;

    public AvaloniaList<EntryVariation> Variations {
      get => _variations;
      set {
        this.RaiseAndSetIfChanged(ref _variations, value);
        this.RaisePropertyChanged(nameof(Variation));
        this.RaisePropertyChanged(nameof(CarCount));
      }
    }

    public string Name {
      get => _name;
      set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public string DirectoryName {
      get => _directoryName;
      set => this.RaiseAndSetIfChanged(ref _directoryName, value);
    }

    public Bitmap? Preview {
      get => _preview;
      set => this.RaiseAndSetIfChanged(ref _preview, value);
    }

    public string CarCount => $"[{Variations.Count(x => !x.IsConnected)}/{Variations.Count}]";

    public void Dispose() {
      _preview?.Dispose();
    }
  }
}
