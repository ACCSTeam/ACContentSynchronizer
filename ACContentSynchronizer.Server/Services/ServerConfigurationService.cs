using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.Extensions;
using ACContentSynchronizer.Models;
using ACContentSynchronizer.Server.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ACContentSynchronizer.Server.Services {
  public class ServerConfigurationService {
    private readonly IConfiguration _configuration;
    private readonly ContentService _content;
    private readonly IniProvider _iniProvider;
    private readonly SignalRService _signalRService;
    private readonly string _preset;

    public ServerConfigurationService(IConfiguration configuration,
                                      SignalRService signalRService,
                                      ContentService content,
                                      IHttpContextAccessor context) {
      _configuration = configuration;
      _signalRService = signalRService;
      _content = content;

      var gamePath = _configuration.GetValue<string>("GamePath");
      _preset = context.HttpContext?.GetServerPreset() ?? Constants.DefaultServerPreset;
      _iniProvider = new(Path.Combine(gamePath, Constants.ServerPresetsPath, _preset));
    }

    public List<ServerPreset> GetAllowedServers() {
      var gamePath = _configuration.GetValue<string>("GamePath");
      var serverPath = Path.Combine(gamePath, Constants.ServerPresetsPath);
      if (!Directory.Exists(serverPath)) {
        return new();
      }

      var presets = Directory.GetDirectories(serverPath);
      return presets.Select(path => {
        var preset = DirectoryUtils.Name(path);
        var serverConfig = new IniProvider(Path.Combine(serverPath, preset));
        var serverName = serverConfig.GetServerConfig().V("SERVER", "NAME", preset);
        return new ServerPreset {
          Name = serverName,
          Preset = preset,
        };
      }).ToList();
    }

    public async Task GetArchive(HttpRequest request, string connectionId) {
      DirectoryUtils.DeleteIfExists(connectionId);
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
          SendProgress(length, stream.Length);
        }
      } finally {
        ArrayPool<byte>.Shared.Return(buffer);
      }

      stream.Close();
    }

    private void SendProgress(long length, long read) {
      var progress = (double) read / length * 100;
      _signalRService.Send(HubMethods.Progress, progress);
    }

    public async Task UpdateConfig(UploadManifest manifest) {
      var entryList = new IniFile();

      if (!string.IsNullOrEmpty(manifest.Track?.Name)) {
        manifest.ServerConfig["SERVER"]["TRACK"] = manifest.Track.Name;

        manifest.ServerConfig["SERVER"]["CONFIG_TRACK"] = manifest.Track.SelectedVariation ?? "";
      }

      if (manifest.Cars.Any()) {
        manifest.ServerConfig["SERVER"]["CARS"] = string.Join(';', manifest.Cars
          .DistinctBy(x => x.Name)
          .Select(x => x.Name));

        for (var i = 0; i < manifest.Cars.Count; i++) {
          var car = manifest.Cars[i];

          if (string.IsNullOrEmpty(car.SelectedVariation)) {
            continue;
          }

          entryList.Add(
            $"CAR_{i}", new(new() {
              { "MODEL", car.Name },
              { "SKIN", car.SelectedVariation },
              { "SPECTATOR_MODE", "0" },
              { "DRIVERNAME", "" },
              { "TEAM", "" },
              { "GUID", "" },
              { "BALLAST", "0" },
              { "RESTRICTOR", "0" },
            }));
        }
      }

      await _iniProvider.SaveConfig(Constants.EntryList, entryList);
      await _iniProvider.SaveConfig(Constants.ServerCfg, manifest.ServerConfig);
    }

    public async Task RunServer() {
      var gamePath = _configuration.GetValue<string>("GamePath");
      var serverPath = Path.Combine(gamePath, "server");
      var serverExecutableName = "acServer";
      var serverExecutablePath = Path.Combine(serverPath, $"{serverExecutableName}.bat");

      while (await ServerNotIsEmpty()) {
        await Task.Delay(TimeSpan.FromSeconds(5));
      }

      var runningProcess = Process.GetProcesses().FirstOrDefault(x => x.ProcessName == serverExecutableName);
      runningProcess?.Kill();

      _content.ExecuteCommand(serverExecutablePath, _preset);
    }

    public IniFile GetServerConfig() {
      return _iniProvider.GetServerConfig();
    }

    public IniFile GetEntryList() {
      return _iniProvider.GetEntryList();
    }

    private string GetLocalPort() {
      return _iniProvider.GetServerConfig().V("SERVER", "HTTP_PORT", "8081");
    }

    private KunosClient Client() {
      var port = GetLocalPort();
      return new("localhost", port);
    }

    private async Task<bool> ServerNotIsEmpty() {
      try {
        var serverInfo = await Client().GetServerInfo();
        return serverInfo is { Clients: > 0 };
      } catch {
        return false;
      }
    }

    public string? GetTrackName() {
      return _iniProvider.GetServerConfig().V<string?>("SERVER", "TRACK", null);
    }

    public ServerProps GetServerProps() {
      var serverConfig = _iniProvider.GetServerConfig();
      return new() {
        Name = serverConfig.V("SERVER",
          "NAME", ""),
        HttpPort = serverConfig.V("SERVER",
          "HTTP_PORT", ""),
      };
    }

    public string[] GetCars() {
      return _iniProvider.GetEntryList().Source
        .Select(x => x.Value.V("MODEL", ""))
        .ToArray();
    }
  }
}
