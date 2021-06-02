using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ACContentSynchronizer.Server.Controllers {
  public class SynchronizationController {
    //TODO
    private const string GamePath = "/home/fest/Test";

    [HttpGet("getContent")]
    public async Task<FileResult> GetContent() {
      //TODO
      var content = await ContentUtils.GetContent(GamePath, "atron_drift_gp2019");
      return new FileContentResult(await content.Pack(), Constants.ContentType);
    }
  }
}
