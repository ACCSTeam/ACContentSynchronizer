using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.Client;
using ACContentSynchronizer.Client.Models;
using ACContentSynchronizer.ClientGui.Modals;

namespace ACContentSynchronizer.ClientGui.Models {
  public class ValidationTask : TaskViewModel {
    public ValidationTask(ServerEntry serverEntry) {
      ServerEntry = serverEntry;
    }

    private CancellationTokenSource Canceller { get; set; } = new();
    private Task? Worker { get; set; }
    private ServerEntry ServerEntry { get; set; }

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

    public override void Run() {
      Canceller = new();
      Worker = Task.Run(async () => {
        try {
          Canceller.Token.ThrowIfCancellationRequested();
          var settings = Settings.Instance;
          var dataReceiver = new DataReceiver(ServerEntry.Http);

          State = "Downloading manifest...";
          Canceller.Token.ThrowIfCancellationRequested();
          var manifest = await dataReceiver.DownloadManifest();
          State = "Manifest downloaded";
          Canceller.Token.ThrowIfCancellationRequested();

          if (manifest != null) {
            State = "Content comparing";
            Canceller.Token.ThrowIfCancellationRequested();
            var comparedManifest = dataReceiver.CompareContent(settings.GamePath, manifest);

            if (comparedManifest.Cars.Any() || comparedManifest.Track != null) {
              State = "Preparing content...";
              var clientId = await SubscribeToProgress(ServerEntry);
              var session = await dataReceiver.PrepareContent(comparedManifest, clientId);

              dataReceiver.OnProgress += SetProgress;
              dataReceiver.OnComplete += () => Task.Run(() => {
                try {
                  State = "Downloaded";
                  Canceller.Token.ThrowIfCancellationRequested();
                  dataReceiver.OnProgress -= SetProgress;

                  State = "Trying to save content...";
                  Canceller.Token.ThrowIfCancellationRequested();
                  dataReceiver.SaveData();
                  State = "Content saved";
                  Canceller.Token.ThrowIfCancellationRequested();

                  State = "Applying changes...";
                  Canceller.Token.ThrowIfCancellationRequested();
                  dataReceiver.Apply(settings.GamePath);
                  State = "Done!";
                  Canceller.Token.ThrowIfCancellationRequested();
                } catch (Exception e) {
                  State = $"ERROR: {e.Message}";
                }
              });

              State = "Downloading content...";
              Canceller.Token.ThrowIfCancellationRequested();
              dataReceiver.DownloadContent(session, clientId);
            } else {
              State = "Content no need to update";
            }
          }
        } catch (OperationCanceledException) {
          Worker?.Dispose();
          Worker = null;
          Toast.Open("Task canceled");
        } catch (Exception e) {
          State = $"ERROR: {e.Message}";
        }
      }, Canceller.Token);
    }

    public override void Cancel() {
      Canceller.Cancel();
      Canceller.Dispose();
    }

    public override void Dispose() {
      Canceller.Dispose();
      Worker?.Dispose();
    }
  }
}
