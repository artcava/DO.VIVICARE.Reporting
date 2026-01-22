# 🚀 ROADMAP: Implementazione Auto-Updater System

**Data Creazione:** 22 Gennaio 2026  
**Status:** 🚀 READY TO START  
**Tempo Totale Stimato:** 1 settimana full-time (40 ore)  

---

## 📋 Fase 0: Setup Locale (GIÀ COMPLETATO ✅)

### ✅ File GitHub Creati

```
Repository Root (NEW):
├─ manifest.json (NEW) - Source of truth per versioni
├─ DEPLOYMENT.md (UPDATED) - Strategia completa
│
DO.VIVICARE.UI/:
├─ MDIParent.cs (UPDATED) - Auto-check updates on startup
├─ PluginManager.cs (NEW) - Gestisce manifest.json
│
docs/ (NEW - Completa Documentazione):
├─ UPDATE_SYSTEM_ANALYSIS.md - Perché il vecchio sistema NON funzionava
├─ UPDATE_SYSTEM_TESTING.md - Step-by-step testing guide
└─ IMPLEMENTATION_ROADMAP.md (questo file)
```

### ✅ File Già su GitHub

- ✅ `manifest.json` - Creato
- ✅ `DO.VIVICARE.UI/MDIParent.cs` - Aggiornato con auto-update logic
- ✅ `DO.VIVICARE.UI/PluginManager.cs` - Creato con version checking
- ✅ `DEPLOYMENT.md` - Aggiornato con Auto-Updater section
- ✅ Documentazione completa in `docs/`

---

## 🎯 Prossimi Passi Richiesti

### STEP 1: Aggiorna Git Locale (15 min)

```powershell
cd "C:\Users\MarcoCavallo\source\repos\artcava\Reporting\DO.VIVICARE.Reporting"

# Sincronizza con master remoto
git fetch origin
git checkout master
git pull origin master

# Verifica i file creati
git status

# Output atteso:
# On branch master
# Your branch is up to date with 'origin/master'.
# nothing to commit, working tree clean
```

### STEP 2: Verifica File Nuovi Localmente (5 min)

```powershell
# Controlla manifest.json
Get-Content manifest.json | ConvertFrom-Json | Format-Table

# Output atteso:
# version     : 1.0.0
# releaseDate : 2026-01-22

# Controlla MDIParent.cs modificato
Select-String "CheckForApplicationUpdatesAsync" "DO.VIVICARE.UI/MDIParent.cs"

# Output atteso:
# (mostra il metodo nel file)

# Controlla PluginManager.cs creato
Test-Path "DO.VIVICARE.UI/PluginManager.cs"

# Output atteso: True
```

### STEP 3: Compila Localmente per Verificare (10 min)

```powershell
# Apri Visual Studio
# File > Open Project > DO.VIVICARE.Reporting.sln

# Build > Build Solution (Ctrl+Shift+B)
# Aspetta che finisca...

# Output atteso: Build succeeded (0 errors, 0 warnings)
```

**Checkpoint:** ✅ Build successfull, nessun errore

---

## 📝 STEP 4: Compila ed Esegui Test Locale (30 min)

### 4.1 Verifica Versione nel Binario

```powershell
# Dopo build, controlla versione
[System.Diagnostics.FileVersionInfo]::GetVersionInfo(
  "DO.VIVICARE.UI/bin/Release/DO.VIVICARE.UI.exe"
).FileVersion

# Output atteso: 1.0.0.0 (o versione corrente)
```

### 4.2 Esegui l'App Localmente

```powershell
.\DO.VIVICARE.UI\bin\Release\DO.VIVICARE.UI.exe
```

**Cosa Aspettarsi:**
1. ✅ App si avvia
2. ✅ Title bar mostra: "Reporting [1.0.0]"
3. ✅ Nessun error dialog
4. ✅ Dopo 2-3 secondi: Potrebbe mostrare "Update available" se c'è versione più recente

### 4.3 Controlla Debug Output

```
Visual Studio > Debug > Windows > Output
Filtra: Debug

Dovrebbe vedere:
"Current: 1.0.0, Available: 1.0.0"
oppure
"Error checking update: ..." (se network non disponibile)
```

