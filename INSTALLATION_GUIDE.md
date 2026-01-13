# DO.VIVICARE Reporting - Installation & Plugin Management

## 📋 Architettura

```
APPLICAZIONE (v1.2.0)
├─ Single installer: DO.VIVICARE-Setup-1.2.0.msi
├─ Include: UI + Reporter library
└─ Versionamento: Unificato per app

LIBRERIE PLUGIN (Indipendenti)
├─ Document Libraries (14 modules)
│  ├─ DO.VIVICARE.Document.ADIAltaIntensita
│  ├─ DO.VIVICARE.Document.ADIBassaIntensita
│  ├─ DO.VIVICARE.Document.ASST
│  ├─ DO.VIVICARE.Document.Comuni
│  ├─ DO.VIVICARE.Document.LazioHealthWorker
│  ├─ DO.VIVICARE.Document.MinSan
│  ├─ DO.VIVICARE.Document.Prestazioni
│  ├─ DO.VIVICARE.Document.Prezzi
│  ├─ DO.VIVICARE.Document.Rendiconto
│  ├─ DO.VIVICARE.Document.Report16
│  ├─ DO.VIVICARE.Document.Report18
│  ├─ DO.VIVICARE.Document.Valorizzazione
│  ├─ DO.VIVICARE.Document.ValorizzazioniADIAlta
│  └─ DO.VIVICARE.Document.ZSDFatture
│
└─ Report Libraries (3 modules)
   ├─ DO.VIVICARE.Report.AllegatoADI
   ├─ DO.VIVICARE.Report.Dietetica
   └─ DO.VIVICARE.Report.Valorizzazione

VERSIONAMENTO:
├─ App versioning: MAJOR.MINOR.PATCH (e.g., 1.2.0)
└─ Plugin versioning: Indipendente per ogni libreria (e.g., 1.0.0, 1.1.0, etc.)
```

---

## 🚀 Installazione Utenti

### 1️⃣ Prima Installazione

```bash
# Scarica da GitHub Releases
https://github.com/artcava/DO.VIVICARE.Reporting/releases/latest

# File unico:
DO.VIVICARE-Setup-1.2.0.msi
```

**Procedura:**
1. Doppio-click su `DO.VIVICARE-Setup-1.2.0.msi`
2. Segui wizard installazione
3. App si installa in `C:\Program Files\DO.VIVICARE\`
4. Icona desktop creata automaticamente
5. Avvia app

✅ **Risultato:** App funzionante con capacità di scaricare plugin

---

### 2️⃣ Aggiungere/Aggiornare Plugin (Librerie)

**In-App:**

1. Avvia `DO.VIVICARE.UI.exe`
2. Menu: **Tools → Plugin Manager**
3. Tab: **Available Plugins**
4. Vedi lista di:
   - ✅ Document Libraries (14)
   - ✅ Report Libraries (3)
   - ✅ Versione installata
   - ✅ Versione disponibile online

5. Per ogni plugin:
   ```
   [Plugin Name]     [Installed v1.0.0]  [Available v1.1.0]  [↓ Download]
   ```

6. Click `[↓ Download]` → Scarica e installa automaticamente
7. ✅ Nessun riavvio richiesto (hot-reload)

---

## 🛠️ Distribuzione per Sviluppatori

### Release APP (es. v1.2.0)

**Solo quando cambia l'applicativo principale (UI + Reporter)**

```bash
# 1. Aggiorna versione in AssemblyInfo.cs
# [assembly: AssemblyVersion("1.2.0.0")]

# 2. Commit e push
git add .
git commit -m "Release v1.2.0: UI and Reporter updates"
git push origin master

# 3. Crea tag (GitHub Actions si avvia automaticamente)
git tag -a v1.2.0 -m "Release version 1.2.0"
git push origin v1.2.0
```

**GitHub Actions automaticamente:**
- ✅ Build MSI installer: `DO.VIVICARE-Setup-1.2.0.msi`
- ✅ Carica in GitHub Releases
- ✅ Genera checksum SHA256

---

### Release SINGOLO PLUGIN (es. Document.ADIAltaIntensita v1.1.0)

**Quando aggiorni UNA sola libreria**

```bash
# 1. Modifica solo il progetto interessato
# Esempio: DO.VIVICARE.Document.ADIAltaIntensita/Properties/AssemblyInfo.cs
# [assembly: AssemblyVersion("1.1.0.0")]

