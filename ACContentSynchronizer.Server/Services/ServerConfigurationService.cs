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
    private readonly ServerPresetService _presetService;

    public ServerConfigurationService(IConfiguration configuration,
                                      IHubContext<NotificationHub> hub,
                                      ServerPresetService presetService) {
      _configuration = configuration;
      _hub = hub;
      _presetService = presetService;
    }

    public void CheckAccess(string password) {
      var serverConfig = _presetService.GetServerPath();

      if (!string.IsNullOrEmpty(serverConfig)) {
        var adminPassword = _presetService.GetStringValue(serverConfig, "SERVER", "ADMIN_PASSWORD");

        if (adminPassword == password) {
          return;
        }
      }

      throw new("");
    }

    public async Task GetArchive(HttpRequest request, string connectionId, string client) {
      DirectoryUtils.DeleteIfExists(connectionId, true);
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
      var presetPath = _presetService.GetServerPath();

      if (!string.IsNullOrEmpty(presetPath)) {
        var serverConfig = _presetService.GetServerConfig(presetPath);
        var entryList = new Dictionary<string, Dictionary<string, string>>();

        var gamePath = _configuration.GetValue<string>("GamePath");
        var contentPath = Path.Combine(gamePath, Constants.ContentFolder);

        if (!string.IsNullOrEmpty(manifest.Track?.Name)) {
          serverConfig["SERVER"]["TRACK"] = manifest.Track.Name;

          var trackPath = Path.Combine(contentPath, Constants.TracksFolder, manifest.Track.Name, "ui");
          var variants = Directory.GetDirectories(trackPath);

          if (variants.Any() || !string.IsNullOrEmpty(manifest.Track.SelectedVariation)) {
            serverConfig["SERVER"]["CONFIG_TRACK"] = manifest.Track.SelectedVariation
                                                     ?? DirectoryUtils.Name(variants.First());
          }
        }

        if (manifest.Cars.Any()) {
          serverConfig["SERVER"]["CARS"] = string.Join(';', manifest.Cars
            .DistinctBy(x => x.Name)
            .Select(x => x.Name));

          for (var i = 0; i < manifest.Cars.Count; i++) {
            var car = manifest.Cars[i];
            var carPath = Path.Combine(contentPath, Constants.CarsFolder, car.Name, "skins");
            var skins = Directory.GetDirectories(carPath);

            if (!skins.Any() && string.IsNullOrEmpty(car.SelectedVariation)) {
              continue;
            }

            entryList.Add(
              $"CAR_{i}", new() {
                { "MODEL", car.Name },
                { "SKIN", car.SelectedVariation ?? DirectoryUtils.Name(skins.First()) },
                { "SPECTATOR_MODE", "0" },
                { "DRIVERNAME", "" },
                { "TEAM", "" },
                { "GUID", "" },
                { "BALLAST", "0" },
                { "RESTRICTOR", "0" },
              });
          }
        }

        await _presetService.SaveConfig(presetPath, Constants.EntryList, entryList);
        await _presetService.SaveConfig(presetPath, Constants.ServerCfg, serverConfig);
      }
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

      ExecuteCommand(serverExecutablePath);
    }

    private static void ExecuteCommand(string command) {
      Task.Run(() => {
        ProcessStartInfo processInfo = new("cmd.exe", $"/k \"{command}\"") {
          CreateNoWindow = true,
          UseShellExecute = false,
          RedirectStandardError = true,
          RedirectStandardOutput = true,
          WorkingDirectory = Directory.GetDirectoryRoot(command),
        };

        var process = Process.Start(processInfo);
        process?.WaitForExit();

        var output = process?.StandardOutput.ReadToEnd();
        var error = process?.StandardError.ReadToEnd();

        var exitCode = process?.ExitCode;

        Console.WriteLine("output>>" + (string.IsNullOrEmpty(output) ? "(none)" : output));
        Console.WriteLine("error>>" + (string.IsNullOrEmpty(error) ? "(none)" : error));
        Console.WriteLine("ExitCode: " + exitCode, "ExecuteCommand");
        process?.Close();
      });
    }

    public Dictionary<string, Dictionary<string, string>>? GetServerInfo() {
      var presetPath = _presetService.GetServerPath();

      return !string.IsNullOrEmpty(presetPath)
        ? _presetService.GetServerConfig(presetPath)
        : null;
    }

    public async Task<IEnumerable<CarsUpdate>?> GetCarsUpdate(long steamId) {
      var presetPath = _presetService.GetServerPath();

      if (string.IsNullOrEmpty(presetPath)) {
        return null;
      }

      var serverConfig = _presetService.GetEntriesConfig(presetPath);
      var cars = serverConfig.GroupBy(x => x.Value["MODEL"])
        .ToList();

      try {
        var state = await Client().GetServerState(steamId);
        var stateCars = state.Cars.GroupBy(x => x.Model);

        return cars.Select(x => new CarsUpdate {
          Name = x.Key,
          Count = x.Count(),
          Allowed = stateCars.Where(s => s.Key == x.Key)
            .Sum(s => s.Count(c => !c.IsConnected)),
        });
      } catch {
        return cars.Select(x => new CarsUpdate {
          Name = x.Key,
          Count = x.Count(),
        });
      }
    }

    private string GetLocalPort() {
      const string defaultPort = "8081";
      var serverConfig = _presetService.GetServerPath();
      var port = !string.IsNullOrEmpty(serverConfig)
        ? _presetService.GetStringValue(serverConfig, "SERVER", "HTTP_PORT")
          ?? defaultPort
        : defaultPort;

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
      var serverConfig = _presetService.GetServerPath();
      return !string.IsNullOrEmpty(serverConfig)
        ? _presetService.GetStringValue(serverConfig, "SERVER", "TRACK")
        : null;
    }

    public ServerProps? GetServerProps() {
      var serverConfig = _presetService.GetServerPath();
      return !string.IsNullOrEmpty(serverConfig)
        ? new ServerProps {
          Name = _presetService.GetStringValue(serverConfig,
            "SERVER",
            "NAME") ?? "",
          HttpPort = _presetService.GetStringValue(serverConfig,
            "SERVER",
            "HTTP_PORT") ?? "",
        }
        : null;
    }

    public string[] GetCars() {
      var serverConfig = _presetService.GetServerPath();
      if (string.IsNullOrEmpty(serverConfig)) {
        return Array.Empty<string>();
      }

      var cars = _presetService.GetStringValue(serverConfig, "SERVER", "CARS");

      return string.IsNullOrEmpty(cars)
        ? Array.Empty<string>()
        : cars.Split(';');
    }
  }
}
