using System;
using System.IO;
using System.Threading.Tasks;
using ACContentSynchronizer.Models;
using ACContentSynchronizer.Server.Hubs;
using ACContentSynchronizer.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

namespace ACContentSynchronizer.Server.Controllers {
  [ApiController]
  public class SynchronizationController : ControllerBase {
    private readonly IConfiguration _configuration;
    private readonly IHubContext<NotificationHub> _hub;
    private readonly ServerConfigurationService _serverConfiguration;

    public SynchronizationController(IConfiguration configuration,
                                     ServerConfigurationService serverConfiguration,
                                     IHubContext<NotificationHub> hubContext) {
      _configuration = configuration;
      _serverConfiguration = serverConfiguration;
      _hub = hubContext;
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
    public async Task<FileStreamResult> DownloadContent(string session) {
      var path = Path.Combine(session, Constants.ContentArchive);
      await using var stream = System.IO.File.OpenRead(path);
      return File(stream, Constants.ContentType);
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
    public async Task UpdateContent(string adminPassword) {
      _serverConfiguration.CheckAccess(adminPassword);

      var gamePath = _configuration.GetValue<string>("GamePath");
      await _serverConfiguration.GetArchive(Request.Body, HttpContext.Connection.Id);
      ContentUtils.UnpackContent(HttpContext.Connection.Id);
      ContentUtils.ApplyContent(gamePath, HttpContext.Connection.Id);

      await _hub.Clients.All.SendAsync(HubMethods.Message.ToString(), "Content uploaded");
    }

    [HttpPost("refreshServer")]
    public async Task RefreshServer(Manifest manifest, string adminPassword) {
      _serverConfiguration.CheckAccess(adminPassword);

      var presetPath = await _serverConfiguration.UpdateConfig(manifest);
      if (!string.IsNullOrEmpty(presetPath)) {
        await _serverConfiguration.RunServer(presetPath);
      }

      await _hub.Clients.All.SendAsync(HubMethods.Message.ToString(), "Server rebooted");
    }
  }
}
