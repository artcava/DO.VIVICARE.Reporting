using DO.VIVICARE.Reporter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DO.VIVICARE.UI
{
    /// <summary>
    /// Gestisce gli aggiornamenti dell'applicazione e dei plugin da GitHub
    /// </summary>
    public partial class PluginManager
    {
        private const string MANIFEST_URL =
            "https://raw.githubusercontent.com/artcava/DO.VIVICARE.Reporting/master/manifest.json";

        private const string GITHUB_RELEASES_URL =
            "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download";

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
                if (!Directory.Exists(Manager.Plugins))
                    return null;

                Version latestVersion = null;
                string latestVersionString = null;

                // Cerca file DLL che corrisponda al pluginId
                var dllFiles = Directory.GetFiles(Manager.Plugins, $"*{pluginId}*.dll");

                foreach (var dllFile in dllFiles)
                {
                    var fileName = Path.GetFileNameWithoutExtension(dllFile);

                    Version currentVersion = null;

                    // Prova a estrarre versione dal nome file
                    // Formato atteso: PluginName-Version.dll
                    if (fileName.Contains("-"))
                    {
                        var parts = fileName.Split('-');
                        if (parts.Length >= 2)
                        {
                            // Ultimo elemento è la versione
                            var versionStr = parts[parts.Length - 1];

                            Version.TryParse(versionStr, out currentVersion);
                        }
                    }

                    if (currentVersion==null)
                    {
                        // Fallback: prova a leggere version da AssemblyVersion
                        try
                        {
                            var assembly = System.Reflection.Assembly.LoadFrom(dllFile);
                            currentVersion = assembly.GetName().Version;
                        }
                        catch { continue; }
                    }

                    if (currentVersion != null)
                    {
                        if (latestVersion == null || currentVersion > latestVersion)
                        {
                            latestVersion = currentVersion;
                            latestVersionString = $"{currentVersion.Major}.{currentVersion.Minor}.{currentVersion.Build}";
                        }
                    }
                }

                return latestVersionString;
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

                if (!Version.TryParse(installed.Version, out var installedVer))
                    return false;

                if (!Version.TryParse(available.Version, out var availableVer))
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

                // Crea directory se non esiste
                if (!Directory.Exists(Manager.Plugins))
                    Directory.CreateDirectory(Manager.Plugins);

                var outputPath = Path.Combine(Manager.Plugins, plugin.FileName ?? $"{plugin.Id}.dll");

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

                try
                {
                    int deleted = CleanupOldVersions(plugin.Id, plugin.Version);
                    if (deleted > 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Cleaned up {deleted} old version(s) of {plugin.Id}");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Warning: Failed to cleanup old versions: {ex.Message}");
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
                if (!Version.TryParse(v1, out var version1))
                    version1 = new Version("1.0.0.0");

                if (!Version.TryParse(v2, out var version2))
                    version2 = new Version("1.0.0.0");

                return version1.CompareTo(version2);
            }
            catch
            {
                return 0;
            }
        }

        #region CleanUpOldPlugins
        // DO.VIVICARE.UI/PluginManager.cs

        /// <summary>
        /// Rimuove tutte le versioni obsolete di un plugin, mantenendo solo l'ultima
        /// </summary>
        /// <param name="pluginId">ID del plugin</param>
        /// <param name="pluginType">Tipo di plugin ("Document" o "Report")</param>
        /// <param name="keepVersion">Versione da mantenere (opzionale, se null mantiene solo la più recente)</param>
        /// <returns>Numero di file rimossi</returns>
        public int CleanupOldVersions(string pluginId, string keepVersion = null)
        {
            try
            {
                // Determina directory
                string pluginDirectory = Manager.Plugins;
                if (!Directory.Exists(pluginDirectory))
                    return 0;

                var dllFiles = Directory.GetFiles(pluginDirectory, $"*{pluginId}*.dll");

                // Lista di file con le loro versioni
                var fileVersions = new List<(string Plugin, Version Version)>();

                foreach (var dllFile in dllFiles)
                {
                    var fileName = Path.GetFileNameWithoutExtension(dllFile);
                    Version version = null;

                    // Estrai versione dal nome
                    if (fileName.Contains("-"))
                    {
                        var parts = fileName.Split('-');
                        if (parts.Length >= 2)
                        {
                            var versionString = parts[parts.Length - 1];
                            Version.TryParse(versionString, out version);
                        }
                    }

                    // Fallback: leggi da assembly
                    if (version == null)
                    {
                        try
                        {
                            var assembly = System.Reflection.Assembly.LoadFrom(dllFile);
                            version = assembly.GetName().Version;
                        }
                        catch { }
                    }

                    if (version != null)
                    {
                        fileVersions.Add((dllFile, version));
                    }
                }

                if (fileVersions.Count == 0)
                    return 0;

                // Determina quale versione mantenere
                Version versionToKeep;
                if (!string.IsNullOrEmpty(keepVersion) && Version.TryParse(keepVersion, out versionToKeep))
                {
                    // Mantieni versione specifica
                }
                else
                {
                    // Mantieni la più recente
                    versionToKeep = fileVersions.Max(fv => fv.Version);
                }

                // Elimina tutte le versioni tranne quella da mantenere
                int deletedCount = 0;
                foreach (var (filePath, version) in fileVersions)
                {
                    if (version < versionToKeep)
                    {
                        try
                        {
                            File.Delete(filePath);
                            deletedCount++;
                            System.Diagnostics.Debug.WriteLine($"Deleted old version: {Path.GetFileName(filePath)}");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to delete {filePath}: {ex.Message}");
                        }
                    }
                }

                return deletedCount;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error cleaning up old versions: {ex.Message}");
                return 0;
            }
        }
        #endregion
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
