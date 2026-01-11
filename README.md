# DO.VIVICARE Reporting

Sistema avanzato di generazione report e documenti per strutture sanitarie italiane. Questo progetto gestisce la creazione automatizzata di report complessi in formato Excel per assistenza domiciliare integrata (ADI), prestazioni sanitarie e rendiconti amministrativi.

## 📋 Indice

- [Panoramica](#panoramica)
- [Requisiti](#requisiti)
- [Installazione e Setup](#installazione-e-setup)
- [Architettura del Progetto](#architettura-del-progetto)
- [Moduli Document](#moduli-document)
- [Moduli Report](#moduli-report)
- [Configurazione](#configurazione)
- [Utilizzo](#utilizzo)
- [Tecnologie](#tecnologie)
- [Roadmap](#roadmap)
- [Contribuire](#contribuire)
- [Licenza](#licenza)

---

## Panoramica

DO.VIVICARE Reporting è una soluzione completa per la gestione e la generazione di report sanitari in conformità alle normative italiane. Il sistema è specializzato nella creazione di:

- **Report ADI** (Assistenza Domiciliare Integrata) - Alta e Bassa Intensità
- **Rendiconti Amministrativi** - Tracciamento costi e prestazioni
- **Report per Enti Locali** - Comuni, ASST, Ministero della Sanità
- **Allegati Specializzati** - Dietetici, di valorizzazione, fatturazione
- **Report Legislativi** - Report 16 e Report 18

### Caratteristiche Principali

✅ **Architettura Modulare**: Ogni specialità sanitaria in modulo dedicato  
✅ **Generazione Excel Avanzata**: Formattazione, formule e layout complessi  
✅ **Configurazione Centralizzata**: Gestione via XML per impostazioni runtime  
✅ **Separazione Responsabilità**: Document layer e Report layer independenti  
✅ **Estensibilità**: Facile aggiunta di nuovi report e specialità  

---

## Requisiti

### Sistemi Operativi Supportati
- Windows 7 SP1 o superiore
- Windows Server 2008 R2 o superiore

### Software Prerequisiti
- **.NET Framework 4.6.1 o superiore** (consigliato 4.8)
- **Visual Studio 2019** o superiore (per sviluppo)
- **Microsoft Excel** (facoltativo, per visualizzazione output)

### Dipendenze NuGet
- **EPPlus** 4.5.x - Libreria per generazione Excel
- Altre dipendenze specifiche per modulo (vedere `packages.config`)

### Requisiti di Compilazione
- Processore: x86/x64 compatibile
- RAM: minimo 2 GB (consigliato 4 GB)
- Spazio disco: ~500 MB per la soluzione completa

---

## Installazione e Setup

### 1. Clonare il Repository

```bash
git clone https://github.com/artcava/DO.VIVICARE.Reporting.git
cd DO.VIVICARE.Reporting
```

### 2. Aprire la Soluzione

```bash
# Con Visual Studio
start DO.VIVICARE.Reporting.sln

# Oppure via linea di comando
devenv DO.VIVICARE.Reporting.sln
```

### 3. Ripristinare le Dipendenze NuGet

Visual Studio ripristinerà automaticamente i package, oppure manualmente:

```bash
nuget restore DO.VIVICARE.Reporting.sln
```

### 4. Compilare la Soluzione

**Via Visual Studio:**
- Menu: Build → Build Solution (Ctrl+Shift+B)
- Selezionare la configurazione: Debug o Release

**Via MSBuild:**

```bash
msbuild DO.VIVICARE.Reporting.sln /p:Configuration=Release /p:Platform="Any CPU"
```

### 5. Verificare l'Installazione

Dopoché la compilazione è completata:

```bash
# I file compilati saranno in:
# bin/Release/ o bin/Debug/ di ogni modulo

# Verificare la creazione dei .dll principali:
dir DO.VIVICARE.Reporter/bin/Release/DO.VIVICARE.Reporter.dll
dir DO.VIVICARE.UI/bin/Release/DO.VIVICARE.UI.exe
```

---

## Architettura del Progetto

```
DO.VIVICARE.Reporting/
├── DO.VIVICARE.Reporter/          [CORE ENGINE]
│   ├── Manager.cs                 # Orchestrazione della generazione report
│   ├── Model.cs                   # Modello dati principale
│   ├── ExcelManager.cs            # Gestione file Excel (formattazione, formule)
│   └── XMLSettings.cs             # Gestione configurazione XML
│
├── DO.VIVICARE.UI/                [INTERFACCIA UTENTE]
│   └── [Componenti UI per interazione utente]
│
├── DO.VIVICARE.Document.*/        [LAYER DOCUMENTO - 14 MODULI]
│   ├── ADIAltaIntensita/          # ADI ad alta intensità
│   ├── ADIBassaIntensita/         # ADI a bassa intensità
│   ├── ASST/                      # Aziende Socio-Sanitarie Territoriali
│   ├── Comuni/                    # Report per enti locali
│   ├── MinSan/                    # Ministero della Sanità
│   ├── Prestazioni/               # Gestione prestazioni sanitarie
│   ├── Prezzi/                    # Catalogo prezzi
│   ├── Rendiconto/                # Rendiconti amministrativi
│   ├── Report16/                  # Report legislativo 16
│   ├── Report18/                  # Report legislativo 18
│   ├── Valorizzazione/            # Valorizzazione prestazioni
│   ├── ZSDFatture/                # Gestione fatturazione ZSD
│   ├── LazioHealthWorker/         # Specifico per Lazio
│   └── AllegatiADI/               # Allegati per ADI
│
├── DO.VIVICARE.Report.*/          [LAYER REPORT - 3 MODULI]
│   ├── AllegatoADI/               # Logica report ADI
│   ├── Dietetica/                 # Logica report dietetici
│   └── Valorizzazione/            # Logica report valorizzazione
│
├── Example/                        [ESEMPI DI UTILIZZO]
├── AllegatiADIAlta/                [TEMPLATE E ALLEGATI]
└── DO.VIVICARE.Reporting.sln       [FILE SOLUZIONE PRINCIPALE]
```

### Flusso Architetturale

```
┌─────────────────────────────┐
│   DO.VIVICARE.UI            │  [Interfaccia Utente]
│   (Raccolta parametri)      │
└──────────────┬──────────────┘
               │
               ▼
┌─────────────────────────────┐
│   DO.VIVICARE.Reporter      │  [Manager] Orchestrazione
│   (Manager.cs)              │
└──────────────┬──────────────┘
               │
       ┌───────┴────────┐
       ▼                ▼
┌────────────┐   ┌──────────────┐
│ Document   │   │ XMLSettings  │  [Configurazione]
│ Modules    │   │ (Lettura XML)│
└────────────┘   └──────────────┘
       │
       ▼
┌─────────────────────────────┐
│ ExcelManager.cs             │  [Generazione Excel]
│ (Formattazione, Formule)    │
└──────────────┬──────────────┘
               │
               ▼
        ┌─────────────┐
        │ File .xlsx  │  [Output]
        └─────────────┘
```

---

## Moduli Document

I moduli Document definiscono la **struttura e la rappresentazione dei dati** specifici per ogni ambito sanitario.

### ADI (Assistenza Domiciliare Integrata)

**DO.VIVICARE.Document.ADIAltaIntensita**
- Gestisce pazienti e prestazioni ad alta intensità di cura
- Traccia interventi specialistici
- Calcola costi per livello di intensità

**DO.VIVICARE.Document.ADIBassaIntensita**
- Pazienti con bassa intensità di cura domiciliare
- Monitoraggio prestazioni basilari
- Rendiconti semplificati

### Enti e Istituzioni

**DO.VIVICARE.Document.ASST** - Aziende Socio-Sanitarie Territoriali
- Dati aggregati per ASST della Lombardia
- Reporting verso sistema sanitario regionale

**DO.VIVICARE.Document.Comuni** - Enti Locali
- Reporting verso comuni e province
- Conformità normative locali

**DO.VIVICARE.Document.MinSan** - Ministero della Sanità
- Report verso ministero
- Conformità normative nazionali

### Prestazioni e Economico

**DO.VIVICARE.Document.Prestazioni**
- Catalogo prestazioni fornite
- Associazione a percorsi terapeutici

**DO.VIVICARE.Document.Prezzi**
- Listino prezzi per prestazione
- Tariffazione per ente pagante

**DO.VIVICARE.Document.Valorizzazione**
- Valorizzazione economica prestazioni
- Calcoli costi e ricavi

**DO.VIVICARE.Document.ZSDFatture**
- Gestione fatturazione verso ZSD
- Conformità tracciamento fatture

### Report Legislativi

**DO.VIVICARE.Document.Report16 e Report18**
- Report obbligatori per legge
- Struttura e format specificati da normativa

**DO.VIVICARE.Document.Rendiconto**
- Rendiconti amministrativi
- Tracciamento economico

### Specialistici

**DO.VIVICARE.Document.LazioHealthWorker**
- Specifico per operatori sanitari nel Lazio
- Gestione certificazioni regionali

**DO.VIVICARE.Document.Dietetica**
- Piani dietetici e monitoraggio nutrizione
- Report dietetici specializzati

---

## Moduli Report

I moduli Report implementano la **logica di generazione** dei report partendo dai dati strutturati nei Document.

### DO.VIVICARE.Report.AllegatoADI

**Responsabilità:**
- Lettura dati da DO.VIVICARE.Document.ADIAltaIntensita e ADIBassaIntensita
- Generazione allegati ADI
- Calcoli statistici e aggregazioni
- Formattazione output Excel

**Output:**
- File Excel con allegati ADI completi
- Tabelle pivot e grafici

### DO.VIVICARE.Report.Dietetica

**Responsabilità:**
- Generazione report dietetici
- Gestione piani alimentari
- Monitoraggio compliance nutrizionale

**Output:**
- Piani dietetici in Excel
- Report di aderenza

### DO.VIVICARE.Report.Valorizzazione

**Responsabilità:**
- Calcolo valorizzazione economica
- Generazione report finanziari
- Aggregazione costi per ente pagante

**Output:**
- Report finanziari
- Rendiconti economici

---

## Configurazione

### File XMLSettings.cs

La configurazione centralizzata viene gestita tramite file XML. Questo file definisce:

- Percorsi di input/output
- Parametri di generazione report
- Impostazioni di formattazione
- Connessioni a database (se applicabile)

### Struttura Configurazione (Esempio)

```xml
<?xml version="1.0" encoding="utf-8"?>
<Configuration>
    <ReportSettings>
        <OutputPath>C:\Reports\</OutputPath>
        <TemplatesPath>C:\Templates\</TemplatesPath>
        <LogPath>C:\Logs\</LogPath>
    </ReportSettings>
    
    <ExcelSettings>
        <DefaultFont>Calibri</DefaultFont>
        <DefaultFontSize>11</DefaultFontSize>
        <PageFormat>A4</PageFormat>
        <Orientation>Portrait</Orientation>
    </ExcelSettings>
    
    <DataSettings>
        <DateFormat>dd/MM/yyyy</DateFormat>
        <NumberFormat>###,##0.00</NumberFormat>
        <CurrencyFormat>€ #,##0.00</CurrencyFormat>
    </DataSettings>
</Configuration>
```

### Come Configurare

1. **Modifica il file XML** in base alle esigenze
2. **XMLSettings.cs** leggerà automaticamente i parametri
3. **Manager.cs** utilizzerà le impostazioni durante la generazione

### Variabili di Ambiente Supportate

Oltre al file XML, è possibile utilizzare variabili di ambiente:

```bash
set VIVICARE_REPORT_OUTPUT=C:\CustomReports\
set VIVICARE_REPORT_TEMPLATES=C:\CustomTemplates\
```

---

## Utilizzo

### Scenario 1: Generazione Report ADI via UI

1. Avviare `DO.VIVICARE.UI.exe`
2. Selezionare "ADI Alta Intensità"
3. Caricare i dati (CSV, Excel o DB)
4. Configurare parametri (periodo, struttura, etc.)
5. Cliccare "Genera Report"
6. Il file Excel sarà salvato nella cartella configurata

### Scenario 2: Generazione Programmatica

```csharp
using DO.VIVICARE.Reporter;

// Inizializzare il Manager
var manager = new Manager();

// Caricare configurazione
var settings = XMLSettings.Load("config.xml");

// Preparare dati
var adiData = new ADIAltaIntensitaData { /* ... */ };

// Generare report
var outputPath = manager.GenerateADIReport(adiData, settings);

Console.WriteLine($"Report generato: {outputPath}");
```

### Scenario 3: Batch Processing

```csharp
// Generare più report in batch
var files = Directory.GetFiles(@"C:\InputData", "*.csv");

foreach (var file in files)
{
    var data = Parser.ParseCSV(file);
    manager.GenerateReport(data, settings);
}
```

---

## Tecnologie

### Linguaggi e Framework
- **Linguaggio**: C# 6.0+ (.NET Framework 4.6.1+)
- **Framework**: .NET Framework 4.8 (consigliato)
- **Paradigmi**: Object-Oriented Programming (OOP)

### Librerie Principali
- **EPPlus 4.5.x**: Generazione e manipolazione file Excel senza dipendenza da Office
- **System.Xml**: Parsing configurazione XML
- **System.Data**: Accesso dati (se connessione DB)

### Strumenti di Sviluppo
- Visual Studio 2019 Community Edition (o superiore)
- Git 2.30+ per version control
- NuGet Package Manager

---

## Roadmap

### Breve Termine (Q1-Q2 2026)
- [ ] Aggiornamento a .NET 6+ (da .NET Framework)
- [ ] Implementazione di test unitari (xUnit)
- [ ] Aggiunta documentazione API (XML comments)
- [ ] Setup CI/CD con GitHub Actions

### Medio Termine (Q3-Q4 2026)
- [ ] Migrazione database (da XML a database relazionale)
- [ ] API REST per generazione report
- [ ] Dashboard web per monitoraggio
- [ ] Support per formati output aggiuntivi (PDF, JSON)

### Lungo Termine (2027)
- [ ] Machine Learning per previsioni prestazioni
- [ ] Integration con sistemi sanitari nazionali
- [ ] Mobile app per accesso report
- [ ] Conformità GDPR completa

---

## Contribuire

### Come Contribuire

1. **Fork** il repository
2. Creare un **feature branch**: `git checkout -b feature/nuova-funzionalita`
3. **Commit** i cambiamenti: `git commit -m 'Add: descrizione feature'`
4. **Push** al branch: `git push origin feature/nuova-funzionalita`
5. Aprire una **Pull Request**

### Linee Guida

- Seguire [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Nomi descrittivi per variabili e metodi
- Aggiungere commenti per logica complessa
- Test per nuove funzionalità
- Aggiornare documentazione se necessario

### Branch Strategy

```
master          [Produzione]
 ├── develop    [Sviluppo]
 │   ├── feature/ADI-report
 │   ├── feature/PDF-export
 │   └── bugfix/excel-formatting
 ```

---

## Licenza

**[Specifica la tua licenza]**  
Esempi:
- MIT License
- Apache 2.0
- GPL 3.0
- Proprietaria

---

## Contatti e Supporto

### Autore Principale
- **Nome**: Marco Cavallo
- **Email**: mcavallo@welol.it
- **GitHub**: [@artcava](https://github.com/artcava)
- **Sito Web**: [artcava.net](https://artcava.net/)

### Segnalazione Bug

Se trovi un bug:

1. Vai su [Issues](https://github.com/artcava/DO.VIVICARE.Reporting/issues)
2. Clicca "New Issue"
3. Descrivi il problema in dettaglio
4. Allega screenshot o log se possibile
5. Specifica versione .NET Framework e OS

### Discussioni e Feature Request

Per suggerimenti e discussioni:

1. Vai su [Discussions](https://github.com/artcava/DO.VIVICARE.Reporting/discussions)
2. Crea una nuova discussione
3. Descrivi l'idea o il miglioramento

---

## Changelog

### v1.0.0 - Maggio 2024
- ✅ Migrazione da Azure DevOps a GitHub
- ✅ Core reporting engine stabile
- ✅ 14 moduli Document implementati
- ✅ 3 moduli Report specializzati
- ✅ Interfaccia UI funzionante

---

## Risorse Esterne

- [EPPlus Documentation](https://github.com/EPPlusSharp/EPPlus)
- [C# Documentation](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [.NET Framework Guide](https://docs.microsoft.com/en-us/dotnet/framework/)
- [Normative Sanitarie Italiane](https://www.salute.gov.it/)

---

**Ultima modifica**: 11 Gennaio 2026  
**Mantenuto da**: Marco Cavallo
