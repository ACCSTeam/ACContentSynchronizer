using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui;
using ACContentSynchronizer.Server.Attributes;
using ACContentSynchronizer.Server.Services;
using Ardalis.ApiEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace ACContentSynchronizer.Server.Endpoints.ContentEndpoints {
  public class DownloadContent : BaseAsyncEndpoint.WithRequest<string>.WithoutResponse {
    private readonly ContentService _content;
    private readonly SignalRService _signalR;

    public DownloadContent(SignalRService signalR,
                           ContentService content) {
      _signalR = signalR;
      _content = content;
    }

    [HttpGet(Routes.DownloadContent)]
    [AuthorizedRoute(PasswordType.User)]
    public override async Task<ActionResult> HandleAsync(string session, CancellationToken cancellationToken = new()) {
      var path = Path.Combine(session, Constants.ContentArchive);
      await using var fileStream = System.IO.File.OpenRead(path);

      await _signalR.Send(HubMethods.Message,
        $"{Localization.ContentSize} {_content.ContentSize(fileStream.Length)}");

      Response.Headers["Content-Type"] = Constants.ContentType;
      Response.Headers["Content-Length"] = fileStream.Length.ToString();
      await fileStream.CopyToAsync(Response.Body, cancellationToken);
      fileStream.Close();
      await fileStream.DisposeAsync();
      DirectoryUtils.DeleteIfExists(session);
      return Ok();
    }
  }
}
