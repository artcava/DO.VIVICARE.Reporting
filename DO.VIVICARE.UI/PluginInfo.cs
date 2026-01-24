using System.Collections.Generic;
using Newtonsoft.Json;

namespace DO.VIVICARE.UI
{
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
}
