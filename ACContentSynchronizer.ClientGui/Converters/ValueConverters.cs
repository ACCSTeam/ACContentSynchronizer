using Avalonia.Data.Converters;

namespace ACContentSynchronizer.ClientGui.Converters {
  public static class ValueConverters {
    public static readonly IValueConverter SliderTimeSpanHours = new DoubleToTimeSpanHoursConverter();
    public static readonly IValueConverter SliderTimeSpanMinutes = new DoubleToTimeSpanMinutesConverter();
    public static readonly IValueConverter SliderTimeSpanSeconds = new DoubleToTimeSpanSecondsConverter();
  }
}
