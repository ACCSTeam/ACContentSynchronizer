using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace ACContentSynchronizer.ClientGui.Converters {
  public class DoubleToTimeSpanHoursConverter : IValueConverter {
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

  public class DoubleToTimeSpanMinutesConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      return value is TimeSpan t
        ? t.TotalMinutes
        : default;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      return value is double d
        ? TimeSpan.FromMinutes(d)
        : TimeSpan.Zero;
    }
  }

  public class DoubleToTimeSpanSecondsConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
      return value is TimeSpan t
        ? t.TotalSeconds
        : default;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
      return value is double d
        ? TimeSpan.FromSeconds(d)
        : TimeSpan.Zero;
    }
  }
}
