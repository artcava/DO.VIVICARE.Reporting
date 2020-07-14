using DO.VIVICARE.Report.Dietetica;
using DO.VIVICARE.Reporter;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;

namespace DO.VIVICARE.UI
{
    public partial class frmReports : Form
    {
        public frmReports()
        {
            InitializeComponent();
            lvReport.BackColor = Color.SkyBlue;
            SetDataGrid();
        }

        private void SetDataGrid()
        {
            dgvElenco.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvElenco.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);

            dgvElenco.Columns["Origin"].FillWeight = 200;
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("SUPER PROCESS STARTS!!!");
            // solo per test

            if (lvReport.SelectedItems.Count == 0)
            {
                MessageBox.Show("Nessun report selezionato!", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var report = lvReport.SelectedItems[0];

            var docASST = Manager.GetDocuments().Find(a => a.Attribute.Name == "ASST");
            if (docASST==null)
            {
                MessageBox.Show("File ASST non trovato!", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var list = Manager.Settings.GetDocumentValues(XMLSettings.LibraryType.Document, docASST.Attribute.Name);
            docASST.Document.SourceFilePath = "";
            if (list != null)
            {
                docASST.Document.SourceFilePath = Path.Combine(Manager.Documents, list[0] + list[1]);
            }
            docASST.Document.AttributeName = docASST.Attribute.Name;
            docASST.Document.LoadRecords();
            
            if (docASST.Document.Records.Count==0)
            {
                MessageBox.Show("ASST nessun record caricato!", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var listASST = docASST.Document.Records;

            //var ASSTCode = 30705;
            var ASSTCode = 30707;

            BaseDocument ASST = listASST.Where((dynamic w) => w.ASSTCode == ASSTCode).FirstOrDefault();

            if (ASST==null)
            {
                MessageBox.Show($"ASST {ASSTCode} non trovata!", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }



            var reportDietetica = new Dietetica();

            reportDietetica.CreateParameters();

            var par = reportDietetica.Parameters.FirstOrDefault(p => p.Name == "ASST");
            if (par != null) par.ReturnValue = ASST;

            par = reportDietetica.Parameters.FirstOrDefault(p => p.Name == "Year");
            if (par != null) par.ReturnValue = 2020;

            par = reportDietetica.Parameters.FirstOrDefault(p => p.Name == "Month");
            if (par != null) par.ReturnValue = 5;

            //reportDietetica.SetASST(ASST);
            //reportDietetica.SetYear(2020);
            //reportDietetica.SetMonth(5);


            reportDietetica.LoadDocuments(true);

            reportDietetica.Execute();


            if (reportDietetica.ResultRecords.Count()==0)
            {
                MessageBox.Show("Nessun dato da elaborare per Dietetica!", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show("File excel e file tracciato Dietetica creato correttamente!", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            LoadReports();

            //reportDietetica.ResultRecords.Add(new Dietetica
            //{
            //    ATSCode = Manager.Left("222", 3, ' '),
            //    ASSTCode = Manager.Left("4444", 6, ' '),
            //    Year = DateTime.Now.Year.ToString("0000"),
            //    Month = DateTime.Now.Month.ToString("00"),
            //    FiscalCode = new string('0', 16),
            //    Sex = Manager.SexCV(""),
            //    DateOfBirth = Manager.DatCV(""),
            //    ISTATCode = Manager.Left("123", 6, ' '),
            //    UserHost = Manager.Space(1),
            //    PrescriptionNumber = Manager.Space(14),
            //    DeliveryDate = "19241017",
            //    TypeDescription = Manager.Left("SERVICENAD", 15, ' '),
            //    Typology = "5",
            //    MinsanCode = Manager.Space(30),
            //    MinsanDescription = Manager.Space(30),
            //    Manufacturer = Manager.Space(30),
            //    PiecesPerPack = "001",
            //    UnitOfMeasure = Manager.Left("CMESE", 9, ' '),
            //    Quantity = 2.ToString("0000"),
            //    ManagementChannel = "4",
            //    PurchaseAmount = new string('0', 12),
            //    ServiceChargeAmount = new string('0', 12),
            //    RecordDestination = "N",
            //    ID = Manager.NumProg(100, DateTime.Now.Year, DateTime.Now.Month),
            //    RepCode = Manager.Space(30),
            //    CNDCode = Manager.Space(13),
            //    FlagDM = "F",
            //    Type = Manager.Space(1)
            //});
            //Manager.CreateExcelFile(reportDietetica);
            //Manager.CreateFile(reportDietetica);
            //Manager.CreateFile(reportDietetica, true);


        }

        private void cmbChoose_SelectionChangeCommitted(object sender, EventArgs e)
        {
            switch (cmbChoose.SelectedIndex)
            {
                case 0: // "ICONE GRANDI"
                    lvReport.View = View.LargeIcon;
                    break;
                case 1: // "ICONE PICCOLE"
                    lvReport.View = View.SmallIcon;
                    break;
                case 2: // "DETTAGLI"
                    lvReport.View = View.Details;
                    break;
            }
        }

        private void frmReports_Load(object sender, EventArgs e)
        {
            LoadReports();
        }

        private void lvReport_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point pt = lvReport.PointToScreen(e.Location);
                contextMenuStrip1.Show(pt);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lvReport_Click(object sender, EventArgs e)
        {
            FillGrid(lvReport.SelectedItems[0]);
        }

        private void FillGrid(ListViewItem selectedReport)
        {

            var report = (BaseReport)selectedReport.Tag;

            report.LoadDocuments();
            
            dgvElenco.Rows.Clear();

            var documents = report.Documents;

            foreach (var document in documents)
            {
                var row = new DataGridViewRow();
                var i = dgvElenco.Rows.Add(row);
                var list = Manager.Settings.GetDocumentValues(XMLSettings.LibraryType.Document, document.AttributeName);
                if (list!=null)
                {
                    dgvElenco.Rows[i].Cells["Document"].Value = document.AttributeName;
                    dgvElenco.Rows[i].Cells["FileName"].Value = list.Count == 0 ? "..." : list[1] == null ? "..." : document.AttributeName + list[1];
                    dgvElenco.Rows[i].Cells["Origin"].Value = list.Count == 1 ? "..." : list[2] ?? "...";
                    dgvElenco.Rows[i].Cells["LastModify"].Value = list.Count == 3 ? "..." : list[4] ?? "...";
                }
                else
                {
                    dgvElenco.Rows[i].Cells["Document"].Value = document.AttributeName;
                    dgvElenco.Rows[i].Cells["FileName"].Value = "...";
                    dgvElenco.Rows[i].Cells["Origin"].Value = "...";
                    dgvElenco.Rows[i].Cells["LastModify"].Value = "...";
                }
            }

            // DA SOSTITUIRE CON LA LISTA DEGLI OGGETTI DOCUMENT
            //for (int it = 0; it < 3; it++)
            //{
            //    var row = new DataGridViewRow();
            //    var i = dgvElenco.Rows.Add(row);
            //    dgvElenco.Rows[i].Cells["NomeFileDocument"].Value = nomeReport;
            //    dgvElenco.Rows[i].Cells["NomeFileDocumentCompleto"].Value = nomeReport.ToUpper();
            //    dgvElenco.Rows[i].Cells["Altro1"].Value = "ALTRO 1";
            //    dgvElenco.Rows[i].Cells["Altro2"].Value = "ALRO 2";
            //    dgvElenco.Rows[i].Cells["Altro3"].Value = "ALTRO 3";
            //}
        }

        private void LoadReports()
        {
            try
            {
                lvReport.Clear();
                lvReport.View = View.Details;
                cmbChoose.SelectedIndex = 2;

                foreach (var f in Manager.GetReports())
                {
                    var name = "";
                    // looking for last report created
                    var lastReportValues = Manager.Settings.GetLastReportValues(f.Attribute.Name);
                    if (lastReportValues != null)
                    {
                        if (lastReportValues.Count != 0) name = lastReportValues[0];
                    }
                    var list = Manager.Settings.GetDocumentValues(XMLSettings.LibraryType.Report, name);
                    if (list==null)
                    {
                        lvReport.AddRow(0, f.Attribute.Name, f.Attribute.Description, "..." , "...", "...");
                    }
                    else
                    {
                        lvReport.AddRow(0, f.Attribute.Name, f.Attribute.Description, list[0] == null ? "..." : name + list[2], list[3] ?? "...", list[4] ?? "...");
                    }
                    lvReport.Items[lvReport.Items.Count - 1].Tag = f.Report;
                }
                lvReport.SmallImageList = imageListPiccole;
                lvReport.LargeImageList = imageListGrandi;
                lvReport.MountHeaders(
                       "Nome Report", 100, HorizontalAlignment.Left,
                       "Descrizione", 180, HorizontalAlignment.Left,
                       "File", 120, HorizontalAlignment.Left,
                       "Destinazione File", 200, HorizontalAlignment.Left,
                       "Ultimo creato", 120, HorizontalAlignment.Right
                       );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region ToolStripMenuItem_Click
        private void executeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var report = lvReport.SelectedItems[0];
        }
        #endregion

    }
}
