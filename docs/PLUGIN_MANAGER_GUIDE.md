# PluginManager - Guida Completa

## Panoramica

Il **PluginManager** è un sistema di gestione plugin per DO.VIVICARE Reporting che permette:

- ✅ **Download automatico** di plugin da GitHub Releases
- ✅ **Verifica di integrità** tramite SHA256
- ✅ **Hot-reload** senza riavvio dell'applicazione
- ✅ **Gestione versioni** indipendenti per app e plugin
- ✅ **Risoluzione automatica** di dipendenze

---

## Architettura

### File Principali

```
DO.VIVICARE.UI/
├── PluginManager.cs        # Logica di gestione plugin
├── PluginModels.cs         # Modelli dati (PluginInfo, PluginManifest, etc.)
└── frmSettings.cs          # Form Settings con integrazione PluginManager

Repository root/
└── manifest.json           # Catalogo plugin disponibili
```

### Flusso di Dati

```
manifest.json (GitHub)
       ↓
   HttpClient
       ↓
PluginManager.GetManifestAsync()
       ↓
PluginManifest (list Documents + Reports)
       ↓
frmSettings.PopulateDocumentPlugins()
       ↓
DataGridView (UI)
```

---

## Componenti Principali

### 1. PluginManager.cs

**Responsabilità:**
- Gestire il download del manifest
- Scaricare plugin con progress tracking
- Verificare checksum SHA256
- Caricare assembly in memoria (hot-reload)
- Gestire dipendenze

**Metodi principali:**

```csharp
public async Task<PluginManifest> GetManifestAsync()
// Scarica il manifest.json da GitHub

public async Task<bool> DownloadPluginAsync(
    PluginInfo plugin,
    IProgress<DownloadProgress> progress = null,
    CancellationToken cancellationToken = default)
// Scarica e installa un plugin con progress tracking

public Assembly LoadPluginAssembly(string pluginId)
// Carica un DLL plugin in memoria (hot-reload)

public List<InstalledPlugin> GetInstalledPlugins()
// Restituisce lista di plugin già scaricati

public bool HasUpdate(PluginInfo installed, PluginInfo available)
// Controlla se esiste un aggiornamento disponibile

public List<PluginInfo> ResolveDependencies(
    PluginInfo plugin,
    PluginManifest manifest)
// Risolve tutte le dipendenze di un plugin
```

### 2. PluginModels.cs

Contiene i modelli dati:

```csharp
public class PluginManifest
{
    public AppInfo App { get; set; }
    public List<PluginInfo> Documents { get; set; }
    public List<PluginInfo> Reports { get; set; }
}

public class PluginInfo
{
    public string Id { get; set; }              // "document.adialtaintensita"
    public string Name { get; set; }            // "ADI Alta Intensita"
    public string Version { get; set; }         // "1.0.0"
    public string DownloadUrl { get; set; }    // URL da GitHub Releases
    public string Checksum { get; set; }       // SHA256 per verifica
    public long Size { get; set; }             // Dimensione in byte
    public List<string> Dependencies { get; set; }
}

public class InstalledPlugin
{
    public string Name { get; set; }
    public string Version { get; set; }
    public string FilePath { get; set; }
    public long FileSize { get; set; }
}

public class DownloadProgress
{
    public string PluginId { get; set; }
    public long BytesDownloaded { get; set; }
    public long TotalBytes { get; set; }
    public int PercentComplete { get; set; }
}
```

### 3. manifest.json

File JSON che cataloga tutti i plugin disponibili:

```json
{
  "app": {
    "name": "DO.VIVICARE Reporting",
    "version": "1.2.0",
    "lastUpdate": "2026-01-13T10:00:00Z"
  },
  "documents": [
    {
      "id": "document.adialtaintensita",
      "name": "ADI Alta Intensita",
      "version": "1.0.0",
      "downloadUrl": "https://github.com/.../releases/download/.../document-adialtaintensita-1.0.0.dll",
      "checksum": "sha256:TBD",
      "size": 1048576
    }
    // ... altri plugin
  ],
  "reports": [
    // ... plugin di tipo report
  ]
}
```

---

## Utilizzo

### Scenario 1: Caricamento Manifest al Startup

```csharp
private PluginManager _pluginManager;

public frmSettings()
{
    InitializeComponent();
    _pluginManager = new PluginManager();
}

private async void frmSettings_Shown(object sender, EventArgs e)
{
    await LoadPluginManifestAsync();
}

private async Task LoadPluginManifestAsync()
{
    try
    {
        lblStatus.Text = "Caricamento plugin disponibili...";
        
        var manifest = await _pluginManager.GetManifestAsync();
        
        if (manifest != null)
        {
            PopulateUI(manifest);
            lblStatus.Text = "Pronto";
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Errore: {ex.Message}");
    }
}
```

### Scenario 2: Verificare Aggiornamenti

```csharp
var manifest = await _pluginManager.GetManifestAsync();
var installed = _pluginManager.GetInstalledPlugins();

foreach (var pluginAvailable in manifest.Documents)
{
    // Cerca se installato
    var inst = installed.FirstOrDefault(
        i => i.Name.Contains(pluginAvailable.Id));
    
    if (inst != null && _pluginManager.HasUpdate(inst, pluginAvailable))
    {
        Console.WriteLine($"{pluginAvailable.Name} ha aggiornamento!");
        // Offri all'utente di aggiornare
    }
}
```

