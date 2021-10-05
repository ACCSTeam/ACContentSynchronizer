using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui;
using ACContentSynchronizer.Models;
using ACContentSynchronizer.Server.Attributes;
using ACContentSynchronizer.Server.Services;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace ACContentSynchronizer.Server.Endpoints.ContentEndpoints {
  public class RefreshServer : BaseAsyncEndpoint.WithRequest<UploadManifest>.WithoutResponse {
    private readonly ServerConfigurationService _serverConfiguration;
    private readonly SignalRService _signalR;

    public RefreshServer(ServerConfigurationService serverConfiguration,
                         SignalRService signalR) {
      _serverConfiguration = serverConfiguration;
      _signalR = signalR;
    }

    [HttpPost(Routes.RefreshServer)]
    [AuthorizedRoute(PasswordType.Admin)]
    public override async Task<ActionResult> HandleAsync(UploadManifest manifest,
                                                         CancellationToken cancellationToken = new()) {
      await _serverConfiguration.UpdateConfig(manifest);
      await _serverConfiguration.RunServer();
      await _signalR.SendAll(HubMethods.Message, Localization.ServerRefreshed);

      return Ok();
    }
  }
}
