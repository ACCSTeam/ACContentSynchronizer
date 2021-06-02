using System;
using ACContentSynchronizer.Client;

namespace ACContentSynchronizer.Util {
  internal class Program {
    private static void Main(string[] args) {
      try {
        var serverAddress = args[0];
        var dataPath = args[1];
        var dataReceiver = new DataReceiver(dataPath);

        Console.WriteLine("Downloading content...");

        dataReceiver.DownloadData(serverAddress);

        Console.WriteLine("Content downloaded");
        Console.WriteLine("Trying to save content...");

        dataReceiver.SaveData();

        Console.WriteLine("Content saved");
      } catch (Exception e) {
        Console.WriteLine(e);
      } finally {
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
      }
    }
  }
}
