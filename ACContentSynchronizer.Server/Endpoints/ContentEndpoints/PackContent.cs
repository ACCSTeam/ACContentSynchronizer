using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.Models;
using ACContentSynchronizer.Server.Attributes;
using ACContentSynchronizer.Server.Services;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace ACContentSynchronizer.Server.Endpoints.ContentEndpoints {
  public class PackContent : BaseAsyncEndpoint.WithRequest<string>.WithoutResponse {
    private readonly SignalRService _signalR;

    public PackContent(SignalRService signalR) {
      _signalR = signalR;
    }

    [HttpGet(Routes.PackContent)]
    [AuthorizedRoute(PasswordType.User)]
    public override async Task<ActionResult> HandleAsync(string session, CancellationToken cancellationToken = new()) {
      var content = ProcessedContent.AvailableContents.FirstOrDefault(x => x.Session == session);

      if (content == null) {
        return Ok();
      }

      content.OnProgress += (progress, message) => { _signalR.Send(HubMethods.ProgressMessage, progress, message); };
      await content.Pack(session);
      ProcessedContent.AvailableContents.Remove(content);
      return Ok();
    }
  }
}
