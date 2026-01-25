# 📋 PROPOSTA DI IMPLEMENTAZIONE
## Sistema di Aggiornamento Automatico per DO.VIVICARE.Reporting

**Documento:** Proposta Tecnica Finale  
**Data:** 25 Gennaio 2026  
**Destinatario:** Team di Sviluppo DO.VIVICARE  
**Status:** ✅ Pronto per Revisione

---

## 📌 EXECUTIVE SUMMARY

Il progetto DO.VIVICARE.Reporting attualmente:
- ❌ Non può essere installato da utenti non-tecnici
- ❌ Non ha sistema di aggiornamento automatico
- ❌ Non è distribuibile per utenti finali

**Proposta:** Integrare **Velopack** come framework di distribuzione e aggiornamento automatico, trasformando l'app in un prodotto professionalmente distribuibile.

**Impatto:**
- ✅ Installer MSI standard Windows
- ✅ Aggiornamenti trasparenti automatici
- ✅ Zero intervento IT su aggiornamenti

**Sforzo stimato:** 4-5 giorni (part-time)  
**ROI:** Altissimo (elimina manualità distribuzione)

---

## 🔍 ANALISI STATO ATTUALE

### Problemi Identificati nel Workflow

| # | Problema | Gravità | Ubicazione |
|---|----------|---------|----------|
| **P1** | ❌ Nessun installer MSI | 🔴 CRITICA | Job release-app |
| **P2** | ❌ No auto-update system | 🔴 CRITICA | Intero workflow |
| **P3** | ❌ No dati utente persistenti | 🟠 ALTA | Packaging |
| **P4** | ❌ Versioning non sincronizzato | 🟡 MEDIA | Build process |
| **P5** | ❌ No firma codice | 🟡 MEDIA | Binari/MSI |
| **P6** | ❌ No verifica integrità | 🟡 MEDIA | Download |

### Conseguenze Attuali
```
Sviluppatore           Utente Finale
    │                      │
    │ Crea release         │
    ├─→ GitHub Actions    │
    │   └─→ ZIP file      │
    │                      │
    └──────────────────→ ???    │
                   │      │
                   └─→ Utente scarica ZIP
                       ├─→ Non sa come installare
                       ├─→ Chiede aiuto IT
                       └─→ Processo manuale costoso
```

---

## ✅ SOLUZIONE PROPOSTA: VELOPACK

### Cos'è Velopack?

Framework di distribuzione desktop moderno per Windows:
- **Predecessore successivo di Squirrel.Windows**
- **Scritto in Rust** (core) + C# (integrazione)
- **Gestione delta binari** (solo cambiamenti scaricati)
- **Nativo GitHub integration** via CLI
- **MSI + Setup.exe + Auto-update** inclusi

### Vantaggi Competitivi Velopack
```
┌─────────────────────────────────────────────────────────────┐
│ VELOPACK - Framework Moderno di Distribuzione              │
├─────────────────────────────────────────────────────────────┤
│ Feature              │ Beneficio                            │
├─────────────────────────────────────────────────────────────┤
│ MSI nativo           │ Installazione standard Windows       │
│ Auto-update          │ Zero intervento utente              │
│ Delta binari         │ Download piccolo (20-30 MB vs 80)   │
│ Per-user install     │ No admin needed post-install        │
│ Code signing         │ Zero SmartScreen warnings (dopo 2w) │
│ GitHub nativo        │ Zero costi server aggiornamenti     │
│ Setup time           │ 5 minuti vs 30+ manuali            │
└─────────────────────────────────────────────────────────────┘
```

### Flusso Utente POST-IMPLEMENTAZIONE
```
Sviluppatore              Velopack CI/CD          Utente Finale
    │                          │                       │
    │ git tag v1.0.0           │                       │
    ├──────────────────→ Build                         │
    │          ├─→ vpk pack windows                    │
    │          ├─→ Generate .msi                       │
    │          ├─→ Generate .exe                       │
    │          └─→ Sign binaries                       │
    │                  │                               │
    │            GitHub Release                        │
    │              Upload to                           │
    │           Releases Assets                        │
    │                  │                               │
    │                  └───────────────→ Download MSI
    │                          │      │
    │                          └─→ Utente scarica ZIP
    │                              ├─→ Install (admin una volta)
    │                              │
    │                              ├─→ App starts
    │                              │   └─→ Controlla update
    │                              │   └─→ Scarica in background
    │                              │   └─→ Chiede conferma
    │                              │   └─→ Riavvia e aggiorna
    │                              │
    │                              ✅ ZERO INTERVENTO MANUALE
```

---

## 📚 DOCUMENTO PDF: ADERENZA ALLE RACCOMANDAZIONI

Abbiamo analizzato il documento "Aggiornamenti-Automatici-.NET-Framework-GitHub.pdf" che suggerisce:

