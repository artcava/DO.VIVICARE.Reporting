using DO.VIVICARE.Reporter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private void LoadReports(string report)
        {
            try
            {
                lvReport.Clear();
                lvReport.View = View.Details;
                cmbChoose.SelectedIndex = 2;

                foreach (var r in Manager.Settings.GetReports(report))
                {
                    var list = Manager.Settings.GetDocumentValues(XMLSettings.LibraryType.Report, r);
                    if (list == null)
                    {
                        lvReport.AddRow(0, "...", "...", "...");
                    }
                    else
                    {
                        lvReport.AddRow(0, list[0] == null ? "..." : r +"."+ list[2], list[3] ?? "...", list[4] ?? "...");
                    }
                    lvReport.Items[lvReport.Items.Count - 1].Tag = r;
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
            System.Diagnostics.Process.Start(listViewItemReport.SubItems[1].Text);
        }

        private void openFileCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // APRE FILE csv CON EXCEL SEPARATAMENTE o il programma impostato per i file csv
            var listViewItemReport = lvReport.SelectedItems[0];
            var destination = listViewItemReport.SubItems[1].Text;
            destination = destination.Substring(0, destination.Length - 4) + "csv";
            System.Diagnostics.Process.Start(destination);
        }

        private void openFileTxtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // APRE FILE csv CON EXCEL SEPARATAMENTE o il programma impostato per i file csv
            var listViewItemReport = lvReport.SelectedItems[0];
            var destination = listViewItemReport.SubItems[1].Text;
            destination = destination.Substring(0, destination.Length - 4) + "txt";
            System.Diagnostics.Process.Start(destination);
        }
    }
}
