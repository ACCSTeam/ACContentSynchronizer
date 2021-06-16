using System;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.Client.Models;
using ACContentSynchronizer.ClientGui.Models;
using Avalonia.Markup.Xaml;

namespace ACContentSynchronizer.ClientGui.Modals {
  public class DownloadModal : Modal {
    private readonly DownloadViewModel _vm;

    public DownloadModal() {
      DataContext = _vm = new() {
        Instance = this,
      };
      InitializeComponent();
    }

    public DownloadModal(DownloadViewModel vm) {
      DataContext = _vm = vm;
      _vm.Instance = this;
      InitializeComponent();
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);

      var sizeToContentWorkaroundSubscription =
        SizeToContentProperty.Changed.Subscribe(a => {
          if (SizeToContent != Avalonia.Controls.SizeToContent.Height) {
            SizeToContent = Avalonia.Controls.SizeToContent.Height;
          }
        });

      Download();
    }

    private void SetProgress(double progress) {
      _vm.Progress = progress;
    }

    private Task<string> SubscribeToProgress(ServerEntry server) {
      return Hubs.NotificationHub<double, string, HubMethods>(server, HubMethods.PackProgress,
        (progress, entry) => {
          _vm.Progress = progress;
          _vm.State = $"Packed {entry}";
          return Task.CompletedTask;
        });
    }

    private async Task Download() {
      try {
        var settings = Settings.Instance;
        var dataReceiver = _vm.DataReceiver;

        _vm.State = "Downloading manifest...";
        var manifest = await dataReceiver.DownloadManifest();
        _vm.State = "Manifest downloaded";

        if (manifest != null) {
          _vm.State = "Content comparing";
          var comparedManifest = dataReceiver.CompareContent(settings.GamePath, manifest);

          if (comparedManifest.Cars.Any() || comparedManifest.Track != null) {
            _vm.State = "Preparing content...";
            var clientId = await SubscribeToProgress(_vm.ServerEntry);
            var session = await dataReceiver.PrepareContent(comparedManifest, clientId);

            dataReceiver.OnProgress += SetProgress;
            dataReceiver.OnComplete += () => Task.Factory.StartNew(() => {
              try {
                _vm.State = "Downloaded";
                dataReceiver.OnProgress -= SetProgress;

                _vm.State = "Trying to save content...";
                dataReceiver.SaveData();
                _vm.State = "Content saved";

                _vm.State = "Applying changes...";
                dataReceiver.Apply(settings.GamePath);
                _vm.State = "Done!";
              } catch (Exception e) {
                _vm.State = $"ERROR: {e.Message}";
              } finally {
                _vm.CanClose = true;
              }
            });

            _vm.State = "Downloading content...";
            dataReceiver.DownloadContent(session, clientId);
          } else {
            _vm.State = "Content no need to update";
            _vm.CanClose = true;
          }
        }
      } catch (Exception e) {
        _vm.State = $"ERROR: {e.Message}";
        _vm.CanClose = true;
      }
    }
  }
}
