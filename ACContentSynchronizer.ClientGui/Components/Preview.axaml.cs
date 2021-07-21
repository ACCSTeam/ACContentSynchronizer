using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using SkiaSharp;

namespace ACContentSynchronizer.ClientGui.Components {
  public class Preview : UserControl {
    public static readonly DirectProperty<Preview, Bitmap?> SourceProperty =
      AvaloniaProperty.RegisterDirect<Preview, Bitmap?>(
        nameof(Source),
        o => o.Source,
        (o, v) => o.Source = v);

    public static readonly DirectProperty<Preview, Bitmap?> BlurredProperty =
      AvaloniaProperty.RegisterDirect<Preview, Bitmap?>(
        nameof(Blurred),
        o => o.Blurred);

    private Bitmap? _source;

    public Preview() {
      InitializeComponent();
    }

    public Bitmap? Source {
      get => _source;
      set {
        SetAndRaise(SourceProperty, ref _source, value);
        RaisePropertyChanged(BlurredProperty, Optional<Bitmap?>.Empty, BindingValue<Bitmap?>.Unset);
      }
    }

    public Bitmap? Blurred {
      get {
        if (Source == null) {
          return null;
        }

        using MemoryStream ms = new();
        Source?.Save(ms);
        ms.Position = 0;
        const int blurAmount = 10;
        var width = (int) (Source?.Size.Width ?? 0);
        var height = (int) (Source?.Size.Height ?? 0);
        var blurOffsetX = width * 15 / 100;
        var blurOffsetY = height * 15 / 100;
        var data = SKData.Create(ms);
        var image = SKImage.FromEncodedData(data);
        var filter = SKImageFilter.CreateBlur(blurAmount, blurAmount);
        var clip = SKRectI.Create(width, height);

        var rect = SKRectI.Create(blurOffsetX, blurOffsetY,
          width - blurOffsetX * 2,
          height - blurOffsetY * 2);

        var blurredImage = image.ApplyImageFilter(filter, clip, rect,
          out _, out SKPoint _);

        return new(blurredImage.Encode(SKEncodedImageFormat.Jpeg, 100).AsStream());
      }
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
