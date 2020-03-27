using DO.VIVICARE.Reporter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            dgvElenco.Columns["NomeFileDocument"].FillWeight = 300;
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            MessageBox.Show("SUPER PROCESS STARTS!!!");
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
            try
            {
                cmbChoose.SelectedIndex = 0;

                foreach (var f in Manager.GetReports(Path.Combine(Properties.Settings.Default["UserPathDefault"].ToString(),
                                                                  Properties.Settings.Default["UserFolderReports"].ToString())))
                {
                    lvReport.AddRow(0, f.Name, "param1", "param2", "param3", "param4");
                }

                lvReport.SmallImageList = imageListPiccole;
                lvReport.LargeImageList = imageListGrandi;
                lvReport.MountHeaders(
                        "File", 180, HorizontalAlignment.Left,
                        "Altro1", 120, HorizontalAlignment.Left,
                        "Altro2", 120, HorizontalAlignment.Left,
                        "Altro3", 120, HorizontalAlignment.Left,
                        "Altro4", 120, HorizontalAlignment.Left);

            }
            catch (Exception)
            {
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lvReport_Click(object sender, EventArgs e)
        {
            FillGrid(lvReport.SelectedItems[0].ToString());
        }

        private void FillGrid(string nomeReport)
        {
            dgvElenco.Rows.Clear();

            // DA SOSTITUIRE CON LA LISTA DEGLI OGGETTI DOCUMENT
            for (int it = 0; it < 3; it++)
            {
                var row = new DataGridViewRow();
                var i = dgvElenco.Rows.Add(row);
                dgvElenco.Rows[i].Cells["NomeFileDocument"].Value = nomeReport;
                dgvElenco.Rows[i].Cells["NomeFileDocumentCompleto"].Value = nomeReport.ToUpper();
                dgvElenco.Rows[i].Cells["Altro1"].Value = "ALTRO 1";
                dgvElenco.Rows[i].Cells["Altro2"].Value = "ALRO 2";
                dgvElenco.Rows[i].Cells["Altro3"].Value = "ALTRO 3";
            }
        }
    }
}
