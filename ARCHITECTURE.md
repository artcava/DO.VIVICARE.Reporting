# Architettura DO.VIVICARE Reporting

Documento dettagliato sull'architettura tecnica del sistema.

## Indice

1. [Principi Architetturali](#principi-architetturali)
2. [Layers Applicativi](#layers-applicativi)
3. [Descrizione Dettagliata Componenti](#descrizione-dettagliata-componenti)
4. [Flussi di Dati](#flussi-di-dati)
5. [Design Patterns](#design-patterns)
6. [Dependency Graph](#dependency-graph)
7. [Estensibilità](#estensibilità)
8. [Performance e Scalabilità](#performance-e-scalabilità)

---

## Principi Architetturali

### 1. Separazione dei Livelli (Layered Architecture)

Il sistema è organizzato in tre layer distinti:

```
┌─────────────────────────────┐
│   PRESENTATION LAYER        │
│   DO.VIVICARE.UI            │
│   (WinForms / WPF UI)       │
└──────────────┬──────────────┘
               │
┌──────────────────────────────────┐
│   BUSINESS LOGIC LAYER           │
│   DO.VIVICARE.Reporter (Manager) │
│   - Orchestrazione generazione   │
│   - Logica di business           │
│   - Coordinamento moduli         │
└──────────────┬───────────────────┘
               │
               │ Comunica con
               │
┌─────────────────────────────┐
│   DATA LAYER                │
│   Document + Report Modules │
│   - Strutturazione dati     │
│   - Mapping entità          │
│   - Validazione             │
└─────────────────────────────┘
```

**Benefici:**
- Testabilità: Ogni layer può essere testato indipendentemente
- Manutenibilità: Modifiche isolate per ogni layer
- Estensibilità: Facile aggiungere nuovi moduli
- Riusabilità: Layer condivisi tra report diversi

### 2. Modularità

Ciascun dominio sanitario ha un modulo dedicato.

**Vantaggi:**
- Team paralleli possono lavorare su moduli diversi
- Bug isolati in modulo specifico
- Deployment selettivo di moduli
- Versionamento indipendente

### 3. Configurazione Centralizzata

Tutti i parametri sono gestiti centralmente via XML.

**Vantaggi:**
- Cambio parametri senza ricompilazione
- Facile deployment su diversi ambienti
- Audit trail delle configurazioni
- Versionamento della configurazione (tramite Git)

---

## Layers Applicativi

### Presentation Layer (DO.VIVICARE.UI)

**Responsabilità:**
- Interfaccia utente (WinForms/WPF)
- Raccolta parametri dall'utente
- Validazione input UI-side
- Visualizzazione risultati e stato

**Tecnologie:**
- Windows Forms o WPF
- .NET Framework 4.6.1+

**Componenti Principali:**
- Forms per selezione tipo report
- Data entry form per parametri
- Progress indicator durante generazione
- Message boxes per feedback

**Non conosce:**
- Dettagli di generazione Excel
- Struttura DB (se presente)
- Logica di report complessa

### Business Logic Layer (DO.VIVICARE.Reporter)

**Responsabilità:**
- Orchestrazione della generazione report
- Validazione business rules
- Coordinamento tra moduli
- Gestione stato applicazione

### Data Layer (Document + Report Modules)

**Responsabilità:**
- Rappresentazione dati specifica per dominio
- Mapping tra sorgenti esterne e modello interno
- Validazione dei dati
- Trasformazione per generazione report

**Moduli Document** (~50 in totale tra i 14 progetti)

**Moduli Report** (Logica di generazione)

---

## Descrizione Dettagliata Componenti

### DO.VIVICARE.Reporter

**Scopo**: Core engine di generazione report

**Contenuti**:
- `Manager.cs` - Orchestrazione
- `Model.cs` - Modelli dati condivisi
- `ExcelManager.cs` - Gestione Excel
- `XMLSettings.cs` - Configurazione
- `Properties/` - Metadati assembly
- `packages.config` - Dipendenze NuGet
- `app.config` - Configurazione app

**Dipendenze**:
- EPPlus 4.5.x (generazione Excel)
- System.Xml (parsing XML)
- System.Data (se DB)

### DO.VIVICARE.UI

**Scopo**: Interfaccia utente per generazione interattiva

**Funzionalità:**
- Selezione tipo report
- Data entry parametri
- Visualizzazione progress
- Apertura file output

**Riferimenti**:
- DO.VIVICARE.Reporter (per logica)
- System.Windows.Forms (per UI)

### Moduli Document (14 in totale)

**Struttura comune:**
```
DO.VIVICARE.Document.[NomeDominio]/
├── [NomeDominio].cs          # Classe principale
├── *.csproj                   # Configurazione progetto
├── app.config                 # Configurazione runtime
└── Properties/                # Metadati
```

**Responsabilità comuni:**
- Definire strutture dati specifiche
- Validazione dati di dominio
- Mapping da sorgenti esterne
- Export in formati per report

### Moduli Report (3 in totale)

**Specializzazioni:**

1. **AllegatoADI**: Combina ADIAltaIntensita + ADIBassaIntensita
2. **Dietetica**: Gestisce dati nutrizionali
3. **Valorizzazione**: Aggregazione economica

---

## Flussi di Dati

### Flusso 1: Generazione Report ADI

```
1. UI raccoglie parametri utente
   │ (data inizio, data fine, struttura, etc.)
   │
2. Manager valida parametri
   │
3. Manager carica XMLSettings
   │
4. Manager istanzia ADIAltaIntensita module
   │
5. ADIAltaIntensita carica/trasforma dati
   │
6. Manager istanzia AllegatoADIReport
   │
7. AllegatoADIReport genera worksheet con:
      - Intestazioni
      - Dati pazienti
      - Aggregazioni
      - Formule di calcolo
      - Grafici
   │
8. ExcelManager formatta e protegge
   │
9. ExcelManager salva file .xlsx
   │
10. UI apre file risultante
```

### Flusso 2: Generazione Report Batch

```
1. Applicazione console legge lista di report da generare
2. Per ogni report:
   a. Carica configurazione
   b. Carica dati
   c. Invoca Manager.GenerateReport()
   d. Salva risultato
   e. Log risultato
3. Riepilogo risultati (successi/errori)
```

---

## Design Patterns

### 1. Factory Pattern

```csharp
public class ReportFactory
{
    public static IReport CreateReport(ReportType type)
    {
        switch(type)
        {
            case ReportType.ADI:
                return new AllegatoADIReport();
            case ReportType.Dietetica:
                return new DieticaReport();
            case ReportType.Valorizzazione:
                return new ValorizzazioneReport();
            default:
                throw new ArgumentException();
        }
    }
}
```

**Vantaggio**: Isolamento della logica di creazione

### 2. Strategy Pattern

```csharp
public interface IExcelFormatter
{
    void Format(ExcelPackage package, FormattingStrategy strategy);
}

public class StandardFormatter : IExcelFormatter { }
public class AlternateFormatter : IExcelFormatter { }
```

**Vantaggio**: Switching dinamico tra strategie di formattazione

### 3. Builder Pattern

```csharp
public class ReportBuilder
{
    public ReportBuilder WithData(ReportData data) { /* ... */ }
    public ReportBuilder WithFormatting(ExcelFormatting format) { /* ... */ }
    public ReportBuilder WithCharts() { /* ... */ }
    public ExcelPackage Build() { /* ... */ }
}
```

**Vantaggio**: Costruzione step-by-step di report complessi

### 4. Singleton Pattern

```csharp
public class XMLSettings
{
    private static XMLSettings _instance;
    private static readonly object _lock = new object();
    
    public static XMLSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = Load(ConfigPath);
                }
            }
            return _instance;
        }
    }
}
```

**Vantaggio**: Unica istanza di configurazione in memoria

---

## Dependency Graph

```
DO.VIVICARE.UI
  └── DO.VIVICARE.Reporter
        ├── DO.VIVICARE.Document.* (selettivi)
        ├── DO.VIVICARE.Report.* (selettivi)
        ├── EPPlus
        └── System.*

DO.VIVICARE.Report.*
  └── DO.VIVICARE.Document.* (dipendenza specifica)

DO.VIVICARE.Document.*
  └── System.*
```

**Regola d'Oro**: Layer superiore può dipendere da layer inferiore, MA NON viceversa.

---

## Estensibilità

### Come Aggiungere un Nuovo Report

1. **Creare modulo Document**:
   ```
   DO.VIVICARE.Document.NuovoDominio/
   ├── NuovoDominio.cs
   ├── DO.VIVICARE.Document.NuovoDominio.csproj
   └── Properties/
   ```

2. **Creare modulo Report**:
   ```
   DO.VIVICARE.Report.NuovoDominio/
   ├── NuovoDominioReport.cs (implementa IReport)
   ├── DO.VIVICARE.Report.NuovoDominio.csproj
   └── Properties/
   ```

3. **Implementare interfaccia IReport**:
   ```csharp
   public interface IReport
   {
       ExcelWorksheet GenerateReport(IEnumerable<T> data);
   }
   ```

4. **Registrare in ReportFactory**:
   ```csharp
   case ReportType.NuovoDominio:
       return new NuovoDominioReport();
   ```

5. **Aggiornare UI** con nuova opzione

### Come Aggiungere Nuovo Formato Output

Creare interfaccia adapter:

```csharp
public interface IOutputWriter
{
    void Write(ReportData data, string outputPath);
}

public class PDFWriter : IOutputWriter { /* ... */ }
public class JSONWriter : IOutputWriter { /* ... */ }
```

---

## Performance e Scalabilità

### Considerazioni Attuali

**Colli di bottiglia noti:**
- Generazione Excel grande (>100MB) può essere lenta
- Caricamento dati da file CSV su dischi lenti
- Formattazione condizionale su grande numero di righe

### Strategie di Ottimizzazione

1. **Chunking dei Dati**:
   - Processare dati in batch di 10K righe
   - Scrivere incrementalmente su file

2. **Async Operations** (futuro):
   ```csharp
   public async Task<string> GenerateReportAsync(
       ReportType type,
       ReportData data)
   {
       return await Task.Run(() => GenerateReport(type, data));
   }
   ```

3. **Caching**:
   - Cache configurazione (già fatto con XMLSettings)
   - Cache formule ricorrenti
   - Cache aggregazioni intermedie

4. **Multithreading**:
   - Processing parallelo di moduli indipendenti
   - Uso di Parallel.ForEach per loop-heavy

### Benchmark Target

- **Report piccolo** (<5K righe): <5 secondi
- **Report medio** (5-50K righe): <30 secondi
- **Report grande** (50K+ righe): <2 minuti

---

## Roadmap Architetturale

### Phase 1: Modernizzazione (.NET 6+)
- Migrazione da Framework
- Async/await patterns
- Dependency Injection container

### Phase 2: API Layer
- REST API per generazione
- gRPC per alta performance
- GraphQL per query flessibili

### Phase 3: Database
- Sostituzione file/XML con DB relazionale
- ORM (Entity Framework Core)
- Migrazioni versionate

### Phase 4: Cloud
- Serverless functions (Azure Functions)
- Scalabilità orizzontale
- Multi-tenancy support

---

**Documento aggiornato**: 13 Gennaio 2026
