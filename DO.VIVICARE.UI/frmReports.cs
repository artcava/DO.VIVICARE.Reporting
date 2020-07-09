using DO.VIVICARE.Report.Dietetica;
using DO.VIVICARE.Reporter;
using System;
using System.Drawing;
using System.IO;
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
            //MessageBox.Show("SUPER PROCESS STARTS!!!");
            // solo per test
           
            var reportDietetica = new Dietetica();
            //input ASST
            reportDietetica.SetYear(DateTime.Now.Year);
            reportDietetica.SetMonth(DateTime.Now.Month);
            reportDietetica.SetLastProgressiveNumber(100);

            reportDietetica.LoadDocuments();

            reportDietetica.Execute();

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
            MessageBox.Show("File excel e file tracciato Dietetica creato correttamente!", "Avviso", MessageBoxButtons.OK, MessageBoxIcon.Information);

            
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

                foreach (var f in Manager.GetReports())
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
