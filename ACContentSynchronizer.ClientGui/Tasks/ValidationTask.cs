using System;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Services;
using ACContentSynchronizer.ClientGui.ViewModels;

namespace ACContentSynchronizer.ClientGui.Tasks {
  public class ValidationTask : TaskViewModel {
    public ValidationTask(ServerEntryViewModel server,
                          HubService hubService)
      : base(server,hubService) {
    }

    private void SetProgress(double progress) {
      Canceller.Token.ThrowIfCancellationRequested();
      Progress = progress;
    }

    public override void Run() {
      Canceller = new();
      Worker = Task.Run(async () => {
        try {
          Canceller.Token.ThrowIfCancellationRequested();
          Message = Localization.DownloadingManifest;
          var manifest = await DataService.DownloadManifest();

          Canceller.Token.ThrowIfCancellationRequested();
          Message = Localization.ManifestDownloaded;
          Message = Localization.ContentComparing;
          if (manifest != null) {
            var comparedManifest = DataService.CompareContent(Application.Settings.GamePath, manifest);

            if (comparedManifest.Cars.Any() || comparedManifest.Track != null) {
              Canceller.Token.ThrowIfCancellationRequested();
              Message = Localization.PreparingContent;
              await DataService.PrepareContent(comparedManifest);

              Canceller.Token.ThrowIfCancellationRequested();
              Message = Localization.PackContent;
              await DataService.PackContent();
              var applyTask = new Task(() => {
                if (Progress < 100) {
                }

                try {
                  Canceller.Token.ThrowIfCancellationRequested();
                  Message = Localization.Downloaded;
                  Message = Localization.TryingToSave;
                  DataService.SaveData();

                  Canceller.Token.ThrowIfCancellationRequested();
                  Message = Localization.ContentSaved;
                  Message = Localization.ApplyingChanges;
                  DataService.Apply(Application.Settings.GamePath);

                  Message = Localization.Done;
                } catch (Exception e) {
                  Message = $"{Localization.Error} {e.Message}";
                }
              });

              DataService.OnProgress += SetProgress;
              DataService.OnComplete += () => applyTask.Start();

              Canceller.Token.ThrowIfCancellationRequested();
              DataService.DownloadContent();
              await applyTask;
            } else {
              Message = Localization.ContentNoNeedToUpdate;
            }
          }
        } catch (OperationCanceledException) {
          Message = Localization.TaskCanceled;
          await DataService.CancelPack();
        } catch (Exception e) {
          Message = $"{Localization.Error}: {e.Message}";
        }
      });
    }

    public override void Dispose() {
      Worker.Dispose();
      Canceller.Dispose();
      DataService.Dispose();
    }
  }
}
