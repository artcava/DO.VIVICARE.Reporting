using DO.VIVICARE.Reporter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace DO.VIVICARE.UI
{
    public partial class frmDocuments : Form
    { 
        public frmDocuments()
        {
            InitializeComponent();
        }
        private void cmbChoose_SelectionChangeCommitted(object sender, EventArgs e)
        {
            switch (cmbChoose.Text)
            { 
                case "ICONE GRANDI":
                    lvReport.View = View.LargeIcon;
                    break;
                case "ICONE PICCOLE":
                    lvReport.View = View.SmallIcon;
                    break;
            }
        }

        private void frmDocuments_Load(object sender, EventArgs e)
        {
            cmbChoose.SelectedIndex = 0;

            FileInfo assemblyFile = new FileInfo(Assembly.GetExecutingAssembly().Location);
            foreach (var f in Manager.GetDocuments(assemblyFile.Directory.FullName + "\\Libraries"))
            {
                lvReport.AddRow(0, f.Name.ToUpper());
            }

            lvReport.SmallImageList = imageListPiccole;
            lvReport.LargeImageList = imageListGrandi;


        }

        private List<string> RecuperaReport()
        {
            return new List<string> { "REPORT16", "REPORT18", "REPORT20", "REPORT22", "REPORT24", "REPORT26", "REPORT36", "REPORT42", "REPORT45", "REPORT48", "REPORT50", "REPORT52", "REPORT60", "REPORT62", "REPORT73" };
        }
    }
}
