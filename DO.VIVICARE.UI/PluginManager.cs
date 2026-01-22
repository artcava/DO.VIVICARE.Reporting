using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace DO.VIVICARE.UI
{
    /// <summary>
    /// Gestisce il download, verifica e caricamento dei plugin da GitHub
    /// </summary>
    public class PluginManager
    {
        // Download manifest directly from GitHub (works with public repos)
        // URL format: https://raw.githubusercontent.com/owner/repo/branch/path
        private const string MANIFEST_URL =
            "https://raw.githubusercontent.com/artcava/DO.VIVICARE.Reporting/master/manifest.json";

        private readonly string _pluginDirectory;

        public PluginManager(string pluginDirectory = null)
        {
            _pluginDirectory = pluginDirectory ??
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                            "DO.VIVICARE", "Plugins");

            if (!Directory.Exists(_pluginDirectory))
                Directory.CreateDirectory(_pluginDirectory);
        }

        /// <summary>
        /// Verifica se esiste una nuova versione dell'applicazione disponibile su GitHub
        /// </summary>
        /// <returns>AppUpdateInfo se un aggiornamento è disponibile, null altrimenti</returns>
        public async Task<AppUpdateInfo> CheckAppUpdateAsync()
        {
            try
            {
                LogDebug("Checking for application updates...");

                var manifest = await GetManifestAsync();
                if (manifest?.App == null)
                {
                    LogError("Unable to retrieve app info from manifest");
                    return null;
                }

                // Ottieni versione corrente dell'applicazione
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                LogDebug($"Current app version: {currentVersion}");

                // Parse versione disponibile dal manifest
                if (!Version.TryParse(manifest.App.Version, out var availableVersion))
                {
                    LogError($"Invalid version format in manifest: {manifest.App.Version}");
                    return null;
                }

                LogDebug($"Available app version: {availableVersion}");

                // Confronta versioni
                if (availableVersion > currentVersion)
                {
                    LogDebug($"Update available: {currentVersion} -> {availableVersion}");
                    return new AppUpdateInfo
                    {
                        CurrentVersion = currentVersion.ToString(),
                        AvailableVersion = manifest.App.Version,
                        DownloadUrl = manifest.App.DownloadUrl,
                        ReleaseDate = manifest.App.ReleaseDate,
                        Checksum = manifest.App.Checksum
                    };
                }

                LogDebug("No update available - current version is up to date");
                return null;
            }
            catch (Exception ex)
            {
                LogError($"Error checking for app updates: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Scarica il manifest con lista di tutti i plugin disponibili da GitHub
        /// </summary>
        public async Task<PluginManifest> GetManifestAsync()
        {
            try
            {
                LogDebug($"Fetching manifest from: {MANIFEST_URL}");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "DO.VIVICARE");
                    client.Timeout = TimeSpan.FromSeconds(30);

                    var response = await client.GetAsync(MANIFEST_URL);

                    LogDebug($"HTTP Status: {response.StatusCode}");

                    if (!response.IsSuccessStatusCode)
                    {
                        LogError($"Failed to download manifest. Status: {response.StatusCode}");
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                            LogError("Manifest not found. Ensure manifest.json exists in repository root.");
                        return null;
                    }

                    var json = await response.Content.ReadAsStringAsync();
                    LogDebug($"Manifest JSON received: {json.Length} bytes");

                    var manifest = JsonConvert.DeserializeObject<PluginManifest>(json);

                    if (manifest == null)
                    {
                        LogError("Failed to deserialize manifest JSON");
                        return null;
                    }

                    LogDebug($"Manifest loaded successfully: {manifest.Documents?.Count ?? 0} documents, {manifest.Reports?.Count ?? 0} reports");
                    return manifest;
                }
            }
            catch (HttpRequestException hre)
            {
                LogError($"HTTP Error: {hre.Message}");
                return null;
            }
            catch (TaskCanceledException tce)
            {
                LogError($"Request timeout: {tce.Message}");
                return null;
            }
            catch (JsonException je)
            {
                LogError($"JSON parse error: {je.Message}");
                return null;
            }
            catch (Exception ex)
            {
                LogError($"Unexpected error loading manifest: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Confronta versione installata con quella disponibile
        /// </summary>
        public bool HasUpdate(PluginInfo installed, PluginInfo available)
        {
            if (installed == null || available == null)
                return false;

            if (!Version.TryParse(installed.Version, out var installedVersion) ||
                !Version.TryParse(available.Version, out var availableVersion))
                return false;

            return availableVersion > installedVersion;
        }

        /// <summary>
        /// Scarica un plugin da GitHub con progress tracking
        /// Supporta sia download diretto DLL che estrazione da ZIP setup package
        /// </summary>
        public async Task<bool> DownloadPluginAsync(
            PluginInfo plugin,
            IProgress<DownloadProgress> progress = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                LogDebug($"Starting download of plugin: {plugin.Name} v{plugin.Version}");

                // Verifica se il plugin è contenuto nel setup ZIP
                if (plugin.DownloadUrl.Contains("DO.VIVICARE-Setup"))
                {
                    return await DownloadPluginFromSetupZipAsync(plugin, progress, cancellationToken);
                }
                else
                {
                    return await DownloadPluginDirectAsync(plugin, progress, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                LogError($"Error downloading plugin {plugin.Name}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Scarica plugin direttamente (quando è un DLL singolo)
        /// </summary>
        private async Task<bool> DownloadPluginDirectAsync(
            PluginInfo plugin,
            IProgress<DownloadProgress> progress,
            CancellationToken cancellationToken)
        {
            try
            {
                // IMPORTANTE: Prima elimina le versioni vecchie dello stesso plugin
                DeleteOldPluginVersions(plugin.Id);

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "DO.VIVICARE");
                    client.Timeout = TimeSpan.FromMinutes(5);

                    var response = await client.GetAsync(
                        plugin.DownloadUrl,
                        HttpCompletionOption.ResponseHeadersRead,
                        cancellationToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        LogError($"Download failed for {plugin.Name}. Status: {response.StatusCode}");
                        return false;
                    }

                    var totalBytes = response.Content.Headers.ContentLength ?? 0L;
                    var filePath = Path.Combine(_pluginDirectory, plugin.FileName);

                    LogDebug($"Downloading {plugin.FileName} ({totalBytes} bytes) to {filePath}");

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, useAsync: true))
                    {
                        var totalRead = 0L;
                        var buffer = new byte[8192];
                        int bytesRead;

                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) != 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                            totalRead += bytesRead;

                            progress?.Report(new DownloadProgress
                            {
                                PluginId = plugin.Id,
                                BytesDownloaded = totalRead,
                                TotalBytes = totalBytes,
                                PercentComplete = (int)(totalRead * 100 / Math.Max(totalBytes, 1))
                            });
                        }
                    }

                    // Verifica checksum SHA256 (skip se è "sha256:TBD")
                    if (!string.IsNullOrEmpty(plugin.Checksum) && !plugin.Checksum.Equals("sha256:TBD", StringComparison.OrdinalIgnoreCase))
                    {
                        LogDebug($"Verifying checksum for {plugin.FileName}");
                        if (!await VerifyChecksumAsync(filePath, plugin.Checksum))
                        {
                            LogError($"Checksum verification failed for {plugin.Name}");
                            File.Delete(filePath);
                            return false;
                        }
                    }
                    else
                    {
                        LogDebug($"Skipping checksum verification for {plugin.FileName} (checksum: {plugin.Checksum})");
                    }

                    LogDebug($"Plugin {plugin.Name} v{plugin.Version} downloaded and verified successfully");
                    return true;
                }
            }
            catch (OperationCanceledException)
            {
                LogError($"Download cancelled for {plugin.Name}");
                return false;
            }
        }

        /// <summary>
        /// Scarica il setup ZIP e estrae il DLL del plugin richiesto
        /// </summary>
        private async Task<bool> DownloadPluginFromSetupZipAsync(
            PluginInfo plugin,
            IProgress<DownloadProgress> progress,
            CancellationToken cancellationToken)
        {
            var setupZipPath = null as string;
            try
            {
                // IMPORTANTE: Prima elimina le versioni vecchie dello stesso plugin
                DeleteOldPluginVersions(plugin.Id);

                setupZipPath = Path.Combine(_pluginDirectory, "setup_temp.zip");

                LogDebug($"Downloading setup package from {plugin.DownloadUrl}");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "DO.VIVICARE");
                    client.Timeout = TimeSpan.FromMinutes(10);

                    var response = await client.GetAsync(
                        plugin.DownloadUrl,
                        HttpCompletionOption.ResponseHeadersRead,
                        cancellationToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        LogError($"Download failed for setup package. Status: {response.StatusCode}");
                        return false;
                    }

                    var totalBytes = response.Content.Headers.ContentLength ?? 0L;

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    using (var fileStream = new FileStream(setupZipPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, useAsync: true))
                    {
                        var totalRead = 0L;
                        var buffer = new byte[8192];
                        int bytesRead;

                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) != 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                            totalRead += bytesRead;

                            progress?.Report(new DownloadProgress
                            {
                                PluginId = plugin.Id,
                                BytesDownloaded = totalRead,
                                TotalBytes = totalBytes,
                                PercentComplete = (int)(totalRead * 100 / Math.Max(totalBytes, 1))
                            });
                        }
                    }
                }

                // Estrai il DLL dal ZIP
                LogDebug($"Extracting {plugin.FileName} from setup package");
                var dllPath = Path.Combine(_pluginDirectory, plugin.FileName);

                using (var archive = System.IO.Compression.ZipFile.OpenRead(setupZipPath))
                {
                    // Cerca il DLL nel ZIP (potrebbe essere in sottocartelle)
                    var entry = archive.Entries.FirstOrDefault(e =>
                        e.Name.Equals(plugin.FileName, StringComparison.OrdinalIgnoreCase) ||
                        e.FullName.EndsWith(plugin.FileName, StringComparison.OrdinalIgnoreCase));

                    if (entry == null)
                    {
                        LogError($"Plugin file {plugin.FileName} not found in setup package");
                        LogDebug($"Available files in setup: {string.Join(", ", archive.Entries.Select(e => e.Name).Distinct())}");
                        return false;
                    }

                    entry.ExtractToFile(dllPath, overwrite: true);
                    LogDebug($"Plugin extracted successfully to {dllPath}");
                }

                // Verifica checksum se disponibile
                if (!string.IsNullOrEmpty(plugin.Checksum) && !plugin.Checksum.Equals("sha256:TBD", StringComparison.OrdinalIgnoreCase))
                {
                    LogDebug($"Verifying checksum for {plugin.FileName}");
                    if (!await VerifyChecksumAsync(dllPath, plugin.Checksum))
                    {
                        LogError($"Checksum verification failed for {plugin.Name}");
                        File.Delete(dllPath);
                        return false;
                    }
                }
                else
                {
                    LogDebug($"Skipping checksum verification for {plugin.FileName} (checksum: {plugin.Checksum})");
                }

                LogDebug($"Plugin {plugin.Name} v{plugin.Version} extracted and verified successfully");
                return true;
            }
            catch (OperationCanceledException)
            {
                LogError($"Download cancelled for {plugin.Name}");
                return false;
            }
            finally
            {
                // Pulizia: elimina il ZIP temporaneo
                if (!string.IsNullOrEmpty(setupZipPath) && File.Exists(setupZipPath))
                {
                    try
                    {
                        File.Delete(setupZipPath);
                        LogDebug("Temporary setup package deleted");
                    }
                    catch (Exception ex)
                    {
                        LogDebug($"Could not delete temporary file: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Verifica integrità file tramite SHA256
        /// </summary>
        private async Task<bool> VerifyChecksumAsync(string filePath, string expectedChecksum)
        {
            try
            {
                using (var sha256 = SHA256.Create())
                using (var stream = File.OpenRead(filePath))
                {
                    var hash = sha256.ComputeHash(stream);
                    var computedChecksum = "sha256:" + BitConverter.ToString(hash).Replace("-", "").ToLower();
                    return computedChecksum.Equals(expectedChecksum, StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                LogError($"Checksum verification error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Carica un DLL plugin in memoria (hot-reload senza riavvio app)
        /// </summary>
        public Assembly LoadPluginAssembly(string pluginId)
        {
            try
            {
                var pluginFile = Directory.GetFiles(_pluginDirectory)
                    .FirstOrDefault(f => f.Contains(pluginId));

                if (pluginFile == null)
                    throw new FileNotFoundException($"Plugin {pluginId} not found");

                var assemblyName = AssemblyName.GetAssemblyName(pluginFile);
                return Assembly.Load(assemblyName);
            }
            catch (Exception ex)
            {
                LogError($"Error loading plugin: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Ottiene la versione installata di un plugin specifico (ricerca per ID)
        /// Restituisce SEMPRE la versione più recente se multiple versioni esistono
        /// </summary>
        public string GetInstalledPluginVersion(string pluginId)
        {
            try
            {
                // Cerca file nel formato: {pluginId-lowercase}-{version}.dll
                var pattern = pluginId.Replace(".", "-").ToLower() + "-*.dll";
                var files = Directory.GetFiles(_pluginDirectory, pattern);

                if (files.Length == 0)
                    return null;

                LogDebug($"Found {files.Length} file(s) matching pattern '{pattern}'");

                // Estrai versione da OGNI file e trova la più recente
                var fileVersions = new List<(string filePath, Version version)>();

                foreach (var file in files)
                {
                    try
                    {
                        var fileName = Path.GetFileNameWithoutExtension(file);
                        var version = ExtractVersionFromFileName(fileName, pluginId);

                        if (version != null)
                        {
                            fileVersions.Add((file, version));
                            LogDebug($"  File: {Path.GetFileName(file)} → Version: {version}");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogDebug($"Could not parse version from {Path.GetFileName(file)}: {ex.Message}");
                    }
                }

                if (fileVersions.Count == 0)
                    return null;

                // Ordina per versione (decrescente) e prendi la più recente
                var latestFile = fileVersions.OrderByDescending(x => x.version).First();
                LogDebug($"Latest version found: {latestFile.version} ({Path.GetFileName(latestFile.filePath)})");

                return latestFile.version.ToString();
            }
            catch (Exception ex)
            {
                LogError($"Error getting installed plugin version for {pluginId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Estrae la versione dal nome file
        /// Esempio: "report-valorizzazione-1.0.6.dll" → Version("1.0.6")
        /// </summary>
        private Version ExtractVersionFromFileName(string fileName, string pluginId)
        {
            try
            {
                // Formato: {id-con-trattini}-{versione}.dll
                // Es: report-valorizzazione-1.0.6
                var idPattern = pluginId.Replace(".", "-").ToLower();

                if (!fileName.StartsWith(idPattern))
                    return null;

                // Rimuovi il pattern di ID dall'inizio
                var remainder = fileName.Substring(idPattern.Length);

                // Rimuovi il trattino iniziale se presente
                if (remainder.StartsWith("-"))
                    remainder = remainder.Substring(1);

                // Quello che rimane dovrebbe essere la versione
                // Es: "1.0.6" oppure "1.0.6-extra" (ignora la parte extra)
                var versionString = remainder.Split(new[] { '-' }, StringSplitOptions.None)[0];

                if (Version.TryParse(versionString, out var version))
                    return version;

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Elimina tutte le versioni precedenti di un plugin
        /// (mantiene solo la versione più recente)
        /// </summary>
        private void DeleteOldPluginVersions(string pluginId)
        {
            try
            {
                var pattern = pluginId.Replace(".", "-").ToLower() + "-*.dll";
                var files = Directory.GetFiles(_pluginDirectory, pattern);

                if (files.Length <= 1)
                    return; // Niente da pulire

                LogDebug($"Cleaning up old versions of {pluginId}... Found {files.Length} version(s)");

                // Estrai versione da OGNI file
                var fileVersions = new List<(string filePath, Version version)>();

                foreach (var file in files)
                {
                    try
                    {
                        var fileName = Path.GetFileNameWithoutExtension(file);
                        var version = ExtractVersionFromFileName(fileName, pluginId);

                        if (version != null)
                            fileVersions.Add((file, version));
                    }
                    catch { /* Skip */ }
                }

                // Ordina per versione e mantieni solo la più recente
                var latestFile = fileVersions.OrderByDescending(x => x.version).First();

                foreach (var (filePath, version) in fileVersions)
                {
                    if (filePath != latestFile.filePath)
                    {
                        try
                        {
                            File.Delete(filePath);
                            LogDebug($"Deleted old version {version}: {Path.GetFileName(filePath)}");
                        }
                        catch (Exception ex)
                        {
                            LogDebug($"Could not delete {Path.GetFileName(filePath)}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogDebug($"Error cleaning up old plugin versions: {ex.Message}");
            }
        }

        /// <summary>
        /// Ottiene lista di plugin installati localmente in MyDocuments\DO.VIVICARE\Plugins
        /// </summary>
        public List<InstalledPlugin> GetInstalledPlugins()
        {
            var installed = new List<InstalledPlugin>();

            try
            {
                var files = Directory.GetFiles(_pluginDirectory, "*.dll");
                foreach (var file in files)
                {
                    try
                    {
                        var fileName = Path.GetFileNameWithoutExtension(file);
                        var assembly = AssemblyName.GetAssemblyName(file);

                        installed.Add(new InstalledPlugin
                        {
                            Name = assembly.Name,
                            Version = assembly.Version?.ToString() ?? "Unknown",
                            FilePath = file,
                            FileSize = new FileInfo(file).Length,
                            FileName = fileName
                        });
                    }
                    catch { /* Skip invalid assemblies */ }
                }
            }
            catch { /* Directory may not exist */ }

            return installed;
        }

        /// <summary>
        /// Log message helper
        /// </summary>
        private static void LogDebug(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[PluginManager] {message}");
        }

        /// <summary>
        /// Log error helper
        /// </summary>
        private static void LogError(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[PluginManager ERROR] {message}");
        }
    }

    /// <summary>
    /// Root object del manifest.json con lista plugin disponibili
    /// </summary>
    public class PluginManifest
    {
        [JsonProperty("app")]
        public AppInfo App { get; set; }

        [JsonProperty("documents")]
        public List<PluginInfo> Documents { get; set; } = new List<PluginInfo>();

        [JsonProperty("reports")]
        public List<PluginInfo> Reports { get; set; } = new List<PluginInfo>();
    }

    /// <summary>
    /// Info app principale
    /// </summary>
    public class AppInfo
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("downloadUrl")]
        public string DownloadUrl { get; set; }

        [JsonProperty("checksum")]
        public string Checksum { get; set; }

        [JsonProperty("releaseDate")]
        public string ReleaseDate { get; set; }
    }

    /// <summary>
    /// Informazioni su un aggiornamento dell'applicazione disponibile
    /// </summary>
    public class AppUpdateInfo
    {
        /// <summary>
        /// Versione attualmente installata
        /// </summary>
        public string CurrentVersion { get; set; }

        /// <summary>
        /// Versione disponibile per il download
        /// </summary>
        public string AvailableVersion { get; set; }

        /// <summary>
        /// URL per scaricare la nuova versione
        /// </summary>
        public string DownloadUrl { get; set; }

        /// <summary>
        /// Data di rilascio della nuova versione
        /// </summary>
        public string ReleaseDate { get; set; }

        /// <summary>
        /// Checksum SHA256 del file di setup
        /// </summary>
        public string Checksum { get; set; }
    }

    /// <summary>
    /// Info singolo plugin dal manifest.json
    /// </summary>
    public class PluginInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("downloadUrl")]
        public string DownloadUrl { get; set; }

        [JsonProperty("checksum")]
        public string Checksum { get; set; }

        [JsonProperty("fileSize")]
        public long FileSize { get; set; }

        [JsonProperty("releaseDate")]
        public string ReleaseDate { get; set; }

        [JsonProperty("dependencies")]
        public List<string> Dependencies { get; set; } = new List<string>();

        [JsonIgnore]
        public string FileName => $"{Id.Replace(".", "-")}-{Version}.dll";
    }

    /// <summary>
    /// Info plugin già installato localmente
    /// </summary>
    public class InstalledPlugin
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }
        public string FileName { get; set; }
    }

    /// <summary>
    /// Progresso download plugin (per IProgress<T>)
    /// </summary>
    public class DownloadProgress
    {
        public string PluginId { get; set; }
        public long BytesDownloaded { get; set; }
        public long TotalBytes { get; set; }
        public int PercentComplete { get; set; }
    }
}
