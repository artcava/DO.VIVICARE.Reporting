# Linee Guida per Contribuire

Grazie per l'interesse nel contribuire a DO.VIVICARE Reporting! Questo documento descrive le modalità e le regole per contribuire al progetto.

## Indice

1. [Codice di Condotta](#codice-di-condotta)
2. [Come Iniziare](#come-iniziare)
3. [Processo di Contribuzione](#processo-di-contribuzione)
4. [Linee Guida di Codice](#linee-guida-di-codice)
5. [Commit e Pull Request](#commit-e-pull-request)
6. [Testing](#testing)
7. [Documentazione](#documentazione)
8. [Risoluzione Conflitti](#risoluzione-conflitti)

---

## Codice di Condotta

### Il Nostro Impegno

Nel nostro progetto, ci impegniamo a fornire un ambiente accogliente per tutti, indipendentemente da:
- Età
- Taglia del corpo
- Disabilità
- Etnia
- Identità di genere
- Livello di esperienza
- Nazionalità
- Apparenza
- Razza
- Religione
- Identità e orientamento sessuale

### Comportamenti Attesi

✅ Usa un linguaggio accogliente e inclusivo
✅ Sii rispettoso verso opinioni differenti
✅ Accetta critiche costruttive con eleganza
✅ Focalizzati su ciò che è meglio per la comunità
✅ Mostra empatia verso altri membri della comunità

### Comportamenti Non Accettati

❌ Linguaggio o immagini sessuali
❌ Commenti offensivi o derisori
❌ Attacchi personali o politici
❌ Molestie pubbliche o private
❌ Pubblicazione di dati privati senza consenso
❌ Altra condotta ragionevolmente ritenuta inappropriata

### Segnalazione

Se riscontri comportamenti inaccettabili:
1. Contatta il maintainer principale via email
2. Fornisci dettagli e prove se disponibili
3. Mantieni la confidenzialità

---

## Come Iniziare

### Prerequisiti

- Git 2.30+
- Visual Studio 2019 Community Edition (o superiore)
- .NET Framework 4.6.1 (o superiore)
- Account GitHub

### Setup Ambiente

1. **Fork il Repository**
   - Vai a https://github.com/artcava/DO.VIVICARE.Reporting
   - Clicca "Fork" in alto a destra
   - Verrà creato: https://github.com/YOUR-USERNAME/DO.VIVICARE.Reporting

2. **Clone il Tuo Fork**
   ```bash
   git clone https://github.com/YOUR-USERNAME/DO.VIVICARE.Reporting.git
   cd DO.VIVICARE.Reporting
   ```

3. **Aggiungi Repository Upstream**
   ```bash
   git remote add upstream https://github.com/artcava/DO.VIVICARE.Reporting.git
   ```

4. **Crea Branch Locale**
   ```bash
   git fetch upstream
   git checkout -b feature/descrizione-feature upstream/master
   ```

5. **Ripristina Dipendenze**
   ```bash
   nuget restore DO.VIVICARE.Reporting.sln
   ```

6. **Compila la Soluzione**
   - Apri DO.VIVICARE.Reporting.sln in Visual Studio
   - Build → Build Solution (Ctrl+Shift+B)
   - Verifica assenza di errori

---

## Processo di Contribuzione

### Tipi di Contribuzioni

#### 1. Bug Report

**Quando segnalare:**
- Comportamento inaspettato
- Errori durante l'esecuzione
- Crash dell'applicazione
- Output errato

**Come segnalare:**
1. Vai su [Issues](https://github.com/artcava/DO.VIVICARE.Reporting/issues)
2. Clicca "New Issue"
3. Usa il template "Bug Report"
4. Compila tutti i campi

**Informazioni Richieste:**
```markdown
## Descrizione
[Descrizione breve del bug]

## Passi per Riprodurre
1. [Primo passo]
2. [Secondo passo]
3. [Comportamento osservato]

## Comportamento Atteso
[Cosa dovrebbe succedere]

## Ambiente
- OS: [es. Windows 10]
- Visual Studio: [es. 2019 Enterprise]
- .NET Framework: [es. 4.8]
- Versione Progetto: [es. v1.0.0]

## Log e Screenshot
[Allega log rilevanti]
[Allega screenshot]
```

#### 2. Feature Request

**Come suggerire:**
1. Vai su [Discussions](https://github.com/artcava/DO.VIVICARE.Reporting/discussions)
2. Crea "New Discussion" categoria "Idea"
3. Descrivi la feature desiderata

**Linee Guida:**
- Spiega chiaramente il caso d'uso
- Fornisci esempi se possibile
- Considera l'impatto architetturale
- Discuti pro/contro

#### 3. Pull Request

**Casi Tipici:**
- Fixing bug
- Implementazione feature
- Aggiornamento documentazione
- Miglioramenti performance
- Correzione typo

---

## Linee Guida di Codice

### Stile e Convenzioni

#### Naming Conventions

```csharp
// Classes: PascalCase
public class ReportManager { }

// Methods: PascalCase
public void GenerateReport() { }

// Parameters: camelCase
public void GenerateReport(string reportName, int pageSize) { }

// Private fields: _camelCase
private string _configPath;

// Constants: UPPER_CASE
public const string DEFAULT_OUTPUT = "Reports";

// Local variables: camelCase
var reportData = LoadData();
```

#### Code Organization

```csharp
public class ExampleClass
{
    // 1. Costanti
    private const int MaxRetries = 3;
    
    // 2. Campi statici
    private static readonly object _lockObject = new object();
    
    // 3. Campi istanza
    private string _configPath;
    private ILogger _logger;
    
    // 4. Proprietà
    public string ConfigPath 
    { 
        get { return _configPath; } 
        set { _configPath = value; } 
    }
    
    // 5. Costruttori
    public ExampleClass(string configPath)
    {
        _configPath = configPath;
    }
    
    // 6. Metodi pubblici
    public void Execute()
    {
        // Implementazione
    }
    
    // 7. Metodi privati
    private void Initialize()
    {
        // Implementazione
    }
}
```

#### Documentazione del Codice

```csharp
/// <summary>
/// Genera un report ADI per il periodo specificato.
/// </summary>
/// <param name="data">Dati ADI da processare</param>
/// <param name="settings">Impostazioni di generazione</param>
/// <returns>Percorso del file generato</returns>
/// <exception cref="ArgumentNullException">Se data o settings sono null</exception>
/// <exception cref="IOException">Se impossibile scrivere il file</exception>
public string GenerateADIReport(
    ADIReportData data,
    ReportSettings settings)
{
    if (data == null)
        throw new ArgumentNullException(nameof(data));
    
    if (settings == null)
        throw new ArgumentNullException(nameof(settings));
    
    // Implementazione
    return outputPath;
}
```

### Linee Guida Specifiche

#### 1. Error Handling

```csharp
// ❌ EVITARE: Eccezioni generiche
catch (Exception ex)
{
    MessageBox.Show("Errore!");
}

// ✅ BUONO: Eccezioni specifiche
try
{
    var report = GenerateReport(data);
}
catch (ArgumentNullException ex)
{
    _logger.Error($"Parametri invalidi: {ex.Message}");
    MessageBox.Show("I dati forniti non sono validi.", "Errore Validazione");
}
catch (IOException ex)
{
    _logger.Error($"Errore I/O: {ex.Message}");
    MessageBox.Show("Impossibile scrivere il file di output.", "Errore I/O");
}
catch (Exception ex)
{
    _logger.Error($"Errore inaspettato: {ex.Message}", ex);
    MessageBox.Show("Si è verificato un errore inaspettato.", "Errore");
}
```

#### 2. Validazione Input

```csharp
public class DataValidator
{
    public static void Validate(ReportData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
        
        if (string.IsNullOrWhiteSpace(data.ReportName))
            throw new ArgumentException("Report name cannot be empty", nameof(data.ReportName));
        
        if (data.Patients?.Count == 0)
            throw new ArgumentException("No patients found", nameof(data.Patients));
        
        if (data.EndDate < data.StartDate)
            throw new ArgumentException("End date must be after start date");
    }
}
```

#### 3. Performance

```csharp
// ❌ EVITARE: String concatenation in loop
string result = "";
foreach (var item in items)
{
    result += item.ToString(); // Inefficiente
}

// ✅ BUONO: StringBuilder
var sb = new StringBuilder();
foreach (var item in items)
{
    sb.Append(item.ToString());
}
string result = sb.ToString();

// ❌ EVITARE: LINQ inefficiente
var filtered = data.Where(x => x.IsValid).ToList();
var mapped = filtered.Select(x => x.Name).ToList();

// ✅ BUONO: LINQ ottimizzato
var result = data
    .Where(x => x.IsValid)
    .Select(x => x.Name)
    .ToList();
```

### Complessità Massima

- **Cyclomatic Complexity**: < 10 per metodo
- **Lunghezza metodo**: < 50 righe (target)
- **Numero parametri**: < 5 (idealmente)
- **Profondità nidificazione**: < 3 livelli

---

## Commit e Pull Request

### Commit Messages

**Formato:**
```
<type>(<scope>): <subject>

<body>

<footer>
```

**Type:**
- `feat`: Nuova feature
- `fix`: Bug fix
- `docs`: Documentazione
- `style`: Formatting (no code change)
- `refactor`: Refactoring codice
- `perf`: Performance improvements
- `test`: Test addition/updates
- `chore`: Maintenance

**Scope:**
Ambito interessato (es. "manager", "excel", "adi-report")

**Esempi Validi:**

```bash
feat(manager): add async report generation
fix(excel): correct formula calculation for ADI
docs(readme): update setup instructions
refactor(model): simplify patient data structure
perf(excel): improve large file writing speed
```

### Pull Request

**Processo:**

1. **Prima del PR:**
   ```bash
   # Aggiorna dal repository upstream
   git fetch upstream
   git rebase upstream/master
   
   # Test localmente
   # Compila e esegui applicazione
   # Verifica assenza errori
   ```

2. **Push del Branch:**
   ```bash
   git push origin feature/descrizione-feature
   ```

3. **Crea Pull Request:**
   - Vai alla pagina del tuo fork
   - Clicca "Compare & pull request"
   - Completa il template

4. **Template PR:**
   ```markdown
   ## Descrizione
   Breve descrizione dei cambiamenti
   
   ## Tipo di Cambiamento
   - [ ] Bug fix
   - [ ] Nuova feature
   - [ ] Breaking change
   - [ ] Aggiornamento documentazione
   
   ## Come Testare
   1. Passo uno
   2. Passo due
   3. Comportamento atteso: ...
   
   ## Checklist
   - [ ] Ho letto CONTRIBUTING.md
   - [ ] Ho testato i cambiamenti localmente
   - [ ] Ho aggiornato la documentazione
   - [ ] Ho aggiunto test se applicabile
   - [ ] I miei commit seguono il formato richiesto
   - [ ] Ho rimosso code di debug
   
   ## Screenshot (se applicabile)
   [Allega screenshot]
   
   ## Note Aggiuntive
   [Aggiungi note se necessario]
   ```

5. **Review Process:**
   - Un maintainer reviewerà il PR
   - Potrebbe richiedere modifiche
   - Una volta approvato, mergerà il PR

---

## Testing

### Test Locali Obbligatori

Prima di inviare PR, testa:

```bash
# 1. Compilazione senza errori
msbuild DO.VIVICARE.Reporting.sln /p:Configuration=Release

# 2. Esecuzione dell'applicazione UI
# Testa flusso completo di generazione report

# 3. Controllo assenza memory leaks
# Usa Visual Studio Memory Profiler

# 4. Verifica codice
# Analyzecsharpsyntax con Roslyn (se disponibile)
```

### Unit Test (futuro)

Non ancora implementato, ma pianificato:

```csharp
[TestClass]
public class ManagerTests
{
    [TestMethod]
    public void GenerateReport_ValidInput_Returnspath()
    {
        // Arrange
        var manager = new Manager();
        var testData = CreateTestData();
        
        // Act
        var result = manager.GenerateReport(testData);
        
        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(File.Exists(result));
    }
}
```

---

## Documentazione

### Quando Aggiornare Documentazione

- [ ] Nuova feature aggiunta
- [ ] API cambiata
- [ ] Comportamento modificato
- [ ] Nuovo modulo aggiunto
- [ ] Setup process cambiato

### File di Documentazione

| File | Contenuto |
|------|----------|
| README.md | Overview e setup |
| ARCHITECTURE.md | Architettura tecnica |
| CONTRIBUTING.md | Questo file |
| docs/API.md | Documentazione API |
| docs/TROUBLESHOOTING.md | Risoluzione problemi |

---

## Risoluzione Conflitti

### Conflitti Durante Merge

Se il tuo branch ha conflitti:

```bash
# 1. Fetch ultimi cambiamenti
git fetch upstream

# 2. Rebase sul master
git rebase upstream/master

# 3. Git indicherà i file in conflitto
# 4. Modifica i file per risolvere conflitti
# 5. Aggiungi file risolti
git add <file-risolto>

# 6. Continua rebase
git rebase --continue

# 7. Force push al tuo branch
git push origin feature/descrizione-feature --force
```

### Best Practices

- Sync spesso con upstream per evitare conflitti
- Rebase piuttosto che merge per storia pulita
- Comunica con maintainer se conflitti significativi

---

## Riconoscimenti

I tuoi contributi saranno:
- Accreditati nel changelog
- Menzionati nella pagina dei contributor
- Apprezzati dalla comunità

---

## Domande?

- Leggi la [Documentazione](README.md)
- Controlla [Discussions](https://github.com/artcava/DO.VIVICARE.Reporting/discussions)
- Contatta il maintainer via email

---

**Grazie per contribuire a DO.VIVICARE Reporting! 🙋**

*Ultimo aggiornamento: 11 Gennaio 2026*
