using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Services;
using ACContentSynchronizer.ClientGui.ViewModels;
using Splat;

namespace ACContentSynchronizer.ClientGui.Modals {
  public class AddNewServerViewModel : ModalViewModel<AddNewServer> {
    private readonly Regex _addressRegex = new($"^{Constants.Pattern}$");
    private readonly ApplicationViewModel _application;

    public AddNewServerViewModel() {
      _application = Locator.Current.GetService<ApplicationViewModel>();
    }

    public ServerEntryViewModel Server { get; set; } = new();

    public async Task Save() {
      try {
        var dataService = new DataService(Server);
        var serverProps = await dataService.GetServerProps();

        if (serverProps != null) {
          Server.Name = serverProps.Name;
          Server.KunosPort = serverProps.HttpPort;
        }
      } catch (HttpRequestException e) {
        if (e.StatusCode == null) {
          Toast.Open("Cant connect to server");
        }
      } finally {
        _application.Servers.Add(Server);
        ControlInstance.Close();
      }
    }
  }
}
