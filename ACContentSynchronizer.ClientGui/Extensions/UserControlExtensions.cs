using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace ACContentSynchronizer.ClientGui.Extensions {
  public static class UserControlExtensions {
    public static void OpenPopup(this UserControl control, string popupName) {
      control.FindControl<Popup>(popupName)?.Open();
    }
  }
}
