using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.Models;
using ACContentSynchronizer.Server.Attributes;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ACContentSynchronizer.Server.Endpoints.ContentEndpoints {
  public class PrepareContent : BaseAsyncEndpoint.WithRequest<Manifest>.WithResponse<string> {
    private readonly IConfiguration _configuration;
    private readonly ContentService _content;

    public PrepareContent(IConfiguration configuration,
                          ContentService content) {
      _configuration = configuration;
      _content = content;
    }

    [HttpPost(Routes.PrepareContent)]
    [AuthorizedRoute(PasswordType.User)]
    public override Task<ActionResult<string>> HandleAsync(Manifest manifest,
                                                           CancellationToken cancellationToken = new()) {
      var gamePath = _configuration.GetValue<string>("GamePath");

      var content = _content.PrepareContent(gamePath, manifest);
      content.Session = HttpContext.Connection.Id;
      ProcessedContent.AvailableContents.Add(content);

      return Task.FromResult<ActionResult<string>>(HttpContext.Connection.Id);
    }
  }
}
