using System;
using System.Windows.Forms;

namespace DO.VIVICARE.UI
{
    public partial class frmArchivioDocumenti : Form
    {
        string nomeFile;
        public frmArchivioDocumenti()
        {
            InitializeComponent();
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Excel File";
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Excel File|*.xlsx;*.xls";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                nomeFile = openFileDialog1.FileName;

                if (nomeFile.Trim() != "")
                {
                    // TO DO
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
