# Strategia di Distribuzione e Deployment DO.VIVICARE Reporting

Documento strategico che definisce come evolvere da Azure Web App a una distribuzione moderna basata su GitHub con automazione CI/CD.

## 📋 Indice

1. [Situazione Attuale](#situazione-attuale)
2. [Problemi Identificati](#problemi-identificati)
3. [Soluzione Proposta](#soluzione-proposta)
4. [Architettura della Distribuzione](#architettura-della-distribuzione)
5. [Implementazione Tecnica](#implementazione-tecnica)
6. [Processo per Sviluppatori](#processo-per-sviluppatori)
7. [Processo per Utenti](#processo-per-utenti)
8. [Roadmap di Implementazione](#roadmap-di-implementazione)
9. [Fallback e Recovery](#fallback-e-recovery)

---

## Situazione Attuale

### Architettura Attuale

```
USER (Utente finale)
  │
  ├─ DO.VIVICARE.UI.exe (ClickOnce / .NET Framework)
  │   Scaricato da: http://artcava.azurewebsites.net/ [NON PIÙ DISPONIBILE]
  │   Aggiornamento: Automatico via ClickOnce (richiede IT auth)
  │
  └─ Librerie Dinamiche
      ├─ Document*.dll
      ├─ Report*.dll
      └─ Scaricate manualmente da: http://artcava.azurewebsites.net/reporting/
          Meccanismo: Click su bottone in frmSettings.cs
          File di lista: listDocuments.txt, listReports.txt

DEVELOPER
  │
  └─ Visual Studio
      ├─ Compila soluzione
      ├─ Pubblica in Azure Web App (ClickOnce)
      └─ Upload delle librerie sul server
```

### Problemi

#### Problema 1: Distribuzione UI - ClickOnce Obsoleto
- ❌ Azure Web App non è più disponibile
- ❌ ClickOnce richiede autenticazione IT ogni volta che si scarica l'app
- ❌ Difficile gestire versioni multiple
- ❌ Nessun versionamento chiaro
- ❌ Nessuno storico di release
- ❌ Nessuna automazione: tutto manuale

#### Problema 2: Distribuzione Librerie - Semi-Manuale
- ⚠️ Sviluppatore compila e upload manuale
- ⚠️ File di lista (listDocuments.txt) gestito manualmente
- ✓ Utente può scaricare (migliore di UI, ma non ottimale)
- ❌ Nessuna traccia di versioni
- ❌ Nessun controllo di integrità
- ❌ Nessun rollback facile

#### Problema 3: Mancanza di Governance
- ❌ Nessun CI/CD pipeline
- ❌ Build manuale e testing manuale
- ❌ Nessun controllo di qualità automatizzato
- ❌ Nessun audit trail di deployment

---

## Soluzione Proposta

### Vision Finale

```
┌─────────────────────────────────────────────────────────────────┐
│ GitHub DO.VIVICARE.Reporting                                    │
│ ├─ Master branch (stable)                                       │
│ ├─ Develop branch (dev)                                         │
│ └─ GitHub Releases (versionate)                                 │
│    ├─ DO.VIVICARE.UI v1.2.0                                    │
│    ├─ Document.ADI v1.2.0                                      │
│    └─ Report.AllegatoADI v1.2.0                                │
└─────────────────────────────────────────────────────────────────┘
          │
          ▼
┌─────────────────────────────────────────────────────────────────┐
│ GitHub Actions (CI/CD Pipeline)                                 │
│ ├─ Trigger: Push su develop/master o PR                         │
│ ├─ Build: MSBuild soluzione                                     │
│ ├─ Test: Esecuzione test suite (futuro)                        │
│ ├─ Package: Creazione .zip e .msi                              │
│ ├─ Release: Upload su GitHub Releases                          │
│ └─ Notify: Email agli stakeholder                              │
└─────────────────────────────────────────────────────────────────┘
          │
          ├─ UI Exe (ClickOnce moderno o MSIX)
          ├─ Document DLLs
          ├─ Report DLLs
          └─ Release Notes
          │
          ▼
┌─────────────────────────────────────────────────────────────────┐
│ Utenti Finali                                                    │
│                                                                  │
│ OPZIONE 1: Auto-Update (Raccomandato)                           │
│ ├─ App controlla GitHub API ogni avvio                          │
│ ├─ Se nuova versione: Scarica e installa automaticamente       │
│ └─ IT approva solo prima installazione                          │
│                                                                  │
│ OPZIONE 2: Manual Update (Fallback)                             │
│ ├─ Vai a GitHub Releases                                        │
│ ├─ Scarica DO.VIVICARE-Setup-v1.2.0.msi                        │
│ └─ Esegui installer (IT-approved)                              │
│                                                                  │
│ OPZIONE 3: Plugin Manager (Librerie)                            │
│ ├─ Bottone in frmSettings: "Check for Updates"                 │
│ ├─ Scarica automaticamente nuove versioni                       │
│ └─ Nessuna richiesta IT (già autorizzate nel setup iniziale)   │
└─────────────────────────────────────────────────────────────────┘
```

---

## Architettura della Distribuzione

### 1. GitHub Releases Structure

```
Repository: https://github.com/artcava/DO.VIVICARE.Reporting

Releases (Semantic Versioning: MAJOR.MINOR.PATCH)
│
├─ v1.2.0 (Latest)
│  ├─ DO.VIVICARE-UI-1.2.0.zip
│  ├─ DO.VIVICARE-UI-1.2.0.msi
│  ├─ Document.ADI-1.2.0.dll
│  ├─ Document.ADI-1.2.0.zip
│  ├─ Report.AllegatoADI-1.2.0.dll
│  ├─ Report.AllegatoADI-1.2.0.zip
│  ├─ RELEASE_NOTES.md
│  ├─ CHECKSUM.sha256
│  └─ MANIFEST.json
│
├─ v1.1.5
│  └─ [Precedenti asset]
│
└─ v1.0.0
   └─ [Iniziali asset]

Asset Manifest (MANIFEST.json)
{
  "version": "1.2.0",
  "releaseDate": "2026-01-15",
  "releaseNotes": "url_to_release_notes",
  "assets": [
    {
      "type": "ui",
      "name": "DO.VIVICARE.UI",
      "version": "1.2.0",
      "file": "DO.VIVICARE-UI-1.2.0.zip",
      "checksum": "sha256:...",
      "size": "15MB",
      "minFramework": "4.8"
    },
    {
      "type": "document",
      "name": "Document.ADI",
      "version": "1.2.0",
      "file": "Document.ADI-1.2.0.dll",
      "checksum": "sha256:...",
      "size": "2.5MB",
      "dependencies": []
    }
  ]
}
```

### 2. Versioning Strategy

**Semantic Versioning (MAJOR.MINOR.PATCH)**

```
1.2.3
│ │ └─ PATCH: Bug fixes, piccole correzioni
│ └──── MINOR: Nuove features compatibili
└────── MAJOR: Cambiamenti incompatibili

Esempi:
- 1.0.0 → 1.0.1: Bug fix in Report engine
- 1.0.1 → 1.1.0: Nuovo modulo Document.Dietetica
- 1.1.0 → 2.0.0: Migrazione da .NET Framework 4.8 a .NET 6

Tagging in Git:
v1.2.0-ui      (UI specifico)
v1.2.0-docs    (Document modules)
v1.2.0-reports (Report modules)
v1.2.0         (Release completa)
```

---

## Implementazione Tecnica

### Phase 1: Setup GitHub Actions CI/CD

**File: `.github/workflows/build-and-release.yml`**

```yaml
name: Build and Release

on:
  push:
    branches:
      - master
      - develop
    tags:
      - 'v*'
  workflow_dispatch:

jobs:
  build:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v3
      with:
        fetch-depth: 0
    
    - name: Setup MSBuild
      uses: microsoft/setup-msbuild@v1.1
    
    - name: Setup NuGet
      uses: NuGet/setup-nuget@v1
    
    - name: Restore NuGet packages
      run: nuget restore DO.VIVICARE.Reporting.sln
    
    - name: Build solution
      run: msbuild DO.VIVICARE.Reporting.sln /p:Configuration=Release /p:Platform="Any CPU"
    
    - name: Run Tests (future)
      run: dotnet test --configuration Release
      continue-on-error: true
    
    - name: Extract version from tag
      id: version
      run: |
        if ("${{ github.ref }}" -match "refs/tags/v(.*)$") {
          $version = $matches[1]
        } else {
          $version = "1.0.0-dev"
        }
        echo "version=$version" >> $env:GITHUB_OUTPUT
    
    - name: Package UI binaries
      run: |
        mkdir -p artifacts\ui
        copy DO.VIVICARE.UI\bin\Release\*.exe artifacts\ui\
        copy DO.VIVICARE.UI\bin\Release\*.dll artifacts\ui\
        copy DO.VIVICARE.UI\bin\Release\*.config artifacts\ui\
        Compress-Archive -Path artifacts\ui -DestinationPath DO.VIVICARE-UI-${{ steps.version.outputs.version }}.zip
    
    - name: Package Document DLLs
      run: |
        $docModules = @(
          "DO.VIVICARE.Document.ADIAltaIntensita",
          "DO.VIVICARE.Document.ADIBassaIntensita",
          "DO.VIVICARE.Document.ASST",
          # ... altri moduli
        )
        
        foreach ($module in $docModules) {
          $moduleVersion = "${{ steps.version.outputs.version }}"
          Compress-Archive -Path "$module\bin\Release\$module.dll" -DestinationPath "$module-$moduleVersion.zip"
        }
    
    - name: Package Report DLLs
      run: |
        $reportModules = @(
          "DO.VIVICARE.Report.AllegatoADI",
          "DO.VIVICARE.Report.Dietetica",
          "DO.VIVICARE.Report.Valorizzazione"
        )
        
        foreach ($module in $reportModules) {
          $moduleVersion = "${{ steps.version.outputs.version }}"
          Compress-Archive -Path "$module\bin\Release\$module.dll" -DestinationPath "$module-$moduleVersion.zip"
        }
    
    - name: Generate checksums
      run: |
        Get-Item *.zip | ForEach-Object {
          (Get-FileHash $_.FullName -Algorithm SHA256).Hash + " " + $_.Name | Out-File -Append -Encoding ASCII CHECKSUM.sha256
        }
    
    - name: Create Release
      if: startsWith(github.ref, 'refs/tags/')
      uses: softprops/action-gh-release@v1
      with:
        files: |
          *.zip
          *.msi
          CHECKSUM.sha256
        body_path: RELEASE_NOTES.md
        draft: false
        prerelease: false
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
    
    - name: Notify Success
      run: |
        echo "Build completed successfully: v${{ steps.version.outputs.version }}"
```

### Phase 2: Update Manager per UI

**File: `DO.VIVICARE.UI/UpdateManager.cs`**

```csharp
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

public class GitHubUpdateManager
{
    private const string GITHUB_API_URL = "https://api.github.com/repos/artcava/DO.VIVICARE.Reporting";
    private const string RELEASES_URL = GITHUB_API_URL + "/releases";
    private const string LATEST_RELEASE_URL = GITHUB_API_URL + "/releases/latest";
    private static readonly HttpClient _httpClient = new HttpClient();

    static GitHubUpdateManager()
    {
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "DO.VIVICARE.UI");
    }

    public class ReleaseInfo
    {
        public string TagName { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public string Body { get; set; } // Release notes
        public DateTime PublishedAt { get; set; }
        public bool IsPrerelease { get; set; }
        public string DownloadUrl { get; set; }
    }

    /// <summary>
    /// Controlla se esiste una nuova versione su GitHub
    /// </summary>
    public static async Task<ReleaseInfo> CheckForUpdatesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync(LATEST_RELEASE_URL);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var release = ParseGitHubRelease(json);

            return release;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error checking updates: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Confronta versioni semantiche
    /// </summary>
    public static bool IsUpdateAvailable(string currentVersion, string newVersion)
    {
        if (!Version.TryParse(currentVersion, out var current) ||
            !Version.TryParse(newVersion, out var latest))
            return false;

        return latest > current;
    }

    /// <summary>
    /// Download aggiornamento
    /// </summary>
    public static async Task<bool> DownloadUpdateAsync(
        string downloadUrl,
        string destinationPath,
        IProgress<DownloadProgressChangedEventArgs> progress = null)
    {
        try
        {
            using (var response = await _httpClient.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? 0L;
                var canReportProgress = totalBytes != -1 && progress != null;

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, useAsync: true))
                {
                    var totalRead = 0L;
                    var buffer = new byte[8192];
                    int read;

                    while ((read = await contentStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, read);
                        totalRead += read;

                        if (canReportProgress)
                        {
                            var args = new DownloadProgressChangedEventArgs(
                                bytesReceived: totalRead,
                                totalBytesToReceive: totalBytes,
                                userToken: null
                            );
                            progress.Report(args);
                        }
                    }
                }

                return true;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error downloading update: {ex.Message}");
            return false;
        }
    }

    private static ReleaseInfo ParseGitHubRelease(string json)
    {
        // Parsing JSON (alternare a Newtonsoft.Json se preferibile)
        // Questo è pseudo-codice, usa JSON library appropriata
        var release = new ReleaseInfo
        {
            // Parse dalla risposta GitHub API
            // ...
        };
        return release;
    }
}
```

**Integrazione in MDIParent.cs:**

```csharp
public partial class MDIParent : Form
{
    private async void CheckForUpdatesOnStartup()
    {
        try
        {
            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var latestRelease = await GitHubUpdateManager.CheckForUpdatesAsync();

            if (latestRelease != null &&
                GitHubUpdateManager.IsUpdateAvailable(currentVersion, latestRelease.Version))
            {
                var result = MessageBox.Show(
                    $"È disponibile un nuovo aggiornamento:\n\n" +
                    $"Versione: {latestRelease.Version}\n" +
                    $"Data: {latestRelease.PublishedAt:dd/MM/yyyy}\n\n" +
                    $"Note:\n{latestRelease.Body}\n\n" +
                    $"Scaricare e installare agora?",
                    "Aggiornamento Disponibile",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Information
                );

                if (result == DialogResult.Yes)
                {
                    await DownloadAndInstallUpdateAsync(latestRelease);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in update check: {ex.Message}");
        }
    }

    private async Task DownloadAndInstallUpdateAsync(GitHubUpdateManager.ReleaseInfo release)
    {
        try
        {
            var tempPath = Path.Combine(Path.GetTempPath(), "DO.VIVICARE.Update.zip");
            var progress = new Progress<DownloadProgressChangedEventArgs>(percent =>
            {
                lblStatus.Text = $"Downloading update... {percent.ProgressPercentage}%";
            });

            var success = await GitHubUpdateManager.DownloadUpdateAsync(
                release.DownloadUrl,
                tempPath,
                progress
            );

            if (success)
            {
                // Estrai e installa
                InstallUpdate(tempPath);
                MessageBox.Show("Aggiornamento installato! L'applicazione verrà riavviata.", "Successo");
                Application.Restart();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Errore durante l'aggiornamento: {ex.Message}", "Errore");
        }
    }
}
```

### Phase 3: Enhanced Plugin Manager per Librerie

**File: `DO.VIVICARE.UI/frmSettings.cs` (Versione Aggiornata)**

```csharp
public partial class frmSettings : Form
{
    private GitHubLibraryManager _libraryManager;

    public frmSettings()
    {
        InitializeComponent();
        _libraryManager = new GitHubLibraryManager();
        SetDataGrid();
    }

    private async void frmSettings_Shown(object sender, EventArgs e)
    {
        await LoadLibrariesFromGitHubAsync();
    }

    private async Task LoadLibrariesFromGitHubAsync()
    {
        try
        {
            lblStatus.Text = "Caricamento elenco librerie...";
            
            var manifest = await _libraryManager.GetManifestAsync();
            
            if (manifest != null)
            {
                PopulateDocumentGrid(manifest.Documents);
                PopulateReportGrid(manifest.Reports);
            }

            lblStatus.Text = "Pronto";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Errore: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void PopulateDocumentGrid(List<LibraryInfo> documents)
    {
        dgvElencoDocuments.DataSource = documents.Select(d => new
        {
            Nome = d.Name,
            VersioneAttuale = GetInstalledVersion(d.Name),
            VersioneDisponibile = d.Version,
            EsceAggiornamento = d.Version != GetInstalledVersion(d.Name),
            File = d.FileName
        }).ToList();
    }

    private async void dgvElencoDocuments_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex == 0) // Download button
        {
            var row = dgvElencoDocuments.Rows[e.RowIndex];
            var fileName = row.Cells["File"].Value.ToString();
            
            try
            {
                lblStatus.Text = $"Scaricamento {fileName}...";
                var destinationPath = Path.Combine(Manager.DocumentLibraries, fileName);
                
                var progress = new Progress<DownloadProgressChangedEventArgs>(p =>
                {
                    pbDownload.Value = p.ProgressPercentage;
                });

                await _libraryManager.DownloadLibraryAsync(fileName, destinationPath, progress);
                
                lblStatus.Text = "Download completato!";
                MessageBox.Show("Libreria scaricata con successo!", "Successo");
                
                // Ricarica l'elenco
                await LoadLibrariesFromGitHubAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Errore download: {ex.Message}", "Errore");
            }
        }
    }

    private string GetInstalledVersion(string libraryName)
    {
        try
        {
            var assemblyPath = Path.Combine(Manager.DocumentLibraries, $"{libraryName}.dll");
            if (File.Exists(assemblyPath))
            {
                var version = AssemblyName.GetAssemblyName(assemblyPath).Version;
                return version?.ToString() ?? "Sconosciuta";
            }
        }
        catch { }
        return "Non installata";
    }
}

public class GitHubLibraryManager
{
    private const string MANIFEST_URL = 
        "https://raw.githubusercontent.com/artcava/DO.VIVICARE.Reporting/master/manifest.json";
    private const string RELEASES_URL = 
        "https://api.github.com/repos/artcava/DO.VIVICARE.Reporting/releases";

    public async Task<LibraryManifest> GetManifestAsync()
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "DO.VIVICARE.UI");
            var response = await client.GetAsync(MANIFEST_URL);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LibraryManifest>(json);
        }
    }

    public async Task<bool> DownloadLibraryAsync(
        string fileName,
        string destinationPath,
        IProgress<DownloadProgressChangedEventArgs> progress)
    {
        using (var client = new HttpClient())
        {
            var downloadUrl = $"https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/latest/{fileName}";
            
            using (var response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? 0L;
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var file = new FileStream(destinationPath, FileMode.Create))
                {
                    var buffer = new byte[8192];
                    var totalRead = 0L;
                    int bytesRead;

                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        await file.WriteAsync(buffer, 0, bytesRead);
                        totalRead += bytesRead;
                        
                        var args = new DownloadProgressChangedEventArgs(
                            totalRead, totalBytes, null);
                        progress?.Report(args);
                    }
                }
            }
        }
        return true;
    }
}

public class LibraryManifest
{
    public string Version { get; set; }
    public List<LibraryInfo> Documents { get; set; }
    public List<LibraryInfo> Reports { get; set; }
}

public class LibraryInfo
{
    public string Name { get; set; }
    public string Version { get; set; }
    public string FileName { get; set; }
    public string Checksum { get; set; }
    public long Size { get; set; }
}
```

---

## Processo per Sviluppatori

### Come Rilasciare una Nuova Versione

**Step 1: Preparare il Release**

```bash
# 1. Verificare che tutto sia committato su master
git status

# 2. Aggiornare versione in AssemblyInfo.cs
# Esempio: Modificare
# [assembly: AssemblyVersion("1.2.0.0")]

# 3. Aggiornare RELEASE_NOTES.md
# Descrivere cambiamenti, bug fix, features nuove

# 4. Commit finale
git add .
git commit -m "Release v1.2.0"
```

**Step 2: Creare il Tag Git**

```bash
# Creare tag semantico
git tag -a v1.2.0 -m "Release version 1.2.0"

# Push tag al repository (trigger GitHub Actions)
git push origin v1.2.0

# Oppure push tutto
git push origin master --tags
```

**Step 3: GitHub Actions Automatizza il Resto**

```
GitHub Actions rileva il tag v1.2.0
  ↓
Build soluzione
  ↓
Crea package .zip
  ↓
Calcola checksum SHA256
  ↓
Crea GitHub Release
  ↓
Upload asset (UI, DLLs, Notes)
  ↓
✅ Release pubblicata e disponibile
```

### Monitoring Build Status

Vai su: https://github.com/artcava/DO.VIVICARE.Reporting/actions

```
Build History
├─ ✅ v1.2.0 - master - 2026-01-15 14:32
├─ ✅ v1.1.5 - master - 2026-01-10 10:15
├─ ✅ v1.1.4 - master - 2026-01-05 16:45
└─ ❌ v1.1.3 - develop - 2025-12-28 09:20 (Build failed)
```

---

## Processo per Utenti

### Scenario 1: Auto-Update (Raccomandato)

**Primo Setup (Richiede IT Auth):**
1. Scarica `DO.VIVICARE-Setup-v1.2.0.msi` da GitHub Releases
2. Esegui installer (IT approva installazione)
3. App si installa con auto-update abilitato

**Aggiornamenti Successivi (NO IT Auth):**
1. App si avvia
2. Controlla GitHub API se nuova versione
3. Se disponibile, mostra notifica
4. Click "Update Now"
5. Scarica e installa automaticamente
6. Riavvia app

### Scenario 2: Manual Download

1. Vai a https://github.com/artcava/DO.VIVICARE.Reporting/releases
2. Scarica `DO.VIVICARE-Setup-v1.2.0.msi`
3. Esegui installer
4. Autorizza aggiornamento automatico nel dialog di setup

### Scenario 3: Aggiorna Librerie (Documento e Report)

1. Apri DO.VIVICARE.UI
2. Menu: Tools → Settings
3. Tab: "Librerie e Moduli"
4. Click: "Controlla Aggiornamenti"
5. App scarica manifest da GitHub
6. Mostra quali librerie hanno aggiornamenti
7. Click download per ciascuna libreria desiderata
8. Auto-scarica e installa
9. Nessun riavvio richiesto (librerie hotload)

---

## Roadmap di Implementazione

### Phase 1 (Week 1-2): Setup Infrastruttura
- [ ] Creato .github/workflows/build-and-release.yml
- [ ] GitHub Actions testato
- [ ] Versioning strategy documentato
- [ ] Release manifesto creato

**Effort:** ~8 ore

### Phase 2 (Week 3-4): Update Manager UI
- [ ] UpdateManager.cs implementato
- [ ] MDIParent.cs integrato
- [ ] Test auto-update localmente
- [ ] Documentazione sviluppatore

**Effort:** ~12 ore

### Phase 3 (Week 5): Enhanced Plugin Manager
- [ ] GitHubLibraryManager.cs implementato
- [ ] frmSettings.cs refactor
- [ ] Manifest JSON schema definito
- [ ] Test download e install

**Effort:** ~10 ore

### Phase 4 (Week 6): Testing e Deployment
- [ ] Test complete end-to-end
- [ ] Beta release (v1.2.0-beta.1)
- [ ] Feedback collection
- [ ] Bug fixing
- [ ] Production release (v1.2.0)

**Effort:** ~12 ore

**Total Effort:** ~42 ore (≈ 1 settimana di lavoro a tempo pieno)

---

## Fallback e Recovery

### Caso: Build GitHub Actions Fallisce

**Soluzione:**
1. Vai su https://github.com/artcava/DO.VIVICARE.Reporting/actions
2. Clicca sulla build fallita
3. Analizza log di errore
4. Modifica il codice
5. Push su develop branch
6. GitHub Actions retry automaticamente

### Caso: Utente Scarica Versione Sbagliata

**Protezione:**
- Manifest.json ha campo `minFramework`
- App controlla compatibilità prima di install
- Se incompatibile, mostra messaggio e blocca

### Caso: Checksum Non Corrisponde

**Protezione:**
```csharp
public bool VerifyChecksum(string filePath, string expectedChecksum)
{
    var computedChecksum = ComputeSHA256(filePath);
    if (computedChecksum != expectedChecksum)
    {
        File.Delete(filePath);
        throw new Exception("File corrupted or tampered!");
    }
    return true;
}
```

### Caso: Rollback Versione Precedente

**Per Utenti:**
```
GitHub Releases → v1.1.5 → Download e installa
```

**Per Sviluppatori:**
```bash
# Se release è buggy:
git tag -d v1.2.0  # Elimina tag locale
git push origin :refs/tags/v1.2.0  # Elimina da GitHub

# Risolvi bug
git commit -am "Hotfix"

# Re-release
git tag v1.2.1
git push origin v1.2.1
```

---

## Comparativa: Before vs After

| Aspetto | Before | After |
|---------|--------|-------|
| **Distribuzione UI** | Azure ClickOnce (⚠️ non disponibile) | GitHub MSI + Auto-update ✅ |
| **Autorizzazione IT** | Ogni volta (noioso) | Solo prima volta ✅ |
| **Distribuzione Librerie** | Semi-manuale + Click button | Auto-update + GitHub Releases ✅ |
| **Versioning** | Nessuno (confuso) | Semantic versioning ✅ |
| **Release Notes** | Nessuno | GitHub Releases + MANIFEST ✅ |
| **CI/CD** | Manuale (0%) | GitHub Actions (100%) ✅ |
| **Checksum Verification** | Nessuna | SHA256 + Validation ✅ |
| **Rollback** | Difficile | Facile (GitHub Releases) ✅ |
| **Automazione Developer** | Manuale build + upload | Automatico con tag ✅ |
| **Monitoring** | Nessuno | GitHub Actions logs ✅ |

---

## Conclusione

Questa soluzione trasforma DO.VIVICARE da distribuzione "legacy" manuale a una **distribuzione moderna, automatizzata e supportata da infrastruttura cloud**.

**Vantaggi:**
- ✅ Zero downtime updates
- ✅ Versionamento chiaro e tracciabile
- ✅ Autorizzazione IT una sola volta
- ✅ Automazione completa per developers
- ✅ Rollback facile
- ✅ Audit trail completo
- ✅ Community-friendly (GitHub standard)

**Timeline:** 6 settimane per implementazione completa

**ROI:** Altissimo - riduce overhead operativo per sempre

---

**Documento aggiornato**: 11 Gennaio 2026