# 2. Commit
git add DO.VIVICARE.Document.ADIAltaIntensita/
git commit -m "Update: Document.ADIAltaIntensita v1.1.0"
git push origin master

# 3. Tag specifico per questo plugin
git tag -a plugin/document.adialtaintensita/1.1.0 -m "ADI Alta Intensita update"
git push origin plugin/document.adialtaintensita/1.1.0
```

**GitHub Actions automaticamente:**
- ✅ Build solo `DO.VIVICARE.Document.ADIAltaIntensita-1.1.0.dll`
- ✅ Carica in GitHub Releases
- ✅ Aggiorna manifest.json

---

## 📄 Manifest Schema

**File: `manifest.json` (su GitHub)**

```json
{
  "app": {
    "version": "1.2.0",
    "name": "DO.VIVICARE Reporting UI",
    "downloadUrl": "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/v1.2.0/DO.VIVICARE-Setup-1.2.0.msi",
    "checksum": "sha256:...",
    "releaseDate": "2026-01-15",
    "minFramework": "4.8"
  },
  "documents": [
    {
      "id": "document.adialtaintensita",
      "name": "ADI Alta Intensita",
      "version": "1.0.5",
      "downloadUrl": "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/plugin/document.adialtaintensita/1.0.5/DO.VIVICARE.Document.ADIAltaIntensita-1.0.5.dll",
      "checksum": "sha256:...",
      "releaseDate": "2025-12-10",
      "dependencies": []
    },
    {
      "id": "document.adibassaintensita",
      "name": "ADI Bassa Intensita",
      "version": "1.0.3",
      "downloadUrl": "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/plugin/document.adibassaintensita/1.0.3/DO.VIVICARE.Document.ADIBassaIntensita-1.0.3.dll",
      "checksum": "sha256:...",
      "releaseDate": "2025-11-28",
      "dependencies": []
    },
    {
      "id": "document.asst",
      "name": "ASST",
      "version": "1.0.2",
      "downloadUrl": "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/plugin/document.asst/1.0.2/DO.VIVICARE.Document.ASST-1.0.2.dll",
      "checksum": "sha256:...",
      "releaseDate": "2025-11-15",
      "dependencies": []
    },
    {
      "id": "document.comuni",
      "name": "Comuni",
      "version": "1.0.1",
      "downloadUrl": "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/plugin/document.comuni/1.0.1/DO.VIVICARE.Document.Comuni-1.0.1.dll",
      "checksum": "sha256:...",
      "releaseDate": "2025-11-01",
      "dependencies": []
    },
    {
      "id": "document.laziohealthworker",
      "name": "Lazio Health Worker",
      "version": "1.0.0",
      "downloadUrl": "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/plugin/document.laziohealthworker/1.0.0/DO.VIVICARE.Document.LazioHealthWorker-1.0.0.dll",
      "checksum": "sha256:...",
      "releaseDate": "2025-10-20",
      "dependencies": []
    },
    {
      "id": "document.minsan",
      "name": "Ministero Sanità",
      "version": "1.0.0",
      "downloadUrl": "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/plugin/document.minsan/1.0.0/DO.VIVICARE.Document.MinSan-1.0.0.dll",
      "checksum": "sha256:...",
      "releaseDate": "2025-10-15",
      "dependencies": []
    },
    {
      "id": "document.prestazioni",
      "name": "Prestazioni",
      "version": "1.0.0",
      "downloadUrl": "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/plugin/document.prestazioni/1.0.0/DO.VIVICARE.Document.Prestazioni-1.0.0.dll",
      "checksum": "sha256:...",
      "releaseDate": "2025-10-10",
      "dependencies": []
    },
    {
      "id": "document.prezzi",
      "name": "Prezzi",
      "version": "1.0.0",
      "downloadUrl": "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/plugin/document.prezzi/1.0.0/DO.VIVICARE.Document.Prezzi-1.0.0.dll",
      "checksum": "sha256:...",
      "releaseDate": "2025-10-05",
      "dependencies": []
    },
    {
      "id": "document.rendiconto",
      "name": "Rendiconto",
      "version": "1.0.0",
      "downloadUrl": "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/plugin/document.rendiconto/1.0.0/DO.VIVICARE.Document.Rendiconto-1.0.0.dll",
      "checksum": "sha256:...",
      "releaseDate": "2025-09-30",
      "dependencies": []
    },
    {
      "id": "document.report16",
      "name": "Report 16",
      "version": "1.0.0",
      "downloadUrl": "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/plugin/document.report16/1.0.0/DO.VIVICARE.Document.Report16-1.0.0.dll",
      "checksum": "sha256:...",
      "releaseDate": "2025-09-25",
      "dependencies": []
    },
    {
      "id": "document.report18",
      "name": "Report 18",
      "version": "1.0.0",
      "downloadUrl": "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/plugin/document.report18/1.0.0/DO.VIVICARE.Document.Report18-1.0.0.dll",
      "checksum": "sha256:...",
      "releaseDate": "2025-09-20",
      "dependencies": []
    },
    {
      "id": "document.valorizzazione",
      "name": "Valorizzazione",
      "version": "1.0.0",
      "downloadUrl": "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/plugin/document.valorizzazione/1.0.0/DO.VIVICARE.Document.Valorizzazione-1.0.0.dll",
      "checksum": "sha256:...",
      "releaseDate": "2025-09-15",
      "dependencies": []
    },
    {
      "id": "document.valorizzazioniadialta",
      "name": "Valorizzazioni ADI Alta",
      "version": "1.0.0",
      "downloadUrl": "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/plugin/document.valorizzazioniadialta/1.0.0/DO.VIVICARE.Document.ValorizzazioniADIAlta-1.0.0.dll",
      "checksum": "sha256:...",
      "releaseDate": "2026-01-13",
      "dependencies": []
    },
    {
      "id": "document.zsdfatture",
      "name": "ZSD Fatture",
      "version": "1.0.0",
      "downloadUrl": "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/plugin/document.zsdfatture/1.0.0/DO.VIVICARE.Document.ZSDFatture-1.0.0.dll",
      "checksum": "sha256:...",
      "releaseDate": "2025-09-10",
      "dependencies": []
    }
  ],
  "reports": [
    {
      "id": "report.allegatoadi",
      "name": "Allegato ADI",
      "version": "1.0.3",
      "downloadUrl": "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/plugin/report.allegatoadi/1.0.3/DO.VIVICARE.Report.AllegatoADI-1.0.3.dll",
      "checksum": "sha256:...",
      "releaseDate": "2025-12-15",
      "dependencies": []
    },
    {
      "id": "report.dietetica",
      "name": "Dietetica",
      "version": "1.0.2",
      "downloadUrl": "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/plugin/report.dietetica/1.0.2/DO.VIVICARE.Report.Dietetica-1.0.2.dll",
      "checksum": "sha256:...",
      "releaseDate": "2025-12-01",
      "dependencies": []
    },
    {
      "id": "report.valorizzazione",
      "name": "Valorizzazione",
      "version": "1.0.1",
      "downloadUrl": "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/plugin/report.valorizzazione/1.0.1/DO.VIVICARE.Report.Valorizzazione-1.0.1.dll",
      "checksum": "sha256:...",
      "releaseDate": "2025-11-20",
      "dependencies": []
    }
  ]
}
```

---

## 🔄 GitHub Actions Workflow (Aggiornato)

**File: `.github/workflows/build-and-release.yml`**

### Trigger 1: Release APP (tag `v*`)
```
git tag v1.2.0
git push origin v1.2.0
  ↓
