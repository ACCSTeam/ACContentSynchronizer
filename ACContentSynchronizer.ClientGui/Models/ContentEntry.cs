using System;
using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Models {
  public class ContentEntry : ViewModelBase, IDisposable {
    private int _allowed;

    private int _count;

    private string _directoryName = "";

    private string _name = "";

    private Bitmap? _preview;

    private string _variation = "";

    public string Variation {
      get => _variation;
      set => this.RaiseAndSetIfChanged(ref _variation, value);
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

    public int Count {
      get => _count;
      set {
        this.RaiseAndSetIfChanged(ref _count, value);
        this.RaisePropertyChanged(nameof(CarCount));
      }
    }

    public int Allowed {
      get => _allowed;
      set {
        this.RaiseAndSetIfChanged(ref _allowed, value);
        this.RaisePropertyChanged(nameof(CarCount));
      }
    }

    public string CarCount => $"[{Allowed}/{Count}]";

    public bool IsEnabled => Allowed > 0;

    public void Dispose() {
      _preview?.Dispose();
    }
  }
}
