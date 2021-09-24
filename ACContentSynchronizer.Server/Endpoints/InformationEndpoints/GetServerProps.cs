using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.Models;
using ACContentSynchronizer.Server.Attributes;
using ACContentSynchronizer.Server.Services;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace ACContentSynchronizer.Server.Endpoints.InformationEndpoints {
  public class GetServerProps : BaseAsyncEndpoint.WithoutRequest.WithResponse<ServerProps> {
    private readonly ServerConfigurationService _serverConfiguration;

    public GetServerProps(ServerConfigurationService serverConfiguration) {
      _serverConfiguration = serverConfiguration;
    }

    [HttpGet(Routes.GetServerProps)]
    [AuthorizedRoute(PasswordType.User)]
    public override Task<ActionResult<ServerProps>> HandleAsync(CancellationToken cancellationToken = new()) {
      return Task.FromResult<ActionResult<ServerProps>>(_serverConfiguration.GetServerProps());
    }
  }
}
