using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ACContentSynchronizer.Extensions {
  public static class HttpClientExtensions {
    public static async Task<T?> GetJson<T>(this HttpClient client, string action) {
      var json = await client.GetStringAsync(action);
      return JsonConvert.DeserializeObject<T>(json);
    }

    public static async Task<T?> PostJson<T, TBody>(this HttpClient client, string action, TBody body) {
      var response = await client.PostAsJsonAsync(action, body);
      var json = await response.Content.ReadAsStringAsync();
      return JsonConvert.DeserializeObject<T>(json);
    }

    public static async Task<string> PostString<TBody>(this HttpClient client, string action, TBody body) {
      var response = await client.PostAsJsonAsync(action, body);
      return await response.Content.ReadAsStringAsync();
    }
  }
}
