using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ACContentSynchronizer.Extensions {
  public static class HttpClientExtensions {
    public static HttpRequestMessage PostMessage(string action, object body,
                                                 (string header, string value)[] headers) {
      var httpRequestMessage = new HttpRequestMessage {
        Method = HttpMethod.Post,
        RequestUri = new(action),
        Headers = {
          { HttpRequestHeader.Accept.ToString(), "application/json" },
        },
        Content = new StringContent(JsonConvert.SerializeObject(body)),
      };

      foreach (var (header, value) in headers) {
        httpRequestMessage.Headers.Add(header, value);
      }

      return httpRequestMessage;
    }

    public static async Task<T?> GetJson<T>(this HttpClient client, string action) {
      var json = await client.GetStringAsync(action);
      return JsonConvert.DeserializeObject<T>(json);
    }

    public static async Task<T?> PostJson<T>(this HttpClient client, string action, object body) {
      var response = await client.PostAsJsonAsync(action, body);
      var json = await response.Content.ReadAsStringAsync();
      return JsonConvert.DeserializeObject<T>(json);
    }

    public static async Task<string> PostString(this HttpClient client, string action, object body) {
      var response = await client.PostAsJsonAsync(action, body);
      return await response.Content.ReadAsStringAsync();
    }
  }
}
