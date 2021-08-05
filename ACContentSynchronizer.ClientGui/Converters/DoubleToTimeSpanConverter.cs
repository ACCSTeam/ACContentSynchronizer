using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ACContentSynchronizer.ClientGui.Converters {
  public class DoubleToTimeSpanConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      return value is TimeSpan t
        ? t.TotalHours
        : default;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      return value is double d
        ? TimeSpan.FromHours(d)
        : TimeSpan.Zero;
    }
  }
}
