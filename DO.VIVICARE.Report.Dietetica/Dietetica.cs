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
        private int _year;
        private int _month;

        public void SetYear(int year)
        {
            _year = year;
        }

        public void SetMonth(int month)
        {
            _month = month;
        }

        /// <summary>
        /// 
        /// </summary>
        public Dietetica()
        {
            //string[] docs = new string[] { "ASST", "Comuni", "Report16", "Report18", "ZSDFatture" };
            //foreach (var document in Manager.GetDocuments())
            //{
            //    if (!docs.Contains(document.Attribute.Name)) continue;
            //    Documents.Add(document.Document);
            //}
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
                doc.Document.SourceFilePath = "C:\\DoSrlDEV\\VIVICARE.Reporting\\ZSD_FATTURE GENNAIO.XLSX";
                var listZSDFatture = doc.Document.GetData();
                if (listZSDFatture == null)
                {
                    throw new Exception("ZSDFatture non caricato!");
                }
                doc = documents.Find(x => x.Attribute.Name == "Report16");
                if (doc == null)
                {
                    throw new Exception("Report16 non trovato!");
                }
                doc.Document.SourceFilePath = "C:\\DoSrlDEV\\VIVICARE.Reporting\\Report 16 Gennaio 2020.xls";
                var listReport16 = doc.Document.GetData();
                if (listReport16 == null)
                {
                    throw new Exception("Report16 non caricato!");
                }

                doc = documents.Find(x => x.Attribute.Name == "ASST");
                if (doc == null)
                {
                    throw new Exception("ASST non trovato!");
                }
                doc.Document.SourceFilePath = "C:\\DoSrlDEV\\VIVICARE.Reporting\\ASST.xlsx";
                var listASST = doc.Document.GetData();
                if (listASST == null)
                {
                    throw new Exception("ASST non caricato!");
                }

                var report = listZSDFatture
                    .Join(
                        listASST, 
                        f => f.GetType().GetProperty("Customer"),
                        a => a.GetType().GetProperty("SAPCode"),
                        (f, a) => new { f, a })
                    .Select(fa => new Dietetica
                    {
                        ATSCode = Manager.Left((string)fa.a.GetType().GetProperty("ATSCode").GetValue(fa.a),3,' '),
                        ASSTCode = Manager.Left((string)fa.a.GetType().GetProperty("ASSTCode").GetValue(fa.a), 3, ' '),
                        Year = _year.ToString("0000"),
                        Month = _month.ToString("00"),
                        FiscalCode = Manager.Left((string)fa.f.GetType().GetProperty("FiscalCode").GetValue(fa.f),16,' '),
                    });

                //var list = context.Packages
                //.Join(context.Containers, p => p.ContainerID, c => c.ID, (p, c) => new { p, c })
                //.Join(context.UserHasPackages, pc => pc.p.ID, u => u.PackageID, (pc, u) => new { pc.p, pc.c, u })
                //.Where(pcu => pcu.u.UserID == "SomeUser")
                //.Select(pcu => new
                //{
                //    pcu.p.ID,
                //    pcu.c.Name,
                //    pcu.p.Code,
                //    pcu.p.Code2
                //});
                //Left(d.Patient.Surname,40, ' ')
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

        //[ReportMemberReference(Column = "G", Position = 7, ColumnName = "Sesso", Length = 1, Required = true)]
        //public string Sex { get; set; }

        //[ReportMemberReference(Column = "H", Position = 8, ColumnName = "Data di nascita", Length = 8, Required = true)]
        //public string DateOfBirth { get; set; }

        //[ReportMemberReference(Column = "I", Position = 9, ColumnName = "Comune residenza", Length = 6, Required = true)]
        //public string ISTATCode { get; set; }

        //[ReportMemberReference(Column = "J", Position = 10, ColumnName = "Utente ospite RSA o RSD", Length = 1, Required = true)]
        //public string UserHost { get; set; }

        //[ReportMemberReference(Column = "K", Position = 11, ColumnName = "Numero Prescrizione", Length = 14)]
        //public string PrescriptionNumber { get; set; }

        #endregion
    }
}
