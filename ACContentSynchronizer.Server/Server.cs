using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ACContentSynchronizer.Server {
  [Export(typeof(IServer))]
  public class Server : IServer {
    private IHost? _host;

    public void EntryPoint() {
      _host = CreateHostBuilder().Build();
      _host.Run();
    }

    public Task Stop() {
      return _host == null ? Task.CompletedTask : _host.StopAsync();
    }

    private IHostBuilder CreateHostBuilder() {
      return Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(webBuilder => {
          webBuilder
            .UseStartup<Startup>();
        });
    }
  }
}
