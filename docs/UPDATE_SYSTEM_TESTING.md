# 🧪 TEST DETTAGLIATO: Sistema di Aggiornamento

**Documento:** Guida Step-by-Step per testare il sistema completo  
**Data:** 22 Gennaio 2026  
**Difficoltà:** Media  
**Tempo:** ~2-3 ore per full test  

---

## 🤖 OBIETTIVO DEL TEST

Verificare che l'intero ciclo di aggiornamento funziona end-to-end:

1. ✅ Developer crea tag v1.2.1
2. ✅ GitHub Actions compila e crea release
3. ✅ manifest.json è aggiornato correttamente
4. ✅ Utente apre app v1.2.0
5. ✅ App legge manifest.json da GitHub
6. ✅ Mostra dialog "Update disponibile"
7. ✅ User clicca "Sì"
8. ✅ App scarica ZIP
9. ✅ Verifica SHA256 checksum
10. ✅ Estrae i file
11. ✅ Riavvia con v1.2.1
12. ✅ Nessun browser aperto
13. ✅ Nessun download manuale

---

## 🔨 FASE 1: Preparazione dell'Ambiente (30 min)

### 1.1 Clone Repository
```powershell
cd "C:\Users\MarcoCavallo\source\repos\artcava\Reporting"
git clone https://github.com/artcava/DO.VIVICARE.Reporting.git
cd DO.VIVICARE.Reporting
git status
```

**Verifica:**
- ✅ Repository clonato
- ✅ Branch: master
- ✅ File manifest.json esiste
- ✅ .github/workflows/ci-cd.yml esiste

### 1.2 Compilare Localmente
```powershell
# Apri Visual Studio
# File > Open Project > DO.VIVICARE.Reporting.sln
# Build > Build Solution (Ctrl+Shift+B)

# O via PowerShell:
msbuild DO.VIVICARE.Reporting.sln /p:Configuration=Release /p:Platform="Any CPU"
```

**Verifica:**
- ✅ Build successful
- ✅ Nessun errore
- ✅ DO.VIVICARE.UI.exe creato in bin/Release
- ✅ Eseguibile ha versione 1.0.0

---

## 🚀 FASE 2: Testare Workflow Automatico (45 min)

### 2.1 Creare Tag di Test
```powershell
# Verifica versione corrente
Select-String "AssemblyVersion" "DO.VIVICARE.UI\Properties\AssemblyInfo.cs"
# Output: [assembly: AssemblyVersion("1.0.0.0")]

# Crea tag di test
git tag -a v1.2.1 -m "Test release v1.2.1"

# Verifica tag
git tag -l "v1.2.1" -n1
# Output: v1.2.1  Test release v1.2.1

# Push tag (triggerizza workflow)
git push origin v1.2.1
```

**Verifica:**
- ✅ Tag creato localmente
- ✅ Tag pushato su GitHub

### 2.2 Monitorare Workflow
```powershell
# Apri browser
start "https://github.com/artcava/DO.VIVICARE.Reporting/actions"
```

**Cosa aspettarsi:**
```
Workflow Run: "feat/implement-auto-update"
  ├─ build-and-test: ✅ (5-10 min)
  │  ├─ Checkout
  │  ├─ Setup MSBuild
  │  ├─ Restore NuGet
  │  ├─ Build solution
  │  ├─ Run Unit Tests
  │  ├─ Run Integration Tests
  │  └─ Upload artifacts
  ├─ release-app: ✅ (3-5 min)
  │  ├─ Extract version: v1.2.1
  │  ├─ Update AssemblyInfo.cs: ✅
  │  ├─ Rebuild solution: ✅
  │  ├─ Update manifest.json: ✅
  │  ├─ Create package: ✅
  │  ├─ Generate checksum: ✅
  │  └─ Create Release: ✅
  └─ release-plugin: SKIPPED (non è tag plugin)
```

### 2.3 Verificare Release su GitHub
```powershell
# Apri browser
start "https://github.com/artcava/DO.VIVICARE.Reporting/releases"
```

**Verifica:**
- ✅ Release v1.2.1 esiste
- ✅ Data: oggi
- ✅ File caricati:
  - DO.VIVICARE-Setup-1.2.1.zip
  - CHECKSUM.txt
  - manifest.json
