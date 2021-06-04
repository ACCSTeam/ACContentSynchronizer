using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ACContentSynchronizer.Client.Models {
  public class Settings {
    private static Settings _instance;

    public string GamePath { get; set; } = "";

    public List<string> Servers { get; set; } = new();

    public static Settings Instance() {
      return _instance ??= Load();
    }

    public async Task Save() {
      var json = JsonSerializer.Serialize(this);
      await FileUtils.CreateIfNotExists(Constants.SettingPath);
      await File.WriteAllTextAsync(Constants.SettingPath, json);
    }

    private static Settings Load() {
      if (!File.Exists(Constants.SettingPath)) {
        return new Settings();
      }
      var json = File.ReadAllText(Constants.SettingPath);
      return JsonSerializer.Deserialize<Settings>(json);
    }
  }
}
