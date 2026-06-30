using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;

namespace Ini
{
    public class SimpleIni
    {
        private readonly string _path;
        private readonly Dictionary<string, Dictionary<string, string>> _data = new();
        private FileSystemWatcher _watcher;
        private Timer _reloadTimer;

        public SimpleIni(string path, bool enableHotReload = true)
        {
            _path = path;
            Parse();
            if (enableHotReload) EnableHotReload();
        }

        private void Parse()
        {
            _data.Clear();
            string currentSection = "[]";
            foreach (var rawLine in File.ReadLines(_path))
            {
                var line = rawLine.Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith(";") || line.StartsWith("#")) continue;
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSection = line;
                    if (!_data.ContainsKey(currentSection)) _data[currentSection] = new Dictionary<string, string>();
                    continue;
                }
                var parts = line.Split('=', 2);
                if (parts.Length != 2) continue;
                string key = parts[0].Trim();
                string value = parts[1].Trim();
                if (!_data.ContainsKey(currentSection)) _data[currentSection] = new Dictionary<string, string>();
                _data[currentSection][key] = value;
            }
        }

        public string Get(string key, string section = "", string defaultValue = "")
        {
            section = Normalize(section);
            if (_data.TryGetValue(section, out var sec)) if (sec.TryGetValue(key, out var value)) return value;
            return defaultValue;
        }
        public T Get<T>(string key, string section = "", T defaultValue = default)
        {
            string value = Get(key, section);
            if (string.IsNullOrEmpty(value)) return defaultValue;
            try
            {
                Type t = typeof(T);
                if (t == typeof(int)) return (T)(object)int.Parse(value);
                if (t == typeof(float)) return (T)(object)float.Parse(value, CultureInfo.InvariantCulture);
                if (t == typeof(double)) return (T)(object)double.Parse(value, CultureInfo.InvariantCulture);
                if (t == typeof(bool)) return (T)(object)ParseBool(value);
                if (t == typeof(string)) return (T)(object)value;
                return defaultValue;
            }
            catch { return defaultValue; }
        }

        private bool ParseBool(string value)
        {
            value = value.ToLower();
            return value == "1" || value == "true" || value == "yes" || value == "on";
        }

        public void Set(string key, string value, string section = "")
        {
            section = Normalize(section);
            if (!_data.ContainsKey(section)) _data[section] = new Dictionary<string, string>();
            _data[section][key] = value;
        }

        public void Save()
        {
            using var writer = new StreamWriter(_path);
            foreach (var section in _data)
            {
                writer.WriteLine(section.Key);
                foreach (var kv in section.Value) writer.WriteLine($"{kv.Key}={kv.Value}");
                writer.WriteLine();
            }
        }

        private string Normalize(string section)
        {
            if (string.IsNullOrWhiteSpace(section)) return "[]";
            if (!section.StartsWith("[")) return $"[{section}]";
            return section;
        }

        private void EnableHotReload()
        {
            _watcher = new FileSystemWatcher(Path.GetDirectoryName(_path))
            {
                Filter = Path.GetFileName(_path),
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };
            _watcher.Changed += (_, _) => ScheduleReload();
            _watcher.Created += (_, _) => ScheduleReload();
        }

        private void ScheduleReload()
        {
            _reloadTimer?.Dispose();
            _reloadTimer = new Timer(_ =>
            {
                try { Parse(); }
                catch { }
            }, null, 150, Timeout.Infinite);
        }
    }
}
