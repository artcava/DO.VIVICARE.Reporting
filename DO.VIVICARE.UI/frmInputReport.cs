using System;
using System.Windows.Forms;

namespace DO.VIVICARE.UI
{
    public partial class frmInputReport : Form
    {
        public frmInputReport()
        {
            InitializeComponent();
        }

        private void btnShowReport_Click(object sender, EventArgs e)
        {
            frmShowReport f = new frmShowReport();
            f.ShowDialog();
        }
    }
}
