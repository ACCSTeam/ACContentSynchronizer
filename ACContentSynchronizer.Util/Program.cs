using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ACContentSynchronizer.Client;
using Microsoft.VisualBasic;

namespace ACContentSynchronizer.Util {
  internal static class Program {
    private static async Task Main(string[] args) {
      try {
        var serverAddress = args[0];
        var gamePath = args[2];
        var dataReceiver = new DataReceiver(serverAddress);

        Console.WriteLine($"Server address: {serverAddress}");
        Console.WriteLine($"Game path: {gamePath}");
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

            await dataReceiver.DownloadData(updatableEntries);

            Console.WriteLine("Content downloaded");
            Console.WriteLine("Trying to save content...");

            dataReceiver.SaveData();

            Console.WriteLine("Content saved");
            Console.WriteLine("Applying changes...");

            dataReceiver.Apply(gamePath);
          }
        }

        Console.WriteLine("Done!");
      } catch (Exception e) {
        Console.WriteLine(e);
      } finally {
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
      }
    }
  }
}
