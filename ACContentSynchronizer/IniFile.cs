using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ACContentSynchronizer {
  public class IniFile : IEnumerable<KeyValuePair<string, IniSection>> {
    private readonly Dictionary<string, IniSection> _source = new();
    public Dictionary<string, Dictionary<string, string>> Source =>
      _source.ToDictionary(x => x.Key,
        x => x.Value.Source);

    public IniSection this[string key] {
      get => _source.ContainsKey(key)
        ? _source[key]
        : new();
      set => _source[key] = value;
    }

    public int Count => _source.Count;

    public IEnumerator<KeyValuePair<string, IniSection>> GetEnumerator() {
      return _source.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    public void Add(string key, IniSection section) {
      _source.Add(key, section);
    }

    public T V<T>(string key, string value, T defaultValue) {
      return _source.ContainsKey(key)
        ? _source[key].V(value, defaultValue)
        : defaultValue;
    }
  }

  public class IniSection : IEnumerable<KeyValuePair<string, object?>> {
    private readonly Dictionary<string, object?> _source = new();
    public Dictionary<string, string> Source => _source
      .ToDictionary(x => x.Key, x
        => x.Value?.ToString() ?? "");

    public IniSection() {
    }

    public IniSection(Dictionary<string, object?> source) {
      _source = source;
    }

    public object? this[string key] {
      get => _source.ContainsKey(key)
        ? _source[key]
        : null;
      set => _source[key] = value;
    }

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() {
      return _source.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    public T V<T>(string key, T defaultValue) {
      if (!_source.ContainsKey(key)) {
        return defaultValue;
      }

      var value = _source[key];
      return value != null
        ? (T) value
        : defaultValue;
    }
  }
}
