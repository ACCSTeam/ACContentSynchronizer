using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.ClientGui.Views;
using ACContentSynchronizer.Models;

namespace ACContentSynchronizer.ClientGui.Tasks {
  public class UploadTask : TaskViewModel {
    private readonly UploadViewModel _content;
    private readonly ServerEntry _server;

    public UploadTask(ServerEntry server, UploadViewModel content) {
      _server = server;
      _content = content;
    }

    public override void Dispose() {
      _content.Dispose();
      Worker.Dispose();
    }

    public override void Run() {
      Worker = Task.Run(async () => {
        try {
          var dataReceiver = new DataReceiver(_server.Http);
          var settings = Settings.Instance;
          await settings.SaveAsync();

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

          State = "Content comparing";
          var comparedManifest = await dataReceiver.GetUpdateManifest(manifest);
          State = "Pack content...";

          if (comparedManifest != null && (comparedManifest.Cars.Any() || comparedManifest.Track != null)) {
            await UpdateContent(dataReceiver, settings.GamePath, comparedManifest);
          }

          State = "Refreshing server...";
          await dataReceiver.RefreshServer(_server.Password, manifest);
          State = "Server refreshed";
        } catch (Exception e) {
          State = $"ERROR: {e.Message}";
        }
      });
    }

    private async Task UpdateContent(DataReceiver dataReceiver, string gamePath, Manifest manifest) {
      var content = ContentUtils.PrepareContent(gamePath, manifest);
      content.OnProgress += Pack;
      await content.Pack(Constants.Client);
      var contentArchive = Path.Combine(Constants.Client, Constants.ContentArchive);

      State = "Uploading...";
      var client = await Hubs.NotificationHub<double, HubMethods>(_server, HubMethods.Progress, progress => {
        Progress = progress;
        return Task.CompletedTask;
      });

      await using var stream = File.OpenRead(contentArchive);
      await dataReceiver.Client.PostAsync($"updateContent?adminPassword={_server.Password}&client={client}",
        new StreamContent(stream));

      content.OnProgress -= Pack;
    }

    private void Pack(double progress, string entry) {
      Progress = progress;
      State = entry;
    }

    public override void Cancel() {
    }
  }
}
