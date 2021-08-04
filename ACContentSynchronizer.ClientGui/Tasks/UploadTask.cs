using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Components.Server;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.Models;

namespace ACContentSynchronizer.ClientGui.Tasks {
  public class UploadTask : TaskViewModel {
    private readonly UploadViewModel _content;

    public UploadTask(ServerEntry server, UploadViewModel content) : base(server) {
      _content = content;
    }

    public override void Run() {
      Worker = Task.Run(async () => {
        try {
          var dataReceiver = new DataReceiver(Server.Http);
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

          Canceller.Token.ThrowIfCancellationRequested();
          State = "Content comparing";
          var comparedManifest = await dataReceiver.GetUpdateManifest(manifest);
          State = "Pack content...";

          if (comparedManifest != null && (comparedManifest.Cars.Any() || comparedManifest.Track != null)) {
            await UpdateContent(dataReceiver, settings.GamePath, comparedManifest);
          }

          Canceller.Token.ThrowIfCancellationRequested();
          State = "Refreshing server...";
          await dataReceiver.RefreshServer(Server.Password, manifest);
          State = "Server refreshed";
        } catch (Exception e) {
          State = $"ERROR: {e.Message}";
        }
      });
    }

    private async Task UpdateContent(DataReceiver dataReceiver, string gamePath, Manifest manifest) {
      Canceller.Token.ThrowIfCancellationRequested();
      var content = ContentUtils.PrepareContent(gamePath, manifest);
      content.OnProgress += Pack;

      Canceller.Token.ThrowIfCancellationRequested();
      await content.Pack(Constants.Client);
      var contentArchive = Path.Combine(Constants.Client, Constants.ContentArchive);

      Canceller.Token.ThrowIfCancellationRequested();
      State = "Uploading...";
      await using var stream = File.OpenRead(contentArchive);
      await dataReceiver.Client.PostAsync($"updateContent?adminPassword={Server.Password}&client={ClientId}",
        new StreamContent(stream));

      content.OnProgress -= Pack;
    }

    private void Pack(double progress, string entry) {
      Progress = progress;
      State = entry;
    }
  }
}
