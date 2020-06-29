using DO.VIVICARE.Reporter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DO.VIVICARE.Report.Dietetica
{
    /// <summary>
    /// 
    /// </summary>
    [ReportReference(Name = "Dietetica", Description = "Report inerente il consuntivo dietetica")]
    public class Dietetica : BaseReport
    {
        /// <summary>
        /// 
        /// </summary>
        public Dietetica()
        {
            string[] docs = new string[] { "ASST", "Comuni", "Report16", "Report18", "ZSDFatture" };
            foreach (var document in Manager.GetDocuments())
            {
                if (!docs.Contains(document.Attribute.Name)) continue;
                Documents.Add(document.Document);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Execute()
        {
            //base.Execute();

            try
            {
                //Retrieve documents with Object Document and Attributes
                var documents = Manager.GetDocuments();

                var doc = documents.Find(x => x.Attribute.Name == "ZSDFatture");
                if (doc == null)
                {
                    throw new Exception("ZSDFatture non trovato!");
                }
                var listZSDFatture = doc.Document.GetData();

                doc = documents.Find(x => x.Attribute.Name == "Report16");
                if (doc == null)
                {
                    throw new Exception("Report16 non trovato!");
                }
                var listReport16 = doc.Document.GetData();

                var report = listZSDFatture.Join(
                    listReport16, 
                    r => new KeyValuePair<object, object>(r.GetType().GetProperty("FiscalCode"), r.GetType().GetProperty("ErogationDate")),
                    f => new KeyValuePair<object, object>(f.GetType().GetProperty("FiscalCode"), f.GetType().GetProperty("ErogationDate")),
                    (f,r) => new Dietetica
                    {
                        ATSCode = "",
                        ASSTCode = "",
                        FiscalCode = (string)r.GetType().GetProperty("FiscalCode").GetValue(r),
                        Year = "",
                        Month = ""
                    });

                foreach (var item in report)
                {

                }
                

            }
            catch (System.Exception)
            {

                throw;
            }
        }

        #region Members
        [ReportMemberReference(Column = "A", Position = 1, ColumnName = "ATS", Length = 3, Required = true)]
        public string ATSCode { get; set; }

        [ReportMemberReference(Column = "B", Position = 2, ColumnName = "ASST", Length = 6, Required = true)]
        public string ASSTCode { get; set; }

        [ReportMemberReference(Column = "C", Position = 3, ColumnName = "Tipo record", Length = 1)]
        public string PlanType { get { return "I"; } }

        [ReportMemberReference(Column = "D", Position = 4, ColumnName = "Anno", Length = 4)]
        public string Year { get; set; }

        [ReportMemberReference(Column = "E", Position = 5, ColumnName = "Mese", Length = 2)]
        public string Month { get; set; }

        [ReportMemberReference(Column = "F", Position = 6, ColumnName = "Codice Fiscale", Length = 16, Required = true)]
        public string FiscalCode { get; set; }
        #endregion
    }
}
