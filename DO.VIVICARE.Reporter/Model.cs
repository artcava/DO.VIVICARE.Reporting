using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using Tuple = System.Tuple;

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
            Filters = new List<FilterDocument>();
        }

        public List<FilterDocument> Filters { get; set; }

        public string SourceFilePath { get; set; }

        public string AttributeName { get; set; }

        public List<BaseDocument> Records { get; }

        public bool LoadRecords()
        {
            Records.Clear();
            var name = string.Empty;
            var list = new List<Tuple<string, string, string>>();
            ExcelManager manExcel = null;
            try
            {
                int rowStart = 1;
                var ua = (DocumentReferenceAttribute)GetType().GetCustomAttribute(typeof(DocumentReferenceAttribute));
                if (ua != null)
                {
                    rowStart = ua.RowStart;
                    name = ua.Name;
                }

                if (string.IsNullOrEmpty(name))
                {
                    list.Add(Tuple.Create($"Riga: 0", $"Colonna: 0", $"NameClass vuoto"));
                    return false;
                }

                if (string.IsNullOrEmpty(SourceFilePath))
                {
                    list.Add(Tuple.Create($"Riga: 0", $"Colonna: 0", $"File inesistente o campo [SourceFilePath] vuoto"));
                    return false;
                }

                manExcel = new ExcelManager();
                if (!manExcel.LoadFile(SourceFilePath, name))
                {
                    list.Add(Tuple.Create($"Riga: 0", $"Colonna: 0", $"File {SourceFilePath} non caricato!"));
                    return false;
                }

                if (manExcel.Extension.ToLower() == ".xlsx") manExcel.Open(false, SourceFilePath);
                else manExcel.Open(false);

                var columns = Manager.GetDocumentColumns(this);

                IEnumerable<Row> rows = null;

                // IEnumerable<Row> rows = manExcel.GetRows(this.Filters).Skip(rowStart - 1);
                if (this.Filters != null)
                {
                    if (this.Filters.Count()>0)
                    {
                        rows = manExcel.GetRows(this.Filters);
                    }
                    else
                    {
                        rows = manExcel.GetRows(null).Skip(rowStart - 1);
                    }                   
                }
                else
                {
                    rows = manExcel.GetRows(null).Skip(rowStart - 1);
                }
                

                var type = this.GetType();
                Assembly assembly = Assembly.GetAssembly(this.GetType());

                foreach (var row in rows)
                {
                    var o = assembly.CreateInstance(type.FullName);
                    var element = (BaseDocument)o;
                    var cells = row.Descendants<Cell>();
                    foreach (var col in columns)
                    {
                        var nameField = col.FieldName;
                        var propField = element.GetType().GetProperty(nameField);
                        var cell = cells.FirstOrDefault(c => manExcel.GetColumnName(c.CellReference) == col.Column);
                        if (cell == null)
                        {
                            list.Add(Tuple.Create($"Riga: {row.RowIndex}", $"Colonna: {col.Column}", $"Colonna inesistente o campo vuoto"));
                            SetDefault(element, propField);
                        }
                        else
                        {
                            string value = manExcel.GetCellValue(cell);   
                            SetValue(element, propField, value);
                        }
                    }
                    Records.Add(element);
                }

                manExcel.Dispose();

                return true;
            }
            catch (Exception ex)
            {
                list.Add(Tuple.Create("Riga: 0", "Colonna: 0", $"Errore interno: {ex.Message}"));
                return false;
            }
            finally
            {
                if (manExcel != null) manExcel.Dispose();
                WriteLog(list, name);
            }
        }

        public bool LoadRecordsOLD()
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
            ExcelManager manExcel = null;
            try
            {
                int rowStart = 1;
                var ua = (DocumentReferenceAttribute)GetType().GetCustomAttribute(typeof(DocumentReferenceAttribute));
                if (ua != null)
                {
                    rowStart = ua.RowStart;
                    name = ua.Name;
                }
                if (string.IsNullOrEmpty(name))
                {
                    list.Add(Tuple.Create($"Riga: 0", $"Colonna: 0", $"NameClass vuoto"));
                    return false;
                }
                if (string.IsNullOrEmpty(SourceFilePath))
                {
                    list.Add(Tuple.Create($"Riga: 0", $"Colonna: 0", $"File inesistente o campo [SourceFilePath] vuoto"));
                    return false;
                }

                manExcel = new ExcelManager();
                if (!manExcel.LoadFile(SourceFilePath, name))
                {
                    list.Add(Tuple.Create($"Riga: 0", $"Colonna: 0", $"File {SourceFilePath} non caricato!"));
                    return false;
                }

                if (manExcel.Extension.ToLower() == ".xlsx") manExcel.Open(false, SourceFilePath);
                else manExcel.Open(false);
                
                var columns = Manager.GetDocumentColumns(this);

                IEnumerable<Row> rows = manExcel.GetRows(null).Skip(rowStart - 1);
                int rowCount = rows.Count();
                int i = rowStart;
                foreach (var row in rows)
                {
                    var cells = row.Descendants<Cell>();
                    if (cells!=null)
                    {
                        foreach (var col in columns)
                        {
                            var cell = cells.FirstOrDefault(c => manExcel.GetColumnName(c.CellReference) == col.Column);
                            if (cell == null)
                                list.Add(Tuple.Create($"Riga: {row.RowIndex}", $"Colonna: {col.Column}", $"Colonna inesistente o campo vuoto"));
                        }
                    }
                    //progress.Report(rowCount / i++); // PER TENER TRACCIA DELLO STATO
                }

              
                manExcel.Dispose();
                
                return true;
            }
            catch (Exception ex)
            {
                list.Add(Tuple.Create("Riga: 0", "Colonna: 0", $"Errore interno: {ex.Message}"));
                return false;
            }
            finally
            {
                if (manExcel != null) manExcel.Dispose();
                WriteLog(list, name);
            }
        }

        public bool CheckFieldsOLD(IProgress<int> progress)
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
                else if (p.PropertyType.FullName == "System.DateTime" &&
                                    (value.GetType().FullName == "System.String"
                                     ))
                {
                    var dtmValue = ConvertDate((string)value);
                    p.SetValue(el, dtmValue);
                }
                else p.SetValue(el, value);
            }
            catch (Exception ex)
            {

                //throw;
            }
        }

        private DateTime ConvertDate(string value)
        {
            DateTime ret = DateTime.MinValue;
            if (!DateTime.TryParse(value, out ret))
            {
                try
                {
                    double d = 0;
                    try
                    {
                        d = double.Parse(value, CultureInfo.InvariantCulture);
                    }
                    catch (Exception exDouble)
                    {
                        var err = exDouble;
                    }
                    ret = DateTime.FromOADate(d);
                }
                catch (Exception ex)
                {

                    throw new FormatException("Not valid format", ex);
                }
            }
            return ret;
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

    public class FilterDocument
    {
        public string Column { get; set; }
        public string Value { get; set; }
    }

    public class DataCombo
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public BaseDocument Data { get; set; }
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

        public List<ReportParameter> Parameters { get; }

        public virtual void CreateParameters() { }

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
            Parameters = new List<ReportParameter>();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ReportingReport
    {
        /// <summary>
        /// 
        /// </summary>
        public BaseReport Report { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ReportReferenceAttribute Attribute { get; set; }
    }

    public class ReportParameter
    {
        public string Description { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// value parameter type Int, Long, Decimal, String, Document(BaseDocument), Bool, DateTime
        /// </summary>
        public string Type { get; set; }
        public string DocumentName { get; set; }
        public string DocumentFieldText { get; set; }
        public string DocumentFieldId { get; set; }
        //public string DocumentValueText { get; set; }
        public string DocumentValueId { get; set; }
        public object ReturnValue { get; set; }
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
