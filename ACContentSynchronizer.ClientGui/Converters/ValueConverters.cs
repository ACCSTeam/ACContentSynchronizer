using System;
using ACContentSynchronizer.ClientGui.Converters;
using Avalonia.Data.Converters;

namespace ACContentSynchronizer.ClientGui {
  public static class ValueConverters {
    public static readonly IValueConverter SliderTimeSpanHours = new DoubleToTimeSpanHoursConverter();
    public static readonly IValueConverter SliderTimeSpanMinutes = new DoubleToTimeSpanMinutesConverter();
  }
}
