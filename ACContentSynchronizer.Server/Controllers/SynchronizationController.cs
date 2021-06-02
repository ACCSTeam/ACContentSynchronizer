using System;
using System.Threading.Tasks;
using ACContentSynchronizer.Models;
using ACContentSynchronizer.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ACContentSynchronizer.Server.Controllers {
  public class SynchronizationController {
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
    public async Task<FileResult> GetContent([FromBody] string[] updatableContent) {
      var gamePath = _configuration.GetValue<string>("GamePath");

      var content = ContentUtils.GetContent(gamePath, updatableContent);
      return new FileContentResult(await content.Pack(), Constants.ContentType);
    }
  }
}
