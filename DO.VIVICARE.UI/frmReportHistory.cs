using DO.VIVICARE.Reporter;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DO.VIVICARE.UI
{
    public partial class frmReportHistory : Form
    {
        string Report { get; set; }

        public frmReportHistory()
        {
            InitializeComponent();
        }

        public frmReportHistory(string title, string report)
        {
            InitializeComponent();
            this.Text = title;
            Report = report;
        }

        private void frmReportHistory_Load(object sender, EventArgs e)
        {
            LoadReports(Report);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void LoadReports(string report)
        {
            try
            {
                lvReport.Clear();
                lvReport.View = View.Details;
                cmbChoose.SelectedIndex = 2;

                BaseReport objReport = null;

                var reports = Manager.GetReports();
                if (reports.Count > 0)
                {
                    var reportingReport = reports.FirstOrDefault(r => r.Attribute.Name == report);
                    if (reportingReport != null) objReport = reportingReport.Report;
                }
                lvReport.Tag = objReport;

                foreach (var r in Manager.Settings.GetReports(report))
                {
                    var list = Manager.Settings.GetDocumentValues(XMLSettings.LibraryType.Report, r);
                    if (list == null)
                    {
                        lvReport.AddRow(0, "...", "...", "...");
                    }
                    else
                    {
                        lvReport.AddRow(0, list[0] == null ? "..." : r + "." + list[2], list[3] ?? "...", list[4] ?? "...");
                    }
                }
                lvReport.SmallImageList = imageListPiccole;
                lvReport.LargeImageList = imageListGrandi;
                lvReport.MountHeaders(
                       "File", 300, HorizontalAlignment.Left,
                       "Destinazione File", 350, HorizontalAlignment.Left,
                       "Data creazione", 150, HorizontalAlignment.Right
                       );
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void lvReport_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Point pt = lvReport.PointToScreen(e.Location);
                contextMenuStrip1.Show(pt);
            }
        }

        private void openFileExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // APRE FILE xlsx CON EXCEL SEPARATAMENTE o il programma impostato per i file xlsx
            var listViewItemReport = lvReport.SelectedItems[0];
            var destination = listViewItemReport.SubItems[1].Text;
            try
            {
                System.Diagnostics.Process.Start(destination);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"file [{destination}] errore [{ex.Message}]!", "Attenzione!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void openFileCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // APRE FILE csv CON EXCEL SEPARATAMENTE o il programma impostato per i file csv
            var listViewItemReport = lvReport.SelectedItems[0];
            var destination = listViewItemReport.SubItems[1].Text;
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

        private void openFileTxtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // APRE FILE txt CON il programma impostato per i file csv
            var listViewItemReport = lvReport.SelectedItems[0];
            var destination = listViewItemReport.SubItems[1].Text;
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

        private void openFileErrorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // APRE FILE log CON il programma impostato per i file csv
            var listViewItemReport = lvReport.SelectedItems[0];
            var destination = listViewItemReport.SubItems[1].Text;
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

        private void regenerateCSVTxtFromFileExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BaseReport report = (BaseReport)lvReport.Tag;
            var listViewItemReport = lvReport.SelectedItems[0];
            var returnMessage = string.Empty;
            if (Regenerate(listViewItemReport, report, out returnMessage)) MessageBox.Show(returnMessage, "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else MessageBox.Show(returnMessage, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool Regenerate(ListViewItem selectedReport, BaseReport report, out string message)
        {
            message = string.Empty;
            try
            {
                var fileNoExt = selectedReport.SubItems[0].Text;
                fileNoExt = fileNoExt.Substring(0, fileNoExt.Length - 5);

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
