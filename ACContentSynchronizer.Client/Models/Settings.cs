using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ACContentSynchronizer.Client.Models {
  public class Settings {
    private static Settings? _instance;

    public string GamePath { get; set; } = "";

    public string AdminPassword { get; set; } = "";

    public List<string> Servers { get; set; } = new();

    public static Settings Instance() {
      return _instance ??= Load();
    }

    public async Task SaveAsync() {
      var json = JsonSerializer.Serialize(this);
      await FileUtils.CreateIfNotExistsAsync(Constants.SettingPath);
      await File.WriteAllTextAsync(Constants.SettingPath, json);
    }

    public void Save() {
      var json = JsonSerializer.Serialize(this);
      FileUtils.CreateIfNotExists(Constants.SettingPath);
      File.WriteAllText(Constants.SettingPath, json);
    }

    private static Settings Load() {
      if (!File.Exists(Constants.SettingPath)) {
        return new Settings();
      }
      var json = File.ReadAllText(Constants.SettingPath);
      return JsonSerializer.Deserialize<Settings>(json) ?? new();
    }
  }
}
