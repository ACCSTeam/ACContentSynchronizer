using ACContentSynchronizer.ClientGui.ViewModels;
using Splat;

namespace ACContentSynchronizer.ClientGui {
  public class Startup {
    public static void Register(IMutableDependencyResolver services) {
      /* services */
      services.Register(() => new ContentService());

      /* views */
      var application = new ApplicationViewModel();
      services.Register(() => application);
    }
  }
}
