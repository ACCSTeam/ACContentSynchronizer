using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Models;
using ACContentSynchronizer.ClientGui.ViewModels;
using ACContentSynchronizer.ClientGui.Views;
using Avalonia.Data;
using ReactiveUI;

namespace ACContentSynchronizer.ClientGui.Modals {
  public class AddNewServerViewModel : ModalViewModel<AddNewServer> {
    private readonly Regex _addressRegex = new($"^{Constants.Pattern}$");

    private string _ip = "";

    private string _password = "";

    private string _port = "";

    public string Ip {
      get => _ip;
      set {
        if (!_addressRegex.IsMatch(value)) {
          throw new DataValidationException("Invalid server address");
        }

        this.RaiseAndSetIfChanged(ref _ip, value);
      }
    }

    public string Port {
      get => _port;
      set => this.RaiseAndSetIfChanged(ref _port, value);
    }

    public string Password {
      get => _password;
      set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public DateTime DateTime { get; set; } = DateTime.Now;

    public async Task Save() {
      var serverEntry = new ServerEntry {
        Ip = Ip,
        Port = Port,
        Password = Password,
        DateTime = DateTime,
      };

      try {
        using var dataReceiver = new DataReceiver(serverEntry.Http);
        var serverProps = await dataReceiver.GetServerProps();

        serverEntry.Name = serverProps.Name;
        serverEntry.HttpPort = serverProps.HttpPort;
      } catch (HttpRequestException e) {
        if (e.StatusCode == null) {
          Toast.Open("Cant connect to server");
        }
      } finally {
        Sidebar.Instance.Save(serverEntry);
        Instance.Close();
      }
    }
  }
}
