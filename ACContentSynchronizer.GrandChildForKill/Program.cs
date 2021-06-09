using System.Diagnostics;
using System.IO;
using System.Text;

namespace ACContentSynchronizer.GrandChildForKill {
  internal class Program {
    private static void Main(string[] args) {
      var exePath = args[0];
      var cfgPath = args[1];
      var entryPath = args[2];

      var process = new Process {
        StartInfo = {
          FileName = exePath,
          Arguments = $"-c \"{cfgPath}\" -e \"{entryPath}\"",
          UseShellExecute = false,
          WorkingDirectory = Path.GetDirectoryName(exePath) ?? "",
          RedirectStandardOutput = true,
          CreateNoWindow = true,
          RedirectStandardError = true,
          StandardOutputEncoding = Encoding.UTF8,
          StandardErrorEncoding = Encoding.UTF8,
        },
      };

      process.Start();
    }
  }
}
