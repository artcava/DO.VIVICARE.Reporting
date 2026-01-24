using System;

namespace DO.VIVICARE.UI
{
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
}
