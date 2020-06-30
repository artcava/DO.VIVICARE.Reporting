using DO.VIVICARE.Reporter;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
            string[] docs = new string[] { "ASST", "ZSDFatture" };
            foreach (var document in Manager.GetDocuments())
            {
                if (!docs.Contains(document.Attribute.Name)) continue;
                //SourceFilePath recuperare da Settings
                var list = Manager.Settings.GetDocumentValues(XMLSettings.LibraryType.Document, document.Attribute.Name);
                document.Document.SourceFilePath = "";
                if (list!=null)
                {
                    if (list[2]!=null)
                    {
                        document.Document.SourceFilePath = list[2];
                    }
                }
                document.Document.AttributeName = document.Attribute.Name;
                if (document.Document.LoadRecords())
                {
                    Documents.Add(document.Document);
                }   
            }
        }

        public Dietetica(bool value)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        public override void Execute()
        {
            //base.Execute();

            try
            {
                
                var documents = Documents;

                var doc = documents.Find(x => x.AttributeName == "ZSDFatture");
                if (doc == null)
                {
                    throw new Exception("ZSDFatture non trovato!");
                }
                var listZSDFatture = doc.Records;
                if (listZSDFatture == null)
                {
                    throw new Exception("ZSDFatture non caricato!");
                }
                //doc = documents.Find(x => x.AttributeName == "Report16");
                //if (doc == null)
                //{
                //    throw new Exception("Report16 non trovato!");
                //}
                //var listReport16 = doc.Records;
                //if (listReport16 == null)
                //{
                //    throw new Exception("Report16 non caricato!");
                //}
                doc = documents.Find(x => x.AttributeName == "ASST");
                if (doc == null)
                {
                    throw new Exception("ASST non trovato!");
                }
                var listASST = doc.Records;
                if (listASST == null)
                {
                    throw new Exception("ASST non caricato!");
                }
                
                var report = listZSDFatture.Select((dynamic f) => new { ZSDF = f, ASST = listASST.Where((dynamic a) => a.SAPCode == f.Customer) }).
                    Select((dynamic fa) => new Dietetica(false)
                    {
                        ATSCode = ((IEnumerable<dynamic>)fa.ASST).FirstOrDefault().ATSCode.ToString(),
                        ASSTCode = ((IEnumerable<dynamic>)fa.ASST).FirstOrDefault().ASSTCode.ToString(),
                        Year = _year.ToString("0000"),
                        Month = _month.ToString("00"),
                        FiscalCode = fa.ZSDF.FiscalCode
                    }).
                    ToList();

                ResultRecords.AddRange(report);
            }
            catch (System.Exception ex)
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
