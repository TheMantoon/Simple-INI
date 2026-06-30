# Simple INI ⚙️

A lightweight, fast and dependency-free INI parser for C#.

Supports both modern INI format and legacy config styles used in older applications and game launchers.

---

## 🚀 Features

- No external dependencies
- Fast in-memory caching (parses once)
- Hot reload support (FileSystemWatcher)
- Typed getters (int, float, bool, string)
- Supports:
  - `[SECTION]` format
  - legacy `[]` global section format
- Simple save/write support
- Easy integration into games and tools

---

## 📁 Supported INI formats

### Modern format
[GENERAL]
Version=1.0.5
Path=C:\Games\MyGame
Fullscreen=true

---

### Legacy format
[]
Version=1.0.5
Path=C:\Games\MyGame
Fullscreen=true

---

## 🧑‍💻 Usage

### Add namespace
```csharp
var ini = new SimpleIni("config.ini");
```

### Create instance
```csharp
using Ini;
```

### Get string value
```csharp
string version = ini.Get<string>("Version");
```

### Typed values
```csharp
int versionCode = ini.Get<int>("VersionCode");
float volume = ini.Get<float>("Volume");
bool fullscreen = ini.Get<bool>("Fullscreen");
```

### Default values
```csharp
int quality = ini.Get<int>("GraphicsQuality", defaultValue: 3);
```

### Set values
```csharp
ini.Set("Version", "1.0.6");
```

### Save file
```csharp
ini.Save();
```

---

## 🔄 Hot Reload

```csharp
var ini = new SimpleIni("config.ini", enableHotReload: true);
```

File will auto-reload when changed.

---

## 💡 Use cases

- Game launchers
- Mods/config systems
- Small tools
- Cross-platform apps

---

## ⚡ Philosophy

Minimal, fast, predictable.  
No reflection, no dependencies, no magic.

---

## 📌 Limitations

- No nested structures
- No arrays
- No comment persistence
- Not full INI standard (by design)

---

## 📜 License

Simple INI is licensed under the [MIT](https://choosealicense.com/licenses/mit). Please review the [LICENSE](LICENSE) file.
