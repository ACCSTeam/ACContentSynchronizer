using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.Extensions;
using ACContentSynchronizer.Models;
using ACContentSynchronizer.Server.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

namespace ACContentSynchronizer.Server.Services {
  public class ServerConfigurationService {
    private readonly IConfiguration _configuration;
    private readonly IHubContext<NotificationHub> _hub;
    private readonly IniProvider _iniProvider;

    public ServerConfigurationService(IConfiguration configuration,
                                      IHubContext<NotificationHub> hub) {
      _configuration = configuration;
      _hub = hub;

      var gamePath = _configuration.GetValue<string>("GamePath");
      var preset = _configuration.GetValue<string>("Preset");
      _iniProvider = new(Path.Combine(gamePath, Constants.ServerPresetsPath, preset));
    }

    public void CheckAccess(string password) {
      var adminPassword = _iniProvider.GetStringValue("SERVER", "ADMIN_PASSWORD");

      if (adminPassword == password) {
        return;
      }

      throw new("");
    }

    public async Task GetArchive(HttpRequest request, string connectionId, string client) {
      DirectoryUtils.DeleteIfExists(connectionId);
      DirectoryUtils.CreateIfNotExists(connectionId);

      var sessionPath = Path.Combine(connectionId, Constants.ContentArchive);
      var data = request.Body;
      var length = request.ContentLength ?? 0;

      await FileUtils.CreateIfNotExistsAsync(sessionPath);
      await using var stream = File.OpenWrite(sessionPath);

      byte[] buffer = ArrayPool<byte>.Shared.Rent(Constants.BufferSize);
      try {
        while (true) {
          var bytesRead = await data.ReadAsync(new(buffer)).ConfigureAwait(false);
          if (bytesRead == 0) {
            break;
          }
          await stream.WriteAsync(new(buffer, 0, bytesRead)).ConfigureAwait(false);
          SendProgress(client, length, stream.Length);
        }
      } finally {
        ArrayPool<byte>.Shared.Return(buffer);
      }

      stream.Close();
    }

    private void SendProgress(string client, long length, long read) {
      var progress = (double) read / length * 100;
      _hub.Clients.Client(client).SendAsync(HubMethods.Progress.ToString(), progress);
    }

    public async Task UpdateConfig(Manifest manifest) {
      var serverConfig = _iniProvider.GetServerConfig();
      var entryList = new IniFile();

      if (!string.IsNullOrEmpty(manifest.Track?.Name)) {
        serverConfig["SERVER"]["TRACK"] = manifest.Track.Name;

        serverConfig["SERVER"]["CONFIG_TRACK"] = manifest.Track.SelectedVariation ?? "";
      }

      if (manifest.Cars.Any()) {
        serverConfig["SERVER"]["CARS"] = string.Join(';', manifest.Cars
          .DistinctBy(x => x.Name)
          .Select(x => x.Name));

        for (var i = 0; i < manifest.Cars.Count; i++) {
          var car = manifest.Cars[i];

          if (string.IsNullOrEmpty(car.SelectedVariation)) {
            continue;
          }

          entryList.Add(
            $"CAR_{i}", new(new() {
              { "MODEL", car.Name },
              { "SKIN", car.SelectedVariation },
              { "SPECTATOR_MODE", "0" },
              { "DRIVERNAME", "" },
              { "TEAM", "" },
              { "GUID", "" },
              { "BALLAST", "0" },
              { "RESTRICTOR", "0" },
            }));
        }
      }

      await _iniProvider.SaveConfig(Constants.EntryList, entryList);
      await _iniProvider.SaveConfig(Constants.ServerCfg, serverConfig);
    }

    public async Task RunServer() {
      var gamePath = _configuration.GetValue<string>("GamePath");
      var serverPath = Path.Combine(gamePath, "server");
      var serverExecutableName = "acServer";
      var serverExecutablePath = Path.Combine(serverPath, $"{serverExecutableName}.bat");

      while (await ServerNotIsEmpty()) {
        await Task.Delay(TimeSpan.FromSeconds(5));
      }

      var runningProcess = Process.GetProcesses().FirstOrDefault(x => x.ProcessName == serverExecutableName);
      runningProcess?.Kill();

      ContentUtils.ExecuteCommand(serverExecutablePath);
    }

    public IniFile GetServerInfo() {
      return _iniProvider.GetServerConfig();
    }

    private string GetLocalPort() {
      const string defaultPort = "8081";
      var port = _iniProvider.GetStringValue("SERVER", "HTTP_PORT")
                 ?? defaultPort;

      return port;
    }

    private KunosClient Client() {
      var port = GetLocalPort();
      return new("localhost", port);
    }

    private async Task<bool> ServerNotIsEmpty() {
      try {
        var serverInfo = await Client().GetServerInfo();
        return serverInfo is { Clients: > 0 };
      } catch {
        return false;
      }
    }

    public string? GetTrackName() {
      return _iniProvider.GetStringValue("SERVER", "TRACK");
    }

    public ServerProps GetServerProps() {
      return new() {
        Name = _iniProvider.GetStringValue("SERVER",
          "NAME") ?? "",
        HttpPort = _iniProvider.GetStringValue("SERVER",
          "HTTP_PORT") ?? "",
      };
    }

    public string[] GetCars() {
      var cars = _iniProvider.GetStringValue("SERVER", "CARS");

      return string.IsNullOrEmpty(cars)
        ? Array.Empty<string>()
        : cars.Split(';');
    }
  }
}