### Scenario 3: Scaricare con Progress

```csharp
var plugin = manifest.Documents.First();

var progress = new Progress<DownloadProgress>(p =>
{
    progressBar.Value = p.PercentComplete;
    lblStatus.Text = $"Download: {p.PercentComplete}% ({p.BytesDownloaded}/{p.TotalBytes})";
});

var success = await _pluginManager.DownloadPluginAsync(
    plugin,
    progress,
    CancellationToken.None
);

if (success)
{
    MessageBox.Show($"{plugin.Name} installato con successo!");
    // Nessun riavvio richiesto - hot-reload ready
}
```

### Scenario 4: Hot-Reload di un Plugin

```csharp
// L'utente seleziona un documento che richiede un plugin
var assembly = _pluginManager.LoadPluginAssembly("document.adialtaintensita");

if (assembly != null)
{
    // Usa reflection per ottenere il tipo
    var processorType = assembly.GetType(
        "DO.VIVICARE.Document.ADIAltaIntensita.DocumentProcessor");
    
    if (processorType != null)
    {
        // Istanzia il plugin
        var instance = (IDocumentProcessor)Activator.CreateInstance(processorType);
        
        // Usa il plugin
        var result = instance.Process(documentData);
    }
}
```

### Scenario 5: Risoluzione Dipendenze

```csharp
var manifest = await _pluginManager.GetManifestAsync();
var plugin = manifest.Documents.FirstOrDefault(p => p.Id == "report.valorizzazione");

// Ottieni tutte le dipendenze
var dependencies = _pluginManager.ResolveDependencies(plugin, manifest);

// Scarica tutte (se mancanti)
foreach (var dep in dependencies)
{
    var installed = _pluginManager.GetInstalledPlugins();
    if (!installed.Any(i => i.Name.Contains(dep.Id)))
    {
        await _pluginManager.DownloadPluginAsync(dep);
    }
}
```

---

## Configurazione

### Path di Default dei Plugin

```
C:\Program Files\DO.VIVICARE\Plugins\
```

Per cambiarlo:

```csharp
var customPath = "D:\\MyPlugins";
var pluginManager = new PluginManager(customPath);
```

### URL del Manifest

Il manifest viene sempre scaricato da:

```
https://raw.githubusercontent.com/artcava/DO.VIVICARE.Reporting/master/manifest.json
```

Per cambiarla, modifica il costante in `PluginManager.cs`:

```csharp
private const string MANIFEST_URL = 
    "https://tuo-server.com/manifest.json";
```

---

## Flusso Integrazione in frmSettings

### Al Caricamento Form

1. ✅ Crea istanza `PluginManager`
2. ✅ Scarica manifest da GitHub
3. ✅ Popola griglia con plugin disponibili
4. ✅ Confronta con plugin già installati
5. ✅ Mostra stato ("Non installato", "Aggiornato", "Aggiorna disponibile")

### Quando Utente Clicca Download

1. ✅ Recupera `PluginInfo` dal Tag della riga
2. ✅ Mostra progress bar
3. ✅ Chiama `DownloadPluginAsync()`
4. ✅ Aggiorna UI con percentuale
5. ✅ Verifica SHA256
6. ✅ Notifica successo/errore
7. ✅ Ricarica lista plugin

---

## Troubleshooting

### Problema: "Manifest not found"

**Causa**: File `manifest.json` non presente nel branch master

**Soluzione**:
```bash
git add manifest.json
git commit -m "Add manifest"
git push origin master
```

### Problema: "Checksum verification failed"

**Causa**: File DLL scaricato non corrisponde al checksum atteso

**Soluzione**:
- Riprova il download
- Verifica che il manifest.json abbia checksum corretto

### Problema: "Plugin not found in directory"

**Causa**: Plugin non è stato scaricato prima di tentare LoadPluginAssembly

**Soluzione**:
```csharp
// Controlla se installato
var installed = _pluginManager.GetInstalledPlugins();
if (!installed.Any(i => i.Name.Contains(pluginId)))
{
    // Scarica prima
    await _pluginManager.DownloadPluginAsync(plugin);
}

// Ora carica
var assembly = _pluginManager.LoadPluginAssembly(pluginId);
```

---

## Performance

### Tempi Tipici

| Operazione | Tempo |
|-----------|-------|
| Caricamento Manifest | 1-2 sec |
| Download plugin medio (1 MB) | 2-5 sec (dipende dalla rete) |
| Verifica SHA256 | <1 sec |
| Hot-reload Assembly | <100 ms |

### Ottimizzazioni

1. **Cache Manifest**: Salva in locale e ricarica solo se cambiato
2. **Download Asincrono**: Usa sempre `async/await`
3. **Cancellation Support**: Implementa `CancellationToken` per interruzioni

---

## Roadmap Futura

- [ ] Implementare cache manifest locale
- [ ] Supporto per plugin firmati digitalmente
- [ ] Rollback automatico a versione precedente se errore
- [ ] Interfaccia dedicata frmPluginManager (non solo frmSettings)
- [ ] Supporto per pre-download in background
- [ ] Analytics su plugin più usati

---

**Ultimo aggiornamento**: 13 Gennaio 2026
**Versione**: 1.0.0
**Stato**: ✅ Production Ready