**Checkpoint:** ✅ App avvia senza errori

---

## 🏷️ STEP 5: Crea Tag per Test Release (10 min)

### 5.1 Incrementa Versione (LOCAL)

```powershell
# ATTENZIONE: Solo locale! Non pushare ancora!

# Modifica file locale (NON pushare!)
# DO.VIVICARE.UI/Properties/AssemblyInfo.cs

# Cambia da:
# [assembly: AssemblyVersion("1.0.0.0")]

# A:
# [assembly: AssemblyVersion("1.0.1.0")]

# Salva (Ctrl+S)
```

### 5.2 Crea Tag Locale (TEST)

```powershell
# Commit locale (non pushare)
git add DO.VIVICARE.UI/Properties/AssemblyInfo.cs
git commit -m "TEST: Update to v1.0.1" --allow-empty

# Crea tag (non pushare)
git tag -a v1.0.1-test -m "Test release v1.0.1"

# Verifica
git tag -l "v1.0.1*" -n1

# Output: v1.0.1-test  Test release v1.0.1
```

**Checkpoint:** ✅ Tag creato localmente

---

## ⚠️ STEP 6: Pronto per Produzione? ✅

Prima di procedere a v1.0.1 vero, verifichiamo che tutto sia pronto:

### Checklist Pre-Release

- [ ] ✅ Repository aggiornato (git pull)
- [ ] ✅ MDIParent.cs ha CheckForApplicationUpdatesAsync()
- [ ] ✅ PluginManager.cs esiste con CheckAppUpdateAsync()
- [ ] ✅ manifest.json esiste con version 1.0.0
- [ ] ✅ App compila senza errori
- [ ] ✅ App avvia e mostra versione corretta
- [ ] ✅ Debug output coerente (no exceptions)
- [ ] ✅ Documentazione completa in docs/
- [ ] ✅ DEPLOYMENT.md aggiornato

**Risultato:** Tutti ✅? Procedi a STEP 7

---

## 🚀 STEP 7: First Real Release (v1.0.1)

### 7.1 Update Versione Permanente

```powershell
# First, reset locale changes (se non già committate)
git reset --hard HEAD

# Now update properly
Edit "DO.VIVICARE.UI/Properties/AssemblyInfo.cs"

# Update these three lines:
# [assembly: AssemblyVersion("1.0.1.0")]
# [assembly: AssemblyFileVersion("1.0.1.0")]
# [assembly: AssemblyInformationalVersion("1.0.1")]

# Salva
```

### 7.2 Commit e Tag

```powershell
# Commit la versione update
git add DO.VIVICARE.UI/Properties/AssemblyInfo.cs
git commit -m "Release: Update to v1.0.1"

# Crea tag annotato
git tag -a v1.0.1 -m "Release v1.0.1 - Auto-Updater System Implementation"

# Verifica
git tag -l "v1.0.1" -n1

# Output: v1.0.1  Release v1.0.1 - Auto-Updater System Implementation
```

**Checkpoint:** ✅ Tag v1.0.1 creato localmente

### 7.3 Push su GitHub (TRIGGERS WORKFLOW!)

```powershell
# Push commit
git push origin master

# Output atteso:
# To github.com:artcava/DO.VIVICARE.Reporting.git
#    abc1234..def5678  master -> master

# Push tag (QUESTO TRIGGERIZZA IL WORKFLOW!)
git push origin v1.0.1

# Output atteso:
# To github.com:artcava/DO.VIVICARE.Reporting.git
# * [new tag]         v1.0.1 -> v1.0.1
```

**Checkpoint:** ✅ Commit e tag pushati su GitHub

---

## 🔄 STEP 8: Monitora GitHub Actions Workflow (5-10 min)

### 8.1 Apri GitHub Actions

```powershell
# Apri browser
start "https://github.com/artcava/DO.VIVICARE.Reporting/actions"
```

### 8.2 Aspetta il Workflow

Dovrai vedere:

