# 🔧 GUIDA IMPLEMENTAZIONE VELOPACK
## Step-by-Step per DO.VIVICARE.Reporting

**Data:** 25 Gennaio 2026  
**Tempo Stimato:** 4-5 giorni (part-time)

---

## 📋 PREREQUISITI

✅ Visual Studio 2022 (17.8+) o CLI  
✅ .NET 8.0 SDK installed  
✅ GitHub CLI (`gh`) installed  
✅ Code signing certificate (o crearne uno test)  
✅ Administrator access (local machine)

---

## FASE 1: SETUP VELOPACK (2 ore)

### Step 1.1: Installa Velopack NuGet

```bash
cd src/DO.VIVICARE.Reporting
dotnet add package Velopack
```

### Step 1.2: Modifica Program.cs

```csharp
// DOPO:
static void Main()
{
    VelopackApp.Build().Run();
    Application.EnableVisualStyles();
    Application.Run(new MainForm());
}
```

### Step 1.3: Configura appsettings.json

```json
{
  "Velopack": {
    "UpdateUrl": "https://github.com/artcava/DO.VIVICARE.Reporting",
    "AppId": "DO.VIVICARE.Reporting",
    "AutoInstall": true,
    "AutoStart": true
  }
}
```

---

## FASE 2: AGGIORNA ConfigurationService (1 ora)

```csharp
public static string DataFolder => 
    Path.Combine(
        Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData
        ),
        "DO.VIVICARE"
    );
```

---

## FASE 3: IMPLEMENTA UpdateService (1.5 ore)

```csharp
using Velopack;

public class UpdateService
{
    private UpdateManager _updateManager;

    public async Task CheckForUpdatesAsync()
    {
        using var manager = new UpdateManager(
            "https://github.com/artcava/DO.VIVICARE.Reporting"
        );
        var update = await manager.CheckForUpdatesAsync();
        if (update != null)
            await manager.ApplyUpdatesAndRestartAsync(update);
    }
}
```

---

## FASE 4: CODE SIGNING (1.5 ore)

```bash
gh secret set CODESIGN_CERTIFICATE_BASE64 < <(cat certificato.pfx | base64)
gh secret set CODESIGN_PASSWORD "<password>"
```

---

## FASE 5: GITHUB ACTIONS WORKFLOW (2 ore)

```yaml
name: CI/CD Pipeline - Velopack

on:
  push:
    tags: ['v*']

jobs:
  release-app:
    if: startsWith(github.ref, 'refs/tags/v')
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
      - name: Pack with Velopack
        run: vpk pack windows --releaseDir src/DO.VIVICARE.Reporting/bin/Release --outputDir releases --msiDeploymentTool
      - name: Create GitHub Release
        uses: softprops/action-gh-release@v1
        with:
          files: releases/*
```

---

## FASE 6-7: TESTING & LAUNCH (2 ore)

```bash
git tag v1.0.0
git push origin v1.0.0
# Watch workflow run and download MSI
```
