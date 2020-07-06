using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

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
        public static List<ReportReferenceAttribute> GetReports()
        {
            var list = new List<ReportReferenceAttribute>();
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
                    list.Add(ua);
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
                throw;
            }
        }

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

    }
}
