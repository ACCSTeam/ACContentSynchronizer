using System;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Services;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.Models;

namespace ACContentSynchronizer.ClientGui.Tasks {
  public class ValidationTask : TaskViewModel {
    public ValidationTask(ServerEntryViewModel server,
                          HubService hubService)
      : base(server, hubService) {
    }

    private void SetProgress(double progress) {
      Progress = progress;
    }

    public override void Run() {
      Canceller = new();
      Worker = Task.Run(async () => {
        Task? applyTask = null;
        try {
          var manifest = await DownloadManifest();
          var comparedManifest = CompareManifest(manifest);
          if (comparedManifest.Cars.Any() || comparedManifest.Track != null) {
            await PrepareContent(comparedManifest);
            await PackContent();
            applyTask = CreateApplyTask();

            DataService.OnProgress += SetProgress;
            DataService.OnComplete += applyTask.Start;

            DownloadContent();
            await applyTask;
          } else {
            Message = Localization.ContentNoNeedToUpdate;
          }
        } catch (OperationCanceledException) {
          await CancelPack(applyTask);
        } catch (Exception e) {
          Message = $"{Localization.Error}: {e.Message}";
        }
      });
    }

    private async Task CancelPack(Task? applyTask) {
      Message = Localization.TaskCanceled;
      await DataService.CancelPack();
      DataService.OnProgress -= SetProgress;

      if (applyTask != null) {
        DataService.OnComplete -= applyTask.Start;
      }
    }

    private Task CreateApplyTask() {
      return new(() => {
        Message = Localization.Downloaded;
        try {
          SaveData();
          Apply();

          Message = Localization.Done;
        } catch (Exception e) {
          Message = $"{Localization.Error} {e.Message}";
        }
      });
    }

    private void DownloadContent() {
      Canceller.Token.ThrowIfCancellationRequested();
      DataService.DownloadContent();
    }

    private void Apply() {
      Canceller.Token.ThrowIfCancellationRequested();
      Message = Localization.ApplyingChanges;
      DataService.Apply(Application.Settings.GamePath);
    }

    private void SaveData() {
      Canceller.Token.ThrowIfCancellationRequested();
      Message = Localization.TryingToSave;
      DataService.SaveData();
      Message = Localization.ContentSaved;
    }

    private Task PackContent() {
      Canceller.Token.ThrowIfCancellationRequested();
      Message = Localization.PackContent;
      return DataService.PackContent();
    }

    private Task PrepareContent(Manifest comparedManifest) {
      Canceller.Token.ThrowIfCancellationRequested();
      Message = Localization.PreparingContent;
      return DataService.PrepareContent(comparedManifest);
    }

    private Manifest CompareManifest(Manifest manifest) {
      Canceller.Token.ThrowIfCancellationRequested();
      Message = Localization.ContentComparing;
      return DataService.CompareContent(Application.Settings.GamePath, manifest);
    }

    private Task<Manifest> DownloadManifest() {
      Canceller.Token.ThrowIfCancellationRequested();
      Message = Localization.DownloadingManifest;
      return DataService.DownloadManifest();
    }

    public override void Dispose() {
      Worker.Dispose();
      Canceller.Dispose();
      DataService.Dispose();
    }
  }
}