```
Workflow Run: "Release: Update to v1.0.1"
Status: In Progress (running)
  └─ build-and-test: ⏳ Running
     ├─ Checkout
     ├─ Setup MSBuild
     ├─ Restore NuGet
     ├─ Build solution
     ├─ Run Unit Tests
     ├─ Run Integration Tests
     └─ Upload artifacts
  
  Aspetta... (circa 5-7 minuti)
  
  Dopo qualche minuto:
  └─ release-app: ⏳ Running
     ├─ Extract version: 1.0.1 ✅
     ├─ Update AssemblyInfo.cs ✅
     ├─ Rebuild solution ✅
     ├─ Update manifest.json ✅
     ├─ Create package ✅
     ├─ Generate checksum ✅
     └─ Create Release ✅
```

### 8.3 Verifica Risultato

After 10 minutes:

```
Status: ✅ All Jobs Passed
```

**Checkpoint:** ✅ Workflow completato senza errori

---

## 📦 STEP 9: Verifica GitHub Release (5 min)

### 9.1 Apri Releases

```powershell
start "https://github.com/artcava/DO.VIVICARE.Reporting/releases"
```

### 9.2 Verifica File Caricati

Dovrebbe esserci: `v1.0.1`

Con file:
- ✅ `DO.VIVICARE-Setup-1.0.1.zip`
- ✅ `CHECKSUM.sha256`
- ✅ `manifest.json`

### 9.3 Verifica manifest.json

```powershell
# Leggi da GitHub
$url = "https://raw.githubusercontent.com/artcava/DO.VIVICARE.Reporting/master/manifest.json"
(Invoke-WebRequest $url).Content | ConvertFrom-Json | Format-List

# Output atteso:
# version     : 1.0.1
# releaseDate : 2026-01-22
# assets      : {system.object[]}
#   [0] type: ui
#       version: 1.0.1
#       checksum: sha256:abc123...
```

**Checkpoint:** ✅ Release e manifest su GitHub

---

## 🧪 STEP 10: Test Auto-Update Flow (20 min)

### 10.1 Simula User con v1.0.0

```powershell
# Modifica temporaneamente AssemblyInfo.cs
# [assembly: AssemblyVersion("1.0.0.0")]

# Ricompila
msbuild DO.VIVICARE.Reporting.sln /p:Configuration=Release

# Esegui app
.\DO.VIVICARE.UI\bin\Release\DO.VIVICARE.UI.exe
```

### 10.2 Verifica Dialog Update

App inizia → Dopo 2-3 secondi dovrebbe mostrare:

```
Dialog: "A new update is available!

Current Version: 1.0.0
New Version: 1.0.1
Released: 2026-01-22

Download and install now?

[Yes] [No]
```

**Verifica:**
- ✅ Versioni corrette
- ✅ Dialog appare

### 10.3 Clicca "Yes" per Testare Download

**Cosa aspettarsi:**

```
1. Status: "Downloading update..."
   Progress bar visibile
   (Download ~5-10 MB, 10-20 secondi)

2. Status: "Verifying integrity..."
   (SHA256 check, 2-3 secondi)

3. Status: "Installing update..."
   (Unzip, 2-3 secondi)

4. Dialog: "Update installed successfully!
   The application will restart."

5. App riavvia

6. Title bar: "Reporting [1.0.1]" ✅
```

**Verifica:**
- ✅ Download con progress bar
- ✅ SHA256 verificato
- ✅ Estrazione completata
- ✅ App riavvia
- ✅ Versione aggiornata a 1.0.1
- ✅ NO browser aperto

**Checkpoint:** ✅ Auto-update flow completo!

---

## ✅ STEP 11: Documentazione Verifica (10 min)

Verifica che tutta la documentazione sia presente su GitHub:

### Checklist File Documentazione

- [ ] ✅ `DEPLOYMENT.md` - Ha sezione "Auto-Updater System (NEW!)"
- [ ] ✅ `docs/UPDATE_SYSTEM_ANALYSIS.md` - Spiega perché vecchio sistema falliva
- [ ] ✅ `docs/UPDATE_SYSTEM_TESTING.md` - Step-by-step testing guide
- [ ] ✅ `docs/IMPLEMENTATION_ROADMAP.md` - Questo file

```powershell
# Verifica da GitHub
start "https://github.com/artcava/DO.VIVICARE.Reporting/tree/master/docs"

# Dovrebbe mostrare 3 file nella cartella docs/
```

**Checkpoint:** ✅ Documentazione completa

