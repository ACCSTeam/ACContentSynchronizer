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
        try {
          Canceller.Token.ThrowIfCancellationRequested();
          Message = Localization.ContentComparing;
          var comparedManifest = await DataService.GetUpdateManifest(Manifest);
          Message = Localization.PackContent;

          if (comparedManifest != null && (comparedManifest.Cars.Any() || comparedManifest.Track != null)) {
            await UpdateContent(comparedManifest);
          }

          Canceller.Token.ThrowIfCancellationRequested();
          Message = Localization.RefreshingServer;
          await DataService.RefreshServer(Manifest);
          Message = Localization.ServerRefreshed;
        } catch (Exception e) {
          Message = $"{Localization.Error} {e.Message}";
        }
      });
    }

    public override void Dispose() {

    }

    private async Task UpdateContent(Manifest manifest) {
      Canceller.Token.ThrowIfCancellationRequested();
      var content = ContentService.PrepareContent(Application.Settings.GamePath, manifest);
      content.OnProgress += Pack;

      Canceller.Token.ThrowIfCancellationRequested();
      await content.Pack(Constants.Client);

      Canceller.Token.ThrowIfCancellationRequested();
      Message = Localization.Uploading;

      await DataService.UpdateContent();
      content.OnProgress -= Pack;
    }

    private void Pack(double progress, string entry) {
      Progress = progress;
      Message = entry;
    }
  }
}
