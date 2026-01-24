using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DO.VIVICARE.UI
{
    /// <summary>
    /// Gestisce gli aggiornamenti dell'applicazione e dei plugin da GitHub
    /// </summary>
    public class PluginManager
    {
        private const string MANIFEST_URL =
            "https://raw.githubusercontent.com/artcava/DO.VIVICARE.Reporting/master/manifest.json";

        private const string GITHUB_RELEASES_URL =
            "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download";

        private const string PLUGINS_DIR = "Plugins";

        /// <summary>
        /// Informazioni di aggiornamento per l'app principale
        /// </summary>
        public class UpdateInfo
        {
            [JsonProperty("currentVersion")]
            public string CurrentVersion { get; set; }

            [JsonProperty("availableVersion")]
            public string AvailableVersion { get; set; }

            [JsonProperty("releaseDate")]
            public string ReleaseDate { get; set; }

            [JsonProperty("downloadUrl")]
            public string DownloadUrl { get; set; }

            [JsonProperty("checksum")]
            public string Checksum { get; set; }
        }

        /// <summary>
        /// Manifest completo dei plugin da GitHub
        /// </summary>
        public class PluginManifest
        {
            [JsonProperty("version")]
            public string Version { get; set; }

            [JsonProperty("releaseDate")]
            public string ReleaseDate { get; set; }

            [JsonProperty("testedWith")]
            public string TestedWith { get; set; }

            [JsonProperty("documents")]
            public List<PluginInfo> Documents { get; set; } = new List<PluginInfo>();

            [JsonProperty("reports")]
            public List<PluginInfo> Reports { get; set; } = new List<PluginInfo>();
        }

        /// <summary>
        /// Informazioni su un singolo plugin (document o report)
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

            [JsonProperty("category")]
            public string Category { get; set; }

            [JsonProperty("type")]
            public string Type { get; set; } // "Document" o "Report"

            [JsonProperty("size")]
            public long Size { get; set; }

            [JsonProperty("checksum")]
            public string Checksum { get; set; }

            [JsonProperty("downloadUrl")]
            public string DownloadUrl { get; set; }

            [JsonProperty("fileName")]
            public string FileName { get; set; }

            [JsonProperty("dependencies")]
            public List<string> Dependencies { get; set; } = new List<string>();

            [JsonProperty("required")]
            public bool Required { get; set; }

            [JsonProperty("releaseDate")]
            public string ReleaseDate { get; set; }
        }

        /// <summary>
        /// Informazioni su un plugin già installato nel sistema
        /// </summary>
        public class InstalledPlugin
        {
            public string Id { get; set; }
            public string Version { get; set; }
            public string FilePath { get; set; }
            public DateTime InstallDate { get; set; }
        }

        /// <summary>
        /// Progress report per il download di un plugin
        /// </summary>
        public class DownloadProgress
        {
            public long BytesDownloaded { get; set; }
            public long TotalBytes { get; set; }
            public DateTime StartTime { get; set; }

            public int PercentComplete =>
                TotalBytes > 0 ? (int)((BytesDownloaded * 100) / TotalBytes) : 0;

            public TimeSpan ElapsedTime =>
                DateTime.UtcNow - StartTime;

            public TimeSpan EstimatedTimeRemaining
            {
                get
                {
                    if (BytesDownloaded == 0 || ElapsedTime.TotalSeconds == 0)
                        return TimeSpan.Zero;

                    double bytesPerSecond = BytesDownloaded / ElapsedTime.TotalSeconds;
                    long remainingBytes = TotalBytes - BytesDownloaded;
                    double secondsRemaining = remainingBytes / bytesPerSecond;

                    return TimeSpan.FromSeconds(secondsRemaining);
                }
            }
        }

        /// <summary>
        /// Scarica il manifest completo dei plugin da GitHub
        /// </summary>
        public async Task<PluginManifest> GetManifestAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "DO.VIVICARE.UI");
                    client.Timeout = TimeSpan.FromSeconds(30);

                    var manifestJson = await client.GetStringAsync(MANIFEST_URL);
                    var manifest = JsonConvert.DeserializeObject<PluginManifest>(manifestJson);

                    return manifest ?? new PluginManifest();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading manifest: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Ottiene la versione di un plugin installato, se presente
        /// </summary>
        public string GetInstalledPluginVersion(string pluginId)
        {
            try
            {
                var pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PLUGINS_DIR);

                if (!Directory.Exists(pluginsPath))
                    return null;

                // Cerca file DLL che corrisponda al pluginId
                var dllFiles = Directory.GetFiles(pluginsPath, "*.dll");

                foreach (var dllFile in dllFiles)
                {
                    var fileName = Path.GetFileNameWithoutExtension(dllFile);

                    // Prova a estrarre versione dal nome file
                    // Formato atteso: PluginName-Version.dll
                    if (fileName.Contains("-"))
                    {
                        var parts = fileName.Split('-');
                        if (parts.Length >= 2)
                        {
                            // Ultimo elemento è la versione
                            var version = parts[parts.Length - 1];

                            if (System.Version.TryParse(version, out _))
                                return version;
                        }
                    }

                    // Fallback: prova a leggere version da AssemblyVersion
                    try
                    {
                        var assembly = System.Reflection.Assembly.LoadFrom(dllFile);
                        var versionAttr = assembly.GetName().Version;
                        if (versionAttr != null)
                            return $"{versionAttr.Major}.{versionAttr.Minor}.{versionAttr.Build}";
                    }
                    catch { }
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking installed version: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Controlla se un plugin ha un aggiornamento disponibile
        /// </summary>
        public bool HasUpdate(PluginInfo installed, PluginInfo available)
        {
            try
            {
                if (installed?.Version == null || available?.Version == null)
                    return false;

                if (!System.Version.TryParse(installed.Version, out var installedVer))
                    return false;

                if (!System.Version.TryParse(available.Version, out var availableVer))
                    return false;

                return installedVer < availableVer;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Scarica e installa un plugin
        /// </summary>
        public async Task<bool> DownloadPluginAsync(
            PluginInfo plugin,
            IProgress<DownloadProgress> progress,
            CancellationToken cancellationToken)
        {
            try
            {
                if (plugin == null || string.IsNullOrEmpty(plugin.DownloadUrl))
                    return false;

                var pluginsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, PLUGINS_DIR);

                // Crea directory se non esiste
                if (!Directory.Exists(pluginsPath))
                    Directory.CreateDirectory(pluginsPath);

                var outputPath = Path.Combine(pluginsPath, plugin.FileName ?? $"{plugin.Id}.dll");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "DO.VIVICARE.UI");
                    client.Timeout = TimeSpan.FromMinutes(5);

                    using (var response = await client.GetAsync(
                        plugin.DownloadUrl,
                        HttpCompletionOption.ResponseHeadersRead,
                        cancellationToken))
                    {
                        if (!response.IsSuccessStatusCode)
                            return false;

                        var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                        var dlProgress = new DownloadProgress
                        {
                            TotalBytes = totalBytes,
                            BytesDownloaded = 0,
                            StartTime = DateTime.UtcNow
                        };

                        using (var contentStream = await response.Content.ReadAsStreamAsync())
                        using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                        {
                            var buffer = new byte[8192];
                            int bytesRead;

                            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) != 0)
                            {
                                await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken);

                                dlProgress.BytesDownloaded += bytesRead;
                                progress?.Report(dlProgress);
                            }
                        }
                    }
                }

                // Verifica integrità con checksum se disponibile
                if (!string.IsNullOrEmpty(plugin.Checksum))
                {
                    if (!VerifyChecksum(outputPath, plugin.Checksum))
                    {
                        // Cancella il file se checksum non corrisponde
                        if (File.Exists(outputPath))
                            File.Delete(outputPath);
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error downloading plugin: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica l'integrità di un file usando il checksum SHA256
        /// </summary>
        private bool VerifyChecksum(string filePath, string expectedChecksum)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                using (var sha256 = System.Security.Cryptography.SHA256.Create())
                using (var fileStream = File.OpenRead(filePath))
                {
                    var hashBytes = sha256.ComputeHash(fileStream);
                    var calculatedChecksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                    // Normalizza expected (potrebbe avere "sha256:" prefix)
                    var expected = expectedChecksum.Replace("sha256:", "").ToLower();

                    return calculatedChecksum == expected;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Controlla se è disponibile un aggiornamento per l'app principale
        /// </summary>
        public async Task<UpdateInfo> CheckAppUpdateAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "DO.VIVICARE.UI");
                    client.Timeout = TimeSpan.FromSeconds(30);

                    // Download manifest.json da GitHub
                    var manifestJson = await client.GetStringAsync(MANIFEST_URL);
                    dynamic manifest = JsonConvert.DeserializeObject(manifestJson);

                    var availableVersion = manifest.version.ToString();
                    var currentVersion = GetCurrentApplicationVersion();

                    System.Diagnostics.Debug.WriteLine(
                        $"Current: {currentVersion}, Available: {availableVersion}");

                    // Controlla se esiste update
                    if (VersionCompare(currentVersion, availableVersion) < 0)
                    {
                        // Update disponibile
                        if (manifest.app != null)
                        {
                            var appInfo = manifest.app;

                            return new UpdateInfo
                            {
                                CurrentVersion = currentVersion,
                                AvailableVersion = availableVersion,
                                ReleaseDate = manifest.releaseDate?.ToString() ?? "Unknown",
                                DownloadUrl = appInfo.downloadUrl?.ToString(),
                                Checksum = appInfo.checksum?.ToString() ?? null
                            };
                        }
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking update: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Compara versioni semantiche
        /// Return: -1 se v1 &lt; v2, 0 se uguali, 1 se v1 &gt; v2
        /// </summary>
        private int VersionCompare(string v1, string v2)
        {
            try
            {
                // Normalizza versioni (es: 1.2.1 -&gt; 1.2.1.0)
                if (!System.Version.TryParse(v1, out var version1))
                    version1 = new System.Version("1.0.0.0");

                if (!System.Version.TryParse(v2, out var version2))
                    version2 = new System.Version("1.0.0.0");

                return version1.CompareTo(version2);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Ottieni versione corrente dell'applicazione
        /// </summary>
        private string GetCurrentApplicationVersion()
        {
            try
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;

                if (version != null)
                {
                    // Ritorna format MAJOR.MINOR.PATCH (senza build number)
                    return $"{version.Major}.{version.Minor}.{version.Build}";
                }

                return "1.0.0";
            }
            catch
            {
                return "1.0.0";
            }
        }
    }
}
