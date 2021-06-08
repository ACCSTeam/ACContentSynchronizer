using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ACContentSynchronizer.Models;
using Microsoft.Extensions.Configuration;

namespace ACContentSynchronizer.Server.Services {
  public class ServerConfigurationService {
    private readonly IConfiguration _configuration;

    public ServerConfigurationService(IConfiguration configuration) {
      _configuration = configuration;
    }

    public void CheckAccess(string password) {
      var serverConfig = GetServerPath();

      if (!string.IsNullOrEmpty(serverConfig)) {
        var adminPassword = GetStringValue(serverConfig, "SERVER", "ADMIN_PASSWORD");

        if (adminPassword == password) {
          return;
        }
      }

      throw new Exception("");
    }

    public async Task GetArchive(byte[] data) {
      FileUtils.DeleteIfExists(Constants.ContentArchive);
      DirectoryUtils.DeleteIfExists(Constants.DownloadsPath, true);
      await FileUtils.CreateIfNotExistsAsync(Constants.ContentArchive);
      await File.WriteAllBytesAsync(Constants.ContentArchive, data);
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
      var corps = "ACContentSynchronizer.GrandChildForKill";
      var executableCorps = $"{corps}.exe";
      var gamePath = _configuration.GetValue<string>("GamePath");
      var serverPath = Path.Combine(gamePath, "server");
      var serverExecutableName = "acServer";
      var serverExecutablePath = Path.Combine(serverPath, $"{serverExecutableName}.exe");
      var serverCfgPath = Path.Combine(presetPath, Constants.ServerCfg);
      var entryListPath = Path.Combine(presetPath, Constants.EntryList);

      var serverConfig = GetServerConfig(presetPath);
      var port = serverConfig["SERVER"]["HTTP_PORT"];

      var client = new HttpClient {
        BaseAddress = new Uri($"http://localhost:{port}/"),
      };

      while (await ServerNotIsEmpty(client)) {
        await Task.Delay(TimeSpan.FromSeconds(5));
      }

      var runningProcess = Process.GetProcesses().FirstOrDefault(x => x.ProcessName == serverExecutableName);
      runningProcess?.Kill();

      var process = new Process {
        StartInfo = {
          FileName = executableCorps,
          Arguments = $"\"{serverExecutablePath}\" \"{serverCfgPath}\" \"{entryListPath}\"",
          UseShellExecute = false,
          WorkingDirectory = Path.GetDirectoryName(executableCorps) ?? "",
          RedirectStandardOutput = true,
          CreateNoWindow = true,
          RedirectStandardError = true,
          StandardOutputEncoding = Encoding.UTF8,
          StandardErrorEncoding = Encoding.UTF8,
        },
      };

      process.Start();
      var corpsProcess = Process.GetProcesses().FirstOrDefault(x => x.ProcessName == corps);
      corpsProcess?.Kill();
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

      // var backupPath = Path.Combine(path, "backup");
      // DirectoryUtils.CreateIfNotExists(backupPath);
      //
      // var now = DateTime.Now.ToString("yyyy-MM-dd_hh:mm:ss");
      // backupPath = Path.Combine(backupPath, now);
      // DirectoryUtils.CreateIfNotExists(backupPath);

      // File.Move(serverCfgPath, Path.Combine(backupPath, Constants.ServerCfg));

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
