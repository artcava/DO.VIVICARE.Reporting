using DO.VIVICARE.Report.Dietetica;
using DO.VIVICARE.Reporter;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

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

            var listViewItemReport = lvReport.SelectedItems[0];

            var returnMessage = string.Empty;
            if (Execute(listViewItemReport, out returnMessage)) MessageBox.Show(returnMessage, "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else MessageBox.Show(returnMessage, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);

            LoadReports();
            return;

            var docASST = Manager.GetDocuments().Find(a => a.Attribute.Name == "ASST");
            if (docASST == null)
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

            if (docASST.Document.Records.Count == 0)
            {
                MessageBox.Show("ASST nessun record caricato!", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var listASST = docASST.Document.Records;

            //var ASSTCode = 30705;
            var ASSTCode = 30707;

            BaseDocument ASST = listASST.Where((dynamic w) => w.ASSTCode == ASSTCode).FirstOrDefault();

            if (ASST == null)
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


            if (reportDietetica.ResultRecords.Count() == 0)
            {
                MessageBox.Show("Nessun dato da elaborare per Dietetica!", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show("File excel e file tracciato Dietetica creato correttamente!", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            LoadReports();

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
                if (list != null)
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
                    if (list == null)
                    {
                        lvReport.AddRow(0, f.Attribute.Name, f.Attribute.Description, "...", "...", "...");
                    }
                    else
                    {
                        lvReport.AddRow(0, f.Attribute.Name, f.Attribute.Description, list[0] == null ? "..." : name + "." + list[2], list[3] ?? "...", list[4] ?? "...");
                    }
                    lvReport.Items[lvReport.Items.Count - 1].Tag = f.Report;
                }
                lvReport.SmallImageList = imageListPiccole;
                lvReport.LargeImageList = imageListGrandi;
                lvReport.MountHeaders(
                       "Nome Report", 150, HorizontalAlignment.Left,
                       "Descrizione", 250, HorizontalAlignment.Left,
                       "File", 150, HorizontalAlignment.Left,
                       "Destinazione File", 300, HorizontalAlignment.Left,
                       "Ultimo creato", 120, HorizontalAlignment.Right
                       );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool Execute(ListViewItem selectedReport, out string message)
        {
            message = string.Empty;
            try
            {
                BaseReport report = (BaseReport)selectedReport.Tag;

                var nameReport = "unkown";
                var ua = (ReportReferenceAttribute)report.GetType().GetCustomAttribute(typeof(ReportReferenceAttribute));
                if (ua != null)
                {
                    nameReport = ua.Name;
                }

                report.CreateParameters();
                if (report.Parameters.Count > 0)
                {
                    //open window input parameters and fill all ReturnValue parameters

                    frmInputReportParameter f = new frmInputReportParameter($"Parametri report {nameReport}", report.Parameters);
                    DialogResult result = f.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        message = "Operazione annullata dall'utente!";
                        return false;
                    }
                    //check return values
                    var invalidValues = report.Parameters.Exists(p => p.ReturnValue == null);
                    if (invalidValues)
                    {
                        message = "Indicare tutti i valori per i parametri richiesti!";
                        return false;
                    }
                }

                Cursor.Current = Cursors.WaitCursor;

                report.LoadDocuments(true);

                report.Execute();

                Cursor.Current = Cursors.Default;

                if (report.ResultRecords.Count() == 0)
                {
                    message = $"Nessun dato da elaborare per {nameReport}!";
                    return false;
                }

                message = $"File excel e file tracciato {nameReport} creato correttamente!";
                return true;
            }
            catch (Exception ex)
            {
                message = $"Internal error:{ex.Message}";
                return false;
            }
        }

        #region ToolStripMenuItem_Click
        private void executeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var listViewItemReport = lvReport.SelectedItems[0];
            var returnMessage = string.Empty;
            if (Execute(listViewItemReport, out returnMessage)) MessageBox.Show(returnMessage, "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else MessageBox.Show(returnMessage, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            LoadReports();
        }
        private void openFileExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // APRE FILE xlsx CON EXCEL SEPARATAMENTE o il programma impostato per i file xlsx
            var listViewItemReport = lvReport.SelectedItems[0];
            if (listViewItemReport.SubItems[2].Text == "...")
                MessageBox.Show($"Non hai ancora eseguito nessun report [{listViewItemReport.SubItems[1].Text}]!", "Attenzione!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                var destination = listViewItemReport.SubItems[3].Text;
                try
                {
                    System.Diagnostics.Process.Start(destination);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"file [{destination}] errore [{ex.Message}]!", "Attenzione!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

        }
        private void openFileCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // APRE FILE csv CON EXCEL SEPARATAMENTE o il programma impostato per i file csv
            var listViewItemReport = lvReport.SelectedItems[0];
            if (listViewItemReport.SubItems[2].Text == "...")
                MessageBox.Show($"Non hai ancora eseguito nessun report [{listViewItemReport.SubItems[1].Text}]!", "Attenzione!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                var destination = listViewItemReport.SubItems[3].Text;
                destination = destination.Substring(0, destination.Length - 4) + "csv";
                try
                {
                    System.Diagnostics.Process.Start(destination);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"file [{destination}] errore [{ex.Message}]!", "Attenzione!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

        }
        private void openFileTxtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // APRE FILE txt CON EXCEL SEPARATAMENTE o il programma impostato per i file txt
            var listViewItemReport = lvReport.SelectedItems[0];
            if (listViewItemReport.SubItems[2].Text == "...")
                MessageBox.Show($"Non hai ancora eseguito nessun report [{listViewItemReport.SubItems[1].Text}]!", "Attenzione!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                var destination = listViewItemReport.SubItems[3].Text;
                destination = destination.Substring(0, destination.Length - 4) + "txt";
                try
                {
                    System.Diagnostics.Process.Start(destination);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"file [{destination}] errore [{ex.Message}]!", "Attenzione!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        private void openFileErrorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // APRE FILE log CON EXCEL SEPARATAMENTE o il programma impostato per i file log
            var listViewItemReport = lvReport.SelectedItems[0];
            if (listViewItemReport.SubItems[2].Text == "...")
                MessageBox.Show($"Non hai ancora eseguito nessun report [{listViewItemReport.SubItems[1].Text}]!", "Attenzione!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                var destination = listViewItemReport.SubItems[3].Text;
                destination = destination.Substring(0, destination.Length - 4) + "log";
                try
                {
                    System.Diagnostics.Process.Start(destination);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"file [{destination}] errore [{ex.Message}]!", "Attenzione!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        private void storicoReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var listViewItemReport = lvReport.SelectedItems[0];
            if (listViewItemReport.SubItems[2].Text == "...")
                MessageBox.Show($"Non hai ancora eseguito nessun report [{listViewItemReport.SubItems[1].Text}]!", "Attenzione!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                var report = listViewItemReport.SubItems[0].Text;
                frmReportHistory f = new frmReportHistory($"Report Storico {report}", report);
                DialogResult result = f.ShowDialog();
            }

        }
        private void regenerateCSVTxtFromFileExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var listViewItemReport = lvReport.SelectedItems[0];
            if (listViewItemReport.SubItems[2].Text == "...")
            {
                MessageBox.Show($"Non hai ancora eseguito nessun report [{listViewItemReport.SubItems[1].Text}]!", "Attenzione!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var returnMessage = string.Empty;
            if (Regenerate(listViewItemReport, out returnMessage)) MessageBox.Show(returnMessage, "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else MessageBox.Show(returnMessage, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        #endregion

        private bool Regenerate(ListViewItem selectedReport, out string message)
        {
            message = string.Empty;
            try
            {

                var fileNoExt = selectedReport.SubItems[2].Text;
                fileNoExt = fileNoExt.Substring(0, fileNoExt.Length - 5);

                BaseReport report = (BaseReport)selectedReport.Tag;

                Cursor.Current = Cursors.WaitCursor;

                report.Regenerate(fileNoExt);

                Cursor.Current = Cursors.Default;

                if (report.ResultRecords.Count() == 0)
                {
                    message = "Nessun dato da rigenerare per Dietetica!";
                    return false;
                }

                message = "File file CSV/Testo Dietetica rigenerati correttamente!";
                return true;
            }
            catch (Exception ex)
            {
                message = $"Internal error:{ex.Message}";
                return false;
            }
        }
    }
}
