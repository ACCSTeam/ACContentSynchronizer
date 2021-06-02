using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace ACContentSynchronizer.Server.Services {
  public class ServerConfigurationService {
    private readonly IConfiguration _configuration;

    public ServerConfigurationService(IConfiguration configuration) {
      _configuration = configuration;
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

    private string? GetStringValue(string path, string section, string key) {
      var serverCfgPath = Path.Combine(path, Constants.ServerCfg);
      var serverCfg = IniToDictionary(serverCfgPath);

      try {
        return serverCfg[section][key];
      } catch {
        return null;
      }
    }

    private string? GetServerPath() {
      var gamePath = _configuration.GetValue<string>("GamePath");
      var server = _configuration.GetValue<string>("Server");
      var serverPath = Path.Combine(gamePath, Constants.ServerPresetsPath, server);

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
  }
}
