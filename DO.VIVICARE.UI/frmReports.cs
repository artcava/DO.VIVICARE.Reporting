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
            // For testing purposes only

            if (lvReport.SelectedItems.Count == 0)
            {
                MessageBox.Show("No report selected!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var listViewItemReport = lvReport.SelectedItems[0];

            var returnMessage = string.Empty;
            if (Execute(listViewItemReport, out returnMessage)) MessageBox.Show(returnMessage, "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else MessageBox.Show(returnMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            LoadReports();
            return;

            var docASST = Manager.GetDocuments().Find(a => a.Attribute.Name == "ASST");
            if (docASST == null)
            {
                MessageBox.Show("ASST file not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("ASST: no records loaded!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var listASST = docASST.Document.Records;

            //var ASSTCode = 30705;
            var ASSTCode = 30707;

            BaseDocument ASST = listASST.Where((dynamic w) => w.ASSTCode == ASSTCode).FirstOrDefault();

            if (ASST == null)
            {
                MessageBox.Show($"ASST {ASSTCode} not found!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("No data to process for Dietary report!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                MessageBox.Show("Excel file and Dietary trace file created successfully!", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            LoadReports();

        }

        private void cmbChoose_SelectionChangeCommitted(object sender, EventArgs e)
        {
            switch (cmbChoose.SelectedIndex)
            {
                case 0: // "LARGE ICONS"
                    lvReport.View = View.LargeIcon;
                    break;
                case 1: // "SMALL ICONS"
                    lvReport.View = View.SmallIcon;
                    break;
                case 2: // "DETAILS"
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

            // TO BE REPLACED WITH THE DOCUMENT OBJECTS LIST
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
                    // Looking for last created report
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
                       "Report Name", 150, HorizontalAlignment.Left,
                       "Description", 250, HorizontalAlignment.Left,
                       "File", 150, HorizontalAlignment.Left,
                       "File Destination", 300, HorizontalAlignment.Left,
                       "Last Created", 120, HorizontalAlignment.Right
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

                var nameReport = "unknown";
                var ua = (ReportReferenceAttribute)report.GetType().GetCustomAttribute(typeof(ReportReferenceAttribute));
                if (ua != null)
                {
                    nameReport = ua.Name;
                }

                report.CreateParameters();
                if (report.Parameters.Count > 0)
                {
                    // Open window for input parameters and fill all ReturnValue parameters

                    frmInputReportParameter f = new frmInputReportParameter($"Report parameters {nameReport}", report.Parameters);
                    DialogResult result = f.ShowDialog();
                    if (result != DialogResult.OK)
                    {
                        message = "Operation cancelled by user!";
                        return false;
                    }
                    // Check return values
                    var invalidValues = report.Parameters.Exists(p => p.ReturnValue == null);
                    if (invalidValues)
                    {
                        message = "Please provide all required parameter values!";
                        return false;
                    }
                }

                Cursor.Current = Cursors.WaitCursor;

                report.LoadDocuments(true);

                report.Execute();

                Cursor.Current = Cursors.Default;

                if (report.ResultRecords.Count() == 0)
                {
                    message = $"No data to process for {nameReport}!";
                    return false;
                }

                message = $"Excel file and trace file {nameReport} created successfully!";
                return true;
            }
            catch (Exception ex)
            {
                message = $"Internal error: {ex.Message}";
                return false;
            }
        }

        #region ToolStripMenuItem_Click
        private void executeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var listViewItemReport = lvReport.SelectedItems[0];
            var returnMessage = string.Empty;
            if (Execute(listViewItemReport, out returnMessage)) MessageBox.Show(returnMessage, "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else MessageBox.Show(returnMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            LoadReports();
        }
        private void openFileExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Opens the Excel file separately or with the default program set for xlsx files
            var listViewItemReport = lvReport.SelectedItems[0];
            if (listViewItemReport.SubItems[2].Text == "...")
                MessageBox.Show($"You haven't executed any report [{listViewItemReport.SubItems[1].Text}] yet!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                var destination = listViewItemReport.SubItems[3].Text;
                try
                {
                    System.Diagnostics.Process.Start(destination);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"File [{destination}] error [{ex.Message}]!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

        }
        private void openFileCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Opens the CSV file separately or with the default program set for csv files
            var listViewItemReport = lvReport.SelectedItems[0];
            if (listViewItemReport.SubItems[2].Text == "...")
                MessageBox.Show($"You haven't executed any report [{listViewItemReport.SubItems[1].Text}] yet!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    MessageBox.Show($"File [{destination}] error [{ex.Message}]!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

        }
        private void openFileTxtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Opens the text file separately or with the default program set for txt files
            var listViewItemReport = lvReport.SelectedItems[0];
            if (listViewItemReport.SubItems[2].Text == "...")
                MessageBox.Show($"You haven't executed any report [{listViewItemReport.SubItems[1].Text}] yet!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    MessageBox.Show($"File [{destination}] error [{ex.Message}]!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        private void openFileErrorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Opens the log file separately or with the default program set for log files
            var listViewItemReport = lvReport.SelectedItems[0];
            if (listViewItemReport.SubItems[2].Text == "...")
                MessageBox.Show($"You haven't executed any report [{listViewItemReport.SubItems[1].Text}] yet!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                    MessageBox.Show($"File [{destination}] error [{ex.Message}]!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        private void storicoReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var listViewItemReport = lvReport.SelectedItems[0];
            if (listViewItemReport.SubItems[2].Text == "...")
                MessageBox.Show($"You haven't executed any report [{listViewItemReport.SubItems[1].Text}] yet!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                var report = listViewItemReport.SubItems[0].Text;
                frmReportHistory f = new frmReportHistory($"Report History {report}", report);
                DialogResult result = f.ShowDialog();
            }

        }
        private void regenerateCSVTxtFromFileExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var listViewItemReport = lvReport.SelectedItems[0];
            if (listViewItemReport.SubItems[2].Text == "...")
            {
                MessageBox.Show($"You haven't executed any report [{listViewItemReport.SubItems[1].Text}] yet!", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var returnMessage = string.Empty;
            if (Regenerate(listViewItemReport, out returnMessage)) MessageBox.Show(returnMessage, "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else MessageBox.Show(returnMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    message = "No data to regenerate for Dietary report!";
                    return false;
                }

                message = "CSV/Text files for Dietary report regenerated successfully!";
                return true;
            }
            catch (Exception ex)
            {
                message = $"Internal error: {ex.Message}";
                return false;
            }
        }
    }
}
