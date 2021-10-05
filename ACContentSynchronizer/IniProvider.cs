using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACContentSynchronizer {
  public class IniProvider {
    private readonly string _folderPath;

    public IniProvider(string folderPath) {
      if (!Directory.Exists(folderPath)) {
        Directory.CreateDirectory(folderPath);
      }

      _folderPath = folderPath;
    }

    public IniFile GetConfig(string config) {
      var serverCfgPath = Path.Combine(_folderPath, $"{config}.ini");
      return IniToDictionary(serverCfgPath);
    }

    public IniFile GetServerConfig() {
      return GetConfig(Constants.ServerCfg);
    }

    public IniFile GetEntryList() {
      return GetConfig(Constants.EntryList);
    }

    private static IniFile IniToDictionary(string path) {
      if (!File.Exists(path)) {
        return new();
      }

      var lines = File.ReadAllLines(path)
        .Where(line => !string.IsNullOrEmpty(line))
        .ToList();

      var lastIndex = 0;
      var sections = new List<int>();
      var ini = new IniFile();

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
        ini.Add(RemoveComment(lines[index]
            .Replace("[", "")
            .Replace("]", "")),
          new(sectionLines.ToDictionary(entry => RemoveComment(entry[..entry.IndexOf("=", StringComparison.Ordinal)]),
            entry => RemoveComment(entry[(entry.IndexOf("=", StringComparison.Ordinal) + 1)..]))));
      }

      return ini;
    }

    private static string RemoveComment(string source) {
      var commentStartIndex = source.IndexOf(" ;", StringComparison.Ordinal);
      return commentStartIndex > -1
        ? source[..commentStartIndex]
        : source;
    }

    public async Task SaveConfig(string cfg, IniFile data) {
      var cfgPath = Path.Combine(_folderPath, $"{cfg}.ini");
      var config = new StringBuilder();

      await FileUtils.CreateIfNotExistsAsync(cfgPath);

      foreach (var (section, values) in data.Source) {
        config.Append($"[{section}]\n");

        foreach (var (key, value) in values.Source) {
          config.Append($"{key}={value}\n");
        }
      }

      await File.WriteAllTextAsync(cfgPath, config.ToString());
    }
  }
}
