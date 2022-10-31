using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Excel = Microsoft.Office.Interop.Excel;
using Tuple = System.Tuple;

namespace DO.VIVICARE.Reporter
{
    public static class Manager
    {
        public static string ReportingRoot { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Reporting"); } }
        public static string DocumentLibraries { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Reporting", "DocumentLibraries"); } }
        public static string ReportLibraries { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Reporting", "ReportLibraries"); } }
        public static string Documents { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Reporting", "Documents"); } }
        public static string Reports { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Reporting", "Reports"); } }
        /// <summary>
        /// 
        /// </summary>
        private static XMLSettings _settings = null;
        /// <summary>
        /// 
        /// </summary>
        public static XMLSettings Settings 
        { 
            get 
            {
                if (_settings == null) _settings = new XMLSettings();
                return _settings;
            } 
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<ReportingDocument> GetDocuments()
        {
            var list = new List<ReportingDocument>();
            foreach (var file in Directory.GetFiles(DocumentLibraries))
            {
                FileInfo f = new FileInfo(file);
                if (!f.Exists) continue;

                var a = Assembly.LoadFile(f.FullName);
                var objList = (from type in a.GetExportedTypes()
                               where type.BaseType.Name.Equals("BaseDocument")
                               select (BaseDocument)a.CreateInstance(type.FullName)).ToList();

                foreach (var obj in objList)
                {
                    var ua = (DocumentReferenceAttribute)obj.GetType().GetCustomAttribute(typeof(DocumentReferenceAttribute));
                    if (ua == null) continue;
                    list.Add(new ReportingDocument { Document = obj, Attribute = ua });
                    Settings.AddLibrary(XMLSettings.LibraryType.Document, ua.Name);
                }
            }
            return list;
        }
        /// <summary>
        /// Search for every BaseReport in the path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<ReportingReport> GetReports()
        {
            var list = new List<ReportingReport>();
            foreach (var file in Directory.GetFiles(ReportLibraries))
            {
                FileInfo f = new FileInfo(file);
                if (!f.Exists) continue;

                var a = Assembly.LoadFile(f.FullName);
                var objList = (from type in a.GetExportedTypes()
                               where type.BaseType.Name.Equals("BaseReport")
                               select (BaseReport)a.CreateInstance(type.FullName)).ToList();

                foreach (var obj in objList)
                {
                    var ua = (ReportReferenceAttribute)obj.GetType().GetCustomAttribute(typeof(ReportReferenceAttribute));
                    if (ua == null) continue;
                    list.Add(new ReportingReport { Report = obj, Attribute = ua });
                }
            }
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static List<DocumentMemberReferenceAttribute> GetDocumentColumns(BaseDocument document)
        {
            var list = new List<DocumentMemberReferenceAttribute>();
            foreach (var prop in document.GetType().GetProperties())
            {
                var fa = (DocumentMemberReferenceAttribute)prop.GetCustomAttribute(typeof(DocumentMemberReferenceAttribute), false);
                if (fa == null) continue;
                list.Add(fa);
            }
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public static List<ReportMemberReferenceAttribute> GetReportColumns(BaseReport report)
        {
            var list = new List<ReportMemberReferenceAttribute>();
            foreach (var prop in report.GetType().GetProperties())
            {
                var fa = (ReportMemberReferenceAttribute)prop.GetCustomAttribute(typeof(ReportMemberReferenceAttribute), false);
                if (fa == null) continue;
                list.Add(fa);
            }
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        public static List<ReportMemberReferenceAttribute> GetSheetReportColumns(BaseSheet sheet)
        {
            var list = new List<ReportMemberReferenceAttribute>();

            foreach (var prop in sheet.GetType().GetProperties())
            {
                var ra = (ReportMemberReferenceAttribute)prop.GetCustomAttribute(typeof(ReportMemberReferenceAttribute), false);
                if (ra == null) continue;
                list.Add(ra);
            }
            return list;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="report"></param>
        /// <param name="fileWithoutExt"></param>
        /// <returns></returns>
        public static bool CreateExcelFile(BaseReport report, string fileWithoutExt)
        {
            var list = new List<Tuple<string, string, string>>();
            ExcelManager manExcel = new ExcelManager();
            try
            {
                var ua = (ReportReferenceAttribute)report.GetType().GetCustomAttribute(typeof(ReportReferenceAttribute));
                if (ua != null)
                {
                    string name = ua.Name;
                }


                var destinationFilePath = Path.Combine(Manager.Reports, $"{fileWithoutExt}.xlsx");

                if (!manExcel.Create(destinationFilePath, fileWithoutExt)) return false;

                #region Principal sheet
                var columns = Manager.GetReportColumns(report);

                int rowCount = report.ResultRecords.Count();
                int colCount = columns.Count();
                int rowStart = 2;
                // totals to write above last row
                var totals = new Dictionary<ReportMemberReferenceAttribute, decimal>();
                // header row excel sheet
                List<Cell> cells = new List<Cell>();
                foreach (var col in columns)
                {
                    var cell = new Cell
                    {
                        CellReference = $"{col.Column}1",
                        CellValue = new CellValue(col.ColumnName),
                        DataType = CellValues.String
                    };
                    cells.Add(cell);
                    totals.Add(col, 0);
                }
                manExcel.AddRow(cells,1);

                if (rowCount>0)
                {
                    var records = report.ResultRecords;
                   
                    // data rows excel sheet
                    DocumentFormat.OpenXml.UInt32Value rowIndex = 2;
                    for (int i = rowStart; i <= (rowCount+1); i++)
                    {
                        var element = report.ResultRecords[i - 2];
                        cells = new List<Cell>();
                        foreach (var col in columns)
                        {
                            var cell = new Cell
                            {
                                CellReference = $"{col.Column}{i}",
                                CellValue = new CellValue(""),
                                DataType = CellValues.String
                            };
                           
                            var nameField = col.FieldName;
                            var propField = element.GetType().GetProperty(nameField);

                            switch (propField.PropertyType.Name)
                            {
                                case "DateTime":
                                    DateTime dateValue = (DateTime)propField.GetValue(element);
                                    var format = col.Format ?? "yyyyMMdd";
                                    cell.CellValue = new CellValue(dateValue.ToString(format));
                                    break;
                                case "Double":
                                    double doubleValue = (double)propField.GetValue(element);
                                    cell.CellValue = new CellValue(doubleValue.ToString());
                                    if (col.HaveSum) totals[col] += (decimal)doubleValue;
                                    break;
                                case "Decimal":
                                    decimal decimalValue = (decimal)propField.GetValue(element);
                                    cell.CellValue = new CellValue(decimalValue.ToString());
                                    if (col.HaveSum) totals[col] += decimalValue;
                                    break;
                                default:
                                    cell.CellValue = new CellValue((string)propField.GetValue(element));
                                    break;
                            }

                            cells.Add(cell);
                        }
                        manExcel.AddRow(cells, rowIndex++);
                    }
                    manExcel.AddTotals(totals, rowIndex + 2);
                }
                #endregion

                #region other sheets
                if (report.Phantom != null)
                {
                    columns = Manager.GetSheetReportColumns(report.Phantom);

                    foreach (var row in report.ResultRecords)
                    {
                        rowCount = row.SheetRecords.Count();
                        colCount = columns.Count();
                        rowStart = 2;

                        manExcel.AddSheet(row.SheetName);

                        // header row excel sheet
                        cells = new List<Cell>();
                        totals = new Dictionary<ReportMemberReferenceAttribute, decimal>();
                        foreach (var col in columns)
                        {
                            var cell = new Cell
                            {
                                CellReference = $"{col.Column}1",
                                CellValue = new CellValue(col.ColumnName),
                                DataType = CellValues.String
                            };
                            cells.Add(cell);
                            totals.Add(col, 0);
                        }
                        manExcel.AddRow(cells, 1);

                        if (rowCount > 0)
                        {
                            var records = row.SheetRecords;

                            // data rows excel sheet
                            DocumentFormat.OpenXml.UInt32Value rowIndex = 2;
                            for (int i = rowStart; i <= (rowCount + 1); i++)
                            {
                                var element = row.SheetRecords[i - 2];
                                cells = new List<Cell>();
                                foreach (var col in columns)
                                {
                                    var cell = new Cell
                                    {
                                        CellReference = $"{col.Column}{i}",
                                        CellValue = new CellValue(""),
                                        DataType = CellValues.String
                                    };

                                    var nameField = col.FieldName;
                                    var propField = element.GetType().GetProperty(nameField);

                                    switch (propField.PropertyType.Name)
                                    {
                                        case "DateTime":
                                            DateTime dateValue = (DateTime)propField.GetValue(element);
                                            var format = col.Format ?? "yyyyMMdd";
                                            cell.CellValue = new CellValue(dateValue.ToString(format));
                                            break;
                                        case "Double":
                                            double doubleValue = (double)propField.GetValue(element);
                                            cell.CellValue = new CellValue(doubleValue.ToString());
                                            if (col.HaveSum) totals[col] += (decimal)doubleValue;
                                            break;
                                        case "Decimal":
                                            decimal decimalValue = (decimal)propField.GetValue(element);
                                            cell.CellValue = new CellValue(decimalValue.ToString());
                                            if (col.HaveSum) totals[col] += decimalValue;
                                            break;
                                        default:
                                            cell.CellValue = new CellValue((string)propField.GetValue(element));
                                            break;
                                    }

                                    cells.Add(cell);
                                }
                                manExcel.AddRow(cells, rowIndex++);
                            }
                            manExcel.AddTotals(totals, rowIndex + 2);
                        }
                    }

                }

                #endregion

                if (!manExcel.Save()) throw new Exception(string.Format("Unable to save file: {0}", destinationFilePath));

                return true;
            }
            catch (Exception ex)
            {
                list.Add(Tuple.Create("Report", "Creazione file excel", $"Errore interno : {ex.Message}"));
                return false;
            }
            finally
            {
                WriteLog(list, fileWithoutExt);
                if(manExcel!=null) manExcel.Dispose();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="report"></param>
        /// <param name="fileWithoutExt"></param>
        /// <param name="csv"></param>
        /// <returns></returns>
        public static bool CreateFile(BaseReport report, string fileWithoutExt, bool csv = false)
        {
            var name = string.Empty;
            var list = new List<Tuple<string, string, string>>();
            try
            {
                var ua = (ReportReferenceAttribute)report.GetType().GetCustomAttribute(typeof(ReportReferenceAttribute));
                if (ua != null)
                {
                    name = ua.Name;
                }
               
                int rowCount = report.ResultRecords.Count();
                

                List<string> file = new List<string>();
                var records = report.ResultRecords;

                var fields = records[0].GetType().GetProperties().Where(x => x.GetCustomAttribute(typeof(ReportMemberReferenceAttribute), false) != null).ToList();

                foreach (var record in records)
                {
                    var line = string.Empty;
                    foreach (var field in fields)
                    {
                        var nameField = field.Name;
                        var propField = record.GetType().GetProperty(nameField);

                        if (csv)
                        {
                            switch (propField.PropertyType.Name)
                            {
                                case "DateTime":
                                    string stringValue = (string)propField.GetValue(record);
                                    DateTime dateValue = DateTime.MinValue;
                                    if (DateTime.TryParseExact(stringValue, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
                                    {
                                        line += $"\"{dateValue.ToString("ddMMyyyy")}\";";
                                    }
                                    else
                                    {
                                        line += $"\"{ propField.GetValue(record)}\";";
                                    }
                                    break;
                                default:
                                    line += $"\"{ propField.GetValue(record)}\";";
                                    break;
                            }
                        }
                        else
                        {
                            var value = propField.GetValue(record);
                            line += $"{value}";
                        }
                    }
                    line += Environment.NewLine;

                    file.Add(line);
                }

                if (csv)
                {
                    var columns = Manager.GetReportColumns(report);
                    int colCount = columns.Count();
                    var line = string.Empty;
                    foreach (var col in columns)
                    {
                        line += $"\"{col.ColumnName}\";";
                    }
                    line += Environment.NewLine;
                    file.Insert(0, line);
                }

                var dataAsBytes = file
                   .SelectMany(s => Encoding.UTF8.GetBytes(s))
                   .ToArray();

                var ext = csv ? "csv" : "txt";
                var destinationFilePath = Path.Combine(Manager.Reports, $"{fileWithoutExt}.{ext}");

                if (File.Exists(destinationFilePath)) File.Delete(destinationFilePath);

                var f = new FileStream(destinationFilePath, FileMode.Create);

                f.Write(dataAsBytes, 0, dataAsBytes.Length);
                f.Close();

                return true;
            }
            catch (Exception ex)
            {
                list.Add(Tuple.Create("Report", csv ? "Creazione file csv" : "Creazione file txt", $"Errore interno: {ex.Message}"));
                return false;
            }
            finally
            {
                WriteLog(list, fileWithoutExt);
            }
        }
        /// <summary>
        ///     Use this function to preformat a line with brackets for each param.
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        private static string GetPreFormattedLine(int fields)
        {
            var line = string.Empty;
            for (var i = 0; i < fields; i++)
            {
                line += "\"{" + i + "}\"";
            }
            return line + Environment.NewLine;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tuples"></param>
        /// <param name="fileName"></param>
        private static void WriteLog(List<Tuple<string, string, string>> tuples, string fileName)
        {
            try
            {
                var path = Path.Combine(Manager.Reports, fileName + ".log");
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="chars"></param>
        /// <param name="fill"></param>
        /// <returns></returns>
        public static string Left(string value, int chars, char? fill = null)
        {
            try
            {
                if (string.IsNullOrEmpty(value) && fill == null) return value;
                if (string.IsNullOrEmpty(value) && fill != null) return new string((char)fill, chars);
                string result = value;
                if (result.Length >= chars) result = result.Substring(0, chars);
                else
                {
                    if (fill != null) result = result.PadRight(chars, (char)fill);
                }
                return result;
            }
            catch (Exception)
            {

                return "";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string Space(int count)
        {
            return new string(' ', count);
        }
        /// <summary>
        /// Funzione che restituisce 1:Maschio o 2:Femmina a seconda del sesso recuperato dal CF
        /// </summary>
        /// <param name="cv"></param>
        /// <returns></returns>
        public static string SexCV(string cv)
        {
            try
            {
                if (!CheckCV(cv))
                    return "0";

                var day = int.Parse(cv.Substring(9, 2));
                return day > 40 ? "2" : "1";
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// Funzione che restituisce la data di nascita in formato AAAAMMGG recuperata dal CF
        /// </summary>
        /// <param name="cv"></param>
        /// <returns></returns>
        public static string DatCV(string cv)
        {               
            string Months = "ABCDEHLMPRST";

            try
            {
                if (!CheckCV(cv))
                    return null;

                var year = int.Parse(cv.Substring(6, 2));
                if (DateTime.Now.AddYears(-year).Year < 2000) year += 1900;
                else year += 2000;
                var month = Months.IndexOf(cv[8]) + 1;
                var day = int.Parse(cv.Substring(9, 2));
                if (day > 40) day -= 40;
                return new DateTime(year, month, day).ToString("yyyyMMdd");
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cv"></param>
        /// <returns></returns>
        public static bool CheckCV(string cv)
        {
            cv = cv.ToUpper();
            if (cv.Length != 16)
                return false; // errore

            var contatore = cv.Substring(0, 15).Select((t, i) => ValoreDelCarattere(cv.Substring(0, 15).Substring(i, 1), i)).Sum();
            contatore %= 26; // si considera il resto
            return cv.Substring(15, 1) == Convert.ToChar(contatore + 65).ToString(CultureInfo.InvariantCulture);
        }
        private static int ValoreDelCarattere(string carattere, int posizione)
        {
            switch (carattere)
            {
                case "A":
                case "0":
                    return (posizione % 2) == 0 ? 1 : 0;
                case "B":
                case "1":
                    return (posizione % 2) == 0 ? 0 : 1;
                case "C":
                case "2":
                    return (posizione % 2) == 0 ? 5 : 2;
                case "D":
                case "3":
                    return (posizione % 2) == 0 ? 7 : 3;
                case "E":
                case "4":
                    return (posizione % 2) == 0 ? 9 : 4;
                case "F":
                case "5":
                    return (posizione % 2) == 0 ? 13 : 5;
                case "G":
                case "6":
                    return (posizione % 2) == 0 ? 15 : 6;
                case "H":
                case "7":
                    return (posizione % 2) == 0 ? 17 : 7;
                case "I":
                case "8":
                    return (posizione % 2) == 0 ? 19 : 8;
                case "J":
                case "9":
                    return (posizione % 2) == 0 ? 21 : 9;
                case "K":
                    return (posizione % 2) == 0 ? 2 : 10;
                case "L":
                    return (posizione % 2) == 0 ? 4 : 11;
                case "M":
                    return (posizione % 2) == 0 ? 18 : 12;
                case "N":
                    return (posizione % 2) == 0 ? 20 : 13;
                case "O":
                    return (posizione % 2) == 0 ? 11 : 14;
                case "P":
                    return (posizione % 2) == 0 ? 3 : 15;
                case "Q":
                    return (posizione % 2) == 0 ? 6 : 16;
                case "R":
                    return (posizione % 2) == 0 ? 8 : 17;
                case "S":
                    return (posizione % 2) == 0 ? 12 : 18;
                case "T":
                    return (posizione % 2) == 0 ? 14 : 19;
                case "U":
                    return (posizione % 2) == 0 ? 16 : 20;
                case "V":
                    return (posizione % 2) == 0 ? 10 : 21;
                case "W":
                    return 22;
                case "X":
                    return (posizione % 2) == 0 ? 25 : 23;
                case "Y":
                    return 24;
                case "Z":
                    return (posizione % 2) == 0 ? 23 : 25;
                default:
                    return 0;
            }
        }
        /// <summary>
        /// Se RSA = 1, altrimenti = 2
        /// </summary>
        /// <param name="hostType"></param>
        /// <returns></returns>
        public static string ErogaRSA(string hostType)
        {
            if (string.IsNullOrEmpty(hostType)) return null;
            return hostType.ToUpper()=="RSA"?"1":"2";
        }
        /// <summary>                                                                      
        /// Funzione che in base documento e record in ingresso restituisce l'importo formattato NNNNNNNNNNDD
        /// </summary>
        /// <param name="hostType"></param>
        /// <returns></returns>
        public static string Amount(string document, dynamic record, decimal PRZPrice = 0)
        {
            string ret = new string('0', 12);

            try
            {
                if (document == "ZSDFatture")
                {
                    decimal netAmount = record.Price;
                    string stringVAT = record.VAT;
                    decimal VAT = 0;
                    decimal.TryParse(Regex.Match(stringVAT, @"\d+").Value, out VAT);
                    decimal amount = netAmount + netAmount * VAT / 100;
                    var stringValue = amount.ToString("0000000000.00");
                    ret = stringValue.Substring(0, 10) + stringValue.Substring(11, 2);
                }
                else if (document == "Report16")
                {
                    decimal price = PRZPrice;
                    decimal quantity = record.Quantity;
                    decimal amount = quantity * price;
                    var stringValue = amount.ToString("0000000000.00");
                    ret = stringValue.Substring(0, 10) + stringValue.Substring(11, 2);
                }
            }
            catch (Exception ex)
            {
                ret = new string('0', 12);
                //throw;
            }

            return ret;
        }
        /// <summary>
        /// Restituisce numero progressivo (ID) con in ingresso ultimo numero progressivo e anno e mese
        /// </summary>
        /// <param name="lpn"></param>
        /// <param name="y"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string NumProg(long lpn, int y, int m)
        {
            return $"VIVPEZZ{y.ToString("0000")}{m.ToString("00")}{lpn.ToString("0000000")}";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static TimeSpan ConvertToTimeSpan(string value, string format = null)
        {
            TimeSpan ret;
            if (format != null)
            {
                TimeSpan.TryParseExact(value, format, CultureInfo.InvariantCulture, TimeSpanStyles.None, out ret);
            }
            else if (!TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out ret))
            {
                if (!TimeSpan.TryParse(value, new CultureInfo("it-IT"), out ret))
                {
                    if (!TimeSpan.TryParseExact(value, "hh:mm", CultureInfo.InvariantCulture, out ret))
                    {
                        ret = new TimeSpan(0, 0, 0);
                    }
                }
            }
            return ret;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DateTime ConvertDate(string value, string format=null)
        {
            DateTime ret;
            if (format != null)
            {
                DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out ret);
            }
            else if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out ret))
            {
                if (!DateTime.TryParse(value, new CultureInfo("it-IT"), DateTimeStyles.None, out ret))
                {
                    if (!DateTime.TryParseExact(value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out ret))
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
                }                  
            }
            return ret;
        }

    }
}
