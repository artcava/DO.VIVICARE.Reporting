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

## FASE 2: PREPARA CONFIGURAZIONE PER VELOPACK (1 ora)

> **⚠️ IMPORTANTE:** Questa fase non riguarda un inesistente "ConfigurationService", ma il refactor della classe **XMLSettings.cs** già presente nel progetto.

### Step 2.1: Refactor XMLSettings.cs - Aggiorna Percorso Config (20 minuti)

**File da modificare:** `DO.VIVICARE.Reporter/XMLSettings.cs`

Localizza il costruttore:

```csharp
public XMLSettings()
{
    _XmlFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
        "Reporting", 
        "Settings.xml"
    );
    LoadDocument();
}
```

**Sostituisci con:**

```csharp
private string _XmlFilePath { get; }

public XMLSettings()
{
    // Percorso portable in AppData (persiste tra aggiornamenti)
    string appDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "DO.VIVICARE.Reporting"
    );
    
    // Assicura directory esista
    if (!Directory.Exists(appDataPath))
        Directory.CreateDirectory(appDataPath);
    
    _XmlFilePath = Path.Combine(appDataPath, "Settings.xml");
    
    LoadDocument();
}
```

**Perché:** 
- ✅ AppData persiste tra aggiornamenti Velopack
- ✅ Percorso standard Windows
- ✅ Non dipende da MyDocuments
- ✅ Funziona con auto-updates

### Step 2.2: Aggiungi Backup Automatico (15 minuti)

**Nel metodo `Save()` di XMLSettings.cs:**

Sostituisci:

```csharp
public void Save()
{
    if (File.Exists(_XmlFilePath))
        Save(_XmlFilePath);
}
```

**Con:**

```csharp
public void Save()
{
    try
    {
        if (File.Exists(_XmlFilePath))
        {
            // Crea backup prima di salvare (protezione corruzione)
            string backupPath = _XmlFilePath + ".backup";
            if (File.Exists(backupPath))
                File.Delete(backupPath);
            
            // Backup del file corrente
            File.Copy(_XmlFilePath, backupPath, overwrite: true);
            
            // Salva nuovo config
            Save(_XmlFilePath);
            
            // Cleanup: elimina backup vecchi (> 30 giorni)
            CleanOldBackups(maxDays: 30);
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Errore durante Save: {ex.Message}");
        throw;
    }
}

private void CleanOldBackups(int maxDays = 30)
{
    string backupPath = _XmlFilePath + ".backup";
    if (File.Exists(backupPath))
    {
        var fileInfo = new FileInfo(backupPath);
        if ((DateTime.Now - fileInfo.LastWriteTime).TotalDays > maxDays)
        {
            try
            {
                File.Delete(backupPath);
            }
            catch { /* silent */ }
        }
    }
}
```

**Perché:**
- ✅ Protezione da corruzione file
- ✅ Rollback a versione precedente disponibile
- ✅ Cleanup automatico per evitare bloat disco

### Step 2.3: Aggiungi Versionamento Configurazione (15 minuti)

**Nel metodo `LoadDocument()` di XMLSettings.cs:**

Sostituisci:

```csharp
private void LoadDocument()
{
    if (DocumentElement != null) return;

    if (!File.Exists(_XmlFilePath))
    {
        LoadXml("<SETTINGS></SETTINGS>");
        Libraries = DocumentElement.AppendChild(CreateElement("LIBRARIES"));
        Documents = Libraries.AppendChild(CreateElement("DOCUMENTS"));
        Reports = Libraries.AppendChild(CreateElement("REPORTS"));
        Save(_XmlFilePath);
    }
    else
    {
        base.Load(_XmlFilePath);
        Libraries = DocumentElement.FirstChild;
        Documents = Libraries.SelectSingleNode("DOCUMENTS");
        Reports = Libraries.SelectSingleNode("REPORTS");
    }
}
```

**Con:**

