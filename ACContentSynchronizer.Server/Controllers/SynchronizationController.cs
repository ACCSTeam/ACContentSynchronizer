using System.Collections.Generic;
using System.Threading.Tasks;
using ACContentSynchronizer.Models;
using Microsoft.AspNetCore.Mvc;

namespace ACContentSynchronizer.Server.Controllers {
  public class SynchronizationController {
    //TODO
    private const string GamePath = "/home/fest/Test";

    [HttpPost("getContent")]
    public async Task<FileResult> GetContent([FromBody]string[] updatableContent) {
      //TODO
      var content = ContentUtils.GetContent(GamePath, "atron_drift_gp2019", updatableContent);
      return new FileContentResult(await content.Pack(), Constants.ContentType);
    }

    [HttpGet("getManifest")]
    public Manifest GetManifest() {
      return ContentUtils.GetManifest(GamePath, "atron_drift_gp2019");
    }
  }
}
