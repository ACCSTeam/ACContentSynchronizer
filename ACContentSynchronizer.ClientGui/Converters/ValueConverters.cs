using System;
using ACContentSynchronizer.ClientGui.Converters;
using Avalonia.Data.Converters;

namespace ACContentSynchronizer.ClientGui {
  public static class ValueConverters {
    public static readonly IValueConverter SliderTimeSpan = new DoubleToTimeSpanConverter();
  }
}
