using Microsoft.AspNetCore.Http;

namespace ACContentSynchronizer.Server.Extensions {
  public static class HttpContentExtensions {
    public static string GetHeader(this HttpContext context, DefaultHeaders header) {
      return context.Request.Headers[header.ToString()].ToString();
    }

    public static string GetServerPreset(this HttpContext context) {
      var preset = context.GetHeader(DefaultHeaders.ServerPreset);
      return !string.IsNullOrEmpty(preset)
        ? preset
        : Constants.DefaultServerPreset;
    }
  }
}
