using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ACContentSynchronizer.ServerWorker {
  public class Program {
    public static void Main(string[] args) {
      CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) {
      var pathToExe = Process.GetCurrentProcess().MainModule?.FileName;
      var pathToContentRoot = Path.GetDirectoryName(pathToExe);
      Directory.SetCurrentDirectory(pathToContentRoot);
      return Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) => services.AddHostedService<Updater>())
        .UseContentRoot(pathToContentRoot)
        .UseWindowsService();
    }
  }
}
