using System;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Services;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.Models;

namespace ACContentSynchronizer.ClientGui.Tasks {
  public class ServerConfigurationTask : TaskViewModel {
    public ServerConfigurationTask(ServerEntryViewModel server,
                                   HubService hubService)
      : base(server, hubService) {
    }

    public UploadManifest Manifest { get; set; } = new();

    public override void Run() {
      Worker = Task.Run(async () => {
        AvailableContent? content = null;
        try {
          var comparedManifest = await GetUpdateManifest();
          if (comparedManifest != null && (comparedManifest.Cars.Any() || comparedManifest.Track != null)) {
            content = PrepareContent(comparedManifest);
            await PackContent(content);
            await UploadContent(content);
          }

          await RefreshServer();
          Message = Localization.Done;
        } catch (OperationCanceledException) {
          content?.AbortPacking();
        }
        catch (Exception e) {
          Message = $"{Localization.Error} {e.Message}";
        }
      });
    }

    private Task RefreshServer() {
      Canceller.Token.ThrowIfCancellationRequested();
      Message = Localization.RefreshingServer;
      return DataService.RefreshServer(Manifest);
    }

    private Task UploadContent(AvailableContent content) {
      Canceller.Token.ThrowIfCancellationRequested();
      Message = Localization.Uploading;
      content.OnProgress -= ProgressEvent;
      return DataService.UpdateContent();
    }

    private Task PackContent(AvailableContent content) {
      Canceller.Token.ThrowIfCancellationRequested();
      Message = Localization.PackContent;
      content.OnProgress += ProgressEvent;
      return content.Pack(Constants.Client);
    }

    private AvailableContent PrepareContent(Manifest comparedManifest) {
      Canceller.Token.ThrowIfCancellationRequested();
      Message = Localization.PreparingContent;
      return ContentService.PrepareContent(Application.Settings.GamePath, comparedManifest);
    }

    private Task<Manifest?> GetUpdateManifest() {
      Canceller.Token.ThrowIfCancellationRequested();
      Message = Localization.ContentComparing;
      return DataService.GetUpdateManifest(Manifest);
    }

    public override void Dispose() {
    }

    private void ProgressEvent(double progress, string entry) {
      Progress = progress;
      Message = entry;
    }
  }
}
