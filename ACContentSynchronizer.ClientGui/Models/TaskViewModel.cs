using System;
using ACContentSynchronizer.ClientGui.ViewModels;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Models {
  public abstract class TaskViewModel : ViewModelBase, IDisposable {
    private double _progress;

    private string _state = "";

    public double Progress {
      get => _progress;
      set => this.RaiseAndSetIfChanged(ref _progress, value);
    }

    public string State {
      get => _state;
      set => this.RaiseAndSetIfChanged(ref _state, value);
    }

    public abstract void Dispose();

    public abstract void Run();

    public abstract void Cancel();
  }
}
