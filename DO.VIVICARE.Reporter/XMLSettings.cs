using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace DO.VIVICARE.Reporter
{
    public class XMLSettings : XmlDocument
    {
        #region Costruttore e Variabili private
        private string _XmlFilePath;
        private XmlNode Libraries;
        private XmlNode Documents;
        private XmlNode Reports;
        /// <summary>
        /// Costruttore.
        /// </summary>
        public XMLSettings()
        {
            _XmlFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Reporting", "Settings.xml");
            LoadDocument();
        }

        ~XMLSettings()
        {
            if (File.Exists(_XmlFilePath))
                Save(_XmlFilePath);
        }
        #endregion

        #region Public functions

        public void Save()
        {
            if (File.Exists(_XmlFilePath))
                Save(_XmlFilePath);
        }

        public bool AddLibrary(LibraryType library, string name)
        {
            try
            {
                XmlNode node;
                switch (library)
                {
                    case LibraryType.Document:
                        node =
                            Documents.SelectSingleNode($"DOCUMENT[@name='{name}']")
                            ?? Documents.AppendChild(CreateElement("DOCUMENT"));
                        break;
                    case LibraryType.Report:
                        node =
                            Reports.SelectSingleNode($"REPORT[@name='{name}']")
                            ?? Documents.AppendChild(CreateElement("REPORT"));
                        break;
                    default:
                        return false;
                }
                var nameAttr = node.Attributes.Append(CreateAttribute("name"));
                nameAttr.Value = name;

                //Save(_XmlFilePath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="extension"></param>
        /// <param name="origin"></param>
        /// <param name="upload"></param>
        /// <param name="modify"></param>
        /// <param name="verify"></param>
        public void UpdateDocument(string name, string extension, string origin, DateTime? upload, DateTime? modify, DateTime? verify, DocumentStatus status)
        {
            XmlNode node = Documents.SelectSingleNode($"DOCUMENT[@name='{name}']");

            var extAttr = node.Attributes["ext"];
            if (extAttr == null) extAttr = node.Attributes.Append(CreateAttribute("ext"));
            extAttr.Value = extension;

            var origAttr = node.Attributes["orig"];
            if (origAttr == null) origAttr = node.Attributes.Append(CreateAttribute("orig"));
            origAttr.Value = origin;

            var lastAttr = node.Attributes["last"];
            if (lastAttr == null) lastAttr = node.Attributes.Append(CreateAttribute("last"));
            if (upload != null) lastAttr.Value = upload.Value.ToString("yyyy-MM-dd HH:mm");

            var modAttr = node.Attributes["mod"];
            if (modAttr == null) modAttr = node.Attributes.Append(CreateAttribute("mod"));
            if (modify != null) modAttr.Value = modify.Value.ToString("yyyy-MM-dd HH:mm");

            var verAttr = node.Attributes["ver"];
            if (verAttr == null) verAttr = node.Attributes.Append(CreateAttribute("ver"));
            if (verify != null) verAttr.Value = verify.Value.ToString("yyyy-MM-dd HH:mm");

            var statusAttr = node.Attributes["status"];
            if (statusAttr == null) statusAttr = node.Attributes.Append(CreateAttribute("status"));
            statusAttr.Value = ((int)status).ToString();
        }


        public void UpdateReport(string name, string report, string extension, string destination, DateTime? create, ReportStatus status)
        {
            XmlNode node = Reports.SelectSingleNode($"REPORT[@name='{name}']");
            if (node == null)
            {
                node = Reports.AppendChild(CreateElement("REPORT"));
                var nameAttr = node.Attributes.Append(CreateAttribute("name"));
                nameAttr.Value = name;
            }
            var reportAttr = node.Attributes["report"];
            if (reportAttr == null) reportAttr = node.Attributes.Append(CreateAttribute("report"));
            reportAttr.Value = report;

            var extAttr = node.Attributes["ext"];
            if (extAttr == null) extAttr = node.Attributes.Append(CreateAttribute("ext"));
            extAttr.Value = extension;

            var destinationAttr = node.Attributes["destination"];
            if (destinationAttr == null) destinationAttr = node.Attributes.Append(CreateAttribute("destination"));
            destinationAttr.Value = destination;

            var createAttr = node.Attributes["create"];
            if (createAttr == null) createAttr = node.Attributes.Append(CreateAttribute("create"));
            if (create != null) createAttr.Value = create.Value.ToString("yyyy-MM-dd HH:mm");

            var statusAttr = node.Attributes["status"];
            if (statusAttr == null) statusAttr = node.Attributes.Append(CreateAttribute("status"));
            statusAttr.Value = ((int)status).ToString();
            Save();
        }


        public List<string> GetDocumentValues(LibraryType library, string name)
        {
            try
            {
                XmlNode node;
                List<string> attributes = new List<string> { name };
                if (library == LibraryType.Document)
                {
                    node = Documents.SelectSingleNode($"DOCUMENT[@name='{name}']");
                    if (node == null) return null;

                    var extAttr = node.Attributes["ext"];
                    attributes.Add(extAttr?.Value);
                    var origAttr = node.Attributes["orig"];
                    attributes.Add(origAttr?.Value);
                    var lastAttr = node.Attributes["last"];
                    attributes.Add(lastAttr?.Value);
                    var modAttr = node.Attributes["mod"];
                    attributes.Add(modAttr?.Value);
                    var verAttr = node.Attributes["ver"];
                    attributes.Add(verAttr?.Value);
                    var statusAttr = node.Attributes["status"];
                    attributes.Add(statusAttr?.Value);
                }
                else if (library == LibraryType.Report)
                {
                    if (string.IsNullOrEmpty(name)) return null;

                    node = Reports.SelectSingleNode($"REPORT[@name='{name}']");
                    if (node == null) return null;

                    var reportAttr = node.Attributes["report"];
                    attributes.Add(reportAttr?.Value);
                    var extAttr = node.Attributes["ext"];
                    attributes.Add(extAttr?.Value);
                    var destinationAttr = node.Attributes["destination"];
                    attributes.Add(destinationAttr?.Value);
                    var createAttr = node.Attributes["create"];
                    attributes.Add(createAttr?.Value);
                    var statusAttr = node.Attributes["status"];
                    attributes.Add(statusAttr?.Value);
                }
                else
                {
                    return null;
                }
                return attributes;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<string> GetLastReportValues(string report)
        {
            try
            {
                List<string> attributes = new List<string>();
                IEnumerable<XmlNode> nodes = Reports.SelectNodes($"REPORT[@report='{report}']").Cast<XmlNode>().OrderByDescending(r => Convert.ToDateTime(r.Attributes["create"].Value));

                if (nodes == null) return null;
                if (nodes.Count() == 0) return null;
                var node = nodes.FirstOrDefault();
                if (node == null) return null;

                var nameAttr = node.Attributes["name"];
                attributes.Add(nameAttr?.Value);

                attributes.Add(report);

                var extAttr = node.Attributes["ext"];
                attributes.Add(extAttr?.Value);
                var destinationAttr = node.Attributes["destination"];
                attributes.Add(destinationAttr?.Value);
                var createAttr = node.Attributes["create"];
                attributes.Add(createAttr?.Value);
                var statusAttr = node.Attributes["status"];
                attributes.Add(statusAttr?.Value);

                return attributes;
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        public List<string> GetReports(string report)
        {
            try
            {
                List<string> reports = new List<string>();
                IEnumerable<XmlNode> nodes = Reports.SelectNodes($"REPORT[@report='{report}']").Cast<XmlNode>().OrderByDescending(r => Convert.ToDateTime(r.Attributes["create"].Value));

                if (nodes == null) return null;
                if (nodes.Count() == 0) return null;

                foreach (var node in nodes)
                {
                    var nameAttr = node.Attributes["name"];
                    reports.Add(nameAttr?.Value);
                }

                return reports;
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        #endregion

        #region Private functions
        private void LoadDocument()
        {
            if (DocumentElement != null) return;

            if (!File.Exists(_XmlFilePath))
            {
                LoadXml("<SETTINGS></SETTINGS>");
                Libraries = DocumentElement.AppendChild(CreateElement("LIBRARIES"));
                Documents = Libraries.AppendChild(CreateElement("DOCUMENTS"));
                Reports = Libraries.AppendChild(CreateElement("REPORTS"));
                Save(_XmlFilePath);
            }
            else
            {
                base.Load(_XmlFilePath);
                Libraries = DocumentElement.FirstChild;
                Documents = Libraries.SelectSingleNode("DOCUMENTS");
                Reports = Libraries.SelectSingleNode("REPORTS");
            }

        }
        #endregion

        #region Enumerates
        public enum LibraryType : int
        {
            Undefined = 0,
            Document = 1,
            Report = 2
        }

        public enum DocumentStatus : int
        {
            Undefined = 0,
            NoFile = 1,
            FileInError = 2,
            FileToVerify = 3,
            FileOK = 4
        }

        public enum ReportStatus : int
        {
            Undefined = 0,
            Error = 1,
            FileOK = 2
        }
        #endregion
    }
}
