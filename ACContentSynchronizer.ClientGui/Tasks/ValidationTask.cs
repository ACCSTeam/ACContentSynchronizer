using System;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;

namespace ACContentSynchronizer.ClientGui.Tasks {
  public class ValidationTask : TaskViewModel {
    public ValidationTask(ServerEntry server) : base(server) {
    }

    private void SetProgress(double progress) {
      Canceller.Token.ThrowIfCancellationRequested();
      Progress = progress;
    }

    public override void Run() {
      Canceller = new();
      Worker = Task.Run(async () => {
        var dataReceiver = new DataReceiver(Server.Http);
        var session = "";
        try {
          var settings = Settings.Instance;

          Canceller.Token.ThrowIfCancellationRequested();
          State = "Downloading manifest...";
          var manifest = await dataReceiver.DownloadManifest();

          Canceller.Token.ThrowIfCancellationRequested();
          State = "Manifest downloaded";
          State = "Content comparing";
          if (manifest != null) {
            var comparedManifest = dataReceiver.CompareContent(settings.GamePath, manifest);

            if (comparedManifest.Cars.Any() || comparedManifest.Track != null) {
              Canceller.Token.ThrowIfCancellationRequested();
              State = "Preparing content...";
              session = await dataReceiver.PrepareContent(comparedManifest);

              Canceller.Token.ThrowIfCancellationRequested();
              State = "Pack content...";
              await dataReceiver.PackContent(session, ClientId);
              var applyTask = new Task(() => {
                try {
                  Canceller.Token.ThrowIfCancellationRequested();
                  State = "Downloaded";
                  State = "Trying to save content...";
                  dataReceiver.SaveData(session);

                  Canceller.Token.ThrowIfCancellationRequested();
                  State = "Content saved";
                  State = "Applying changes...";
                  dataReceiver.Apply(settings.GamePath, session);

                  State = "Done!";
                } catch (Exception e) {
                  State = $"ERROR: {e.Message}";
                }
              });

              dataReceiver.OnProgress += SetProgress;
              dataReceiver.OnComplete += () => applyTask.Start();

              Canceller.Token.ThrowIfCancellationRequested();
              State = "Downloading content...";
              dataReceiver.DownloadContent(session, ClientId);
              await applyTask;
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
      });
    }
  }
}
