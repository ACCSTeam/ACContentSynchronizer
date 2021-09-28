using System;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Services;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.ClientGui.Views.ServerViews;
using ACContentSynchronizer.Models;

namespace ACContentSynchronizer.ClientGui.Tasks {
  public class UploadTask : TaskViewModel, IDisposable {
    private readonly ServerSettingsViewModel _content;

    public UploadTask(ServerEntryViewModel server,
                      HubService hubService,
                      ServerSettingsViewModel content)
      : base(server, hubService) {
      _content = content;
    }

    public override void Run() {
      Worker = Task.Run(async () => {
        try {
          await Application.SaveAsync();

          var manifest = new Manifest {
            Cars = _content.SelectedCars.Select(x => new EntryManifest(x.Path, DirectoryUtils.Size(x.Path),
              x.SelectedVariation
              ?? x.Variations.FirstOrDefault())).ToList(),
          };

          if (_content.SelectedTrack != null) {
            manifest.Track = new(_content.SelectedTrack.Path,
              DirectoryUtils.Size(_content.SelectedTrack.Path),
              _content.SelectedTrack.SelectedVariation);
          }

          Canceller.Token.ThrowIfCancellationRequested();
          Message = Localization.ContentComparing;
          var comparedManifest = await DataService.GetUpdateManifest(manifest);
          Message = Localization.PackContent;

          if (comparedManifest != null && (comparedManifest.Cars.Any() || comparedManifest.Track != null)) {
            await UpdateContent(comparedManifest);
          }

          Canceller.Token.ThrowIfCancellationRequested();
          Message = Localization.RefreshingServer;
          await DataService.RefreshServer(manifest);
          Message = Localization.ServerRefreshed;
        } catch (Exception e) {
          Message = $"{Localization.Error} {e.Message}";
        }
      });
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

    public override void Dispose() {
      Worker.Dispose();
      Canceller.Dispose();
      DataService.Dispose();
      _content.Dispose();
    }
  }
}
