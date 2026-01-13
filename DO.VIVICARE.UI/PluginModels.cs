using System.Collections.Generic;

namespace DO.VIVICARE.UI
{
    /// <summary>
    /// Rappresenta il manifest.json con la lista di tutti i plugin disponibili
    /// </summary>
    public class PluginManifest
    {
        public AppInfo App { get; set; }
        public List<PluginInfo> Documents { get; set; }
        public List<PluginInfo> Reports { get; set; }
    }

    /// <summary>
    /// Informazioni sull'applicazione principale
    /// </summary>
    public class AppInfo
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string LastUpdate { get; set; }
    }

    /// <summary>
    /// Informazioni su un singolo plugin
    /// </summary>
    public class PluginInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public long Size { get; set; }
        public string Checksum { get; set; }
        public string DownloadUrl { get; set; }
        public string FileName { get; set; }
        public List<string> Dependencies { get; set; }
        public bool Required { get; set; }

        /// <summary>
        /// Helper: formatta size leggibile
        /// </summary>
        public string FormattedSize
        {
            get
            {
                if (Size < 1024) return $"{Size} B";
                if (Size < 1024 * 1024) return $"{Size / 1024} KB";
                return $"{Size / (1024 * 1024)} MB";
            }
        }
    }

    /// <summary>
    /// Plugin già installato localmente
    /// </summary>
    public class InstalledPlugin
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string FilePath { get; set; }
        public long FileSize { get; set; }

        public string FormattedSize
        {
            get
            {
                if (FileSize < 1024) return $"{FileSize} B";
                if (FileSize < 1024 * 1024) return $"{FileSize / 1024} KB";
                return $"{FileSize / (1024 * 1024)} MB";
            }
        }
    }

    /// <summary>
    /// Progress del download di un plugin
    /// </summary>
    public class DownloadProgress
    {
        public string PluginId { get; set; }
        public long BytesDownloaded { get; set; }
        public long TotalBytes { get; set; }
        public int PercentComplete { get; set; }
    }
}
