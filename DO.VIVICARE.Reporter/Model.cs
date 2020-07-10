using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public BaseDocument()
        {
            Records = new List<BaseDocument>();
        }

        public string SourceFilePath { get; set; }
        
        public string AttributeName { get; set; }

        public List<BaseDocument> Records { get; }
        
        public bool LoadRecords()
        {
            Records.Clear();
            var name = string.Empty;
            var list = new List<Tuple<string, string, string>>();
            try
            {
                if (string.IsNullOrEmpty(SourceFilePath))
                {
                    list.Add(Tuple.Create($"Riga: 0", $"Colonna: 0", $"File inesistente o campo [SourceFilePath] vuoto"));
                    return false;
                }
                Excel.Application appXls = new Excel.Application();
                Excel.Workbook cartellaXls = appXls.Workbooks.Open(SourceFilePath);
                Excel._Worksheet foglioXls = cartellaXls.Sheets[1];
                Excel.Range rangeXls = foglioXls.UsedRange;

                int rowCount = rangeXls.Rows.Count;
                int colCount = rangeXls.Columns.Count;

                var columns = Manager.GetDocumentColumns(this);
                int rowStart = 1;
                var ua = (DocumentReferenceAttribute)GetType().GetCustomAttribute(typeof(DocumentReferenceAttribute));
                if (ua != null)
                {
                    rowStart = ua.RowStart;
                    name = ua.Name;
                }
                for (int i = rowStart; i <= rowCount; i++)
                {
                    var type = this.GetType();

                    Assembly assembly = Assembly.GetAssembly(this.GetType());
                    var o = assembly.CreateInstance(type.FullName);
                    var element = (BaseDocument)o;
                    //var fields = element.GetType().GetProperties().Where(x=>x.GetCustomAttribute(typeof(DocumentMemberReferenceAttribute), false)!=null).ToList();
                    foreach (var col in columns)
                    {
                        var nameField = col.FieldName;
                        var propField = element.GetType().GetProperty(nameField);
                        if (rangeXls.Cells[i, col.Position] == null || rangeXls.Cells[i, col.Position].Value == null)
                        {
                            list.Add(Tuple.Create($"Riga: {i}", $"Colonna: {col.Column}", $"Colonna inesistente o campo vuoto"));

                            //var field = fields.FirstOrDefault(x => ((DocumentMemberReferenceAttribute)x.GetCustomAttribute(typeof(DocumentMemberReferenceAttribute), false)).Position == col.Position);
                            //if (field!=null)
                            //{
                            //    var nameField = field.Name;
                            //    var propField = element.GetType().GetProperty(nameField);
                            //    SetDefault(element, propField);
                            //}
                           
                            SetDefault(element, propField);
                        }
                        else
                        {
                            //var field = fields.FirstOrDefault(x => ((DocumentMemberReferenceAttribute)x.GetCustomAttribute(typeof(DocumentMemberReferenceAttribute), false)).Position == col.Position);
                            //if (field != null)
                            //{
                            //    var nameField = field.Name;
                            //    var propField = element.GetType().GetProperty(nameField);
                            //    object value = rangeXls.Cells[i, col.Position].Value;
                            //    SetValue(element, propField, value);
                            //}
                           
                            object value = rangeXls.Cells[i, col.Position].Value;
                            SetValue(element, propField, value);
                        }
                    }
                    Records.Add(element);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Marshal.ReleaseComObject(rangeXls);
                Marshal.ReleaseComObject(foglioXls);

                cartellaXls.Close();
                Marshal.ReleaseComObject(cartellaXls);

                appXls.Quit();
                Marshal.ReleaseComObject(appXls);

                return true;
            }
            catch (Exception ex)
            {
                list.Add(Tuple.Create("Riga: 0", "Colonna: 0", $"Errore interno: {ex.Message}"));
                return false;
            }
            finally
            {
                WriteLog(list, name);
            }
        }
        
        public bool CheckFields(IProgress<int> progress)
        {
            var name = string.Empty;
            var list = new List<Tuple<string, string, string>>();
            try
            {
                if (string.IsNullOrEmpty(SourceFilePath))
                {
                    list.Add(Tuple.Create($"Riga: 0", $"Colonna: 0", $"File inesistente o campo [SourceFilePath] vuoto"));
                    return false;
                }
                Excel.Application appXls = new Excel.Application();
                Excel.Workbook cartellaXls = appXls.Workbooks.Open(SourceFilePath);
                Excel._Worksheet foglioXls = cartellaXls.Sheets[1];
                Excel.Range rangeXls = foglioXls.UsedRange;

                int rowCount = rangeXls.Rows.Count;
                int colCount = rangeXls.Columns.Count;

                var columns = Manager.GetDocumentColumns(this);
                int rowStart = 1;
                var ua = (DocumentReferenceAttribute)GetType().GetCustomAttribute(typeof(DocumentReferenceAttribute));
                if (ua != null)
                {
                    rowStart = ua.RowStart;
                    name = ua.Name;
                }


                // Excel comincia a contare da 1
                for (int i = rowStart; i <= rowCount; i++)
                {
                    foreach (var col in columns)
                    {
                        if (rangeXls.Cells[i, col.Position] == null || rangeXls.Cells[i, col.Position].Value == null)
                            list.Add(Tuple.Create($"Riga: {i}", $"Colonna: {col.Column}", $"Colonna inesistente o campo vuoto"));
                    }
                    progress.Report(rowCount / i); // PER TENER TRACCIA DELLO STATO
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Marshal.ReleaseComObject(rangeXls);
                Marshal.ReleaseComObject(foglioXls);

                cartellaXls.Close();
                Marshal.ReleaseComObject(cartellaXls);

                appXls.Quit();
                Marshal.ReleaseComObject(appXls);

                return true;
            }
            catch (Exception ex)
            {
                list.Add(Tuple.Create("Riga: 0", "Colonna: 0", $"Errore interno: {ex.Message}"));
                return false;
            }
            finally
            {
                WriteLog(list, name);
            }
        }
        
        private void SetValue(BaseDocument el, PropertyInfo p, object value)
        {
            try
            {
                if (p.PropertyType.FullName == "System.Decimal" &&
                                (value.GetType().FullName == "System.Double" ||
                                 value.GetType().FullName == "System.Int32" ||
                                 value.GetType().FullName == "System.Int64" ||
                                 value.GetType().FullName == "System.String"
                                 ))
                {
                    var decimalValue = Convert.ToDecimal(value);
                    p.SetValue(el, decimalValue);
                }
                else if (p.PropertyType.FullName == "System.Int32" &&
                                    (value.GetType().FullName == "System.Double" ||
                                     value.GetType().FullName == "System.Decimal" ||
                                     value.GetType().FullName == "System.Int64" ||
                                     value.GetType().FullName == "System.String"
                                     ))
                {
                    var int32Value = Convert.ToInt32(value);
                    p.SetValue(el, int32Value);
                }
                else if (p.PropertyType.FullName == "System.Int64" &&
                                    (value.GetType().FullName == "System.Double" ||
                                     value.GetType().FullName == "System.Decimal" ||
                                     value.GetType().FullName == "System.Int32" ||
                                     value.GetType().FullName == "System.String"
                                     ))
                {
                    var int64Value = Convert.ToInt64(value);
                    p.SetValue(el, int64Value);
                }
                else if (p.PropertyType.FullName == "System.Double" &&
                                    (value.GetType().FullName == "System.Int64" ||
                                     value.GetType().FullName == "System.Decimal" ||
                                     value.GetType().FullName == "System.Int32" ||
                                     value.GetType().FullName == "System.String"
                                     ))
                {
                    var intDoubleValue = Convert.ToDouble(value);
                    p.SetValue(el, intDoubleValue);
                }
                else if (p.PropertyType.FullName == "System.String" &&
                                    (value.GetType().FullName == "System.Int64" ||
                                     value.GetType().FullName == "System.Decimal" ||
                                     value.GetType().FullName == "System.Int32" ||
                                     value.GetType().FullName == "System.Double"
                                     ))
                {
                    var stringValue = Convert.ToString(value);
                    p.SetValue(el, stringValue);
                }
                else p.SetValue(el, value);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void SetDefault(BaseDocument el, PropertyInfo p)
        {
            switch (p.PropertyType.FullName)
            {
                case "System.String":
                    SetValue(el, p, "");
                    break;
                case "System.Int32":
                    SetValue(el, p, 0);
                    break;
                case "System.Int64":
                    SetValue(el, p, 0);
                    break;
                case "System.Decimal":
                    SetValue(el, p, 0);
                    break;
                case "System.Double":
                    SetValue(el, p, 0);
                    break;
                case "System.Boolean":
                    SetValue(el, p, false);
                    break;
                case "System.DateTime":
                    SetValue(el, p, new DateTime(1, 1, 1));
                    break;
                default:
                    break;
            }
        }

        private void WriteLog(List<Tuple<string, string, string>> tuples, string fileName)
        {
            try
            {
                var path = Path.Combine(Manager.Documents, fileName + ".log");
                var f = new FileStream(path, FileMode.Create);
                string text = null;


                foreach (var tuple in tuples)
                {
                    text += tuple.ToString() + "\r\n";
                }

                if (text != null)
                {
                    var buffer = GetBytes(text);
                    f.Write(buffer, 0, buffer.Length);
                    f.Close();
                }
            }
            catch { }
        }
        /// <summary>
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static byte[] GetBytes(string str)
        {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }

    /// <summary>
    /// Attributo a livello di classe per indicare a quale file facciamo riferimento
    /// </summary>
    public class DocumentReferenceAttribute : Attribute
    {
        public string Name { get; set; }
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
        //08-07-2020
        //Penso che bisognerebbe aggiungere anche FieldName per poter identificare in modo diretto
        //il campo della classe BaseDocument
        public string FieldName { get; set; }
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

        public List<BaseReport> ResultRecords { get; }

        //public string DestinationFilePath { get; set; }

        //public virtual void LoadDocuments(bool withRecords = false) { }

        protected string[] DocumentNames { get; set; }

        public virtual void LoadDocuments(bool withRecords = false)
        {
            Documents.Clear();
            foreach (var document in Manager.GetDocuments())
            {
                if (!DocumentNames.Contains(document.Attribute.Name)) continue;

                var list = Manager.Settings.GetDocumentValues(XMLSettings.LibraryType.Document, document.Attribute.Name);
                document.Document.SourceFilePath = "";
                if (list != null)
                {
                    document.Document.SourceFilePath = Path.Combine(Manager.Documents, list[0] + list[1]);
                }

                document.Document.AttributeName = document.Attribute.Name;

                if (withRecords) document.Document.LoadRecords();
                Documents.Add(document.Document);
            }
        }

        public virtual void Execute() { }

        public BaseReport()
        {
            Documents = new List<BaseDocument>();
            ResultRecords = new List<BaseReport>();
        }
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
        public int DecimalDigits { get; set; }
        public bool IsDate { get; set; }
        public string ColumnName { get; set; }
        public bool Required { get; set; }
        public string FillValue { get; set; }
        public Alignment FillAlignment { get; set; }
        //08-07-2020
        //Penso che bisognerebbe aggiungere anche FieldName per poter identificare in modo diretto
        //il campo della classe BaseDocument
        public string FieldName { get; set; }
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
