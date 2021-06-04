using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.Client;

namespace ACContentSynchronizer.Cli {
  internal static class Program {
    private static async Task Main(string[] args) {
      try {
        var serverAddress = args[0];
        var gamePath = args[1];
        var hasPassword = args.Length > 2;
        var dataReceiver = new DataReceiver(serverAddress);

        Console.WriteLine($"Server address: {serverAddress}");
        Console.WriteLine($"Game path: {gamePath}");

        if (hasPassword) {
          var adminPassword = args[2];

          Console.WriteLine("1. Get data\n2. Set data");
          var key = Console.ReadKey();
          Console.WriteLine();

          switch (key.Key) {
            case ConsoleKey.D1:
              await GetData(dataReceiver, gamePath);
              break;
            case ConsoleKey.D2:
              await SetData(dataReceiver, gamePath, adminPassword);
              break;
          }
        } else {
          await GetData(dataReceiver, gamePath);
        }
      } catch (Exception e) {
        Console.WriteLine(e);
      } finally {
        Console.WriteLine("Done!");
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
      }
    }

    private static async Task GetData(DataReceiver dataReceiver, string gamePath) {
      Console.WriteLine("Downloading manifest...");

      await dataReceiver.DownloadManifest();

      Console.WriteLine("Manifest downloaded");
      Console.WriteLine("Content comparing");

      var updatableEntries = dataReceiver.CompareContent(gamePath);

      if (updatableEntries.Any()) {
        var entriesNames = string.Join("\n ", updatableEntries
          .Select(entry => new DirectoryInfo(entry).Name));

        Console.WriteLine($"Entries:\n {entriesNames}\nneed updates. \nDownload now (Y/N)");

        var key = Console.ReadKey();
        Console.WriteLine();

        if (key.Key == ConsoleKey.Y) {
          Console.WriteLine("Downloading content...");

          dataReceiver.OnDownload += progress =>
            Console.WriteLine(progress < 0 ? "Preparing..." : $"{progress}%");

          dataReceiver.DownloadContent("");

          Console.WriteLine("Content downloaded");
          Console.WriteLine("Trying to save content...");

          dataReceiver.SaveData();

          Console.WriteLine("Content saved");
          Console.WriteLine("Applying changes...");

          dataReceiver.Apply(gamePath);
        }
      }
    }

    private static async Task SetData(DataReceiver dataReceiver, string gamePath, string adminPassword) {
      var content = ContentUtils.GetContentNames(gamePath);
      var carSelect = new Checkbox("Select cars:", true, false, content.cars);
      var trackSelect = new Checkbox("Select track:", false, false, content.tracks);

      var entries = CreateCheckbox(carSelect)
        .Union(CreateCheckbox(trackSelect))
        .ToList();

      Console.Clear();
      Console.WriteLine("Uploading files on server...");

      await dataReceiver.SetContent(adminPassword, gamePath, entries);

      Console.WriteLine("Uploading complete");
    }

    private static IEnumerable<string> CreateCheckbox(Checkbox checkbox) {
      Console.Clear();
      return checkbox.Select()
        .Select(car => car.Option);
    }
  }
}