- ✅ Note release presenti

### 2.4 Controllare manifest.json
```powershell
# Leggi il file da GitHub
$manifest = Invoke-RestMethod "https://raw.githubusercontent.com/artcava/DO.VIVICARE.Reporting/master/manifest.json"

$manifest | ConvertTo-Json

# Verifica:
# {
#   "version": "1.2.1",
#   "releaseDate": "2026-01-22",
#   "assets": [
#     {
#       "version": "1.2.1",
#       "file": "DO.VIVICARE-Setup-1.2.1.zip",
#       "checksum": "sha256:abc123..."
#     }
#   ]
# }
```

**Verifica:**
- ✅ version = 1.2.1
- ✅ file = DO.VIVICARE-Setup-1.2.1.zip
- ✅ checksum non vuoto

---

## 🔠 FASE 3: Testare Update Check Locale (30 min)

### 3.1 Eseguire Applicazione con v1.2.0
```powershell
# Abbassa versione temporaneamente per test
Edit DO.VIVICARE.UI\Properties\AssemblyInfo.cs
# Cambia [assembly: AssemblyVersion("1.0.0.0")] 
# In:     [assembly: AssemblyVersion("1.2.0.0")]

# Ricompila
msbuild DO.VIVICARE.Reporting.sln /p:Configuration=Release

# Esegui app
.\DO.VIVICARE.UI\bin\Release\DO.VIVICARE.UI.exe
```

### 3.2 Verificare Title Bar
```
Finestra: "Reporting [1.2.0]"
```

**Verifica:**
- ✅ Title bar mostra versione 1.2.0
- ✅ Nessun errore nella console debug

### 3.3 Attendere Dialog di Update
```
Attendi 2-3 secondi...
```

**Cosa aspettarsi:**
Dialog deve apparire:
```
È disponibile una nuova versione: 1.2.1

Versione corrente: 1.2.0
Data rilascio: 2026-01-22

Vuoi scaricare e installare l'aggiornamento ora?

Nota: L'applicazione verrà riavviata automaticamente.

[Sì]  [No]
```

**Verifica:**
- ✅ Dialog appare
- ✅ Versioni corrette (1.2.0 vs 1.2.1)
- ✅ Data corretta

---

## 💽 FASE 4: Testare Download e Install (45 min)

### 4.1 Cliccare "Sì" per Scaricare

**Cosa aspettarsi:**
```
Status bar: "Downloading update... 0%"
```

After some seconds:
```
Status bar: "Downloading update... 25%"
Status bar: "Downloading update... 50%"
Status bar: "Downloading update... 75%"
Status bar: "Downloading update... 100%"
```

**Verifica:**
- ✅ Download bar visibile
- ✅ Progress aumenta
- ✅ NO browser aperto
- ✅ NO download manager
- ✅ App rimane in foreground

### 4.2 Verifica Integrità (Checksum)
```
Status bar: "Verifying integrity..."
```

**Verifica:**
- ✅ SHA256 calcolato
- ✅ Checksum corrisponde

### 4.3 Estrazione File
```
Status bar: "Installing update..."
```

**Verifica:**
- ✅ ZIP estratto
- ✅ File rimpiazzati in Application.StartupPath

### 4.4 Riavvio App

**Dialog:**
```
✅ Update installed successfully!

The application will restart now to apply changes.

[OK]
```

**Verifica:**
- ✅ Dialog appare
- ✅ App si chiude
- ✅ App si riavvia

### 4.5 Verificare Nuova Versione

Dopo riavvio:
```
Finestra: "Reporting [1.2.1]"
```

**Verifica:**
- ✅ Title bar mostra 1.2.1
- ✅ Update completato con successo
- ✅ Nessun dialog di update

---

## 🔍 FASE 5: Verifiche Dettagliate (30 min)

### 5.1 Verificare File
```powershell
# Leggi version dal binario
[System.Diagnostics.FileVersionInfo]::GetVersionInfo(".\DO.VIVICARE.UI\bin\Release\DO.VIVICARE.UI.exe")

# Output atteso:
# FileVersion: 1.2.1.0
# ProductVersion: 1.2.1
```

