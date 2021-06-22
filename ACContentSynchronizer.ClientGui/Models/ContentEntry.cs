using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia.Media.Imaging;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Models {
  public class ContentEntry : ViewModelBase {
    private int _count;

    private string _directoryName = "";

    private bool _isEnabled;

    private string _name = "";

    private Bitmap? _preview;

    private int _used;

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
      set => this.RaiseAndSetIfChanged(ref _count, value);
    }

    public int Used {
      get => _used;
      set => this.RaiseAndSetIfChanged(ref _used, value);
    }

    public bool IsEnabled {
      get => _isEnabled;
      set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
    }
  }
}