```csharp
private const string CONFIG_VERSION = "1.0";

private void LoadDocument()
{
    if (DocumentElement != null) return;

    if (!File.Exists(_XmlFilePath))
    {
        // Crea nuovo config con versione
        LoadXml($"<SETTINGS VERSION='{CONFIG_VERSION}'></SETTINGS>");
        Libraries = DocumentElement.AppendChild(CreateElement("LIBRARIES"));
        Documents = Libraries.AppendChild(CreateElement("DOCUMENTS"));
        Reports = Libraries.AppendChild(CreateElement("REPORTS"));
        Save(_XmlFilePath);
    }
    else
    {
        base.Load(_XmlFilePath);
        
        // Verifica versione config
        string version = DocumentElement.GetAttribute("VERSION");
        if (string.IsNullOrEmpty(version))
        {
            // Config legacy senza versione - aggiorna
            version = "0.0";
            MigrateConfiguration(version);
        }
        else if (version != CONFIG_VERSION)
        {
            // Config da versione diversa - esegui migration
            MigrateConfiguration(version);
        }
        
        Libraries = DocumentElement.FirstChild;
        Documents = Libraries.SelectSingleNode("DOCUMENTS");
        Reports = Libraries.SelectSingleNode("REPORTS");
    }
}

private void MigrateConfiguration(string oldVersion)
{
    // Logica per migrare config da versioni precedenti
    Debug.WriteLine($"Config migrated from v{oldVersion} to {CONFIG_VERSION}");
    
    // Aggiungi versione al root se mancante
    if (DocumentElement.GetAttribute("VERSION") == "")
        DocumentElement.SetAttribute("VERSION", CONFIG_VERSION);
    
    // Salva config migrato
    Save();
}
```

**Perché:**
- ✅ Supporta aggiornamenti app future
- ✅ Previene errori di parsing
- ✅ Consente rollback intelligente

### Step 2.4: TEST - Verifica Persistenza Config (10 minuti)

**Test Case 1: Config persiste in AppData**

```csharp
// Aggiungi questo test in DO.VIVICARE.Tests o crea un test simple

[Test]
public void TestConfigPersistsInAppData()
{
    // Arrange
    var settings = new XMLSettings();
    settings.AddLibrary(XMLSettings.LibraryType.Document, "TestDoc");
    settings.Save();
    
    // Verifica file esiste in AppData
    string appDataPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "DO.VIVICARE.Reporting",
        "Settings.xml"
    );
    
    // Assert
    Assert.IsTrue(File.Exists(appDataPath), "Settings.xml non trovato in AppData");
    
    // Verifica contenuto
    var loadedSettings = new XMLSettings();
    var values = loadedSettings.GetDocumentValues(XMLSettings.LibraryType.Document, "TestDoc");
    Assert.IsNotNull(values, "Document non trovato dopo reload");
}

[Test]
public void TestBackupCreated()
{
    // Arrange
    var settings = new XMLSettings();
    settings.AddLibrary(XMLSettings.LibraryType.Document, "TestDoc1");
    settings.Save();
    
    string backupPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "DO.VIVICARE.Reporting",
        "Settings.xml.backup"
    );
    
    // Assert
    Assert.IsTrue(File.Exists(backupPath), "Backup file non creato");
}

[Test]
public void TestConfigVersioning()
{
    // Arrange
    var settings = new XMLSettings();
    settings.Save();
    
    // Ricarica e verifica versione
    var reloadedSettings = new XMLSettings();
    string version = reloadedSettings.DocumentElement.GetAttribute("VERSION");
    
    // Assert
    Assert.AreEqual("1.0", version, "Versione config non corretta");
}
```

**Eseguire i test:**

```bash
cd DO.VIVICARE.Tests
dotnet test --filter "ConfigPersist or BackupCreated or Versioning"
```

**Checklist Verifica:**
- [ ] ✅ Settings.xml creato in `AppData\DO.VIVICARE.Reporting`
- [ ] ✅ Settings.xml.backup creato dopo primo save
- [ ] ✅ Versione config è "1.0"
- [ ] ✅ Test superati

### Step 2.5: Aggiorna app.config (10 minuti)

**File:** `DO.VIVICARE.Reporter/app.config`

Aggiungi sezione Velopack:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <!-- Velopack Configuration -->
    <add key="Velopack:Enabled" value="true" />
    <add key="Velopack:UpdateCheckInterval" value="3600" />
    <!-- 3600 secondi = 1 ora tra controlli aggiornamenti -->
    
    <add key="Velopack:UpdateUrl" 
         value="https://github.com/artcava/DO.VIVICARE.Reporting/releases/download" />
    
    <!-- Existing Configuration -->
    <!-- ... altre config ... -->
  </appSettings>
</configuration>
```

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
- [ ] XMLSettings.cs refactored con percorso AppData
- [ ] Backup automatico implementato in XMLSettings
- [ ] Versionamento config aggiunto
- [ ] Test persistenza config creati e superati
- [ ] app.config configurato con Velopack settings
- [ ] UpdateService creato
- [ ] Certificate creato e aggiunto a GitHub Secrets
- [ ] Workflow `.github/workflows/velopack-release.yml` creato
- [ ] Build locale riuscito
- [ ] Test MSI download + install funziona
- [ ] Uninstall e reinstall funziona
- [ ] Config persiste tra reinstall
- [ ] Ready per v1.0.0 release

---

**Prossimo step:** Vedi `06_checklist_go_live.md` prima di rilasciare.
