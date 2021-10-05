using System.Net.Http;
using System.Net.Http.Headers;

namespace ACContentSynchronizer.ClientGui.Extensions {
  public static class HttpRequestHeadersExtension {
    public static void AddHeader(this HttpClient client, DefaultHeaders header, string value) {
      client.DefaultRequestHeaders.Remove(header.ToString());
      client.DefaultRequestHeaders.Add(header.ToString(), value);
    }
  }
}
