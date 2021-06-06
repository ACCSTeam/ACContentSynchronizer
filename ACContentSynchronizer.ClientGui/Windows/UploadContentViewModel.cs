using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.Client;
using ACContentSynchronizer.Client.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.Models;
using Avalonia.Collections;

namespace ACContentSynchronizer.ClientGui.Windows {
  public class UploadContentViewModel : ViewModelBase {
    public UploadContentViewModel() {
      var settings = Settings.Instance();
      var carsDirectory = Path.Combine(settings.GamePath, Constants.ContentFolder, Constants.CarsFolder);
      var tracksDirectory = Path.Combine(settings.GamePath, Constants.ContentFolder, Constants.TracksFolder);

      if (Directory.Exists(carsDirectory)) {
        Cars.AddRange(Directory.GetDirectories(carsDirectory).Select(x => new EntryInfo(x)));
      }

      if (Directory.Exists(tracksDirectory)) {
        Tracks.AddRange(Directory.GetDirectories(tracksDirectory).Select(x => new EntryInfo(x)));
      }
    }

    public AvaloniaList<EntryInfo> Cars { get; set; } = new();

    public AvaloniaList<EntryInfo> Tracks { get; set; } = new();

    public AvaloniaList<EntryInfo> SelectedCars { get; set; } = new();

    public EntryInfo? SelectedTrack { get; set; }

    public string Http { get; set; } = "";

    public async Task UploadContent() {
      var dataReceiver = new DataReceiver(Http);
      var settings = Settings.Instance();
      var manifest = new Manifest {
        Cars = SelectedCars.Select(x => new EntryManifest(x.Path, DirectoryUtils.Size(x.Path))).ToList(),
      };

      if (SelectedTrack != null) {
        manifest.Track = new EntryManifest(SelectedTrack.Path, DirectoryUtils.Size(SelectedTrack.Path));
      }

      var comparedManifest = await dataReceiver.GetUpdateManifest(manifest);
      var updateManifest = new UpdateManifest {
        Manifest = manifest,
      };

      if (comparedManifest != null) {
        await dataReceiver.UpdateContent(settings.AdminPassword, settings.GamePath, comparedManifest, updateManifest);
      }
    }
  }
}
