using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace ACContentSynchronizer.ClientGui {
  public static class SteamIdHelper {
    public static IEnumerable<SteamProfile> FindUsers() {
      var steamPath = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
        ? Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? string.Empty, ".steam/steam")
        : Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam")?.ToString();

      if (string.IsNullOrEmpty(steamPath)) {
        yield break;
      }

      var users = File.ReadAllText(Path.Combine(steamPath, "config", "loginusers.vdf"));
      var vdf = Vdf.Parse(users).Children.GetValueOrDefault("users");

      if (vdf == null) {
        yield break;
      }

      foreach (var (key, value) in vdf.Children) {
        if (long.TryParse(key, out var steamId)) {
          yield return new() {
            SteamId = steamId,
            ProfileName = value.Values.GetValueOrDefault("PersonaName"),
          };
        }
      }
    }
  }

  public class SteamProfile {
    public string? ProfileName { get; set; }

    public string DisplayName => $"{ProfileName} ({SteamId})";

    public long SteamId { get; set; }
  }

  public class Vdf {
    public Dictionary<string, string> Values { get; } = new();

    public Dictionary<string, Vdf> Children { get; } = new();

    public static Vdf Parse(string content) {
      return Get(new(content));
    }

    private static Vdf Get(VdfTokenizer tokenizer) {
      var section = new Vdf();
      while (!tokenizer.IsFinished) {
        switch (tokenizer.ReadNext()) {
          case null or VdfToken.End:
            return section;
          case VdfToken.Begin:
            throw new("Unexpected begin of section");
        }

        var key = tokenizer.Consume();
        switch (tokenizer.ReadNext()) {
          case null:
            throw new("Unexpected end of file");
          case VdfToken.End:
            throw new("Unexpected end of section");
          case VdfToken.Begin:
            section.Children[key] = Get(tokenizer);
            break;
          case VdfToken.String:
            section.Values[key] = tokenizer.Consume();
            break;
        }
      }

      return section;
    }

    private enum VdfToken {
      Begin,
      End,
      String,
    }

    private class VdfTokenizer {
      private readonly string _content;
      private int _pos;
      private string _value = "";

      public VdfTokenizer(string content) {
        _content = content;
        _pos = 0;
      }

      public bool IsFinished => _pos >= _content.Length;

      public VdfToken? ReadNext() {
        if (IsFinished) {
          return null;
        }

        while (char.IsWhiteSpace(_content[_pos])) {
          ++_pos;
          if (IsFinished) {
            return null;
          }
        }

        switch (_content[_pos++]) {
          case '{':
            return VdfToken.Begin;
          case '}':
            return VdfToken.End;
          case '"':
            var start = _pos;
            while (_content[_pos] != '"') {
              ++_pos;
              if (IsFinished) {
                return null;
              }
            }

            _value = _content.Substring(start, _pos++ - start);
            return VdfToken.String;
          default:
            throw new("Unexpected token");
        }
      }

      public string Consume() {
        var v = _value;
        _value = "";
        return v;
      }
    }
  }
}