**Verifica:**
- ✅ FileVersion = 1.2.1.0
- ✅ ProductVersion = 1.2.1

### 5.2 Controllare Temp Folder
```powershell
# Verifica che file temporaneo sia stato pulito
Get-ChildItem "$([System.IO.Path]::GetTempPath())" | grep "DO.VIVICARE-Update"

# Risultato atteso: (nessun file)
```

**Verifica:**
- ✅ Temp file eliminato
- ✅ Nessun file corrotto rimasto

### 5.3 Verificare Log Debug
```csharp
// In Visual Studio, Finestra > Output
// Filtra: Debug
```

Atteso:
```
Current: 1.2.0, Available: 1.2.1
Downloading update...
Verifying integrity...
Checksum verified
Installing update...
Application restarting...
```

**Verifica:**
- ✅ Log coerenti
- ✅ No errors

---

## ❌ FASE 6: Test di Errore (20 min)

### 6.1 Testare Download Fallito

**Setup:**
- Sconnettere rete
- Rilasciare app durante download

**Verifica:**
- ✅ Error dialog mostra:
  "❌ Error downloading... Please try again or visit GitHub"
- ✅ Temp file eliminato
- ✅ App rimane funzionante con versione old

### 6.2 Testare Checksum Errato

**Setup:**
- Modificare manifest.json con checksum sbagliato (test locale)
- Triggerare update

**Verifica:**
- ✅ Error dialog:
  "❌ Integrity check failed. File corrupted."
- ✅ ZIP file eliminato
- ✅ Update annullato

---

## 📋 CHECKLIST FINALE

### Workflow Tests
- [ ] Tag v1.2.1 triggerizza workflow
- [ ] build-and-test job completa
- [ ] release-app job completa
- [ ] AssemblyVersion aggiornato a 1.2.1
- [ ] manifest.json generato e caricato
- [ ] Checksum calcolato e incluso
- [ ] GitHub Release creata con file

### Application Tests
- [ ] App mostra versione corretta nel title bar
- [ ] PluginManager legge manifest.json
- [ ] VersionCompare funziona (1.2.0 < 1.2.1)
- [ ] Dialog update appare
- [ ] NO browser aperto
- [ ] Download mostra progress bar
- [ ] Checksum verificato
- [ ] ZIP estratto
- [ ] App riavvia
- [ ] Versione aggiornata a 1.2.1

### Error Handling
- [ ] Network error gestito
- [ ] Corrupted file eliminato
- [ ] Checksum mismatch rilevato
- [ ] User informato di errore
- [ ] App rimane funzionante

---

## 🔧 Troubleshooting

| Problema | Causa | Soluzione |
|----------|-------|----------|
| Dialog non appare | PluginManager.CheckAppUpdateAsync() fallisce | Verifica manifest.json su GitHub raw |
| Workflow non parte | Tag non nel formato `v*.*.*` | Usa `git tag -a v1.2.1 -m "msg"` |
| AssemblyVersion non cambia | Workflow non riesce | Controlla job release-app in Actions |
| Download fallisce | Network o URL sbagliato | Verifica Release ha ZIP file |
| Checksum mismatch | File corrotto durante download | Riprova download |
| App non riavvia | Application.Restart() fallisce | Check permissions nel folder install |

---

## 🎆 Success Criteria

Tutto passa se:

1. ✅ **Workflow:** Completa senza errori
2. ✅ **Versionamento:** AssemblyVersion sincronizzato dal tag
3. ✅ **Manifest:** JSON aggiornato automaticamente
4. ✅ **Download:** Automatico, con progress bar
5. ✅ **Verifica:** SHA256 calcolato e verificato
6. ✅ **Install:** Estrazione automatica
7. ✅ **Riavvio:** App riavvia con nuova versione
8. ✅ **Browser:** NESSUN browser aperto
9. ✅ **UX:** Tutto visibile nell'app, non in explorer
10. ✅ **Errori:** Gestiti elegantemente

---

**Documento di Testing Completo**  
**Status:** ✅ Ready for Execution  
**Data:** 22 Gennaio 2026
