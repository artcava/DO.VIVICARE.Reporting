using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace DO.VIVICARE.Reporter
{
    /// <summary>
    /// Classe base per la gestione dei Documenti
    /// </summary>
    public class BaseDocument
    {
        public string UserPathReport { get; set; }

        public List<Tuple<string, string,string>> CheckFields()
        {
            var list = new List<Tuple<string, string, string>>();
            if (string.IsNullOrEmpty(UserPathReport))
            {
                list.Add(Tuple.Create($"Riga: 0", $"Colonna: 0", $"File inesistente o campo [UserPathReport] vuoto"));
            }
            try
            {
                Excel.Application appXls = new Excel.Application();
                Excel.Workbook cartellaXls = appXls.Workbooks.Open(UserPathReport);
                Excel._Worksheet foglioXls = cartellaXls.Sheets[1];
                Excel.Range rangeXls = foglioXls.UsedRange;

                int rowCount = rangeXls.Rows.Count;
                int colCount = rangeXls.Columns.Count;

                var columns = Manager.GetDocumentColumns(this);
                int rowStart = 1;
                var ua = (DocumentReferenceAttribute)GetType().GetCustomAttribute(typeof(DocumentReferenceAttribute));
                if (ua != null)
                    rowStart = ua.RowStart;


                // Excel comincia a contare da 1
                for (int i = rowStart; i <= rowCount; i++)
                {
                    foreach(var col in columns)
                    {
                        if (rangeXls.Cells[i, col.Position] == null || rangeXls.Cells[i, col.Position].Value == null)
                            list.Add(Tuple.Create($"Riga: {i}", $"Colonna: {col.Column}", $"Colonna inesistente o campo vuoto"));
                    }
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Marshal.ReleaseComObject(rangeXls);
                Marshal.ReleaseComObject(foglioXls);

                cartellaXls.Close();
                Marshal.ReleaseComObject(cartellaXls);

                appXls.Quit();
                Marshal.ReleaseComObject(appXls);

                return list;


                // QUESTO PEZZO GLIELO METTI AL CHIAMANTE DI QUESTA FUNZIONE
                //=======================================================================================
                //var res = CheckFields("pathxxxxxxxx");
                //if (res.Count != 0)
                //{
                //    var msg = "CHECK VALUE!";
                //    foreach (var m in res)
                //    {
                //        msg += "\n" + m;
                //    }
                //    MessageBox.Show("xxxxx", msg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //}
                //=======================================================================================

            }
            catch (Exception ex)
            {
                list.Add(Tuple.Create("Riga: 0", "Colonna: 0", $"Errore interno: {ex.Message}"));
                return list;
            }
        }
    }

    /// <summary>
    /// Attributo a livello di classe per indicare a quale file facciamo riferimento
    /// </summary>
    public class DocumentReferenceAttribute : Attribute
    {
        public string Name { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public int RowStart { get; set; }

        public DocumentReferenceAttribute()
        {
            RowStart = 1;
        }
    }
    /// <summary>
    /// Attributo a livello di membro per indicare la colonna da cui prelevare i dati
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DocumentMemberReferenceAttribute : Attribute
    {
        public string Column { get; set; }
        public int Position { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ReportingDocument
    {
        /// <summary>
        /// 
        /// </summary>
        public BaseDocument Document { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DocumentReferenceAttribute Attribute { get; set; }
    }
    /// <summary>
    /// Classe base per la gestione dei Report
    /// </summary>
    public class BaseReport
    {
        public List<BaseDocument> Documents { get; }

        public virtual void Execute() { }
    }
    /// <summary>
    /// Attributo a livello di classe per indicare a quale file facciamo riferimento
    /// </summary>
    public class ReportReferenceAttribute : Attribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
    /// <summary>
    /// Attributo a livello di membro per indicare la colonna da cui prelevare i dati
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ReportMemberReferenceAttribute : Attribute
    {
        public string Column { get; set; }
        public int Position { get; set; }
        public int Length { get; set; }
        public string ColumnName { get; set; }
        public bool Required { get; set; }
        public string FillValue { get; set; }
        public Alignment FillAlignment { get; set; }
    }
    /// <summary>
    /// Direzione dell'allineamento
    /// </summary>
    public enum Alignment
    {
        Right=0,
        Left=1
    }
}
