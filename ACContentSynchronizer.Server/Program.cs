using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;

namespace ACContentSynchronizer.Server {
  public class Program {
    public static void Main(string[] args) {
      Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(new RenderedCompactJsonFormatter(), "logs/Server.ndjson")
        .WriteTo.Seq(Environment.GetEnvironmentVariable("SEQ_URL")
                     ?? "http://localhost:5341")
        .CreateLogger();

      try {
        Log.Information("Starting up");
        CreateHostBuilder(args).Build().Run();
      } catch (Exception ex) {
        Log.Fatal(ex, "Application start-up failed");
      } finally {
        Log.CloseAndFlush();
      }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) {
      var pathToExe = Process.GetCurrentProcess().MainModule?.FileName;
      var pathToContentRoot = Path.GetDirectoryName(pathToExe);
      Directory.SetCurrentDirectory(pathToContentRoot!);
      return Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureWebHostDefaults(webBuilder => {
          webBuilder
            .UseStartup<Startup>();
        })
        .UseContentRoot(pathToContentRoot);
    }
  }
}
