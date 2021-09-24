using System;
using ACContentSynchronizer.ClientGui.Components;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace ACContentSynchronizer.ClientGui {
  public class LContent {
    public static readonly AvaloniaProperty<string> LNameProperty =
      AvaloniaProperty.RegisterAttached<LContent, AvaloniaObject, string>("LName", "undefined", false,
        BindingMode.OneTime);

    static LContent() {
      LNameProperty.Changed.Subscribe(args => {
        var name = args.Sender.GetValue(LNameProperty);
        var value = Localization.ResourceManager.GetString(name);

        switch (args.Sender) {
          case Preview:
            args.Sender.SetValue(Preview.PlaceholderProperty, value);
            break;
          case IconButton:
            args.Sender.SetValue(IconButton.TextProperty, value);
            break;
          case TabItem:
            args.Sender.SetValue(HeaderedContentControl.HeaderProperty, value);
            break;
          case TextBlock:
            args.Sender.SetValue(TextBlock.TextProperty, value);
            break;
          case ContentControl:
            args.Sender.SetValue(ContentControl.ContentProperty, value);
            break;
          case TextBox:
            args.Sender.SetValue(TextBox.WatermarkProperty, value);
            break;
        }
      });
    }

    public static void SetLName(AvaloniaObject target, string value) {
      target.SetValue(LNameProperty, value);
    }

    public static string GetLName(AvaloniaObject target) {
      return (string) target.GetValue(LNameProperty);
    }
  }
}
