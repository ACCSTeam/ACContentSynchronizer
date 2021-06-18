using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ACContentSynchronizer.ClientGui.Models {
  public class Settings {
    private static Settings? _instance;

    public string GamePath { get; set; } = "";

    public string PlayerName { get; set; } = "";

    public long SteamId { get; set; }

    public bool SidebarMinimized { get; set; }

    public List<ServerEntry> Servers { get; set; } = new();

    public static Settings Instance => _instance ??= Load();

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
        return new();
      }
      var json = File.ReadAllText(Constants.SettingPath);
      return JsonSerializer.Deserialize<Settings>(json) ?? new();
    }
  }
}
