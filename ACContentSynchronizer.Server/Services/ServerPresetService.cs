using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ACContentSynchronizer.Server.Services {
  public class ServerPresetService {
    private readonly IConfiguration _configuration;

    public ServerPresetService(IConfiguration configuration) {
      _configuration = configuration;
    }

    public string? GetServerPath() {
      var gamePath = _configuration.GetValue<string>("GamePath");
      var preset = _configuration.GetValue<string>("Preset");
      var serverPath = Path.Combine(gamePath, Constants.ServerPresetsPath, preset);

      return Directory.Exists(serverPath)
        ? serverPath
        : null;
    }

    public Dictionary<string, Dictionary<string, string>> GetServerConfig(string path) {
      var serverCfgPath = Path.Combine(path, Constants.ServerCfg);
      return IniToDictionary(serverCfgPath);
    }

    public Dictionary<string, Dictionary<string, string>> GetEntriesConfig(string path) {
      var serverCfgPath = Path.Combine(path, Constants.EntryList);
      return IniToDictionary(serverCfgPath);
    }

    private static Dictionary<string, Dictionary<string, string>> IniToDictionary(string path) {
      var lines = File.ReadAllLines(path)
        .Where(line => !string.IsNullOrEmpty(line))
        .ToList();

      var lastIndex = 0;
      var sections = new List<int>();
      var ini = new Dictionary<string, Dictionary<string, string>>();

      while (true) {
        lastIndex = lines.FindIndex(lastIndex,
          line => line.StartsWith("["));

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

    public string? GetStringValue(string path, string section, string key) {
      try {
        var serverCfg = GetServerConfig(path);
        return serverCfg[section][key];
      } catch {
        return null;
      }
    }

    public async Task SaveConfig(string path, string cfg, Dictionary<string, Dictionary<string, string>> data) {
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
