using Velopack;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;

namespace DO.VIVICARE.UI.Services
{
    public class UpdateService
    {
        public async Task CheckForUpdatesAsync()
        {
            try
            {
                var manager = new UpdateManager("https://github.com/artcava/DO.VIVICARE.Reporting");
                var update = await manager.CheckForUpdatesAsync();

                if (update != null)
                {
                    string currentVersion = update.BaseRelease?.Version.ToString() ?? "sconosciuta";
                    string targetVersion = update.TargetFullRelease.Version.ToString();

                    var result = MessageBox.Show(
                        $"Nuova versione disponibile: {targetVersion}\n\nTua versione: {currentVersion}\n\nScaricare e installare adesso?",
                        "Aggiornamento Disponibile",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                    );

                    if (result == DialogResult.Yes)
                    {
                        await manager.DownloadUpdatesAsync(update);
                        manager.ApplyUpdatesAndRestart(update);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Update check failed: {ex.Message}");
            }
        }
    }
}

