using System;
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
                            ?? Documents.AppendChild(CreateElement("REPORT").Attributes.Append(CreateAttribute(name)));
                        break;
                    default:
                        return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Private functions
        private void LoadDocument()
        {
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
