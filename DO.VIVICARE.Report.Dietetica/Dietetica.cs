using DO.VIVICARE.Reporter;
using System.IO;
using System.Linq;
using System.Reflection;

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
            string[] docs = new string[] { "Report16" };
            FileInfo assemblyFile = new FileInfo(Assembly.GetExecutingAssembly().Location);
            foreach (var file in Directory.GetFiles(assemblyFile.Directory.FullName))
                //foreach (var file in Directory.GetFiles(assemblyFile.Directory.FullName + "\\Documents"))
                {
                    FileInfo f = new FileInfo(file);

                if (f.Exists && f.Name.StartsWith("DO.VIVICARE"))
                {
                    var a = Assembly.LoadFile(f.FullName);
                    foreach (var d in docs)
                    {
                        var obj = (from type in a.GetExportedTypes()
                                   where type.Name.Equals(d)
                                   select (BaseDocument)a.CreateInstance(type.FullName)).FirstOrDefault();
                        Documents.Add(obj);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Execute()
        {
            base.Execute();
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
