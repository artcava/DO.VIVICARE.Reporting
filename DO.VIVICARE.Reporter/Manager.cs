using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DO.VIVICARE.Reporter
{
    public static class Manager
    {
        /// <summary>
        /// Search for every BaseDocument in the path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<ReportingDocument> GetDocuments(string path)
        {
            var list = new List<ReportingDocument>();
            foreach (var file in Directory.GetFiles(path))
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
                }
            }
            return list;
        }
        /// <summary>
        /// Search for every BaseReport in the path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<ReportReferenceAttribute> GetReports(string path)
        {
            var list = new List<ReportReferenceAttribute>();
            foreach (var file in Directory.GetFiles(path))
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
    }
}
