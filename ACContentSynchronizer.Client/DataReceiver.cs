using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace ACContentSynchronizer.Client {
  public class DataReceiver {
    private readonly string _dataPath;

    public DataReceiver(string dataPath) {
      _dataPath = dataPath;
    }

    public void DownloadData(string serverAddress) {
      if (string.IsNullOrEmpty(serverAddress)) {
        throw new NullReferenceException(nameof(serverAddress));
      }

      var client = new WebClient {
        BaseAddress = serverAddress,
      };

      if (File.Exists(Constants.DataFile)) {
        File.Delete(Constants.DataFile);
      }

      client.DownloadFile("getContent", Constants.DataFile);
    }

    public void SaveData() {
      if (File.Exists(Constants.DataFile)) {
        if (Directory.Exists(_dataPath)) {
          Directory.Delete(_dataPath, true);
        }

        Directory.CreateDirectory(_dataPath);
        ZipFile.ExtractToDirectory(Constants.DataFile, _dataPath);
      }
    }
  }
}
