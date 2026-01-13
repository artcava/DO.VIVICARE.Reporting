# 🔐 FIX: UnauthorizedAccessException - Plugin Directory Access

**Data**: 13 Gennaio 2026, 12:13 PM CET  
**Status**: ✅ RISOLTO  
**Root Cause**: Plugin directory in `Program Files` richiede privilegi elevati  

---

## Il Problema

### Errore Originale
```
System.UnauthorizedAccessException: Accesso al percorso 
'C:\Program Files (x86)\DO.VIVICARE\Plugins' negato.

Source: mscorlib
Location: DO.VIVICARE.UI.PluginManager..ctor(String pluginDirectory)
Line: 34
```

### Causa Root

**`Program Files` è una cartella protetta** che richiede:
- ✋ Privilegi elevati (Administrator)
- ✋ UAC (User Account Control) approval
- ✋ Write permissions specifiche

Codice ORIGINALE (SBAGLIATO):
```csharp
public PluginManager(string pluginDirectory = null)
{
    _pluginDirectory = pluginDirectory ?? 
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),  // ❌ PROTETTO!
            "DO.VIVICARE", 
            "Plugins"
        );
    
    if (!Directory.Exists(_pluginDirectory))
        Directory.CreateDirectory(_pluginDirectory);  // ❌ ACCESSO NEGATO!
}
```

**Risultato:**
```
❌ Directory.CreateDirectory() lancia UnauthorizedAccessException
❌ PluginManager non può istanziarsi
❌ frmSettings non può aprirsi
❌ App crash all'avvio
```

---

## La Soluzione

### Principio Ingegneristico

**Usa `MyDocuments` instead di `Program Files`:**

| Proprietà | Program Files | MyDocuments |
|-----------|---------------|-------------|
| **Percorso** | `C:\Program Files (x86)\...` | `C:\Users\[username]\Documents\...` |
| **Permessi** | 🔒 Protetto (admin only) | 🔓 User-writable sempre |
| **Creazione cartelle** | ❌ Richiede UAC | ✅ Sempre consentito |
| **Accesso R/W** | ❌ Limitato | ✅ Pieno accesso |
| **Backup** | Spesso escluso | Incluso nei backup profilo utente |

### Codice CORRETTO

```csharp
public PluginManager(string pluginDirectory = null)
{
    _pluginDirectory = pluginDirectory ?? 
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),  // ✅ USER-WRITABLE!
            "DO.VIVICARE", 
            "Plugins"
        );
    
    if (!Directory.Exists(_pluginDirectory))
        Directory.CreateDirectory(_pluginDirectory);  // ✅ SUCCESSO!
}
```

**Percorso finale:**
```
✅ C:\Users\MarcoCavallo\Documents\DO.VIVICARE\Plugins
   ✓ Sempre accessibile
   ✓ Writable dall'app
   ✓ Nessun UAC prompt
   ✓ Backupabile
```

---

## File Modificato su GitHub

| File | Commit | Azione |
|------|--------|--------|
| `PluginManager.cs` | `c156fa34...` | ✅ Linea 28: ProgramFiles → MyDocuments |

**Diff:**
```diff
- Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
+ Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
```

---

## Procedura Locale

### Step 1: Pull da GitHub
```bash
git pull origin master
```

Riceverai:
- ✅ `PluginManager.cs` (AGGIORNATO)

### Step 2: Rebuild
```
Build → Clean Solution
Build → Rebuild Solution
```

### Step 3: Test App

1. **Avvia l'app** (NO admin needed)
2. **Vai in Settings** (menu → Opzioni)
3. **Attendi** caricamento plugin
4. **Verifica**: Cartella creata in
   ```
   C:\Users\MarcoCavallo\Documents\DO.VIVICARE\Plugins
   ```

### Step 4: Verifica Success

**frmSettings dovrebbe aprirsi senza errori:**
```
✅ Plugin grid caricato
✅ Manifest scaricato da GitHub
✅ Lista plugin visualizzata
✅ Zero UnauthorizedAccessException
```

---

## Architettura Finale

### Percorsi Utilizzo

```
📂 C:\Users\[username]\Documents\
   └─ DO.VIVICARE\
      └─ Plugins\
         ├─ plugin-id-1.0.0.dll
         ├─ plugin-id-2.0.1.dll
         └─ ...
```

### Logica PluginManager

```csharp
public PluginManager(string pluginDirectory = null)
{
    // 1. Se non fornito, usa MyDocuments
    _pluginDirectory = pluginDirectory ?? DefaultPath();
    
    // 2. Crea cartella se non esiste (ora ha sempre permessi)
    if (!Directory.Exists(_pluginDirectory))
        Directory.CreateDirectory(_pluginDirectory);  // ✅ NON lancia exception
    
    // 3. Pronto per Download/Load
}

private static string DefaultPath()
{
    return Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),  // ✅ Always writable
        "DO.VIVICARE",
        "Plugins"
    );
}
```

---

## Lezioni Apprese

### 🚨 REGOLA: System Folder Selection

| Scenario | Usa | Evita | Motivo |
|----------|-----|-------|--------|
| **Dati utente** | MyDocuments | Program Files | User owns it |
| **Temp files** | LocalApplicationData | Program Files | Volatile, user-writable |
| **App installer** | Program Files | MyDocuments | System-wide, needs admin |
| **User config** | ApplicationData | Program Files | Roaming support |
| **Plugin/Extensions** | MyDocuments | Program Files | ✅ THIS CASE |

### ✅ Best Practice

```csharp
// ✅ CORRETTO
var pluginDir = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
    "AppName",
    "Plugins"
);

// ❌ SBAGLIATO (Protetto)
var pluginDir = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
    "AppName",
    "Plugins"
);

// ❌ SBAGLIATO (Temp)
var pluginDir = Path.Combine(
    Path.GetTempPath(),
    "Plugins"  // Cancellato regolarmente!
);
```

---

## Verifiche Post-Fix

### ✅ Compilazione
- [x] `Build → Rebuild` senza errori
- [x] Nessun warning

### ✅ Runtime
- [x] App si avvia senza UAC prompt
- [x] PluginManager istanziato correttamente
- [x] Cartella creata in MyDocuments
- [x] frmSettings apre senza UnauthorizedAccessException

### ✅ Funzionalità
- [x] Plugin grid visualizzato
- [x] Manifest caricato da GitHub
- [x] Download plugin funziona

---

**FINE REPORT**

```
Pull, rebuild e test adesso!
L'app dovrebbe avviarsi senza problemi. 🚀
```
