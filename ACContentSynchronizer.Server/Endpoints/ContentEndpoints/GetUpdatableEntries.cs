using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.Models;
using ACContentSynchronizer.Server.Attributes;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ACContentSynchronizer.Server.Endpoints.ContentEndpoints {
  public class GetUpdatableEntries : BaseAsyncEndpoint.WithRequest<Manifest>.WithResponse<Manifest> {
    private readonly IConfiguration _configuration;
    private readonly ContentService _content;

    public GetUpdatableEntries(IConfiguration configuration,
                               ContentService content) {
      _configuration = configuration;
      _content = content;
    }

    [HttpPost(Routes.GetUpdateManifest)]
    [AuthorizedRoute(PasswordType.User)]
    public override Task<ActionResult<Manifest>> HandleAsync(Manifest manifest,
                                                             CancellationToken cancellationToken = new()) {
      var gamePath = _configuration.GetValue<string>("GamePath");
      return Task.FromResult<ActionResult<Manifest>>(_content.CompareContent(gamePath, manifest));
    }
  }
}