✅ Build UI + Reporter
✅ Crea DO.VIVICARE-Setup-1.2.0.msi
✅ Upload in GitHub Releases
```

### Trigger 2: Release PLUGIN (tag `plugin/*`)
```
git tag plugin/document.adialtaintensita/1.1.0
git push origin plugin/document.adialtaintensita/1.1.0
  ↓
✅ Build solo Document.ADIAltaIntensita.dll
✅ Upload in GitHub Releases
✅ Aggiorna manifest.json
```

---

## 📥 Plugin Manager UI (Concetto)

**In-App Window: Tools → Plugin Manager**

```
┌─────────────────────────────────────────────────────────────────┐
│ DO.VIVICARE Plugin Manager                                      │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│ [Available Plugins] [Installed] [Updates]                       │
│                                                                  │
│ Document Libraries (14 total)                                    │
│ ┌──────────────────────────────────────────────────────────────┐
│ │ ADI Alta Intensita        v1.0.5  [↓ Download]  1.2 MB      │
│ │ ADI Bassa Intensita       v1.0.3  [↓ Download]  1.1 MB      │
│ │ ASST                      v1.0.2  [↓ Download]  0.9 MB      │
│ │ Comuni                    v1.0.1  [↓ Download]  0.8 MB      │
│ │ Lazio Health Worker       v1.0.0  [↓ Download]  1.5 MB      │
│ │ Ministero Sanità          v1.0.0  [↓ Download]  0.7 MB      │
│ │ Prestazioni               v1.0.0  [↓ Download]  1.0 MB      │
│ │ Prezzi                    v1.0.0  [↓ Download]  0.6 MB      │
│ │ Rendiconto                v1.0.0  [↓ Download]  1.3 MB      │
│ │ Report 16                 v1.0.0  [↓ Download]  1.1 MB      │
│ │ Report 18                 v1.0.0  [↓ Download]  1.2 MB      │
│ │ Valorizzazione            v1.0.0  [↓ Download]  0.9 MB      │
│ │ Valorizzazioni ADI Alta   v1.0.0  [↓ Download]  0.8 MB      │
│ │ ZSD Fatture               v1.0.0  [↓ Download]  1.4 MB      │
│ └──────────────────────────────────────────────────────────────┘
│                                                                  │
│ Report Libraries (3 total)                                       │
│ ┌──────────────────────────────────────────────────────────────┐
│ │ Allegato ADI              v1.0.3  [✓ Installed]             │
│ │ Dietetica                 v1.0.2  [Update v1.0.3] ↓         │
│ │ Valorizzazione            v1.0.1  [Update v1.0.2] ↓         │
│ └──────────────────────────────────────────────────────────────┘
│                                                                  │
│ Status: Ready                                        [Close]     │
└─────────────────────────────────────────────────────────────────┘
```

---

## ⚙️ Configurazione Utente

**Config file: `%APPDATA%\DO.VIVICARE\config.json`**

```json
{
  "app": {
    "version": "1.2.0",
    "autoUpdate": true,
    "checkInterval": 3600
  },
  "plugins": {
    "autoUpdate": true,
    "checkInterval": 86400,
    "installDir": "C:\\Program Files\\DO.VIVICARE\\Plugins",
    "installed": {
      "document.adialtaintensita": {
        "version": "1.0.5",
        "installedDate": "2025-12-10",
        "enabled": true
      },
      "report.dietetica": {
        "version": "1.0.2",
        "installedDate": "2025-11-30",
        "enabled": true
      }
    }
  }
}
```

---

## 📋 Riassunto Distribuzione

| Aspetto | Prima | Dopo |
|---------|-------|------|
| **Installer APP** | Multipli (UI, Reporter, Libs) | 🎯 **Un unico file: DO.VIVICARE-Setup-1.2.0.msi** |
| **Librerie** | Incluse nell'installer | 🎯 **Scaricabili online da applicativo** |
| **Versionamento** | Confuso | 🎯 **App: MAJOR.MINOR.PATCH** |
| | | **Plugins: Indipendenti** |
| **Aggiornamenti** | Manuale | 🎯 **Auto via applicativo** |
| **Zero Downtime** | ❌ Riavvio richiesto | 🎯 ✅ **Hot-reload plugin** |
| **Controllo Versioni** | Nessuno | 🎯 **Manifest.json centralizzato** |

---

## 🚀 Prossimi Step

1. ✅ Creare manifest.json template
2. ✅ Aggiornare GitHub Actions workflow (tag diversi per app vs plugin)
3. ✅ Implementare Plugin Manager UI in frmSettings.cs
4. ✅ Implementare auto-download + checksum validation
5. ✅ Testare completo end-to-end

**Documento aggiornato**: 13 Gennaio 2026 - Aggiunto ValorizzazioniADIAlta (14° document module)
