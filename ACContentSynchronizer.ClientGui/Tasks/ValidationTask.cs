using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.Client;
using ACContentSynchronizer.ClientGui.Models;

namespace ACContentSynchronizer.ClientGui.Tasks {
  public class ValidationTask : TaskViewModel {
    public ValidationTask(ServerEntry serverEntry) {
      ServerEntry = serverEntry;
    }

    private CancellationTokenSource Canceller { get; set; } = new();
    private Task? Worker { get; set; }
    private ServerEntry ServerEntry { get; }

    private event ExceptionHandler? OnException;

    private void SetProgress(double progress, CancellationToken token) {
      token.ThrowIfCancellationRequested();
      Progress = progress;
    }

    private Task<string> SubscribeToProgress(ServerEntry server, CancellationToken token) {
      return Hubs.NotificationHub<double, string, HubMethods>(server, HubMethods.PackProgress,
        (progress, entry) => {
          try {
            token.ThrowIfCancellationRequested();
            Progress = progress;
            State = $"Packed {entry}";
          } catch (Exception e) {
            OnException?.Invoke(e);
          }

          return Task.CompletedTask;
        });
    }

    public override void Run() {
      Canceller = new();
      Worker = Task.Run(async () => {
        var dataReceiver = new DataReceiver(ServerEntry.Http);
        var session = "";
        try {
          Canceller.Token.ThrowIfCancellationRequested();
          var settings = Settings.Instance;

          Canceller.Token.ThrowIfCancellationRequested();
          State = "Downloading manifest...";
          var manifest = await dataReceiver.DownloadManifest();
          Canceller.Token.ThrowIfCancellationRequested();
          State = "Manifest downloaded";

          if (manifest != null) {
            Canceller.Token.ThrowIfCancellationRequested();
            State = "Content comparing";
            var comparedManifest = dataReceiver.CompareContent(settings.GamePath, manifest);

            if (comparedManifest.Cars.Any() || comparedManifest.Track != null) {
              Canceller.Token.ThrowIfCancellationRequested();
              State = "Preparing content...";

              var clientId = await SubscribeToProgress(ServerEntry, Canceller.Token);
              session = await dataReceiver.PrepareContent(comparedManifest);

              OnException += exception => {
                if (exception is not OperationCanceledException) {
                  return;
                }

                if (!string.IsNullOrEmpty(session)) {
                  dataReceiver.CancelPreparing(session);
                }
              };

              Canceller.Token.ThrowIfCancellationRequested();
              State = "Pack content...";
              await dataReceiver.PackContent(session, clientId);

              dataReceiver.OnProgress += progress => SetProgress(progress, Canceller.Token);
              dataReceiver.OnComplete += () => Task.Run(() => {
                try {
                  Canceller.Token.ThrowIfCancellationRequested();
                  State = "Downloaded";

                  Canceller.Token.ThrowIfCancellationRequested();
                  State = "Trying to save content...";
                  dataReceiver.SaveData(session);
                  Canceller.Token.ThrowIfCancellationRequested();
                  State = "Content saved";

                  Canceller.Token.ThrowIfCancellationRequested();
                  State = "Applying changes...";
                  dataReceiver.Apply(settings.GamePath, session);
                  State = "Done!";
                  Canceller.Token.ThrowIfCancellationRequested();
                } catch (Exception e) {
                  State = $"ERROR: {e.Message}";
                }
              });

              Canceller.Token.ThrowIfCancellationRequested();
              State = "Downloading content...";
              dataReceiver.DownloadContent(session, clientId);
            } else {
              State = "Content no need to update";
            }
          }
        } catch (OperationCanceledException) {
          State = "Task canceled";
          if (!string.IsNullOrEmpty(session)) {
            await dataReceiver.CancelPreparing(session);
          }
        } catch (Exception e) {
          State = $"ERROR: {e.Message}";
        }
      }, Canceller.Token);
    }

    public override void Cancel() {
      Canceller.Cancel();
    }

    public override void Dispose() {
      Canceller.Dispose();
      Worker?.Dispose();
    }

    private delegate void ExceptionHandler(Exception e);
  }
}
