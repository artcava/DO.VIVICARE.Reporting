# 🔧 GUIDA IMPLEMENTAZIONE VELOPACK
## Step-by-Step per DO.VIVICARE.Reporting (.NET Framework 4.8)

**Data:** 25 Gennaio 2026  
**Framework:** .NET Framework 4.8 (con supporto nativo Velopack `NETFRAMEWORK48`)  
**Tempo Stimato:** 4-5 giorni (part-time)

---

## 📋 PREREQUISITI

✅ Visual Studio 2022 (o versione che supporta .NET Framework 4.8)  
✅ .NET SDK 6.0+ installato (solo per il tool `vpk` - l'app resta .NET Framework 4.8)  
✅ GitHub CLI (`gh`) installed  
✅ Code signing certificate (o crearne uno test)  
✅ Administrator access (local machine)

---

## FASE 1: SETUP VELOPACK (2 ore)

### Step 1.1: Installa Velopack NuGet

**Opzione A: Via Visual Studio (consigliato per .NET Framework 4.8)**

1. Apri Visual Studio
2. Soluzione Explorer → Tasto destro su progetto
3. → **Manage NuGet Packages**
4. Ricerca: `Velopack`
5. Seleziona versione recente (0.0.1298+)
6. Click **Install**

**Opzione B: Via Package Manager Console**

```powershell
Install-Package Velopack
```

**Opzione C: Via CLI (richiede .NET SDK moderno)**

```bash
dotnet add package Velopack
```

**Verifica:** Nel tuo .csproj o packages.config deve apparire Velopack.

### Step 1.2: Modifica Program.cs o Program.Main()

Aggiungi all'inizio del main:

```csharp
using Velopack;

static class Program
{
    [STAThread]
    static void Main()
    {
        // AGGIUNGI QUESTA RIGA
        VelopackApp.Build().Run();
        
        // POI IL RESTO DEL TUO CODE
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}
```

**Cosa fa:** Integra il gestore di aggiornamenti di Velopack direttamente nell'app.

### Step 1.3: Configura appsettings.json (o Settings)

Se usi `appsettings.json`:

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

Se usi `app.config` (più tipico per .NET 4.8):

```xml
<configuration>
  <appSettings>
    <add key="VelopackUpdateUrl" value="https://github.com/artcava/DO.VIVICARE.Reporting" />
    <add key="VelopackAppId" value="DO.VIVICARE.Reporting" />
  </appSettings>
</configuration>
```

---

## FASE 2: AGGIORNA ConfigurationService (1 ora)

Sposta la cartella dati da app directory a `%AppData%`:

```csharp
using System;
using System.IO;

public static class ConfigurationService
{
    // PRIMA (SBAGLIATO):
    // public static string DataFolder => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
    
    // DOPO (CORRETTO):
    public static string DataFolder => 
        Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData
            ),
            "DO.VIVICARE"
        );
    
    public static string ConfigFilePath => 
        Path.Combine(DataFolder, "config.json");
    
    public static string DatabasePath => 
        Path.Combine(DataFolder, "database.db");
    
    // Assicurati che cartella esista
    static ConfigurationService()
    {
        if (!Directory.Exists(DataFolder))
            Directory.CreateDirectory(DataFolder);
    }
}
```

**Perché:** Così i dati utente persistono tra aggiornamenti.

---

## FASE 3: IMPLEMENTA UpdateService (1.5 ore)

Crea una nuova classe per gestire gli aggiornamenti:

```csharp
using Velopack;
using System.Threading.Tasks;
using System.Windows.Forms;

public class UpdateService
{
    private UpdateManager _updateManager;

    public async Task InitializeAsync()
    {
        try
        {
            _updateManager = new UpdateManager("https://github.com/artcava/DO.VIVICARE.Reporting");
            await CheckForUpdatesAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Errore inizializzazione update: {ex.Message}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    public async Task CheckForUpdatesAsync()
    {
        try
        {
            using var manager = new UpdateManager("https://github.com/artcava/DO.VIVICARE.Reporting");
            var update = await manager.CheckForUpdatesAsync();
            
            if (update != null)
            {
                var result = MessageBox.Show(
                    $"Nuova versione disponibile: {update.TargetFullRelease.Version}\n\nTua versione: {update.CurrentlyInstalledVersion}\n\nAggiornare adesso?",
                    "Aggiornamento Disponibile",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

                if (result == DialogResult.Yes)
                {
                    await manager.ApplyUpdatesAndRestartAsync(update);
                }
            }
        }
        catch (Exception ex)
        {
            // Silent fail - non bloccare app
            System.Diagnostics.Debug.WriteLine($"Update check failed: {ex.Message}");
        }
    }
}
```

**Nel MainForm, aggiungi:**

```csharp
private UpdateService _updateService;

public MainForm()
{
    InitializeComponent();
    _updateService = new UpdateService();
}

private async void MainForm_Load(object sender, EventArgs e)
{
    // Controlla aggiornamenti al caricamento (background)
    _ = _updateService.InitializeAsync();
    
    // ... resto del load code
}
```

---

## FASE 4: CODE SIGNING (1.5 ore)

### Step 4.1: Crea certificate test (se non ne hai uno)

```powershell
# Crea certificato self-signed (valid 365 giorni)
New-SelfSignedCertificate `
  -CertStoreLocation "cert:\CurrentUser\My" `
  -Subject "CN=DO.VIVICARE.Reporting" `
  -FriendlyName "DO.VIVICARE Code Signing" `
  -Type CodeSigningCert `
  -NotAfter (Get-Date).AddYears(1)

# Esporta in PFX
$cert = Get-ChildItem -Path Cert:\CurrentUser\My | Where-Object {$_.Subject -eq "CN=DO.VIVICARE.Reporting"}
Export-PfxCertificate -Cert $cert -FilePath "certificate.pfx" -Password (ConvertTo-SecureString -String "your-password" -AsPlainText -Force)
```

### Step 4.2: Aggiungi a GitHub Secrets

```bash
# PowerShell
$cert = [Convert]::ToBase64String((Get-Content "certificate.pfx" -Encoding Byte))
echo "Base64 Certificate:" $cert
```

Copia il risultato e:

```bash
gh secret set CODESIGN_CERTIFICATE_BASE64 --body "PASTE_HERE"
gh secret set CODESIGN_PASSWORD --body "your-password"
```

Verifica:

```bash
gh secret list
```

---

## FASE 5: GITHUB ACTIONS WORKFLOW (2 ore)

Crea `.github/workflows/velopack-release.yml`:

```yaml
name: CI/CD Pipeline - Velopack Release

on:
  push:
    tags: ['v*']

jobs:
  release-app:
    if: startsWith(github.ref, 'refs/tags/v')
    runs-on: windows-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET SDK
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      
      - name: Restore
        run: dotnet restore
      
      - name: Build Release
        run: dotnet build -c Release --no-restore
      
      - name: Install Velopack CLI
        run: dotnet tool install -g vpk
      
      - name: Pack with Velopack
        run: vpk pack windows --releaseDir "DO.VIVICARE.UI\bin\Release" --outputDir releases --msiDeploymentTool
      
      - name: Create GitHub Release
        uses: softprops/action-gh-release@v1
        with:
          files: releases/*
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

---

## FASE 6-7: TESTING & LAUNCH (2 ore)

### Step 6.1: Test Locale

```bash
# Build in Release
dotnet build -c Release

# Testa che app parte normalmente
./DO.VIVICARE.UI/bin/Release/DO.VIVICARE.UI.exe
```

### Step 6.2: Create Release Tag

```bash
git tag v1.0.0 -m "Initial Velopack Release"
git push origin v1.0.0
```

GitHub Actions partirà automaticamente.

### Step 6.3: Verifica Artifacts

1. Vai a https://github.com/artcava/DO.VIVICARE.Reporting/releases
2. Controlla che sia stato generato:
   - `DO.VIVICARE.Reporting-1.0.0.msi`
   - `DO.VIVICARE.Reporting-1.0.0-full.nupkg`
   - `DO.VIVICARE.Reporting-1.0.0-delta.nupkg`

### Step 6.4: Test MSI

```bash
# Download MSI
# Double-click e installa
# Test che app funziona
# Check che app compare in "Add/Remove Programs"
```

---

## FASE 8: MONITORAGGIO POST-LAUNCH (1 ora)

Monitora per 2 ore:
- [ ] Installation success rate > 99%
- [ ] No crash reports
- [ ] Update checks working
- [ ] Users happy 😊

---

## 📝 Troubleshooting Rapido

**"vpk command not found"**
```bash
dotnet tool install -g vpk
vpk --version
```

**"Build fails - missing .NET Framework"**
```bash
# Installa .NET Framework 4.8 targeting pack
# Visual Studio Installer → Modify → Desktop Development → .NET Framework 4.8
```

**"Velopack di non riconosce il progetto"**
- Verifica che `ReleaseDir` nel workflow punti a `bin\Release` corretto
- Verifica che il `.csproj` sia valido

**"Certificato scade troppo presto"**
- Rinnovare certificate prima scadenza
- Aggiornare GitHub Secret

---

## ✅ CHECKLIST FASE COMPLETAMENTO

- [ ] Velopack NuGet aggiunto
- [ ] Program.cs modificato con `VelopackApp.Build().Run()`
- [ ] ConfigurationService punta a `%AppData%`
- [ ] UpdateService creato
- [ ] appsettings.json configurato
- [ ] Certificate creato e aggiunto a GitHub Secrets
- [ ] Workflow `.github/workflows/velopack-release.yml` creato
- [ ] Build locale riuscito
- [ ] Test MSI download + install funziona
- [ ] Uninstall e reinstall funziona
- [ ] Config persiste tra reinstall
- [ ] Ready per v1.0.0 release

---

**Prossimo step:** Vedi `06_checklist_go_live.md` prima di rilasciare.