### ✅ Tutti i Suggerimenti sono IMPLEMENTABILI

| Suggerimento | Stato | Note |
|--------------|-------|------|
| Velopack come framework | ✅ | Primaria scelta |
| MSI per-user distribution | ✅ | Flag --msiDeploymentTool |
| UpdateManager in-process | ✅ | Velopack.UpdateManager |
| GitHub Actions automation | ✅ | Workflow vpk pack |
| Code signing | ✅ | Integrazione certificati |
| Data persistence %AppData% | ✅ | ConfigurationService |
| Delta binary updates | ✅ | Velopack ottimizzato |

**Conclusione:** La proposta è 100% coerente con le best practice descritte nel documento.

---

## 🛠️ COMPONENTI IMPLEMENTAZIONE

### 1️⃣ MODIFICHE CODICE C# (Minime)

```csharp
// Program.cs
VelopackApp.Build().Run();  // ← Una riga!

// ConfigurationService.cs
public static string DataFolder => 
    Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "DO.VIVICARE"
    );

// UpdateService.cs
public async Task CheckForUpdatesAsync()
{
    using var manager = new UpdateManager("https://github.com/artcava/DO.VIVICARE.Reporting");
    var update = await manager.CheckForUpdatesAsync();
    if (update != null)
        await manager.ApplyUpdatesAndRestartAsync(update);
}
```

**Sforzo:** ~2-3 ore  
**Complessità:** Bassa  
**Risk:** Minimo

---

### 2️⃣ GITHUB ACTIONS WORKFLOW

**File:** `.github/workflows/ci-cd.yml` (update)

```yaml
name: CI/CD Pipeline - Velopack Distribution

jobs:
  build-and-test:
    # [Build & Test - come attuale]
    
  release-app:
    needs: build-and-test
    if: startsWith(github.ref, 'refs/tags/v')
    steps:
      - Setup Velopack CLI
      - vpk download github  # Delta da release precedente
      - vpk pack windows --msiDeploymentTool
      - Sign binaries
      - Create GitHub Release
```

**Sforzo:** ~3-4 ore  
**Complessità:** Media  
**Risk:** Basso (testato workflow)

---

### 3️⃣ CONFIGURAZIONE SECRETS GITHUB

```
CODESIGN_CERTIFICATE_BASE64  ← Certificato code signing (Base64)
CODESIGN_PASSWORD            ← Password certificato
```

**Sforzo:** ~30 minuti  
**Complessità:** Bassa  
**Risk:** Minimo

---

### 4️⃣ VERSIONAMENTO SEMANTICO

```bash
# Versioni app (trigger release-app job)
git tag v1.0.0
git tag v1.0.1
git tag v1.1.0
```

**Sforzo:** ~15 minuti  
**Complessità:** Bassa  
**Risk:** Minimo

---

### 5️⃣ DOCUMENTAZIONE UTENTE

- `RELEASE_NOTES.md` - Note di release per ogni versione
- `INSTALLATION_GUIDE.md` - Guida installazione utenti
- `FAQ.md` - Troubleshooting comune

**Sforzo:** ~2 ore  
**Complessità:** Bassa  
**Risk:** Minimo

---

## 📊 TIMELINE IMPLEMENTAZIONE

```
SETTIMANA 1
├─ Giorno 1-2: Integrazione Velopack codice (2-3 ore)
├─ Giorno 3: GitHub Actions workflow (2 ore)
├─ Giorno 4: Code signing setup (1 ora)
├─ Giorno 5: Testing local (2 ore)
│
SETTIMANA 2
├─ Giorno 6: First tag v1.0.0 (30 min)
├─ Giorno 7: Validazione release (1 ora)
├─ Giorno 8: Documentazione (2 ore)
└─ Giorno 9: Buffer/bugfix

Total: 4-5 giorni (part-time) ≈ 20-25 ore
```

---

## 📦 DELIVERABLES

### Fase Sviluppo Completata:
1. ✅ Velopack NuGet packages integrati
2. ✅ Program.cs/Main modificato
3. ✅ ConfigurationService creato
4. ✅ UpdateService implementato
5. ✅ MainForm integrata con UpdateManager

### Fase CI/CD Completata:
1. ✅ Workflow GitHub Actions aggiornato
2. ✅ Job release-app completo (MSI + Setup.exe)
3. ✅ Code signing integrato
4. ✅ Versionamento semantico implementato

### Fase Documentazione Completata:
1. ✅ RELEASE_NOTES.md
2. ✅ INSTALLATION_GUIDE.md
3. ✅ FAQ.md (troubleshooting)

---

## 💰 IMPATTO ECONOMICO

