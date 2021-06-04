using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public async Task UpdateConfig(string? track, List<string> cars) {
      var serverConfigPath = GetServerPath();

      if (!string.IsNullOrEmpty(serverConfigPath)) {
        var serverConfig = GetServerConfig(serverConfigPath);

        if (!string.IsNullOrEmpty(track)) {
          serverConfig["SERVER"]["TRACK"] = track;
        }

        if (cars.Any()) {
          serverConfig["SERVER"]["CARS"] = string.Join(';', cars);
        }

        await SaveConfig(serverConfigPath, serverConfig);
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
          ? lines.Count - 1
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

    private async Task SaveConfig(string path, Dictionary<string, Dictionary<string, string>> data) {
      var serverCfgPath = Path.Combine(path, Constants.ServerCfg);
      var now = DateTime.Now.ToString("yyyy-MM-dd_hh:mm:ss");
      var backupPath = Path.Combine(path, "backup", now);
      var config = new StringBuilder();

      DirectoryUtils.CreateIfNotExists(backupPath);

      File.Move(serverCfgPath, Path.Combine(backupPath, Constants.ServerCfg));

      await FileUtils.CreateIfNotExists(serverCfgPath);

      foreach (var (section, values) in data) {
        config.Append($"[{section}]\n");

        foreach (var (key, value) in values) {
          config.Append($"{key}={value}\n");
        }
      }

      await File.WriteAllTextAsync(serverCfgPath, config.ToString());
    }
  }
}
