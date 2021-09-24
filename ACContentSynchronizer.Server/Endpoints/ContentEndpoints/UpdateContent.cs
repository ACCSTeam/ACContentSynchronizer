using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui;
using ACContentSynchronizer.Server.Attributes;
using ACContentSynchronizer.Server.Services;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ACContentSynchronizer.Server.Endpoints.ContentEndpoints {
  public class UpdateContent : BaseAsyncEndpoint.WithoutRequest.WithoutResponse {
    private readonly IConfiguration _configuration;
    private readonly ContentService _content;
    private readonly ServerConfigurationService _serverConfiguration;
    private readonly SignalRService _signalR;

    public UpdateContent(ContentService content,
                         IConfiguration configuration,
                         ServerConfigurationService serverConfiguration,
                         SignalRService signalR) {
      _content = content;
      _configuration = configuration;
      _serverConfiguration = serverConfiguration;
      _signalR = signalR;
    }

    [HttpPost(Routes.UpdateContent)]
    [DisableRequestSizeLimit]
    [AuthorizedRoute(PasswordType.Admin)]
    public override async Task<ActionResult> HandleAsync(CancellationToken cancellationToken = new()) {
      var gamePath = _configuration.GetValue<string>("GamePath");
      await _serverConfiguration.GetArchive(Request,
        HttpContext.Connection.Id);

      _content.UnpackContent(HttpContext.Connection.Id);
      _content.ApplyContent(gamePath, HttpContext.Connection.Id);

      await _signalR.Send(HubMethods.Message, Localization.ContentUploaded);

      return Ok();
    }
  }
}