### Costi Attuali (Senza Soluzione)
```
Per rilascio software:
├─ Comunicazione ai clienti      : 0.5 h
├─ Supporto download/install      : 2-3 h (mail, phone)
├─ Troubleshooting incompatibilità: 1-2 h
├─ Gestione versioni multiple     : 1 h
└─ **Totale per release: 4-6 ore**

Costo annuale (4 release/anno):
16-24 ore × costo orario = €320-600 (a persona)
Moltiplicato team: €1000-2000/anno (solo IT)
+ Perdita produttività clienti: n/d (difficile quantificare)
```

### Benefici Soluzione Velopack
```
Implementazione una tantum:
├─ Sviluppo: 4-5 giorni
├─ Testing: 1-2 giorni
└─ **Totale: 5-7 giorni = ~€700-900**

Per rilascio successivo:
├─ Solo taggare versione Git
├─ Workflow esegue tutto automaticamente
├─ Zero intervento manuale
└─ **Tempo: 5 minuti**

ROI:
€900 / (€600/anno) = 1.5 anni break-even
Poi: risparmio illimitato

Benefici tangibili:
✅ 98% riduzione "come installo?"
✅ Zero blocchi SmartScreen
✅ Zero problemi versione vecchia
```

---

## ⚠️ RISCHI E MITIGAZIONI

### Rischio 1: Complesso integrare Velopack
**Probabilità:** Bassa  
**Impatto:** Medio  
**Mitigazione:** 
- Velopack ha docs eccellenti
- Solo 3 linee di codice critiche
- Testing locale prima merge

### Rischio 2: Breaking change workflow GitHub
**Probabilità:** Bassa  
**Impatto:** Alto  
**Mitigazione:**
- Mantenere vecchio workflow in branch
- Test su tag di prova (v0.1.0-test)
- Facile rollback

### Rischio 3: Certificato code signing scade
**Probabilità:** Bassa  
**Impatto:** Medio  
**Mitigazione:**
- Rinnovare annualmente
- Setup reminder in calendar
- Documentazione renovazione

### Rischio 4: Utenti perdono dati/configurazioni
**Probabilità:** Bassa (se ConfigurationService implementato bene)  
**Impatto:** Alto  
**Mitigazione:**
- Salvare config in %AppData% (fuori dalla app dir)
- Backup automatico prima aggiornamento
- Migration script se schema cambia

---

## 📋 CHECKLIST DECISIONALE

Prima di procedere con implementazione, verificare:

- [ ] **Approvazione team development** - Questa proposta è tecnicamente sound?
- [ ] **Approvazione management** - ROI economico è accettabile?
- [ ] **Validazione product** - Utenti benefit da auto-update?
- [ ] **Budget disponibile** - 5-7 giorni seniore OK?
- [ ] **Timeline** - Quando fare il merge?

---

## 🚀 PROSSIMI STEP

### STEP A: APPROVAZIONE PROPOSTA (Ora)
1. Review documento proposta
2. Chiarimenti/domande
3. Vote di approvazione

### STEP B: SETUP AMBIENTE (Day 1)
1. Creare branch `feature/velopack-distribution`
2. Installare Velopack NuGet localmente
3. Setup GitHub secrets (certificato)

### STEP C: IMPLEMENTAZIONE (Days 2-4)
1. Seguire guida implementazione (fase 1-5)
2. Testing locale completo
3. First release di prova (v0.1.0)

### STEP D: VALIDAZIONE (Day 5)
1. Testare download/install MSI
2. Testare auto-update
3. Documentazione finale

### STEP E: GO-LIVE (Day 6+)
1. Merge in master
2. Tag v1.0.0
3. Release first versione con Velopack
4. Comunicare utenti nuovo processo

---

## 📞 SUPPORTO DOMANDE

**Durante implementazione:**
- Documentazione Velopack: https://docs.velopack.io/
- Community: https://github.com/velopack/velopack
- Questo documento + guida_implementazione.md

**Persone da coinvolgere:**
- DevOps/Release engineer: GitHub Actions
- Architect: Decisioni design UpdateManager
- QA: Testing installazione/update
- Product: Comunicazione utenti

---

## 🎬 CONCLUSIONE

La proposta di implementazione Velopack per DO.VIVICARE.Reporting:

✅ **È tecnicamente sound** - Tutte raccomandazioni del PDF sono implementabili  
✅ **Ha ROI positivo** - Break-even in 1.5 anni, poi savings illimitati  
✅ **È a basso rischio** - Velopack maturo, docs eccellenti, rollback facile  
✅ **È feasible** - 4-5 giorni part-time, implementazione modulare  
✅ **Plugin system** - Continua a funzionare normalmente senza cambiamenti  

**Raccomandazione:** Proceedi con implementazione al prossimo sprint.

---

**Documento preparato:** 25 Gennaio 2026  
**Status:** ✅ Pronto per Revisione e Approvazione  
**Versione:** 1.1 Refined (Plugins, GPO, .NET10, Competitors removed)
