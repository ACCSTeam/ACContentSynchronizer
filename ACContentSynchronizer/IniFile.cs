using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ACContentSynchronizer {
  public class IniFile {
    public Dictionary<string, IniSection> Source { get; set; } = new();

    public IniSection this[string key] {
      get => Source.ContainsKey(key)
        ? Source[key]
        : new();
      set => Source[key] = value;
    }

    public void Add(string key, IniSection section) {
      Source.Add(key, section);
    }

    public T V<T>(string key, string value, T defaultValue) {
      return Source.ContainsKey(key)
        ? Source[key].V(value, defaultValue)
        : defaultValue;
    }
  }

  public class IniSection {
    public Dictionary<string, string> Source { get; } = new();

    public IniSection() {
    }

    public IniSection(Dictionary<string, string> source) {
      Source = source;
    }

    public object? this[string key] {
      get => Source.ContainsKey(key)
        ? Source[key]
        : null;
      set => Source[key] = $"{value}";
    }

    public T V<T>(string key, T defaultValue) {
      return Source.ContainsKey(key)
        ? Converter(Source[key], defaultValue)
        : defaultValue;
    }

    private T Converter<T>(string value, T defaultValue) {
      var type = typeof(T);
      if (IsNumericType(type)) {
        value = string.Join("", value.Where(char.IsDigit));
        if (string.IsNullOrEmpty(value)) {
          return defaultValue;
        }

        var longValue = Convert.ToInt64(value);
        if (type.IsEnum) {
          return (T) Enum.ToObject(typeof(T), longValue);
        }

        return (T) Convert.ChangeType(longValue, type);
      }

      return (T) Convert.ChangeType(value, type);
    }

    private bool IsNumericType(Type type) {
      if (type.IsEnum) {
        return true;
      }

      var typeCode = Type.GetTypeCode(type);
      switch (typeCode) {
        case TypeCode.Byte:
        case TypeCode.SByte:
        case TypeCode.UInt16:
        case TypeCode.UInt32:
        case TypeCode.UInt64:
        case TypeCode.Int16:
        case TypeCode.Int32:
        case TypeCode.Int64:
        case TypeCode.Decimal:
        case TypeCode.Double:
        case TypeCode.Single:
        case TypeCode.Boolean:
          return true;
        default:
          return false;
      }
    }
  }
}
