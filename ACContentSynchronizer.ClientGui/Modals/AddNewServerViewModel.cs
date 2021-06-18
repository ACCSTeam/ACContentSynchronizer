using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ACContentSynchronizer.Client;
using ACContentSynchronizer.Client.Models;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.Views;
using Avalonia.Data;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Modals {
  public class AddNewServerViewModel : ModalViewModel<AddNewServer> {
    private readonly Regex _addressRegex = new($"^{Constants.Pattern}$");

    private string _ip = "";

    private string _password = "";

    public string Ip {
      get => _ip;
      set {
        if (!_addressRegex.IsMatch(value)) {
          throw new DataValidationException("Invalid server address");
        }

        this.RaiseAndSetIfChanged(ref _ip, value);
      }
    }

    public string Password {
      get => _password;
      set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public DateTime DateTime { get; set; } = DateTime.Now;

    public async Task Save() {
      var serverEntry = new ServerEntry {
        Ip = Ip,
        Password = Password,
        DateTime = DateTime,
      };

      try {
        using var dataReceiver = new DataReceiver(serverEntry.Http);
        serverEntry.Name = await dataReceiver.GetServerName();
      } catch (HttpRequestException e) {
        if (e.StatusCode == null) {
          Toast.Open("Cant connect to server");
        }
      } finally {
        Sidebar.Instance.Add(serverEntry);
        Instance.Close();
      }
    }
  }
}
