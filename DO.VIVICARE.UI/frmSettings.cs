using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace DO.VIVICARE.UI
{
    public partial class frmSettings : Form
    {
        WebClient client;
        private const string _path = "http://www.netsasa.it/Libraries/";
        private const string _file = "list.txt";
        private const string _voce = "Scarica";

        public frmSettings()
        {
            InitializeComponent();
            SetDataGrid();
        }

        private void SetDataGrid()
        {
            dgvElenco.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvElenco.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);

        }
        private void btnChoose_Click(object sender, System.EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnAddNew_Click(object sender, System.EventArgs e)
        {
            Properties.Settings.Default["UserPathReport"] = textBox1.Text;
            Properties.Settings.Default.Save();
        }

        private void GetListUpdated()
        {
            using (client = new WebClient())
            {
                try
                {
                    // occhio se https, usare altro metodo
                    WebClient client = new WebClient();
                    Stream stream = client.OpenRead(_path + _file);
                    StreamReader reader = new StreamReader(stream);
                    string content = reader.ReadToEnd();

                    string[] sep = new string[] { "\r\n" };
                    string[] lines = content.Split(sep, StringSplitOptions.None);

                    foreach (var dll in lines)
                    {
                        string[] nome = dll.Split(';');
                        var row = new DataGridViewRow();
                        var i = dgvElenco.Rows.Add(row);
                        dgvElenco.Rows[i].Cells["NomeFile"].Value = nome[0];
                        dgvElenco.Rows[i].Cells["NomeFileCompleto"].Value = nome[1];
                        dgvElenco.Rows[i].Cells["Download"].Value = _voce;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void dgvElenco_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 0) return;
            DataGridViewRow row = dgvElenco.Rows[dgvElenco.CurrentCell.RowIndex];

            switch (dgvElenco.Columns[e.ColumnIndex].Name)
            {
                case "Download":
                    lblResult.Text = string.Empty;

                    Thread thread = new Thread(() =>
                    {
                        client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                        client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);

                        Uri URL = new Uri(_path + row.Cells["NomeFileCompleto"].Value.ToString());
                        client.DownloadFileAsync(URL, Properties.Settings.Default["UserPathReport"] + "/" + row.Cells["NomeFileCompleto"].Value.ToString());

                    });
                    thread.Start();
                    break;
            }
        }
        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                BeginInvoke((MethodInvoker)delegate {
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
            textBox1.Text = Properties.Settings.Default["UserPathReport"].ToString();
        }
        private void btnExit_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void frmSettings_Shown(object sender, EventArgs e)
        {
            LoadDefault();
            GetListUpdated();
        }

    }
}
