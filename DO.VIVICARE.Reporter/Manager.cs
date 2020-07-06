using System;
using System.Collections.Generic;
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
            string ret = "0";

            try
            {

            }
            catch (Exception)
            {

                throw;
            }

            return ret;
        }

        /// <summary>
        /// Funzione che restituisce la data di nascita in formato AAAAMMGG recuperata dal CF
        /// </summary>
        /// <param name="cv"></param>
        /// <returns></returns>
        public static string DatCV(string cv)
        {               
            string ret = new string(' ', 8); 

            try
            {

            }
            catch (Exception)
            {

                throw;
            }

            return ret;
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