---

## 🎉 STEP 12: Finale - Pronto per Produzione!

### Checklist Finale

- [x] ✅ File creati su GitHub
- [x] ✅ MDIParent.cs aggiornato con auto-check
- [x] ✅ PluginManager.cs creato
- [x] ✅ manifest.json creato
- [x] ✅ DEPLOYMENT.md aggiornato
- [x] ✅ Documentazione completa
- [ ] ⏳ Locale compilation test
- [ ] ⏳ Tag v1.0.1 creato e pushato
- [ ] ⏳ Workflow completato
- [ ] ⏳ Release su GitHub
- [ ] ⏳ Auto-update test completato

---

## 📊 Timeline Riassuntivo

```
OGGI (22 Gennaio 2026):
- STEP 1-3: Setup locale (30 min)
- STEP 4: Build test (30 min)
- STEP 5: Tag test (10 min)
└─ TOTALE OGGI: ~1 ora

PROSSIMA SESSIONE (quando pronto per v1.0.1):
- STEP 6: Checklist pre-release (5 min)
- STEP 7: Commit & Tag (10 min)
- STEP 8: Monitor workflow (10 min, tempo di attesa)
- STEP 9: Verifica release (5 min)
- STEP 10: Test auto-update (20 min)
- STEP 11: Documenti check (10 min)
└─ TOTALE PROSSIMA SESSIONE: ~70 min

TOTALE IMPLEMENTAZIONE: ~2 ore (1 per setup, 1 per release+test)
```

---

## 🆘 Troubleshooting

### Problema: Build fallisce dopo update MDIParent.cs

**Soluzione:**
```powershell
# Verifica errori syntax
# Controlla: using statements, parenthesi, etc.

# Ricompila
msbuild DO.VIVICARE.Reporting.sln /p:Configuration=Release

# Se ancora errore:
# Clean > Build
```

### Problema: Workflow non parte dopo tag push

**Soluzione:**
```powershell
# Verifica tag format
git tag -l "v1.0.1"

# Output deve essere: v1.0.1 (esattamente)
# NON: v1.0.1-test o altre varianti

# Se tag sbagliato:
git tag -d v1.0.1-sbagliato  # Cancella locale
git push origin --delete v1.0.1-sbagliato  # Cancella remote

# Ricrea correttamente
git tag -a v1.0.1 -m "msg"
git push origin v1.0.1
```

### Problema: Versione nel binario non si aggiorna

**Soluzione:**
1. Controlla ci-cd.yml has the step "Update AssemblyInfo.cs"
2. Se step manca, il workflow NON aggiornerà la versione
3. Contatta admin per aggiornare workflow

### Problema: manifest.json non aggiornato

**Soluzione:**
1. Verifica che workflow ha step "Update manifest.json"
2. Controlla release asset include manifest.json
3. Se missing, workflow non l'ha creato (check workflow logs)

---

## 📞 Prossimi Passi

1. **OGGI:** Esegui STEP 1-5 (setup locale e tag test)
2. **DOMANI (o quando pronto):** Esegui STEP 6-12 (release reale)
3. **FOLLOW-UP:** Comunica risultati al team
4. **PRODUZIONE:** Usa questo processo per OGNI release futura

---

## 📖 Link Utili

- [UPDATE_SYSTEM_ANALYSIS.md](./UPDATE_SYSTEM_ANALYSIS.md) - Leggi questo prima se non capisci perché il vecchio sistema falliva
- [UPDATE_SYSTEM_TESTING.md](./UPDATE_SYSTEM_TESTING.md) - Testing dettagliato
- [DEPLOYMENT.md](../DEPLOYMENT.md) - Strategia completa
- GitHub Repository: https://github.com/artcava/DO.VIVICARE.Reporting
- GitHub Actions: https://github.com/artcava/DO.VIVICARE.Reporting/actions
- GitHub Releases: https://github.com/artcava/DO.VIVICARE.Reporting/releases

---

**Documento:** Implementation Roadmap  
**Data:** 22 Gennaio 2026  
**Status:** 🚀 READY TO EXECUTE  
**Tempo Totale:** ~2 ore  
**Difficoltà:** Media (tutti step sono guidati)
