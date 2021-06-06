using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ACContentSynchronizer.Models;
using ACContentSynchronizer.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ACContentSynchronizer.Server.Controllers {
  [ApiController]
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
    public string PrepareContent(Manifest manifest) {
      var gamePath = _configuration.GetValue<string>("GamePath");

      var content = ContentUtils.PrepareContent(gamePath, manifest);
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

    [HttpPost("getUpdateManifest")]
    public Manifest GetUpdatableEntries(Manifest manifest) {
      var gamePath = _configuration.GetValue<string>("GamePath");
      return ContentUtils.CompareContent(gamePath, manifest);
    }

    [HttpPost("updateContent")]
    [DisableRequestSizeLimit]
    public async Task UpdateContent(UpdateManifest updateManifest, string adminPassword) {
      var gamePath = _configuration.GetValue<string>("GamePath");
      _serverConfiguration.CheckAccess(adminPassword);

      await _serverConfiguration.GetArchive(updateManifest.Content);
      ContentUtils.UnpackContent();
      var (cars, track) = ContentUtils.ApplyContent(gamePath);

      var presetPath = await _serverConfiguration.UpdateConfig(updateManifest.Manifest);

      if (!string.IsNullOrEmpty(presetPath)) {
        await _serverConfiguration.RunServer(presetPath);
      }
    }
  }
}
