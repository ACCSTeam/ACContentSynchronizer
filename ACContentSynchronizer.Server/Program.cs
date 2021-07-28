using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ACContentSynchronizer.Server {
  public class Program {
    private static IHost? _host;

    public static void Main(string[] args) {
      _host = CreateHostBuilder(args).Build();
      _host.Run();
    }

    public Task Stop() {
      return _host == null
        ? Task.CompletedTask
        : _host.StopAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) {
      return Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder => {
          webBuilder
            .UseStartup<Startup>();
        });
    }

    public void Dispose() {
      _host?.Dispose();
    }
  }
}
