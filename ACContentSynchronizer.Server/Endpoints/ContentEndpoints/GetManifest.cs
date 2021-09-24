using System;
using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.Models;
using ACContentSynchronizer.Server.Attributes;
using ACContentSynchronizer.Server.Services;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ACContentSynchronizer.Server.Endpoints.ContentEndpoints {
  public class GetManifest : BaseAsyncEndpoint.WithoutRequest.WithResponse<Manifest> {
    private readonly IConfiguration _configuration;
    private readonly ContentService _contentService;
    private readonly ServerConfigurationService _serverConfiguration;

    public GetManifest(IConfiguration configuration,
                       ServerConfigurationService serverConfiguration,
                       ContentService contentService) {
      _configuration = configuration;
      _serverConfiguration = serverConfiguration;
      _contentService = contentService;
    }

    [HttpGet(Routes.GetManifest)]
    [AuthorizedRoute(PasswordType.User)]
    public override Task<ActionResult<Manifest>> HandleAsync(
      CancellationToken cancellationToken = new()) {
      var gamePath = _configuration.GetValue<string>("GamePath");
      var trackName = _serverConfiguration.GetTrackName();
      var cars = _serverConfiguration.GetCars();

      if (string.IsNullOrEmpty(trackName)) {
        throw new NullReferenceException(nameof(trackName));
      }

      return Task.FromResult<ActionResult<Manifest>>(_contentService.GetManifest(gamePath, cars, trackName));
    }
  }
}
