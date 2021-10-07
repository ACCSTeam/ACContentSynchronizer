using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using ACContentSynchronizer.ClientGui.Modals;

namespace ACContentSynchronizer.ClientGui.Services {
  public static class RequestWrapper {
    public static Task<T> Wrap<T>(this HttpClient client, Func<HttpClient, Task<T?>> action)
      where T : new() {
      return Wrap(client, action, new());
    }

    public static async Task<T> Wrap<T>(this HttpClient client, Func<HttpClient, Task<T?>> action, T defaultValue) {
      try {
        return await action(client) ?? defaultValue;
      } catch (Exception e) {
        if (e is not HttpRequestException re) {
          return defaultValue;
        }

        switch (re.StatusCode) {
          case HttpStatusCode.Forbidden:
            Toast.Open(Localization.WrongPassword);
            break;
          default:
            Toast.Open(Localization.ConnectionEstablishedError);
            break;
        }
        throw;
      }
    }
  }
}
