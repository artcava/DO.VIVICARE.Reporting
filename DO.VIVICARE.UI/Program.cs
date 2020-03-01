using System;
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

            //Application.Run(new MDIParent());
            Global_formPadre = new MDIParent();
            Application.Run(Global_formPadre);

        }
    }
}
