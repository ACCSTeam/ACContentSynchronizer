using System;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.Client;
using ACContentSynchronizer.Client.Models;
using ACContentSynchronizer.ClientGui.Models;
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

    private void SetProgress(double progress) {
      Progress = progress;
    }

    private Task<string> SubscribeToProgress(ServerEntry server) {
      return Hubs.NotificationHub<double, string, HubMethods>(server, HubMethods.PackProgress,
        (progress, entry) => {
          Progress = progress;
          State = $"Packed {entry}";
          return Task.CompletedTask;
        });
    }

    public async Task GetDataFromServer(ServerEntry server) {
      try {
        var settings = Settings.Instance();
        var dataReceiver = new DataReceiver(server.Http);

        State = "Downloading manifest...";
        var manifest = await dataReceiver.DownloadManifest();
        State = "Manifest downloaded";

        if (manifest != null) {
          State = "Content comparing";
          var comparedManifest = dataReceiver.CompareContent(settings.GamePath, manifest);

          if (comparedManifest.Cars.Any() || comparedManifest.Track != null) {
            State = "Preparing content...";
            var clientId = await SubscribeToProgress(server);
            var session = await dataReceiver.PrepareContent(comparedManifest, clientId);
            Progress = 0;

            dataReceiver.OnProgress += SetProgress;
            dataReceiver.OnComplete += () => Task.Factory.StartNew(() => {
              try {
                State = "Downloaded";
                dataReceiver.OnProgress -= SetProgress;

                State = "Trying to save content...";
                dataReceiver.SaveData();
                State = "Content saved";

                State = "Applying changes...";
                dataReceiver.Apply(settings.GamePath);
                State = "Done!";
              } catch (Exception e) {
                State = $"ERROR: {e.Message}";
              } finally {
                CanClose = true;
              }
            });

            State = "Downloading content...";
            dataReceiver.DownloadContent(session);
          } else {
            State = "Content no need to update";
            CanClose = true;
          }
        }
      } catch (Exception e) {
        State = $"ERROR: {e.Message}";
        CanClose = true;
      }
    }
  }
}
