# Guida di Migrazione: DO.VIVICARE Reporting v1.1.0 → v1.2.0

## 📋 Sommario

Questo documento spiega come migrare da **Azure ClickOnce (v1.1.0)** a **GitHub Releases (v1.2.0)**.

**Novità principale:** L'applicazione viene distribuita tramite **GitHub Releases** anziché Azure, con **aggiornamenti automatici** e **nessuna richiesta IT per i futuri update**.

---

## ✅ Prerequisiti

- ✓ Windows 7 SP1 o successivo
- ✓ .NET Framework 4.8 o successivo
- ✓ 200 MB di spazio libero su disco
- ✓ Accesso a Internet (per download iniziale)

---

## 🚀 Processo di Migrazione

### Step 1: Disinstallare Versione Vecchia

1. **Apri il Pannello di Controllo** → **Programmi e Funzionalità**
2. Cerca **"Reporting"** o **"DO.VIVICARE.UI"**
3. Clicca per selezionarlo e scegli **"Disinstalla"**
4. Segui il wizard di disinstallazione
5. **Riavvia il computer** (consigliato)

```
Pannello di Controllo 
  └─ Programmi e Funzionalità
      └─ Reporting (v1.1.0.6)
          └─ [Disinstalla]
```

---

### Step 2: Scaricare Versione Nuova

#### Opzione A: Download Manuale (Consigliato per Prima Volta)

1. Vai su GitHub Releases:
   **https://github.com/artcava/DO.VIVICARE.Reporting/releases**

2. Cerca il **latest release** (v1.2.0 o superiore)
   
3. Nella sezione **"Assets"**, scarica:
   - `DO.VIVICARE.UI-1.2.0.zip` (oppure il file `.msi` se disponibile)

```
GitHub Releases Page
├─ v1.2.0 (Latest)
│  └─ Assets
│     ├─ DO.VIVICARE.UI-1.2.0.zip ← ⬇️ SCARICA QUESTO
│     ├─ CHECKSUM-SHA256.txt
│     └─ RELEASE-NOTES-v1.2.0.md
│
└─ v1.1.5 (Previous)
```

---

### Step 3: Verificare Integrità Download (Opzionale ma Consigliato)

1. Apri **PowerShell** (click destro sulla cartella download, scegli "Apri PowerShell qui")

2. Esegui:
   ```powershell
   Get-FileHash "DO.VIVICARE.UI-1.2.0.zip" -Algorithm SHA256
   ```

3. Confronta il risultato con il valore in `CHECKSUM-SHA256.txt`
   
   ✅ Se coincide → File integro  
   ❌ Se diverso → Scarica di nuovo

---

### Step 4: Installare Versione Nuova

#### Se hai scaricato `.zip`:

