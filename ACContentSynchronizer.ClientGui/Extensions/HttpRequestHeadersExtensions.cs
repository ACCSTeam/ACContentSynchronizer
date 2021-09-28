using System.Net.Http.Headers;

namespace ACContentSynchronizer.ClientGui.Extensions {
  public static class HttpRequestHeadersExtension {
    public static void Add(this HttpRequestHeaders headers, DefaultHeaders header, string value) {
      headers.Remove(header.ToString());
      headers.Add(header.ToString(), value);
    }
  }
}
