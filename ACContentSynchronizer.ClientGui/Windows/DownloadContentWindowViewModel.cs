using System;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.Client;
using ACContentSynchronizer.Client.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Windows {
  public class DownloadContentWindowViewModel : ViewModelBase {
    private bool _canClose;
    private double _progress;
    private string _state = "";

    public bool CanClose {
      get => _canClose;
      set => this.RaiseAndSetIfChanged(ref _canClose, value);
    }

    public string State {
      get => _state;
      set {
        var state = _state + $"{value}\n";
        this.RaiseAndSetIfChanged(ref _state, state);
      }
    }

    public double Progress {
      get => _progress;
      set => this.RaiseAndSetIfChanged(ref _progress, value);
    }

    public async Task GetDataFromServer(string server) {
      try {
        var settings = Settings.Instance();
        var dataReceiver = new DataReceiver(server);

        State = "Downloading manifest...";
        await dataReceiver.DownloadManifest();
        State = "Manifest downloaded";

        State = "Content comparing";
        var updatableEntries = dataReceiver.CompareContent(settings.GamePath);

        if (updatableEntries.Any()) {
          State = "Preparing content...";
          var session = await dataReceiver.PrepareContent(updatableEntries);

          dataReceiver.OnDownload += progress => Progress = progress;
          dataReceiver.OnComplete += () => Task.Run(() => {
            State = "Downloaded";
            dataReceiver.RemoveSession(session);

            State = "Trying to save content...";
            dataReceiver.SaveData();
            State = "Content saved";

            State = "Applying changes...";
            dataReceiver.Apply(settings.GamePath);
            State = "Done!";
            CanClose = true;
          });

          State = "Downloading content...";
          dataReceiver.DownloadContent(session);
        }
      } catch (Exception e) {
        State = e.Message;
      }
    }
  }
}
