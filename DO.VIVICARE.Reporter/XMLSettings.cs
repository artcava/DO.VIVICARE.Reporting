using System;
using System.Collections.Generic;
using System.IO;
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
            _XmlFilePath = $"{Directory.GetCurrentDirectory()}\\Settings.xml";
            LoadDocument();
        }

        ~XMLSettings()
        {
            if (File.Exists(_XmlFilePath))
                Save(_XmlFilePath);
        }
        #endregion

        #region Public functions
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
        /// <param name="text"></param>
        /// <param name="extension"></param>
        public void UpdateDocument(string name, string extension)
        {
            XmlNode node = Documents.SelectSingleNode($"DOCUMENT[@name='{name}']");
            var extAttr = node.Attributes.Append(CreateAttribute("ext"));
            extAttr.Value = extension;
            var lastAttr = node.Attributes.Append(CreateAttribute("last"));
            lastAttr.Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }

        public List<string> GetDocumentValues(LibraryType library, string name)
        {
            try
            {
                XmlNode node;
                List<string> attributes = new List<string> { name };
                switch (library)
                {
                    case LibraryType.Document:
                        node = Documents.SelectSingleNode($"DOCUMENT[@name='{name}']");
                        break;
                    case LibraryType.Report:
                        node = Reports.SelectSingleNode($"REPORT[@name='{name}']");
                        break;
                    default:
                        return null;
                }
                if (node == null) return null;

                var extAttr = node.Attributes.GetNamedItem("ext");
                attributes.Add((extAttr != null) ? extAttr.Value : null);
                var lastAttr = node.Attributes.GetNamedItem("last");
                attributes.Add((lastAttr != null) ? lastAttr.Value : null);

                return attributes;
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
        #endregion
    }
}
