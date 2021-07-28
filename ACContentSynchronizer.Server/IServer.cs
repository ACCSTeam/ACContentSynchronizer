using System.Threading.Tasks;

namespace ACContentSynchronizer.Server {
  public interface IServer {
    void EntryPoint();

    Task Stop();
  }
}
