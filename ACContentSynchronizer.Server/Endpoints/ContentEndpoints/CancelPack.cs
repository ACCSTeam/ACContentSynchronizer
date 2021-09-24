using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.Models;
using ACContentSynchronizer.Server.Attributes;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace ACContentSynchronizer.Server.Endpoints.ContentEndpoints {
  public class CancelPack : BaseAsyncEndpoint.WithRequest<string>.WithoutResponse {
    [HttpGet(Routes.CancelPack)]
    [AuthorizedRoute(PasswordType.User)]
    public override Task<ActionResult> HandleAsync(string session, CancellationToken cancellationToken = new()) {
      var content = ProcessedContent.AvailableContents.FirstOrDefault(x => x.Session == session);
      if (content == null) {
        return Task.FromResult<ActionResult>(Ok());
      }

      content.AbortPacking();
      ProcessedContent.AvailableContents.Remove(content);

      return Task.FromResult<ActionResult>(Ok());
    }
  }
}
