using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.Models;
using ACContentSynchronizer.Server.Attributes;
using ACContentSynchronizer.Server.Services;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace ACContentSynchronizer.Server.Endpoints.InformationEndpoints {
  public class GetAllowedServers : BaseAsyncEndpoint.WithoutRequest.WithResponse<List<ServerPreset>> {
    private readonly ServerConfigurationService _serverConfiguration;

    public GetAllowedServers(ServerConfigurationService serverConfiguration) {
      _serverConfiguration = serverConfiguration;
    }

    [HttpGet(Routes.GetAllowedServers)]
    public override Task<ActionResult<List<ServerPreset>>> HandleAsync(CancellationToken cancellationToken = new()) {
      return Task.FromResult<ActionResult<List<ServerPreset>>>(_serverConfiguration.GetAllowedServers());
    }
  }
}
