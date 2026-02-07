# Architecture - DO.VIVICARE Reporting

Detailed technical architecture and design of the DO.VIVICARE Reporting system.

## Table of Contents

1. [Architectural Principles](#architectural-principles)
2. [Layered Architecture](#layered-architecture)
3. [Core Components](#core-components)
4. [Data Flow](#data-flow)
5. [Design Patterns](#design-patterns)
6. [Dependency Graph](#dependency-graph)
7. [Extensibility](#extensibility)
8. [Performance Considerations](#performance-considerations)

---

## Architectural Principles

### 1. Separation of Concerns (Layered Architecture)

The system is organized into three distinct layers:

```
┌─────────────────────────────┐
│   PRESENTATION LAYER          │
│   DO.VIVICARE.UI              │
│   (WinForms UI Components)    │
└────────────┬───────────────────┘
               │
┌──────────────────────────────────┐
│   BUSINESS LOGIC LAYER           │
│   DO.VIVICARE.Reporter (Manager) │
│   - Orchestration                │
│   - Coordination                 │
└────────────┬───────────────────┘
               │
┌─────────────────────────────┐
│   DATA LAYER                  │
│   Document + Report Modules   │
│   - Data structures           │
│   - Validation                │
└─────────────────────────────┘
```

**Benefits:**
- **Testability**: Each layer independently testable
- **Maintainability**: Isolated changes per layer
- **Extensibility**: Easy to add new modules
- **Reusability**: Shared layers across report types

### 2. Modularity

Each healthcare domain has a dedicated module:
- Independent team development
- Isolated bug fixes
- Selective deployment
- Independent versioning

### 3. Centralized Configuration

All parameters managed via XML:
- Configuration without recompilation
- Environment-specific settings
- Audit trail via Git
- Version control integration

---

## Layered Architecture

### Presentation Layer (DO.VIVICARE.UI)

**Responsibilities:**
- User interface (WinForms)
- Parameter collection
- Input validation (UI-side)
- Results visualization

**Technologies:**
- Windows Forms
- .NET Framework 4.6.1+

**Key Components:**
- Report type selection forms
- Parameter entry dialogs
- Progress indicators
- Plugin Manager interface

**Does NOT know about:**
- Excel generation details
- Database structures
- Complex report logic

### Business Logic Layer (DO.VIVICARE.Reporter)

**Responsibilities:**
- Report generation orchestration
- Business rule validation
- Module coordination
- State management

**Core Files:**
- `Manager.cs` - Main orchestration
- `Model.cs` - Shared data models
- `ExcelManager.cs` - Excel operations
- `XMLSettings.cs` - Configuration loader

**Key Methods:**
```csharp
public class Manager
{
    public string GenerateADIReport(ReportData data, XMLSettings settings);
    public string GenerateReport(ReportType type, ReportData data);
    public ExcelPackage FormatReport(ExcelPackage package, FormatSettings settings);
}
```

### Data Layer (Document + Report Modules)

**Responsibilities:**
- Domain-specific data structures
- External source mapping
- Data validation
- Transformation for reporting

#### Document Modules (14 total)

Each module defines:
- Data structures for specific healthcare domain
- Validation rules
- Mapping from external sources
- Export capabilities

**Example: ADIAltaIntensita Module**
```
DO.VIVICARE.Document.ADIAltaIntensita/
├─ ADIAltaIntensita.cs         # Main data class
├─ Validation.cs              # Domain validation
├─ Mapping.cs                 # External data mapping
└─ Properties/                # Assembly metadata
```

#### Report Modules (3 total)

Each module implements report generation:

1. **AllegatoADI** - Combines high/low intensity ADI data
2. **Dietetica** - Dietary and nutritional reports
3. **Valorizzazione** - Financial aggregation reports

---

## Core Components

### DO.VIVICARE.Reporter

Central orchestration engine:
- Coordinates report generation across modules
- Manages configuration
- Handles Excel operations
- Provides standardized interfaces

### DO.VIVICARE.UI

User interface:
- WinForms application
- Report selection and configuration
- Plugin Manager integration
- Result display and file management

### Document Modules (14)

Data layer specifications:
- ADIAltaIntensita - High intensity home care data
- ADIBassaIntensita - Low intensity home care data
- ASST - Regional health organization data
- Comuni - Local municipality reports
- LazioHealthWorker - Lazio-specific worker data
- MinSan - Ministry of Health data
- Prestazioni - Healthcare services catalog
- Prezzi - Service pricing data
- Rendiconto - Administrative accounting
- Report16, Report18 - Legislative reports
- Valorizzazione - Service valorization
- ValorizzazioniADIAlta - ADI high valorization
- ZSDFatture - Invoicing data

### Report Modules (3)

Report generation logic:
- AllegatoADI - ADI attachment reports
- Dietetica - Dietary/nutritional reports
- Valorizzazione - Financial reports

---

## Data Flow

### Typical Report Generation Flow

```
1. UI collects user parameters
   └─ Date range, facility, report type, etc.

2. Manager validates parameters

3. Manager loads XMLSettings

4. Manager instantiates Document module
   └─ ADIAltaIntensita, etc.

5. Document module loads/transforms data

6. Manager instantiates Report module
   └─ AllegatoADIReport, etc.

7. Report module generates Excel worksheet
   └─ Headers, data, formulas, charts

8. ExcelManager formats and protects

9. ExcelManager saves .xlsx file

10. UI opens and displays result
```

### Data Transformation Pipeline

```
External Data
   │
   ▼
Document Module (Parsing)
   │
   ▼
Internal Data Model
   │
   ▼
Report Module (Aggregation)
   │
   ▼
Excel Worksheet (Formatting)
   │
   ▼
.xlsx File (Output)
```

---

## Design Patterns

### 1. Factory Pattern

Creating reports based on type:

```csharp
public class ReportFactory
{
    public static IReport CreateReport(ReportType type)
    {
        return type switch
        {
            ReportType.ADI => new AllegatoADIReport(),
            ReportType.Dietetica => new DieticaReport(),
            ReportType.Valorizzazione => new ValorizzazioneReport(),
            _ => throw new ArgumentException()
        };
    }
}
```

**Benefit**: Isolates report creation logic

### 2. Strategy Pattern

Different formatting strategies:

```csharp
public interface IExcelFormatter
{
    void Format(ExcelPackage package, FormattingStrategy strategy);
}

public class StandardFormatter : IExcelFormatter { }
public class AlternateFormatter : IExcelFormatter { }
```

**Benefit**: Switch formatting strategies at runtime

### 3. Builder Pattern

Step-by-step report construction:

```csharp
public class ReportBuilder
{
    public ReportBuilder WithData(ReportData data) { /* ... */ }
    public ReportBuilder WithFormatting(ExcelFormat format) { /* ... */ }
    public ReportBuilder WithCharts() { /* ... */ }
    public ExcelPackage Build() { /* ... */ }
}
```

**Benefit**: Flexible complex object construction

### 4. Singleton Pattern

Single configuration instance:

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

**Benefit**: Single configuration instance in memory

---

## Dependency Graph

```
DO.VIVICARE.UI
  └─ DO.VIVICARE.Reporter
        ├─ DO.VIVICARE.Document.* (selected)
        ├─ DO.VIVICARE.Report.* (selected)
        ├─ EPPlus 4.5.x
        └─ System.*

DO.VIVICARE.Report.*
  └─ DO.VIVICARE.Document.* (specific)

DO.VIVICARE.Document.*
  └─ System.*
```

**Golden Rule**: Upper layers can depend on lower layers, but NOT vice versa.

---

## Extensibility

### Adding a New Report Type

1. **Create Document Module**
   ```
   DO.VIVICARE.Document.NewDomain/
   ├─ NewDomain.cs
   ├─ Validation.cs
   ├─ Mapping.cs
   └─ Properties/
   ```

2. **Create Report Module**
   ```
   DO.VIVICARE.Report.NewDomain/
   ├─ NewDomainReport.cs (implements IReport)
   └─ Properties/
   ```

3. **Implement IReport Interface**
   ```csharp
   public interface IReport
   {
       ExcelWorksheet GenerateReport(IEnumerable<dynamic> data);
   }
   ```

4. **Register in Factory** (if needed)
   ```csharp
   case ReportType.NewDomain:
       return new NewDomainReport();
   ```

5. **Update UI** with new option

### Adding a New Output Format

Create adapter interface:

```csharp
public interface IOutputWriter
{
    void Write(ReportData data, string outputPath);
}

public class PDFWriter : IOutputWriter { /* ... */ }
public class JSONWriter : IOutputWriter { /* ... */ }
```

---

## Performance Considerations

### Known Bottlenecks

- Large Excel files (>100MB) generation can be slow
- CSV file loading on slow disks
- Conditional formatting on large datasets

### Optimization Strategies

**1. Data Chunking**
- Process data in batches of 10K rows
- Write incrementally to file

**2. Async Operations** (future)
```csharp
public async Task<string> GenerateReportAsync(
    ReportType type,
    ReportData data)
{
    return await Task.Run(() => GenerateReport(type, data));
}
```

**3. Caching**
- Configuration caching (already implemented)
- Formula caching
- Aggregation caching

**4. Parallelization**
- Process independent modules in parallel
- Use Parallel.ForEach for data loops

### Performance Targets

- **Small report** (<5K rows): <5 seconds
- **Medium report** (5-50K rows): <30 seconds
- **Large report** (50K+ rows): <2 minutes

---

## Future Enhancements

### Phase 1: Modernization (.NET 6+)
- Migration from Framework to .NET 6+
- Async/await patterns
- Dependency Injection container

### Phase 2: API Layer
- REST API for report generation
- gRPC for high-performance scenarios
- GraphQL for flexible queries

### Phase 3: Database
- Replace XML with relational database
- Entity Framework Core ORM
- Versioned migrations

### Phase 4: Cloud
- Serverless functions (Azure Functions)
- Horizontal scalability
- Multi-tenancy support

---

**Last Updated**: January 13, 2026  
**Maintained by**: Marco Cavallo
