using System.Collections.Generic;
using Newtonsoft.Json;

namespace DO.VIVICARE.UI
{
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
}
