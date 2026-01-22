using System;
using System.IO;
using System.Net.Http;
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
            "https://github.com/artcava/DO.VIVICARE.Reporting/releases/download/latest";

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
                        if (manifest.assets != null && manifest.assets.Count > 0)
                        {
                            var uiAsset = manifest.assets[0];

                            var downloadUrl = $"{GITHUB_RELEASES_URL}/{uiAsset.file}";

                            return new UpdateInfo
                            {
                                CurrentVersion = currentVersion,
                                AvailableVersion = availableVersion,
                                ReleaseDate = manifest.releaseDate?.ToString() ?? "Unknown",
                                DownloadUrl = downloadUrl,
                                Checksum = uiAsset.checksum?.ToString() ?? null
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
