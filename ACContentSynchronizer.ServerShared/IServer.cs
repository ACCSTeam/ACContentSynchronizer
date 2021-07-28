using System;
using System.Threading.Tasks;

namespace ACContentSynchronizer.ServerShared {
  public interface IServer : IDisposable {
    void EntryPoint();

    Task Stop();
  }
}
