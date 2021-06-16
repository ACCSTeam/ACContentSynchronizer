using System;
using ACContentSynchronizer.Client;
using ACContentSynchronizer.Client.Models;
using ACContentSynchronizer.ClientGui.Models;
using Avalonia;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Modals {
  public class DownloadViewModel : ModalViewModel<DownloadModal>,IDisposable {
    private readonly ServerEntry _serverEntry = new();
    private DataReceiver? _dataReceiver;
    public DataReceiver DataReceiver => _dataReceiver ??= new(ServerEntry.Http);

    public ServerEntry ServerEntry {
      get => _serverEntry;
      init => this.RaiseAndSetIfChanged(ref _serverEntry, value);
    }

    private bool _canClose;

    public bool CanClose {
      get => _canClose;
      set => this.RaiseAndSetIfChanged(ref _canClose, value);
    }

    private double _progress;

    public double Progress {
      get => _progress;
      set => this.RaiseAndSetIfChanged(ref _progress, value);
    }

    private string _state = "";

    public string State {
      get => _state;
      set {
        var state = _state + value + "\n";
        this.RaiseAndSetIfChanged(ref _state, state);
        Offset = new(double.NegativeInfinity, double.PositiveInfinity);
      }
    }

    private Vector _offset;

    public Vector Offset {
      get => _offset;
      set => this.RaiseAndSetIfChanged(ref _offset, value);
    }

    public void Dispose()
    {
      _dataReceiver?.Dispose();
    }
  }
}
