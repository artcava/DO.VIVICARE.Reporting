# DO.VIVICARE Reporting

Advanced report and document generation system for Italian healthcare facilities. This project manages the automated creation of complex Excel reports for integrated home care (ADI), health services and administrative accounting.

## 📋 Table of Contents

- [Overview](#overview)
- [Key Features](#key-features)
- [Quick Start](#quick-start)
- [System Requirements](#system-requirements)
- [Project Architecture](#project-architecture)
- [Technology Stack](#technology-stack)
- [Documentation](#documentation)
- [Contributing](#contributing)
- [Support](#support)
- [License](#license)

---

## Overview

DO.VIVICARE Reporting is a comprehensive solution for managing and generating healthcare reports in compliance with Italian regulations. The system specializes in creating:

- **ADI Reports** (Integrated Home Care) - High and Low Intensity
- **Administrative Reports** - Cost and service tracking
- **Reports for Local Entities** - Municipalities, ASST, Ministry of Health
- **Specialized Attachments** - Dietetic, valorization, invoicing
- **Legislative Reports** - Report 16 and Report 18

### Key Features

✅ **Modular Architecture** - Each healthcare specialty in dedicated module  
✅ **Advanced Excel Generation** - Complex formatting, formulas and layouts  
✅ **Centralized Configuration** - Runtime settings via XML  
✅ **Separation of Concerns** - Document and Report layers are independent  
✅ **Extensibility** - Easy addition of new reports and specialties  

---

## Quick Start

### Installation

1. **Clone the Repository**
   ```bash
   git clone https://github.com/artcava/DO.VIVICARE.Reporting.git
   cd DO.VIVICARE.Reporting
   ```

2. **Open Solution**
   ```bash
   # With Visual Studio
   start DO.VIVICARE.Reporting.sln
   ```

3. **Restore NuGet Dependencies**
   ```bash
   nuget restore DO.VIVICARE.Reporting.sln
   ```

4. **Build Solution**
   - Menu: Build → Build Solution (Ctrl+Shift+B)
   - Or via MSBuild:
     ```bash
     msbuild DO.VIVICARE.Reporting.sln /p:Configuration=Release
     ```

### First Report Generation

```csharp
using DO.VIVICARE.Reporter;

// Initialize Manager
var manager = new Manager();

// Load configuration
var settings = XMLSettings.Load("config.xml");

// Prepare data
var adiData = new ADIAltaIntensitaData { /* ... */ };

// Generate report
var outputPath = manager.GenerateADIReport(adiData, settings);
Console.WriteLine($"Report generated: {outputPath}");
```

---

## System Requirements

### Operating Systems
- Windows 7 SP1 or higher
- Windows Server 2008 R2 or higher

### Software
- **.NET Framework 4.6.1 or higher** (4.8 recommended)
- **Visual Studio 2019 or higher** (for development)
- **Microsoft Excel** (optional, for output viewing)

### Hardware
- Processor: x86/x64 compatible
- RAM: 2 GB minimum (4 GB recommended)
- Disk Space: ~500 MB for complete solution

### Dependencies
- **EPPlus 4.5.x** - Excel generation library
- See `packages.config` for other module-specific dependencies

---

## Project Architecture

### Overall Structure

```
DO.VIVICARE.Reporting/
├── DO.VIVICARE.Reporter/          [CORE ENGINE]
│   ├── Manager.cs                 # Report generation orchestration
│   ├── Model.cs                   # Shared data models
│   ├── ExcelManager.cs            # Excel formatting & formulas
│   └── XMLSettings.cs             # XML configuration
│
├── DO.VIVICARE.UI/                [USER INTERFACE]
│
├── DO.VIVICARE.Document.*/        [DOCUMENT LAYER - 14 MODULES]
│   ├── ADIAltaIntensita/          # High-intensity home care
│   ├── ADIBassaIntensita/         # Low-intensity home care
│   ├── ASST/                      # Regional health organizations
│   ├── Comuni/                    # Local entities reports
│   ├── MinSan/                    # Ministry of Health
│   ├── Prestazioni/               # Healthcare services
│   ├── Prezzi/                    # Price catalog
│   ├── Rendiconto/                # Administrative accounting
│   ├── Report16/                  # Legislative Report 16
│   ├── Report18/                  # Legislative Report 18
│   ├── Valorizzazione/            # Service valorization
│   ├── ValorizzazioniADIAlta/     # ADI High valorization
│   ├── ZSDFatture/                # Invoicing management
│   └── LazioHealthWorker/         # Lazio-specific features
│
├── DO.VIVICARE.Report.*/          [REPORT LAYER - 3 MODULES]
│   ├── AllegatoADI/               # ADI report logic
│   ├── Dietetica/                 # Dietetic reports
│   └── Valorizzazione/            # Financial reports
│
└── Example/                        [USAGE EXAMPLES]
```

### Architectural Flow

```
┌─────────────────────────────┐
│   UI Layer                  │  User parameter collection
│   DO.VIVICARE.UI            │
└──────────────┬──────────────┘
               │
┌──────────────────────────────────┐
│   Business Logic Layer           │  Manager orchestration
│   DO.VIVICARE.Reporter           │
└──────────────┬───────────────────┘
               │
       ┌───────┴────────┐
       ▼                ▼
┌────────────┐   ┌──────────────┐
│ Document   │   │ Configuration│
│ Modules    │   │ (XML)        │
└────────────┘   └──────────────┘
       │
       ▼
┌─────────────────────────────┐
│ Excel Generation            │  Excel file formatting
│ ExcelManager.cs             │
└──────────────┬──────────────┘
               │
               ▼
        ┌─────────────┐
        │ .xlsx File  │
        └─────────────┘
```

### Core Components

#### DO.VIVICARE.Reporter
Central orchestration engine that coordinates report generation across all modules.

#### Document Modules (14)
Define data structures specific to each healthcare domain with validation and transformation capabilities.

#### Report Modules (3)
Implement generation logic, combining document data into formatted Excel output:
- **AllegatoADI** - ADI attachments from high/low intensity data
- **Dietetica** - Dietary and nutritional reports
- **Valorizzazione** - Financial aggregation and reporting

---

## Technology Stack

### Languages & Frameworks
- **Language**: C# 6.0+ (.NET Framework 4.6.1+)
- **Framework**: .NET Framework 4.8 (recommended)

### Key Libraries
- **EPPlus 4.5.x** - Excel generation without Office dependency
- **System.Xml** - XML configuration parsing
- **System.Data** - Data access (if database connected)

### Development Tools
- Visual Studio 2019 Community Edition or higher
- Git 2.30+ for version control
- NuGet Package Manager

---

## Documentation

Complete documentation is available in separate guides:

| Document | Purpose |
|----------|---------|
| [INSTALLATION.md](INSTALLATION.md) | Setup, installation and plugin management |
| [ARCHITECTURE.md](ARCHITECTURE.md) | Technical architecture and design patterns |
| [DEPLOYMENT.md](DEPLOYMENT.md) | Release process and versioning |
| [CONTRIBUTING.md](CONTRIBUTING.md) | Development guidelines and contribution process |
| [ROADMAP.md](ROADMAP.md) | Future development plans |
| [MIGRATION.md](MIGRATION.md) | Migration from previous systems |
| [TROUBLESHOOTING.md](TROUBLESHOOTING.md) | Common issues and solutions |
| [PLUGIN_MANAGER.md](PLUGIN_MANAGER.md) | Plugin management guide |

---

## Contributing

### How to Contribute

1. **Fork** the repository
2. Create **feature branch**: `git checkout -b feature/new-feature`
3. **Commit** changes: `git commit -m 'Add: feature description'`
4. **Push** to branch: `git push origin feature/new-feature`
5. Open **Pull Request**

### Guidelines

- Follow [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use descriptive names for variables and methods
- Add comments for complex logic
- Write tests for new features
- Update documentation if needed

---

## Support

### Report Issues

1. Go to [GitHub Issues](https://github.com/artcava/DO.VIVICARE.Reporting/issues)
2. Click "New Issue"
3. Describe the problem with:
   - Clear description
   - Steps to reproduce
   - Expected vs actual behavior
   - .NET Framework version and OS

### Ask Questions

Use [GitHub Discussions](https://github.com/artcava/DO.VIVICARE.Reporting/discussions) for:
- Feature requests
- General questions
- Architecture discussions
- Enhancement proposals

### Contact

- **Author**: Marco Cavallo
- **Email**: mcavallo@welol.it
- **GitHub**: [@artcava](https://github.com/artcava)

---

## License

Specify your license here. Examples:
- MIT License
- Apache 2.0
- GPL 3.0
- Proprietary

---

## Changelog

### v1.0.0 - May 2024
- ✅ Migration from Azure DevOps to GitHub
- ✅ Stable core reporting engine
- ✅ 14 Document modules implemented
- ✅ 3 specialized Report modules
- ✅ Functional UI interface

---

**Last Updated**: January 13, 2026  
**Maintained by**: Marco Cavallo
