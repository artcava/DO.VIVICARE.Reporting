using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DO.VIVICARE.UI
{
    /// <summary>
    /// Gestisce il download, verifica e caricamento dei plugin da GitHub
    /// Usa GitHub API con token da environment variable per compatibilità con repo privati
    /// </summary>
    public class PluginManager
    {
        // GitHub API endpoint per manifest.json
        private const string MANIFEST_API_URL =
            "https://api.github.com/repos/artcava/DO.VIVICARE.Reporting/contents/manifest.json?ref=master";

        private readonly string _pluginDirectory;
        private readonly string _githubToken;

        public PluginManager(string pluginDirectory = null)
        {
            _pluginDirectory = pluginDirectory ??
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                            "DO.VIVICARE", "Plugins");

            if (!Directory.Exists(_pluginDirectory))
                Directory.CreateDirectory(_pluginDirectory);

            // Leggi token da environment variable
            _githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
            
            if (string.IsNullOrEmpty(_githubToken))
            {
                LogDebug("GITHUB_TOKEN environment variable not set. Manifest download may fail with private repos.");
            }
        }

        /// <summary>
        /// Scarica il manifest con lista di tutti i plugin disponibili da GitHub API
        /// Usa token da environment variable per accedere a repo privati
        /// </summary>
        public async Task<PluginManifest> GetManifestAsync()
        {
            try
            {
                LogDebug($"Fetching manifest from GitHub API: {MANIFEST_API_URL}");
                LogDebug($"Authentication: {(string.IsNullOrEmpty(_githubToken) ? "None (public access)" : "Token from GITHUB_TOKEN variable")}");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "DO.VIVICARE");
                    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3.raw");
                    
                    // Aggiungi token se disponibile
                    if (!string.IsNullOrEmpty(_githubToken))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", $"token {_githubToken}");
                    }
                    
                    client.Timeout = TimeSpan.FromSeconds(30);
                    
                    var response = await client.GetAsync(MANIFEST_API_URL);
                    
                    LogDebug($"HTTP Status: {response.StatusCode}");

                    if (!response.IsSuccessStatusCode)
                    {
                        LogError($"Failed to download manifest. Status: {response.StatusCode}");
                        
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                            LogError("manifest.json not found in repository");
                        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                            LogError("Invalid or missing GitHub token. Check GITHUB_TOKEN environment variable.");
                        else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                            LogError("Access forbidden. Token may lack required permissions.");
                        
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
        /// </summary>
        public async Task<bool> DownloadPluginAsync(
            PluginInfo plugin,
            IProgress<DownloadProgress> progress = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                LogDebug($"Starting download of plugin: {plugin.Name} v{plugin.Version}");

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

                    // Verifica checksum SHA256
                    LogDebug($"Verifying checksum for {plugin.FileName}");
                    if (!await VerifyChecksumAsync(filePath, plugin.Checksum))
                    {
                        LogError($"Checksum verification failed for {plugin.Name}");
                        File.Delete(filePath);
                        return false;
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
            catch (Exception ex)
            {
                LogError($"Error downloading plugin {plugin.Name}: {ex.Message}");
                return false;
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
                    return computedChecksum == expectedChecksum.ToLower();
                }
            }
            catch
            {
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
                        var assembly = AssemblyName.GetAssemblyName(file);
                        installed.Add(new InstalledPlugin
                        {
                            Name = assembly.Name,
                            Version = assembly.Version?.ToString() ?? "Unknown",
                            FilePath = file,
                            FileSize = new FileInfo(file).Length
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
