# DO.VIVICARE Reporting - Installation & Plugin Management

Complete guide to installation, setup and plugin management for DO.VIVICARE Reporting.

## Table of Contents

- [System Requirements](#system-requirements)
- [Installation for Users](#installation-for-users)
- [Plugin Management](#plugin-management)
- [Developer Setup](#developer-setup)
- [Troubleshooting](#troubleshooting)

---

## System Requirements

### Operating Systems
- Windows 7 SP1 or higher
- Windows Server 2008 R2 or higher

### Software Prerequisites
- **.NET Framework 4.6.1 or higher** (4.8 recommended)
- **Visual Studio 2019 or higher** (for development only)
- **Microsoft Excel** (optional, for output viewing)

### Hardware
- Processor: x86/x64 compatible
- RAM: 2 GB minimum (4 GB recommended)
- Disk Space: ~500 MB for application

---

## Installation for Users

### First-Time Installation

1. **Download Installer**
   - Go to [GitHub Releases](https://github.com/artcava/DO.VIVICARE.Reporting/releases/latest)
   - Download `DO.VIVICARE-Setup-[version].msi`

2. **Run Installer**
   - Double-click the `.msi` file
   - Follow the installation wizard
   - Default location: `C:\Program Files\DO.VIVICARE\`

3. **Launch Application**
   - Desktop icon created automatically
   - Or navigate to installation directory and run `DO.VIVICARE.UI.exe`

✅ **Result**: Application ready with plugin download capability

### Verify Installation

After installation:

```bash
# Check if application launches
C:\Program Files\DO.VIVICARE\DO.VIVICARE.UI.exe

# Verify .NET Framework
dir %WINDIR%\Microsoft.NET\Framework*\*4.8*
```

---

## Plugin Management

### What are Plugins?

Plugins are independent libraries that extend functionality:

- **Document Libraries** (14 modules) - Data structure definitions
- **Report Libraries** (3 modules) - Report generation logic

### Installing Plugins via UI

1. **Launch Application**
   - Open `DO.VIVICARE.UI.exe`

2. **Access Plugin Manager**
   - Menu: **Tools** > **Plugin Manager**
   - Or use keyboard shortcut

3. **Available Plugins Tab**
   - View all available plugins with versions
   - See installed vs available versions

4. **Install Plugin**
   - Click `[Download]` button next to desired plugin
   - Automatic download and installation
   - ✅ No restart required (hot-reload)

5. **Verify Installation**
   - Switch to "Installed" tab
   - Confirm plugin appears with correct version

### Plugin Architecture

```
APPLICATION (v1.2.0)
├─ Single installer: DO.VIVICARE-Setup-1.2.0.msi
└─ Include: UI + Reporter library

PLUGIN LIBRARIES (Independent versions)
├─ Document Libraries (14 modules)
│  ├─ ADI Alta Intensita
│  ├─ ADI Bassa Intensita
│  ├─ ASST
│  ├─ Comuni
│  ├─ Lazio Health Worker
│  ├─ Min San
│  ├─ Prestazioni
│  ├─ Prezzi
│  ├─ Rendiconto
│  ├─ Report 16
│  ├─ Report 18
│  ├─ Valorizzazione
│  ├─ Valorizzazioni ADI Alta
│  └─ ZSD Fatture
│
└─ Report Libraries (3 modules)
   ├─ Allegato ADI
   ├─ Dietetica
   └─ Valorizzazione
```

### Plugin Manifest

Plugins are managed via `manifest.json` which contains:

```json
{
  "documents": [
    {
      "id": "document.adialtaintensita",
      "name": "ADI Alta Intensita",
      "version": "1.0.5",
      "downloadUrl": "https://...",
      "releaseDate": "2025-12-10"
    }
  ],
  "reports": [
    {
      "id": "report.allegatoadi",
      "name": "Allegato ADI",
      "version": "1.0.3",
      "downloadUrl": "https://...",
      "releaseDate": "2025-12-15"
    }
  ]
}
```

---

## Developer Setup

### Clone Repository

```bash
git clone https://github.com/artcava/DO.VIVICARE.Reporting.git
cd DO.VIVICARE.Reporting
```

### Open Solution

```bash
# With Visual Studio
start DO.VIVICARE.Reporting.sln

# Or from command line
devenv DO.VIVICARE.Reporting.sln
```

### Restore Dependencies

```bash
nuget restore DO.VIVICARE.Reporting.sln
```

Or let Visual Studio handle it automatically.

### Build Solution

**Via Visual Studio:**
- Menu: Build > Build Solution (Ctrl+Shift+B)
- Select configuration: Debug or Release

**Via Command Line:**
```bash
msbuild DO.VIVICARE.Reporting.sln /p:Configuration=Release /p:Platform="Any CPU"
```

### Verify Build

```bash
# Check compiled files
dir DO.VIVICARE.Reporter\bin\Release\DO.VIVICARE.Reporter.dll
dir DO.VIVICARE.UI\bin\Release\DO.VIVICARE.UI.exe
```

### Configuration Files

**Development Configuration:**
- Location: `DO.VIVICARE.Reporter/app.config`
- Settings: Database connections, output paths, logging

**User Configuration** (after installation):
- Location: `%APPDATA%\DO.VIVICARE\config.json`
- Managed by Plugin Manager

---

## Release Process

### For Application Updates (v1.2.0 -> v1.2.1)

1. **Update Version**
   - Edit `Properties/AssemblyInfo.cs`:
   ```csharp
   [assembly: AssemblyVersion("1.2.1.0")]
   ```

2. **Commit & Push**
   ```bash
   git add .
   git commit -m "Release v1.2.1: UI and Reporter updates"
   git push origin master
   ```

3. **Create Release Tag**
   ```bash
   git tag -a v1.2.1 -m "Release version 1.2.1"
   git push origin v1.2.1
   ```

**GitHub Actions automatically:**
- ✅ Builds MSI installer
- ✅ Uploads to GitHub Releases
- ✅ Generates SHA256 checksum

### For Plugin Updates (e.g., ADI Alta Intensita v1.1.0)

1. **Update Plugin Version**
   - Edit `DO.VIVICARE.Document.ADIAltaIntensita/Properties/AssemblyInfo.cs`:
   ```csharp
   [assembly: AssemblyVersion("1.1.0.0")]
   ```

2. **Commit Changes**
   ```bash
   git add DO.VIVICARE.Document.ADIAltaIntensita/
   git commit -m "Update: Document.ADIAltaIntensita v1.1.0"
   git push origin master
   ```

3. **Create Plugin Tag**
   ```bash
   git tag -a plugin/document.adialtaintensita/1.1.0 -m "ADI Alta Intensita update"
   git push origin plugin/document.adialtaintensita/1.1.0
   ```

**GitHub Actions automatically:**
- ✅ Builds only the specified plugin
- ✅ Uploads DLL to GitHub Releases
- ✅ Updates manifest.json

---

## Troubleshooting

### Installation Issues

#### Error: "MSI installation failed"
- Ensure .NET Framework 4.8 is installed
- Check disk space (minimum 500 MB)
- Run installer with administrator privileges
- Check Windows Event Viewer for detailed error

#### Error: "Application won't start"
- Verify .NET Framework 4.6.1+ installed:
  ```bash
  reg query "HKLM\Software\Microsoft\NET Framework Setup\NDP\v4" /s
  ```
- Check application log: `%APPDATA%\DO.VIVICARE\logs\`
- Reinstall application

### Plugin Issues

#### Plugins not appearing in Plugin Manager
- Check internet connection
- Verify manifest.json is accessible
- Clear application cache: `%APPDATA%\DO.VIVICARE\cache\`
- Restart application

#### Plugin download fails
- Check disk space
- Verify write permissions to plugin directory
- Check firewall/antivirus restrictions
- Try manual download from GitHub Releases

#### Plugin not loading after install
- Restart application
- Check plugin version compatibility
- Review application log for errors
- Verify plugin DLL integrity: `certutil -hashfile plugin.dll SHA256`

### Report Generation Issues

#### Report generation is slow
- Close other applications to free memory
- Reduce report data size or date range
- Check disk I/O performance
- See [TROUBLESHOOTING.md](TROUBLESHOOTING.md) for optimization tips

#### Excel file corruption
- Update EPPlus library (see packages.config)
- Increase available disk space
- Check file permissions in output directory
- Try different output location

### Configuration Issues

#### Settings not persisting
- Verify config.json location: `%APPDATA%\DO.VIVICARE\config.json`
- Check file permissions
- Ensure application has write access
- Review configuration format validity

---

## Support & Resources

### Documentation
- [README.md](README.md) - Project overview
- [ARCHITECTURE.md](ARCHITECTURE.md) - Technical architecture
- [DEPLOYMENT.md](DEPLOYMENT.md) - Release and versioning
- [PLUGIN_MANAGER.md](PLUGIN_MANAGER.md) - Detailed plugin guide

### Getting Help
1. Check [GitHub Issues](https://github.com/artcava/DO.VIVICARE.Reporting/issues)
2. Search [GitHub Discussions](https://github.com/artcava/DO.VIVICARE.Reporting/discussions)
3. Review [TROUBLESHOOTING.md](TROUBLESHOOTING.md) for common problems
4. Contact: mcavallo@welol.it

---

**Last Updated**: January 13, 2026  
**Maintained by**: Marco Cavallo
