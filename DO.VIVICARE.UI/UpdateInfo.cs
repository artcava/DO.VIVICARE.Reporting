using Newtonsoft.Json;

namespace DO.VIVICARE.UI
{
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
}
