using DO.VIVICARE.Reporter;
using System.IO;
using System.Linq;

namespace DO.VIVICARE.Report.Valorizzazione
{
    /// <summary>
    /// 
    /// </summary>
    [ReportReference(Name = "Valorizzazione", Description = "Report Valorizzazione ADI Lazio")]
    public class Valorizzazione:BaseReport
    {
        /// <summary>
        /// 
        /// </summary>
        public Valorizzazione()
        {
            DocumentNames = new string[] { "LazioHealthWorker", "ADIAltaIntensita" };
        }
        /// <summary>
        /// 
        /// </summary>
        public override void CreateParameters()
        {
            Parameters.Clear();
        }
    }
}
