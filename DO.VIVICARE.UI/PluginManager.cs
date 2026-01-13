using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DO.VIVICARE.UI
{
    /// <summary>
    /// Gestisce il download, installazione e aggiornamento dei plugin
    /// </summary>
    public class PluginManager
    {
        private const string MANIFEST_URL = 
            "https://raw.githubusercontent.com/artcava/DO.VIVICARE.Reporting/master/manifest.json";
        
        private readonly string _pluginDirectory;

        public PluginManager(string pluginDirectory = null)
        {
            _pluginDirectory = pluginDirectory ?? 
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), 
                            "DO.VIVICARE", "Plugins");
            
            if (!Directory.Exists(_pluginDirectory))
                Directory.CreateDirectory(_pluginDirectory);
        }

        /// <summary>
        /// Scarica il manifest con lista di tutti i plugin disponibili
        /// </summary>
        public async Task<PluginManifest> GetManifestAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "DO.VIVICARE");
                    
                    var response = await client.GetAsync(MANIFEST_URL);
                    response.EnsureSuccessStatusCode();
                    
                    var json = await response.Content.ReadAsStringAsync();
                    var manifest = JsonConvert.DeserializeObject<PluginManifest>(json);
                    
                    return manifest;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading manifest: {ex.Message}");
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
        /// Scarica un plugin con progress tracking
        /// </summary>
        public async Task<bool> DownloadPluginAsync(
            PluginInfo plugin,
            IProgress<DownloadProgress> progress = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "DO.VIVICARE");
                    
                    // Download con progress
                    var response = await client.GetAsync(
                        plugin.DownloadUrl, 
                        HttpCompletionOption.ResponseHeadersRead,
                        cancellationToken);
                    
                    response.EnsureSuccessStatusCode();
                    
                    var totalBytes = response.Content.Headers.ContentLength ?? 0L;
                    var filePath = Path.Combine(_pluginDirectory, plugin.FileName);
                    
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
                    
                    // Verifica checksum solo se non è TBD
                    if (plugin.Checksum != "sha256:TBD" && !string.IsNullOrEmpty(plugin.Checksum))
                    {
                        if (!await VerifyChecksumAsync(filePath, plugin.Checksum))
                        {
                            File.Delete(filePath);
                            throw new Exception($"Checksum verification failed for {plugin.Name}");
                        }
                    }
                    
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error downloading plugin: {ex.Message}");
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
                    var hash = await Task.Run(() => sha256.ComputeHash(stream));
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
        /// Carica un DLL plugin in memoria (hot-reload)
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
                System.Diagnostics.Debug.WriteLine($"Error loading plugin: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Ottiene lista di plugin installati
        /// </summary>
        public List<InstalledPlugin> GetInstalledPlugins()
        {
            var installed = new List<InstalledPlugin>();
            
            try
            {
                if (!Directory.Exists(_pluginDirectory))
                    return installed;

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
        /// Verifica e risolve dipendenze plugin
        /// </summary>
        public List<PluginInfo> ResolveDependencies(PluginInfo plugin, PluginManifest manifest)
        {
            var dependencies = new List<PluginInfo> { plugin };
            var allPlugins = new List<PluginInfo>();
            allPlugins.AddRange(manifest.Documents ?? new List<PluginInfo>());
            allPlugins.AddRange(manifest.Reports ?? new List<PluginInfo>());
            
            foreach (var depId in plugin.Dependencies ?? new List<string>())
            {
                var dep = allPlugins.FirstOrDefault(p => p.Id == depId);
                if (dep != null && !dependencies.Any(d => d.Id == depId))
                {
                    dependencies.AddRange(ResolveDependencies(dep, manifest));
                }
            }
            
            return dependencies;
        }

        /// <summary>
        /// Uninstalla un plugin
        /// </summary>
        public bool UninstallPlugin(string pluginId)
        {
            try
            {
                var pluginFile = Directory.GetFiles(_pluginDirectory)
                    .FirstOrDefault(f => Path.GetFileName(f).Contains(pluginId));
                
                if (pluginFile != null)
                {
                    File.Delete(pluginFile);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error uninstalling plugin: {ex.Message}");
                return false;
            }
        }
    }
}
