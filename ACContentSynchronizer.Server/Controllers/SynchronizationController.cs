using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
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

    [HttpPost("prepareContent")]
    public string PrepareContent([FromBody] List<string> updatableContent) {
      var gamePath = _configuration.GetValue<string>("GamePath");

      var content = ContentUtils.PrepareContent(gamePath, updatableContent);
      Response.Headers.Add("session", HttpContext.Connection.Id);
      content.Pack(HttpContext.Connection.Id);
      return HttpContext.Connection.Id;
    }

    [HttpGet("downloadContent")]
    public async Task<FileResult> DownloadContent(string session) {
      var path = Path.Combine(session, Constants.ContentArchive);
      var content = await System.IO.File.ReadAllBytesAsync(path);
      return new FileContentResult(content, Constants.ContentType);
    }

    [HttpGet("removeSession")]
    public void RemoveSession(string session) {
      DirectoryUtils.DeleteIfExists(session, true);
    }

    [HttpPost("setContent")]
    [DisableRequestSizeLimit]
    public async Task SetContent(string adminPassword) {
      var gamePath = _configuration.GetValue<string>("GamePath");
      _serverConfiguration.CheckAccess(adminPassword);

      await GetArchive();
      ContentUtils.UnpackContent();
      var downloadedContent = ContentUtils.ApplyContent(gamePath);

      // await _serverConfiguration.UpdateConfig(downloadedContent.track, downloadedContent.cars);
    }

    private async Task GetArchive() {
      var isMoreToRead = true;
      int bufferLength;

      if (Request.Body.Length > int.MaxValue) {
        bufferLength = int.MaxValue;
      } else {
        bufferLength = (int) Request.Body.Length;
      }

      var buffer = new byte[bufferLength];

      FileUtils.DeleteIfExists(Constants.ContentArchive);
      DirectoryUtils.DeleteIfExists(Constants.DownloadsPath, true);

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
