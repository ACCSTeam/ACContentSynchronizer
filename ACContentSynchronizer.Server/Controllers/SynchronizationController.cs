using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ACContentSynchronizer.Models;
using ACContentSynchronizer.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ACContentSynchronizer.Server.Controllers {
  public class SynchronizationController : ControllerBase {
    private readonly IConfiguration _configuration;
    private readonly ServerConfigurationService _serverConfiguration;

    public SynchronizationController(IConfiguration configuration,
                                     ServerConfigurationService serverConfiguration) {
      _configuration = configuration;
      _serverConfiguration = serverConfiguration;
    }

    [HttpGet("getServerInformation")]
    public void GetServerInformation() {
    }

    [HttpGet("getManifest")]
    public Manifest GetManifest() {
      var gamePath = _configuration.GetValue<string>("GamePath");
      var trackName = _serverConfiguration.GetTrackName();
      var cars = _serverConfiguration.GetCars();

      if (string.IsNullOrEmpty(trackName)) {
        throw new NullReferenceException(nameof(trackName));
      }

      return ContentUtils.GetManifest(gamePath, cars, trackName);
    }

    [HttpPost("getContent")]
    public async Task<FileResult> GetContent([FromBody] List<string> updatableContent) {
      var gamePath = _configuration.GetValue<string>("GamePath");

      var content = ContentUtils.GetContent(gamePath, updatableContent);
      return new FileContentResult(await content.Pack(), Constants.ContentType);
    }

    [HttpPost("setContent")]
    [DisableRequestSizeLimit]
    public async Task SetContent(string adminPassword) {
      var gamePath = _configuration.GetValue<string>("GamePath");
      _serverConfiguration.CheckAccess(adminPassword);

      await GetArchive();
      ContentUtils.UnpackContent();
      var downloadedContent = ContentUtils.ApplyContent(gamePath);

      await _serverConfiguration.UpdateConfig(downloadedContent.track, downloadedContent.cars);
    }

    private async Task GetArchive() {
      const int bufferLength = 8192;
      var isMoreToRead = true;
      var buffer = new byte[bufferLength];

      FileUtils.DeleteIfExists(Constants.ContentArchive);
      DirectoryUtils.DeleteIfExists(Constants.DownloadsPath);

      await using var fileStream = new FileStream(Constants.ContentArchive,
        FileMode.Create,
        FileAccess.Write,
        FileShare.None,
        bufferLength,
        true);

      do {
        var bytesRead = await Request.Body.ReadAsync(buffer.AsMemory(0, bufferLength));
        if (bytesRead == 0) {
          isMoreToRead = false;
          continue;
        }

        await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
      } while (isMoreToRead);
    }
  }
}