1. **Estrai il file**
   - Click destro su `DO.VIVICARE.UI-1.2.0.zip`
   - Scegli "Estrai tutto..."
   - Seleziona cartella destinazione (es: `C:\Program Files\DO\`)

2. **Esegui l'installer**
   - Doppio click su `DO.VIVICARE.UI.exe`
   - Autorizza il UAC (User Account Control) se richiesto
   - Segui il wizard di installazione

3. **Completa l'installazione**
   - L'applicazione verrà installata
   - Verrà creato un collegamento sul Desktop
   - L'app sarà configurata per auto-update

---

## 📱 Primo Avvio - Configurazione

Al primo avvio dopo l'installazione:

1. **L'app detecta automaticamente:** 
   - Versione installata
   - Disponibilità di aggiornamenti

2. **Se ci sono aggiornamenti:**
   - Mostra notifica: "Update available"
   - Clicca "Download and Install"
   - L'app scarica e installa in background
   - Riavvia automaticamente

3. **Se sei al latest:**
   - Avvio normale dell'applicazione
   - Nessun ritardo

---

## 🔄 Aggiornamenti Futuri (NESSUNA Richiesta IT!)

### Come Ricevere Aggiornamenti

**Automatico:**
- L'app controlla gli aggiornamenti all'avvio
- Se disponibile, mostra notifica
- Click "Update Now" → Scarica e installa
- Nessuna richiesta IT richiesta ✅

**Manuale:**
- Menu: **Tools → Check for Updates**
- Oppure vai a: https://github.com/artcava/DO.VIVICARE.Reporting/releases
- Scarica la nuova versione
- Esegui installer

---

## 📚 Aggiornamento Librerie (Document e Report)

Le librerie (moduloni Document e Report) si aggiornano **indipendentemente** dall'app principale.

### Processo:

1. **Apri DO.VIVICARE UI**

2. **Vai a:**
   ```
   Menu → Tools → Settings
   ```

3. **Seleziona tab:** 
   ```
   "Librerie e Moduli"
   ```

4. **Clicca:**
   ```
   "Controlla Aggiornamenti"
   ```

5. **L'app farà:**
   - Contatta GitHub
   - Scarica lista librerie disponibili
   - Mostra quali hanno aggiornamenti

6. **Per ciascuna libreria con update:**
   - Clicca il bottone **"Scarica"**
   - L'app scarica automaticamente
   - Installa nella cartella corretta
   - **Nessun riavvio richiesto!**

---

## 🔒 Verificare Integrità File Scaricati

GitHub fornisce **checksum SHA256** per ogni versione.

### In PowerShell:

```powershell
# Scaricare il file
Get-FileHash "DO.VIVICARE.UI-1.2.0.zip" -Algorithm SHA256

# Output:
# Algorithm       Hash                                                   Path
# ---------       ----                                                   ----
# SHA256          A1B2C3D4E5F6... [32 caratteri hex]                 ...\
```

### Comparare con CHECKSUM-SHA256.txt:

```
DO.VIVICARE.UI-1.2.0.zip:
  Expected:  A1B2C3D4E5F6G7H8I9J0K1L2M3N4O5P6
  Computed:  A1B2C3D4E5F6G7H8I9J0K1L2M3N4O5P6  ✅ Match!
```

---

## ❓ Domande Frequenti

### P: Ho la v1.1.0, devo davvero disinstallare?
**R:** Sì. La nuova versione (1.2.0) è una versione **completamente nuovo**, quindi disinstalla prima quella vecchia per evitare conflitti.

### P: Perderò i miei dati durante l'aggiornamento?
**R:** **NO.** I dati sono memorizzati separatamente dall'applicazione. L'aggiornamento sostituisce SOLO i file dell'app.

### P: E se il download si interrompe?
**R:** Puoi riavviare il download. GitHub tornerà a dove si era fermato. Oppure cancella il file scaricato e scaricalo di nuovo.

### P: Quanto tempo prende l'installazione?
**R:** 2-5 minuti a seconda della velocità Internet. La app è ~15 MB compresso.

### P: È sicuro disinstallare v1.1.0 prima di installare v1.2.0?
**R:** Completamente sicuro. Prima di disinstallare, puoi esportare le configurazioni se necessario.

### P: Cosa succede se c'è una versione ancora più nuova (es: v1.3.0)?
**R:** L'app notificherà automaticamente e potrai aggiornare nello stesso modo. Niente di nuovo da fare.

### P: Posso stare su v1.2.0 e non aggiornare a versioni future?
**R:** Sì, puoi ignorare gli update e continuare a usare la versione che hai. Ma consigliamo di aggiornare per bug fix e nuove features.

---

## 🐛 Troubleshooting

### Problema: "File corrupted" durante l'estrazione

**Soluzione:**
1. Cancella il file `.zip` scaricato
2. Scaricalo di nuovo
3. Prova a estrarre di nuovo

---

### Problema: ".NET Framework not installed"

**Soluzione:**
1. Scarica .NET Framework 4.8 da:
   https://dotnet.microsoft.com/download/dotnet-framework/net48
2. Installa (richiede riavvio)
3. Prova a installare DO.VIVICARE di nuovo

---

### Problema: "Permission denied" durante l'installazione

**Soluzione:**
1. Esegui PowerShell "Come Amministratore"
2. Naviga alla cartella dove hai estratto i file
3. Esegui di nuovo l'installer

---

### Problema: L'app non avvia

**Soluzione:**
1. Prova a disinstallare e reinstallare
2. Verifica che .NET Framework 4.8 sia installato
3. Prova in Safe Mode
4. Contatta IT per diagnostica

---

## 📞 Support

Se hai problemi:

1. **Controlla i log dell'applicazione:**
   ```
   C:\Users\[TuoUser]\AppData\Local\DO.VIVICARE.UI\Logs\
   ```

2. **Contatta IT con:**
   - Versione app (Help → About)
   - Versione .NET Framework
   - Messaggio di errore esatto
   - File di log se disponibile

---

## 📝 Note Finali

✅ **Vantaggi della nuova versione:**
- Aggiornamenti automatici (nessuna richiesta IT)
- Distribuzione moderna via GitHub
- Checksum SHA256 per integrità file
- Release notes dettagliate
- Migliore supporto e tracciabilità

✅ **Timeline:**
- Il server Azure sarà disattivato dopo 30 giorni
- Assicurati di migrare prima di quella data
- Dopo la migrazione, il vecchio link non funzionerà più

---

**Versione Guida:** 1.0  
**Data:** 11 Gennaio 2026  
**Repository:** https://github.com/artcava/DO.VIVICARE.Reporting
