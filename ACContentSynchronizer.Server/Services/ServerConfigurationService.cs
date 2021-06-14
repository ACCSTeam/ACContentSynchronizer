using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ACContentSynchronizer.Models;
using ACContentSynchronizer.Server.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

namespace ACContentSynchronizer.Server.Services {
  public class ServerConfigurationService {
    private readonly IConfiguration _configuration;
    private readonly IHubContext<NotificationHub> _hub;

    public ServerConfigurationService(IConfiguration configuration,
                                      IHubContext<NotificationHub> hub) {
      _configuration = configuration;
      _hub = hub;
    }

    public void CheckAccess(string password) {
      var serverConfig = GetServerPath();

      if (!string.IsNullOrEmpty(serverConfig)) {
        var adminPassword = GetStringValue(serverConfig, "SERVER", "ADMIN_PASSWORD");

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

    public async Task<string?> UpdateConfig(Manifest manifest) {
      var presetPath = GetServerPath();

      if (!string.IsNullOrEmpty(presetPath)) {
        var serverConfig = GetServerConfig(presetPath);
        var entryList = new Dictionary<string, Dictionary<string, string>>();

        var gamePath = _configuration.GetValue<string>("GamePath");
        var contentPath = Path.Combine(gamePath, Constants.ContentFolder);

        if (!string.IsNullOrEmpty(manifest.Track?.Name)) {
          serverConfig["SERVER"]["TRACK"] = manifest.Track.Name;

          var trackPath = Path.Combine(contentPath, Constants.TracksFolder, manifest.Track.Name, "ui");
          var variants = Directory.GetDirectories(trackPath);

          if (variants.Any()) {
            serverConfig["SERVER"]["CONFIG_TRACK"] = variants.First();
          }
        }

        if (manifest.Cars.Any()) {
          serverConfig["SERVER"]["CARS"] = string.Join(';', manifest.Cars.Select(x => x.Name));

          for (var i = 0; i < manifest.Cars.Count; i++) {
            var car = manifest.Cars[i];
            var carPath = Path.Combine(contentPath, Constants.CarsFolder, car.Name, "skins");
            var skins = Directory.GetDirectories(carPath);

            if (!skins.Any()) {
              continue;
            }

            entryList.Add(
              $"CAR_{i}", new() {
                { "MODEL", car.Name },
                { "SKIN", new DirectoryInfo(skins.First()).Name },
                { "SPECTATOR_MODE", "0" },
                { "DRIVERNAME", "" },
                { "TEAM", "" },
                { "GUID", "" },
                { "BALLAST", "0" },
                { "RESTRICTOR", "0" },
              });
          }
        }

        await SaveConfig(presetPath, Constants.EntryList, entryList);
        await SaveConfig(presetPath, Constants.ServerCfg, serverConfig);
      }

      return presetPath;
    }

    public async Task RunServer(string presetPath) {
      var gamePath = _configuration.GetValue<string>("GamePath");
      var serverPath = Path.Combine(gamePath, "server");
      var serverExecutableName = "acServer";
      var serverExecutablePath = Path.Combine(serverPath, $"{serverExecutableName}.bat");

      var serverConfig = GetServerConfig(presetPath);
      var port = serverConfig["SERVER"]["HTTP_PORT"];

      var client = new HttpClient {
        BaseAddress = new($"http://localhost:{port}/"),
      };

      while (await ServerNotIsEmpty(client)) {
        await Task.Delay(TimeSpan.FromSeconds(5));
      }

      var runningProcess = Process.GetProcesses().FirstOrDefault(x => x.ProcessName == serverExecutableName);
      runningProcess?.Kill();

      ExecuteCommand(serverExecutablePath);
    }

    private static void ExecuteCommand(string command) {
      Task.Factory.StartNew(() => {
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

    private async Task<bool> ServerNotIsEmpty(HttpClient client) {
      try {
        var json = await client.GetStringAsync("INFO");
        var serverInfo = JsonSerializer.Deserialize<ServerInfo>(json, ContentUtils.JsonSerializerOptions);
        return serverInfo is { Clients: > 0 };
      } catch {
        return false;
      }
    }

    public string? GetTrackName() {
      var serverConfig = GetServerPath();
      return !string.IsNullOrEmpty(serverConfig)
        ? GetStringValue(serverConfig, "SERVER", "TRACK")
        : null;
    }

    public string[] GetCars() {
      var serverConfig = GetServerPath();
      if (string.IsNullOrEmpty(serverConfig)) {
        return Array.Empty<string>();
      }

      var cars = GetStringValue(serverConfig, "SERVER", "CARS");

      return string.IsNullOrEmpty(cars)
        ? Array.Empty<string>()
        : cars.Split(';');
    }

    private Dictionary<string, Dictionary<string, string>> GetServerConfig(string path) {
      var serverCfgPath = Path.Combine(path, Constants.ServerCfg);
      return IniToDictionary(serverCfgPath);
    }

    private string? GetStringValue(string path, string section, string key) {
      try {
        var serverCfg = GetServerConfig(path);
        return serverCfg[section][key];
      } catch {
        return null;
      }
    }

    private string? GetServerPath() {
      var gamePath = _configuration.GetValue<string>("GamePath");
      var preset = _configuration.GetValue<string>("Preset");
      var serverPath = Path.Combine(gamePath, Constants.ServerPresetsPath, preset);

      return Directory.Exists(serverPath)
        ? serverPath
        : null;
    }

    private Dictionary<string, Dictionary<string, string>> IniToDictionary(string path) {
      var lines = File.ReadAllLines(path).Where(line => !string.IsNullOrEmpty(line)).ToList();
      var lastIndex = 0;
      var sections = new List<int>();
      var ini = new Dictionary<string, Dictionary<string, string>>();

      while (true) {
        lastIndex = lines.FindIndex(lastIndex, line => line.StartsWith("["));

        if (lastIndex < 0) {
          break;
        }

        sections.Add(lastIndex);
        lastIndex += 1;
      }

      for (var i = 0; i < sections.Count; i++) {
        var index = sections[i];
        var nextIndex = sections.Count - 1 == i
          ? lines.Count
          : sections[i + 1];
        var count = nextIndex - sections[i] - 1;
        var sectionLines = lines.GetRange(index + 1, count);
        ini.Add(lines[index]
            .Replace("[", "")
            .Replace("]", ""),
          sectionLines.ToDictionary(entry => entry[..entry.IndexOf("=", StringComparison.Ordinal)],
            entry => entry[(entry.IndexOf("=", StringComparison.Ordinal) + 1)..]));
      }

      return ini;
    }

    private async Task SaveConfig(string path, string cfg, Dictionary<string, Dictionary<string, string>> data) {
      var cfgPath = Path.Combine(path, cfg);
      var config = new StringBuilder();

      await FileUtils.CreateIfNotExistsAsync(cfgPath);

      foreach (var (section, values) in data) {
        config.Append($"[{section}]\n");

        foreach (var (key, value) in values) {
          config.Append($"{key}={value}\n");
        }
      }

      await File.WriteAllTextAsync(cfgPath, config.ToString());
    }
  }
}
