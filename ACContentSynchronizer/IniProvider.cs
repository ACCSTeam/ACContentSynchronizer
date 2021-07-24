using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACContentSynchronizer {
  public class IniProvider {
    private readonly string _serverPath;

    public IniProvider(string serverPath) {
      _serverPath = Directory.Exists(serverPath)
        ? serverPath
        : throw new("Wrong server path");
    }

    public Dictionary<string, Dictionary<string, string>> GetConfig(string config) {
      var serverCfgPath = Path.Combine(_serverPath, config);
      return IniToDictionary(serverCfgPath);
    }

    public Dictionary<string, Dictionary<string, string>> GetServerConfig() {
      return GetConfig(Constants.ServerCfg);
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

    public string? GetStringValue(string section, string key) {
      try {
        var serverCfg = GetServerConfig();
        return serverCfg[section][key];
      } catch {
        return null;
      }
    }

    public async Task SaveConfig(string cfg, Dictionary<string, Dictionary<string, string>> data) {
      var cfgPath = Path.Combine(_serverPath, cfg);
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
