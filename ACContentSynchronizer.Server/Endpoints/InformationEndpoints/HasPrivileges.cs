using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.Server.Attributes;
using ACContentSynchronizer.Server.Services;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace ACContentSynchronizer.Server.Endpoints.InformationEndpoints {
  public class HasPrivileges : BaseAsyncEndpoint
    .WithoutRequest
    .WithResponse<bool> {
    private readonly ServerConfigurationService _serverConfiguration;

    public HasPrivileges(ServerConfigurationService serverConfiguration) {
      _serverConfiguration = serverConfiguration;
    }

    [Route(Routes.HasPrivileges)]
    [AuthorizedRoute(PasswordType.User)]
    public override Task<ActionResult<bool>> HandleAsync(CancellationToken cancellationToken = new()) {
      return Task.FromResult<ActionResult<bool>>(_serverConfiguration.HasPrivileges(PasswordType.Admin));
    }
  }
}
