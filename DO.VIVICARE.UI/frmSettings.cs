using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace DO.VIVICARE.UI
{
    public partial class frmSettings : Form
    {
        WebClient client;
        private const string _path = "http://www.netsasa.it/Libraries/";

        private const string _fileDocuments = "listDocuments.txt";
        private const string _fileReports = "listReports.txt";
        private const string _voce = "Scarica";

        public frmSettings()
        {
            InitializeComponent();
            SetDataGrid();
        }

        private void SetDataGrid()
        {
            dgvElencoDocuments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvElencoDocuments.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            dgvElencoDocuments.Columns["NomeFileDocument"].FillWeight = 150;

            dgvElencoReports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvElencoReports.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            dgvElencoReports.Columns["NomeFileReport"].FillWeight = 150;
        }

        private void btnChoose_Click(object sender, System.EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                txtPath.Text = folderBrowserDialog1.SelectedPath;
        }

        private void btnAddNew_Click(object sender, System.EventArgs e)
        {
            Properties.Settings.Default["UserPathDefault"] = txtPath.Text;
            Properties.Settings.Default.Save();
        }

        private void GetListDocumentsUpdated()
        {
            using (client = new WebClient())
            {
                try
                {
                    WebClient client = new WebClient();
                    Stream stream = client.OpenRead(_path + _fileDocuments);
                    StreamReader reader = new StreamReader(stream);
                    string content = reader.ReadToEnd();

                    string[] sep = new string[] { "\r\n" };
                    string[] lines = content.Split(sep, StringSplitOptions.None);

                    dgvElencoDocuments.Rows.Clear();
                    foreach (var dll in lines)
                    {
                        string[] nome = dll.Split(';');
                        var row = new DataGridViewRow();
                        var i = dgvElencoDocuments.Rows.Add(row);
                        dgvElencoDocuments.Rows[i].Cells["NomeFileDocument"].Value = nome[0];
                        dgvElencoDocuments.Rows[i].Cells["NomeFileDocumentCompleto"].Value = nome[1];
                        dgvElencoDocuments.Rows[i].Cells["DownloadDocument"].Value = _voce;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void GetListReportsUpdated()
        {
            using (client = new WebClient())
            {
                try
                {
                    WebClient client = new WebClient();
                    Stream stream = client.OpenRead(_path + _fileReports);
                    StreamReader reader = new StreamReader(stream);
                    string content = reader.ReadToEnd();

                    string[] sep = new string[] { "\r\n" };
                    string[] lines = content.Split(sep, StringSplitOptions.None);

                    dgvElencoReports.Rows.Clear();
                    foreach (var dll in lines)
                    {
                        string[] nome = dll.Split(';');
                        var row = new DataGridViewRow();
                        var i = dgvElencoReports.Rows.Add(row);
                        dgvElencoReports.Rows[i].Cells["NomeFileReport"].Value = nome[0];
                        dgvElencoReports.Rows[i].Cells["NomeFileReportCompleto"].Value = nome[1];
                        dgvElencoReports.Rows[i].Cells["DownloadReport"].Value = _voce;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    lblResult.Text = "Aggiornamento completato!";
                });
            }
            else
                MessageBox.Show(e.Error.Message);

            ((WebClient)sender).Dispose();
        }

        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //BeginInvoke((MethodInvoker)delegate {
            //    lblResult.Text = "Aggiornamento in corso... " + e.ProgressPercentage.ToString() + "%";
            //});
        }

        private void LoadDefault()
        {
            txtPath.Text = Properties.Settings.Default["UserPathDefault"].ToString();
        }

        private void btnExit_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void frmSettings_Shown(object sender, EventArgs e)
        {
            LoadDefault();
            GetListDocumentsUpdated();
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == tabControl.TabPages["tabPageDocuments"])
            {
                GetListDocumentsUpdated();
            }
            else if (tabControl.SelectedTab == tabControl.TabPages["tabPageReports"])
            {
                GetListReportsUpdated();
            }
        }

        public bool CheckFolder(string folder)
        {
            string path = Path.Combine(
                                    Properties.Settings.Default["UserPathDefault"].ToString(),
                                    folder);
            try
            {
                if (Directory.Exists(path)) return true;

                DirectoryInfo di = Directory.CreateDirectory(path);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void dgvElencoDocuments_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string cartella = Properties.Settings.Default["UserFolderDocuments"].ToString();
            lblResult.Text = string.Empty;
            if (e.ColumnIndex < 0 || string.IsNullOrEmpty(txtPath.Text) || !CheckFolder(cartella)) return;
            DataGridViewRow row = dgvElencoDocuments.Rows[dgvElencoDocuments.CurrentCell.RowIndex];

            switch (dgvElencoDocuments.Columns[e.ColumnIndex].Name)
            {
                case "DownloadDocument":
                    Thread thread = new Thread(() =>
                    {
                        client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                        client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);

                        Uri URL = new Uri(_path + cartella + "/" + row.Cells["NomeFileDocumentCompleto"].Value.ToString());
                        client.DownloadFileAsync(URL, Path.Combine(
                                                            Properties.Settings.Default["UserPathDefault"].ToString(),
                                                            cartella,
                                                            row.Cells["NomeFileDocumentCompleto"].Value.ToString()));
                    });
                    thread.Start();
                    break;
            }
        }

        private void dgvElencoReports_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string cartella = Properties.Settings.Default["UserFolderReports"].ToString();
            lblResult.Text = string.Empty;
            if (e.ColumnIndex < 0 || string.IsNullOrEmpty(txtPath.Text) || !CheckFolder(cartella)) return;
            DataGridViewRow row = dgvElencoReports.Rows[dgvElencoReports.CurrentCell.RowIndex];

            switch (dgvElencoReports.Columns[e.ColumnIndex].Name)
            {
                case "DownloadReport":

                    Thread thread = new Thread(() =>
                    {
                        client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                        client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);

                        Uri URL = new Uri(_path + cartella + "/" + row.Cells["NomeFileReportCompleto"].Value.ToString());
                        client.DownloadFileAsync(URL, Path.Combine(
                                                            Properties.Settings.Default["UserPathDefault"].ToString(),
                                                            cartella,
                                                            row.Cells["NomeFileReportCompleto"].Value.ToString()));
                    });
                    thread.Start();
                    break;
            }

        }
    }
}
