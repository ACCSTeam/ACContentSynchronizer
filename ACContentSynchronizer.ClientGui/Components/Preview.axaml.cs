using System;
using System.IO;
using System.Linq;
using ACContentSynchronizer.ClientGui.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using SkiaSharp;
using Splat;

namespace ACContentSynchronizer.ClientGui.Components {
  public class Preview : UserControl, IDisposable {
    public static readonly DirectProperty<Preview, string> PlaceholderProperty =
      AvaloniaProperty.RegisterDirect<Preview, string>(
        nameof(Placeholder),
        o => o.Placeholder,
        (o, v) => o.Placeholder = v);

    public static readonly DirectProperty<Preview, string> PreviewNameProperty =
      AvaloniaProperty.RegisterDirect<Preview, string>(
        nameof(PreviewName),
        o => o.PreviewName,
        (o, v) => o.PreviewName = v);

    public static readonly DirectProperty<Preview, int> BlurAmountProperty =
      AvaloniaProperty.RegisterDirect<Preview, int>(
        nameof(BlurAmount),
        o => o.BlurAmount,
        (o, v) => o.BlurAmount = v);

    public static readonly DirectProperty<Preview, Bitmap?> SourceProperty =
      AvaloniaProperty.RegisterDirect<Preview, Bitmap?>(
        nameof(Source),
        o => o.Source,
        (o, v) => o.Source = v);

    public static readonly DirectProperty<Preview, Bitmap?> BlurredProperty =
      AvaloniaProperty.RegisterDirect<Preview, Bitmap?>(
        nameof(Blurred),
        o => o.Blurred);

    private int _blurAmount = 10;

    private string _placeholder = "";

    private string _previewName = "";

    private Bitmap? _source;

    public Preview() {
      InitializeComponent();
    }

    public string Placeholder {
      get => _placeholder;
      set => SetAndRaise(PlaceholderProperty, ref _placeholder, value);
    }

    public string PreviewName {
      get => _previewName;
      set => SetAndRaise(PreviewNameProperty, ref _previewName, value);
    }

    public int BlurAmount {
      get => _blurAmount;
      set => SetAndRaise(BlurAmountProperty, ref _blurAmount, value);
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
        Source.Save(ms);
        ms.Position = 0;
        var width = (int) Source.Size.Width;
        var height = (int) Source.Size.Height;
        var blurOffsetX = width * 15 / 100;
        var blurOffsetY = height * 15 / 100;
        var data = SKData.Create(ms);
        var image = SKImage.FromEncodedData(data);
        var filter = SKImageFilter.CreateBlur(BlurAmount, BlurAmount);
        var clip = SKRectI.Create(width, height);

        var rect = SKRectI.Create(blurOffsetX, blurOffsetY,
          width - blurOffsetX * 2,
          height - blurOffsetY * 2);

        var blurredImage = image.ApplyImageFilter(filter, clip, rect,
          out _, out SKPoint _);

        return new(blurredImage.Encode(SKEncodedImageFormat.Jpeg, 100).AsStream());
      }
    }

    public void Dispose() {
      _source?.Dispose();
    }

    public static Bitmap? GetCarPreview(string entry, string? skinName = null) {
      var contentService = Locator.Current.GetService<ContentService>();
      var settings = Locator.Current.GetService<ApplicationViewModel>().Settings;
      var carDirectory = contentService.GetCarDirectory(entry, settings.GamePath);
      if (string.IsNullOrEmpty(carDirectory)) {
        return null;
      }
      var carSkinsDirectory = Path.Combine(carDirectory, "skins");
      if (!Directory.Exists(carSkinsDirectory)) {
        return null;
      }

      if (!string.IsNullOrEmpty(skinName)) {
        return new(Path.Combine(carSkinsDirectory, skinName, "preview.jpg"));
      }

      var skins = Directory.GetDirectories(carSkinsDirectory);
      var rnd = new Random();
      var skin = skins[rnd.Next(0, skins.Length)];
      return new
        (Path.Combine(skin, "preview.jpg"));
    }

    public static Bitmap? GetTrackPreview(string entry) {
      var contentService = Locator.Current.GetService<ContentService>();
      var settings = Locator.Current.GetService<ApplicationViewModel>().Settings;
      var trackDirectory = contentService.GetTrackDirectories(entry, settings.GamePath)
        .FirstOrDefault();
      if (string.IsNullOrEmpty(trackDirectory)) {
        return null;
      }
      var trackPreview = Path.Combine(trackDirectory, "preview.png");
      if (!File.Exists(trackPreview)) {
        return null;
      }

      return new(trackPreview);
    }

    private void InitializeComponent() {
      AvaloniaXamlLoader.Load(this);
    }
  }
}
