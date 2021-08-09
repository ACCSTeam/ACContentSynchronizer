using System.Collections;
using System.Collections.Generic;

namespace ACContentSynchronizer {
  public class IniFile : IEnumerable<KeyValuePair<string, IniSection>> {
    private readonly Dictionary<string, IniSection> _source = new();

    public IniSection this[string key] {
      get => _source.ContainsKey(key)
        ? _source[key]
        : new();
      set => _source[key] = value;
    }

    public void Add(string key, IniSection section) {
      _source.Add(key, section);
    }

    public IEnumerator<KeyValuePair<string, IniSection>> GetEnumerator() {
      return _source.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }

  public class IniSection : IEnumerable<KeyValuePair<string, object?>> {
    private readonly Dictionary<string, object?> _source = new();

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

    public T? V<T>(string key) {
      var value = _source[key];
      return (T?) value;
    }

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() {
      return _source.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}
