# ❓ FAQ & TROUBLESHOOTING GUIDE

---

## SETUP ISSUES

### Q: "Velopack install fails"
**A:** Clear cache and retry:
```bash
dotnet nuget locals all --clear
dotnet add package Velopack
```

### Q: "Cannot find vpk command"
**A:**
```bash
dotnet tool install Velopack.Cmd -g
vpk --version
```

---

## CODE SIGNING

### Q: "Certificate import fails"
**A:**
```powershell
certutil -importpfx "cert.pfx"
```

### Q: "SmartScreen warns after signing"
**A:** Build reputation takes ~2 weeks. Warnings disappear automatically.

---

## BUILD ISSUES

### Q: "vpk pack fails - 'No assemblies found'"
**A:**
```bash
dotnet build -c Release
vpk pack windows --releaseDir "bin/Release/net8.0-windows" --outputDir releases
```

---

## CONFIGURATION

### Q: "Config files lost after update"
**A:** Verify ConfigurationService uses `%AppData%`:
```csharp
var path = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "DO.VIVICARE",
    "config.json"
);
```

---

## UPDATE ISSUES

### Q: "Update check hangs"
**A:**
```csharp
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
var update = await _updateManager.CheckForUpdatesAsync(cts.Token);
```

---

## GITHUB ACTIONS

### Q: "Release job not triggering"
**A:** Verify workflow trigger:
```yaml
on:
  push:
    tags: ['v*']
```

### Q: "Secrets not available"
**A:**
```bash
gh secret list
gh secret set CODESIGN_CERTIFICATE_BASE64 < cert.pfx
gh secret set CODESIGN_PASSWORD "password"
```

---

## RESOURCES

- Official Docs: https://docs.velopack.io/
- GitHub: https://github.com/velopack/velopack
- Discord: https://discord.gg/velopack
