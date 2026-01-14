using DO.VIVICARE.Reporter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DO.VIVICARE.UI
{
    public partial class frmInputReportParameter : Form
    {
        public List<ReportParameter> Parameters { get; set; }

        public frmInputReportParameter()
        {
            InitializeComponent();
        }

        public frmInputReportParameter(string title, List<ReportParameter> parameters)
        {
            InitializeComponent();

            Parameters = parameters;
            this.Text = title;
            LoadParameters();
        }

        private void LoadParameters()
        {
            dgvParameters.Rows.Clear();
            foreach (var p in Parameters)
            {
                var row = new DataGridViewRow();
                var i = dgvParameters.Rows.Add(row);
                dgvParameters.Rows[i].Cells["ParameterName"].Value = p.Name;
                dgvParameters.Rows[i].Cells["ParameterDescription"].Value = p.Description;
                if (p.Type == "Document")
                {
                    DataGridViewComboBoxCell c = new DataGridViewComboBoxCell
                    {
                        DataSource = GetDataCombo(p),
                        Value = p.DocumentValueId,
                        ValueMember = "Value",
                        DisplayMember = "Text"
                    };
                    c.Tag = p.Name;
                    dgvParameters.Rows[i].Cells["ParameterValue"] = c;
                }
                else
                {
                    DataGridViewTextBoxCell t = new DataGridViewTextBoxCell
                    {
                        Value = p.ReturnValue
                    };
                    dgvParameters.Rows[i].Cells["ParameterValue"] = t;
                }
            }
        }

        private List<DataCombo> GetDataCombo(ReportParameter param)
        {
            var documentName = param.DocumentName;
            var ret = new List<DataCombo>();
            try
            {
                var document = Manager.GetDocuments().Find(a => a.Attribute.Name == documentName);
                if (document == null) return ret;
                var list = Manager.Settings.GetDocumentValues(XMLSettings.LibraryType.Document, document.Attribute.Name);
                document.Document.SourceFilePath = string.Empty;
                if (list != null) document.Document.SourceFilePath = Path.Combine(Manager.Documents, list[0] + list[1]);
                if (string.IsNullOrEmpty(document.Document.SourceFilePath)) return ret;
                document.Document.AttributeName = document.Attribute.Name;
                document.Document.LoadRecords();
                if (document.Document.Records.Count == 0) return ret;
                foreach (var item in document.Document.Records)
                {
                    var propField = item.GetType().GetProperty(param.DocumentFieldId);
                    var value = propField.GetValue(item);
                    propField = item.GetType().GetProperty(param.DocumentFieldText);
                    var text = propField.GetValue(item);
                    ret.Add(new DataCombo
                    {
                        Value = Convert.ToString(value),
                        Text = Convert.ToString(text),
                        Data = item
                    });
                }
                return ret;
            }
            catch (Exception)
            {
                return ret;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            var rows = dgvParameters.Rows;
            foreach (DataGridViewRow row in rows)
            {
                var cell = row.Cells["ParameterName"];
                var parameterName = (string)cell.Value;
                var param = Parameters.FirstOrDefault(p => p.Name == parameterName);
                if (param != null)
                {
                    if (param.Type == "Document")
                    {
                        var value = (string)row.Cells["ParameterValue"].Value;
                        if (!string.IsNullOrEmpty(value))
                        {
                            var cellValue = (DataGridViewComboBoxCell)row.Cells["ParameterValue"];
                            var dt = (List<DataCombo>)cellValue.DataSource;
                            var dataCombo = dt.FirstOrDefault(r => r.Value == value);
                            if (dataCombo != null)
                            {
                                param.DocumentValueId = value;
                                param.ReturnValue = dataCombo.Data;
                            }
                            else
                            {
                                param.DocumentValueId = string.Empty;
                                param.ReturnValue = null;
                            }
                        }
                        else
                        {
                            param.DocumentValueId = string.Empty;
                            param.ReturnValue = null;
                        }
                    }
                    else
                    {
                        var value = (string)row.Cells["ParameterValue"].Value;
                        if (string.IsNullOrEmpty(value))
                        {
                            param.ReturnValue = null;
                        }
                        else
                        {
                            // da completare per i tipi possibili di parametri in input
                            if (param.Type == "Int")
                            {
                                int intValue = 0;
                                if (int.TryParse(value, out intValue)) param.ReturnValue = intValue;
                                else param.ReturnValue = null;
                            }
                            else if (param.Type == "DateTime")
                            {
                                DateTime dtmValue = DateTime.MinValue;
                                if (DateTime.TryParse(value, out dtmValue)) param.ReturnValue = dtmValue;
                                else param.ReturnValue = null;
                            }
                            else
                            {
                                param.ReturnValue = value;
                            }
                        }
                    }
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        //private void dgvParameters_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        //{

        //    ComboBox combo = e.Control as ComboBox;
        //    if (combo != null)
        //    {
        //        // Remove an existing event-handler, if present, to avoid 
        //        // adding multiple handlers when the editing control is reused.
        //        combo.SelectedIndexChanged -=
        //            new EventHandler(ComboBox_SelectedIndexChanged);

        //        // Add the event handler. 
        //        combo.SelectedIndexChanged +=
        //            new EventHandler(ComboBox_SelectedIndexChanged);
        //    }
        //}

        //private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    ComboBox cmb = (ComboBox)sender;

        //    var item = cmb.SelectedItem;

        //    var data = (DataCombo)item;

        //    var row = dgvParameters.CurrentRow;

        //    var cell = dgvParameters.CurrentCell;

        //    var parameterName = (string)cell.Tag;

        //}


    }
}
