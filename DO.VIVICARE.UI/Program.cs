using System;
using System.IO;
using System.Windows.Forms;

namespace DO.VIVICARE.UI
{
    static class Program
    {
        static MDIParent Global_formPadre;

        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ManageFolders();

            Global_formPadre = new MDIParent();
            Application.Run(Global_formPadre);

        }

        private static void ManageFolders()
        {
            try
            {
                var root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Reporting");
                if (!Directory.Exists(root)) Directory.CreateDirectory(root);

                var docLibraries = Path.Combine(root, "DocumentLibraries");
                if (!Directory.Exists(docLibraries)) Directory.CreateDirectory(docLibraries);

                var repLibraries = Path.Combine(root, "ReportLibraries");
                if (!Directory.Exists(repLibraries)) Directory.CreateDirectory(repLibraries);

                var doc = Path.Combine(root, "Documents");
                if (!Directory.Exists(doc)) Directory.CreateDirectory(doc);

                var rep = Path.Combine(root, "Reports");
                if (!Directory.Exists(rep)) Directory.CreateDirectory(rep);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }
    }
}
