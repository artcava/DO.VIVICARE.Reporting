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

        public List<BaseDocument> GetData()
        {
            List<BaseDocument> ret = new List<BaseDocument>();
            var name = string.Empty;
            var list = new List<Tuple<string, string, string>>();
            try
            {
                if (string.IsNullOrEmpty(SourceFilePath))
                {
                    list.Add(Tuple.Create($"Riga: 0", $"Colonna: 0", $"File inesistente o campo [SourceFilePath] vuoto"));
                    return null;
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
                    var fields = element.GetType().GetProperties();
                    var colField = 0;
                    foreach (var col in columns)
                    {
                        if (rangeXls.Cells[i, col.Position] == null || rangeXls.Cells[i, col.Position].Value == null)
                        {
                            list.Add(Tuple.Create($"Riga: {i}", $"Colonna: {col.Column}", $"Colonna inesistente o campo vuoto"));

                            var nameField = fields[colField].Name;
                            var propField = element.GetType().GetProperty(nameField);
                            switch (propField.PropertyType.FullName)
                            {
                                case "System.String":
                                    propField.SetValue(element, "");
                                    break;
                                case "System.Int32":
                                    propField.SetValue(element, 0);
                                    break;
                                case "System.Int64":
                                    propField.SetValue(element, 0);
                                    break;
                                case "System.Decimal":
                                    propField.SetValue(element, 0);
                                    break;
                                case "System.Double":
                                    propField.SetValue(element, 0);
                                    break;
                                case "System.Boolean":
                                    propField.SetValue(element, false);
                                    break;
                                case "System.DateTime":
                                    propField.SetValue(element, new DateTime(1,1,1));
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            var nameField = fields[colField].Name;
                            var propField = element.GetType().GetProperty(nameField);
                            object value = rangeXls.Cells[i, col.Position].Value;
                            if (propField.PropertyType.FullName == "System.Decimal" && 
                                (value.GetType().FullName == "System.Double" || 
                                 value.GetType().FullName == "System.Int32" ||
                                 value.GetType().FullName == "System.Int64" ||
                                 value.GetType().FullName == "System.String"
                                 ))
                            {
                                //var doubleValue = (double)value;
                                var decimalValue = Convert.ToDecimal(value);
                                propField.SetValue(element, decimalValue);
                            }
                            else propField.SetValue(element, value);
                        }
                        colField++;
                    }
                    ret.Add(element);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Marshal.ReleaseComObject(rangeXls);
                Marshal.ReleaseComObject(foglioXls);

                cartellaXls.Close();
                Marshal.ReleaseComObject(cartellaXls);

                appXls.Quit();
                Marshal.ReleaseComObject(appXls);
                
            }
            catch (Exception ex)
            {
                list.Add(Tuple.Create("Riga: 0", "Colonna: 0", $"Errore interno: {ex.Message}"));
                return null;
            }
            finally
            {
                WriteLog(list, name);
            }
            return ret;
        }

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
                    var fields = element.GetType().GetProperties();
                    var colField = 0;
                    foreach (var col in columns)
                    {
                        if (rangeXls.Cells[i, col.Position] == null || rangeXls.Cells[i, col.Position].Value == null)
                        {
                            list.Add(Tuple.Create($"Riga: {i}", $"Colonna: {col.Column}", $"Colonna inesistente o campo vuoto"));

                            var nameField = fields[colField].Name;
                            var propField = element.GetType().GetProperty(nameField);
                            SetDefault(element, propField);
                        }
                        else
                        {
                            var nameField = fields[colField].Name;
                            var propField = element.GetType().GetProperty(nameField);
                            object value = rangeXls.Cells[i, col.Position].Value;
                            SetValue(element, propField, value); 
                        }
                        colField++;
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

        public bool LoadRecordsOld()
        {
            var name = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(SourceFilePath))
                {
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
                    this.GetType();//Creare una nuova istanza dell'oggetto che rappresenta la classe figlia.
                    foreach (var col in columns)
                    {
                        if (rangeXls.Cells[i, col.Position] == null || rangeXls.Cells[i, col.Position].Value == null)
                            return false;
                        // Associare il valore della colonna al membro corrispondente dell'oggetto istanziato prima
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

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void SetValue(BaseDocument el, PropertyInfo p, object value)
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

        private void SetDefault(BaseDocument el, PropertyInfo p)
        {
            switch (p.PropertyType.FullName)
            {
                case "System.String":
                    p.SetValue(el, "");
                    break;
                case "System.Int32":
                    p.SetValue(el, 0);
                    break;
                case "System.Int64":
                    p.SetValue(el, 0);
                    break;
                case "System.Decimal":
                    p.SetValue(el, 0);
                    break;
                case "System.Double":
                    p.SetValue(el, 0);
                    break;
                case "System.Boolean":
                    p.SetValue(el, false);
                    break;
                case "System.DateTime":
                    p.SetValue(el, new DateTime(1, 1, 1));
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

                var buffer = GetBytes(text);

                f.Write(buffer, 0, buffer.Length);
                f.Close();
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
