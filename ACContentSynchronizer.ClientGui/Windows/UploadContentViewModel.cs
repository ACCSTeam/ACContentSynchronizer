using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.Client;
using ACContentSynchronizer.Client.Models;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.Models;
using Avalonia.Collections;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Windows {
  public class UploadContentViewModel : ViewModelBase {
    private bool _canClose;
    private double _progress;
    private string _progressEntry = "";

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

    public bool CanClose {
      get => _canClose;
      set => this.RaiseAndSetIfChanged(ref _canClose, value);
    }

    public double Progress {
      get => _progress;
      set => this.RaiseAndSetIfChanged(ref _progress, value);
    }

    public string ProgressEntry {
      get => _progressEntry;
      set => this.RaiseAndSetIfChanged(ref _progressEntry, value);
    }

    public ServerEntry Server { get; set; } = new();

    public void UploadContent() {
      Task.Factory.StartNew(async () => {
        try {
          CanClose = false;
          var dataReceiver = new DataReceiver(Server.Http);
          var settings = Settings.Instance();
          var manifest = new Manifest {
            Cars = SelectedCars.Select(x => new EntryManifest(x.Path, DirectoryUtils.Size(x.Path))).ToList(),
          };

          if (SelectedTrack != null) {
            manifest.Track = new(SelectedTrack.Path, DirectoryUtils.Size(SelectedTrack.Path));
          }

          var comparedManifest = await dataReceiver.GetUpdateManifest(manifest);
          if (comparedManifest != null) {
            dataReceiver.OnPack += Pack;

            await dataReceiver.UpdateContent(settings.AdminPassword, settings.GamePath, comparedManifest);
            await dataReceiver.RefreshServer(settings.AdminPassword, manifest);
          }
        } finally {
          CanClose = true;
        }
      });
    }

    private void Pack(double progress, string entry) {
      Progress = progress;
      ProgressEntry = entry;
    }
  }
}
