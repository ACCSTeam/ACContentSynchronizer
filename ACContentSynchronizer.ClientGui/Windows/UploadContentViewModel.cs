using System;
using System.IO;
using System.Linq;
using System.Net.Http;
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
    private string _adminPassword = "";
    private bool _canClose = true;
    private double _progress;
    private string _progressMessage = "";

    public UploadContentViewModel() {
      var settings = Settings.Instance();
      var carsDirectory = Path.Combine(settings.GamePath, Constants.ContentFolder, Constants.CarsFolder);
      var tracksDirectory = Path.Combine(settings.GamePath, Constants.ContentFolder, Constants.TracksFolder);
      AdminPassword = settings.AdminPassword;

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

    public string ProgressMessage {
      get => _progressMessage;
      set => this.RaiseAndSetIfChanged(ref _progressMessage, $"{_progressMessage}{value}\n");
    }

    public string AdminPassword {
      get => _adminPassword;
      set => this.RaiseAndSetIfChanged(ref _adminPassword, value);
    }

    public ServerEntry Server { get; set; } = new();

    public void UploadContent() {
      Task.Factory.StartNew(async () => {
        try {
          CanClose = false;
          var dataReceiver = new DataReceiver(Server.Http);
          var settings = Settings.Instance();
          settings.AdminPassword = AdminPassword;
          await settings.SaveAsync();

          var manifest = new Manifest {
            Cars = SelectedCars.Select(x => new EntryManifest(x.Path, DirectoryUtils.Size(x.Path))).ToList(),
          };

          if (SelectedTrack != null) {
            manifest.Track = new(SelectedTrack.Path, DirectoryUtils.Size(SelectedTrack.Path));
          }

          var comparedManifest = await dataReceiver.GetUpdateManifest(manifest);
          if (comparedManifest != null) {
            dataReceiver.OnPack += Pack;

            if (comparedManifest.Cars.Any() || comparedManifest.Track != null) {
              await UpdateContent(dataReceiver, settings.AdminPassword, settings.GamePath, comparedManifest);
            }

            ProgressMessage = "Refreshing server...";
            await dataReceiver.RefreshServer(settings.AdminPassword, manifest);
            ProgressMessage = "Server refreshed";
          }
        } catch (Exception e) {
          ProgressMessage = e.ToString();
        } finally {
          CanClose = true;
        }
      });
    }

    private async Task UpdateContent(DataReceiver dataReceiver, string adminPassword,
                                     string gamePath, Manifest comparedManifest) {
      var content = ContentUtils.PrepareContent(gamePath, comparedManifest);
      content.OnProgress += Pack;
      await content.Pack(Constants.Client);
      var contentArchive = Path.Combine(Constants.Client, Constants.ContentArchive);

      ProgressMessage = "Uploading...";
      var client = await Hubs.NotificationHub<double, HubMethods>(Server, HubMethods.Progress, progress => {
        Progress = progress;
        return Task.CompletedTask;
      });

      await using var stream = File.OpenRead(contentArchive);
      await dataReceiver.Client.PostAsync($"updateContent?adminPassword={adminPassword}&client={client}",
        new StreamContent(stream));

      content.OnProgress -= Pack;
    }

    private void Pack(double progress, string entry) {
      Progress = progress;
      ProgressMessage = entry;
    }
  }
}
