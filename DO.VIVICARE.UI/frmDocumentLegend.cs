using DO.VIVICARE.Reporter;
using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace DO.VIVICARE.UI
{
    public partial class frmDocumentLegend : Form
    {
        BaseDocument Document { get; set; }

        public frmDocumentLegend()
        {
            InitializeComponent();
        }

        public frmDocumentLegend(string title, BaseDocument document)
        {
            InitializeComponent();
            this.Text = title;
            Document = document;
        }

        private void frmReportHistory_Load(object sender, EventArgs e)
        {
            Loadcolumns();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Loadcolumns()
        {
            try
            {
                lvReport.Clear();
                lblHeader.Text = string.Empty;
                lvReport.View = View.Details;

                var ua = (DocumentReferenceAttribute)Document.GetType().GetCustomAttribute(typeof(DocumentReferenceAttribute));
                if (ua != null)
                    lblHeader.Text = $"Documento: {ua.Name}; Descrizione: {ua.Description}; Riga iniziale: {ua.RowStart}";

                var list = Manager.GetDocumentColumns(Document);
                if (list == null)
                {
                    lvReport.AddRow(0, "...", "...", "...");
                }
                else
                {
                    foreach (DocumentMemberReferenceAttribute attribute in list.OrderBy(d => d.Position))
                    {
                        lvReport.AddRow(0, attribute.Column, attribute.Position.ToString(), attribute.FieldName);
                    }
                }

                lvReport.MountHeaders(
                       "Colonna", 100, HorizontalAlignment.Center,
                       "Posizione", 100, HorizontalAlignment.Right,
                       "Nome campo", 300, HorizontalAlignment.Left
                       );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
