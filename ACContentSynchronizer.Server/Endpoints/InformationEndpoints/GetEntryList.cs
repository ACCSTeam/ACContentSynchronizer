using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.Server.Attributes;
using ACContentSynchronizer.Server.Services;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace ACContentSynchronizer.Server.Endpoints.InformationEndpoints {
  public class GetEntryList : BaseAsyncEndpoint
    .WithoutRequest
    .WithResponse<IniFile> {
    private readonly ServerConfigurationService _serverConfiguration;

    public GetEntryList(ServerConfigurationService serverConfiguration) {
      _serverConfiguration = serverConfiguration;
    }

    [HttpGet(Routes.GetEntryConfig)]
    [AuthorizedRoute(PasswordType.User)]
    public override Task<ActionResult<IniFile>> HandleAsync(CancellationToken cancellationToken = new()) {
      return Task.FromResult<ActionResult<IniFile>>(_serverConfiguration.GetEntryList());
    }
  }
}
