using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DO.VIVICARE.Reporter
{
    public class ExcelManager : IDisposable
    {

        protected MemoryStream msExcel;

        SpreadsheetDocument _document;
        WorkbookPart _wbPart;
        Sheet _sheet;
        SheetData _sheetData;

        String[] _columnRefs;

        public string Extension { get; set; }

        public bool LoadFile(string filePath, string className)
        {
            if (string.IsNullOrEmpty(filePath)) return false;
            try
            {
                var file = new FileInfo(filePath);
                if (!file.Exists) throw new Exception(string.Format("Unable to find file: {0}", filePath));

                Extension = file.Extension;
                if (Extension.ToLower() == ".xlsx") return true;

                var connString = ConnectionString(filePath);

                DataTable dt = new DataTable();
                string sheetName = string.Empty;

                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand { Connection = conn };

                    // Get all Sheets in Excel File
                    DataTable dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    // Loop through all Sheets to get data
                    foreach (DataRow dr in dtSheet.Rows)
                    {
                        sheetName = dr["TABLE_NAME"].ToString();

                        // Get all rows from the Sheet
                        cmd.CommandText = "SELECT * FROM [" + sheetName + "]";

                        dt = new DataTable { TableName = sheetName };

                        OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                        da.Fill(dt);

                        break;

                    }

                    cmd = null;
                    conn.Close();
                }

                if (!string.IsNullOrEmpty(sheetName))
                {
                    sheetName = className;
                    msExcel = new MemoryStream();
                    using (ExcelPackage pck = new ExcelPackage(msExcel))
                    {
                        ExcelWorksheet ws = pck.Workbook.Worksheets.Add(sheetName);
                        ws.Cells["A1"].LoadFromDataTable(dt, true);
                        pck.Save();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Create(string filePath, string sheetName)
        {
            try
            {
                if (filePath != null) _document = SpreadsheetDocument.Create(filePath, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook);
                else return false;

                // Add a WorkbookPart to the document.
                WorkbookPart workbookpart = _document.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();

                // Add a WorksheetPart to the WorkbookPart.
                WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                SheetData sheetData = new SheetData();
                worksheetPart.Worksheet = new Worksheet(sheetData);

                // Add Sheets to the Workbook.
                Sheets sheets = _document.WorkbookPart.Workbook.
                    AppendChild<Sheets>(new Sheets());

                // Append a new worksheet and associate it with the workbook.
                Sheet sheet = new Sheet()
                {
                    Id = _document.WorkbookPart.
                    GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = sheetName
                };
                sheets.Append(sheet);

                // Retrieve a reference to the workbook part.
                _wbPart = _document.WorkbookPart;

                // get the first (or named) sheet of workbook

                _sheetData = sheetData;

                _sheet = sheet;

                // Throw an exception if there is no sheet.
                if (_sheet == null)
                {
                    throw new ArgumentException(String.Format("Sheet with name {0} not found", _sheet.Name));
                }
                
                return true;
            }
            catch (Exception ex)
            {
                
                return false;
            }
        }

        public void Open(bool isEditable, string filePath = null)
        {
            if (filePath != null) _document = SpreadsheetDocument.Open(filePath, isEditable);
            else _document = SpreadsheetDocument.Open(msExcel, isEditable);

           

            // Retrieve a reference to the workbook part.
            _wbPart = _document.WorkbookPart;

            // get the first (or named) sheet of workbook
            IEnumerable<Sheet> sheets = _wbPart.Workbook.Descendants<Sheet>();
            _sheet = sheets.FirstOrDefault();

            // Throw an exception if there is no sheet.
            if (_sheet == null)
            {
                throw new ArgumentException(String.Format("Sheet with name {0} not found", _sheet.Name));
            }

            //WorksheetPart worksheetPart = (WorksheetPart)(_wbPart.GetPartById(_sheet.Id));
            //Worksheet workSheet = worksheetPart.Worksheet;
            //SheetData sheetData = workSheet.GetFirstChild<SheetData>();
            //Columns columns = workSheet.GetFirstChild<Columns>();
            //IEnumerable<Row> rows = sheetData.Descendants<Row>();
            

            //Row firstRow = rows.FirstOrDefault();

            //var firstRowCells = firstRow.Descendants<Cell>();

            //var numFirstRowCells = firstRowCells.Count();

            //var totColumn = mumberCols;

            //List<String> columnRefs = new List<String>();

            ////verifico il numero effettivo delle colonne del foglio excel
            //for (int i = 0; i < totColumn; i++)
            //{
            //    String colName = ColumnIndexToColumnLetter(i + 1);
            //    columnRefs.Add(colName);
            //}

            //var cells = row.Descendants<Cell>();
            ////columnRefs.Clear();

            //foreach (var cell in cells)
            //{
            //    String colName = GetColumnName(cell.CellReference);
            //    columnRefs.Add(colName);
            //}

            //_columnRefs = columnRefs.ToArray();

        }
        
        private string ConnectionString(string file)
        {
            var props = new Dictionary<string, string>
                   {
                    {"Provider", "Microsoft.Jet.OLEDB.4.0"},
                    //{"Provider", "Microsoft.ACE.OLEDB.12.0"},
                    {"Extended Properties", "Excel 8.0"},
                    {"Data Source", file}
                    };
            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> prop in props)
            {
                sb.Append(prop.Key);
                sb.Append('=');
                sb.Append(prop.Value);
                sb.Append(';');
            }

            return sb.ToString();
        }

        private string ColumnIndexToColumnLetter(int colIndex)
        {
            int div = colIndex;
            string colLetter = String.Empty;
            int mod = 0;

            while (div > 0)
            {
                mod = (div - 1) % 26;
                colLetter = (char)(65 + mod) + colLetter;
                div = (int)((div - mod) / 26);
            }
            return colLetter;
        }

        public bool Save()
        {
            try
            {
                _wbPart.Workbook.Save();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AddRow(List<Cell> cells, DocumentFormat.OpenXml.UInt32Value rowIndex )
        {
            try
            {
                Row row = new Row() { RowIndex = rowIndex };

                foreach (var cell in cells)
                {
                    row.Append(cell);
                }

                _sheetData.Append(row);

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public IEnumerable<Row> GetRows(List<FilterDocument> filters)
        {
            WorksheetPart worksheetPart = (WorksheetPart)(_wbPart.GetPartById(_sheet.Id));
            Worksheet workSheet = worksheetPart.Worksheet;
            SheetData sheetData = workSheet.GetFirstChild<SheetData>();
            IEnumerable<Row> rows = sheetData.Descendants<Row>();
            if (filters==null) return rows;
            if (filters.Count()==0) return rows;
            var filteredRows = rows.Where(row =>
            {
                var cells = row.Descendants<Cell>();               
                foreach (var f in filters)
                {
                    var cell = cells.FirstOrDefault(c => GetColumnName(c.CellReference) == f.Column);
                    if (cell!=null)
                    {
                        string value = GetCellValue(cell);
                        if (value != f.Value) return false;
                    }
                    else return false;
                }
                return true;
            });
            return filteredRows;
        }

     
        public int GetTotalColumns
        {
            get
            {
                //WorksheetPart worksheetPart = (WorksheetPart)(_wbPart.GetPartById(_sheet.Id));
                //Worksheet workSheet = worksheetPart.Worksheet;
                //SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                //IEnumerable<Row> rows = sheetData.Descendants<Row>();
                //return rows.ElementAt(10).Count();

                //recupero il numero di colonne precedente memorizzato
                return _columnRefs.Count();
            }
        }
        
        public int GetTotalRowsColumns
        {
            get
            {
                WorksheetPart worksheetPart = (WorksheetPart)(_wbPart.GetPartById(_sheet.Id));
                Worksheet workSheet = worksheetPart.Worksheet;
                SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                IEnumerable<Row> rows = sheetData.Descendants<Row>();

                return rows.Count();
            }
        }

        public String GetCellValueByIndex(int rowIndex, int colIndex)
        {
            // just for test
            //if (rowIndex==2 && colIndex==4)
            //{
            //    string value = GetCellValueByIndex(rowIndex, colIndex, false);
            //}
            return GetCellValueByIndex(rowIndex, colIndex, false);
        }

        public String GetCellValueByIndex(int rowIndex, int colIndex, bool cellNameIfNotFound)
        {
            try
            {
                String cellAddress = _columnRefs[colIndex - 1] + rowIndex;
                return GetCellValue(cellAddress, cellNameIfNotFound);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public String GetCellValue(String cellAddress, bool cellNameIfNotFound)
        {
            string value = null;

            // Retrieve a reference to the worksheet part.
            WorksheetPart wsPart = (WorksheetPart)(_wbPart.GetPartById(_sheet.Id));

            // Use its Worksheet property to get a reference to the cell 
            // whose address matches the address you supplied.
            Cell theCell = wsPart.Worksheet.Descendants<Cell>().
                Where(c => c.CellReference == cellAddress).FirstOrDefault();

            // If the cell does not exist, return an empty string.
            if (theCell != null)
            {
                value = GetCellValue(theCell);
            }
            else
            {
                if (cellNameIfNotFound)
                    value = cellAddress;

            }

            return value;
        }

        public String GetCellValue(Cell theCell)
        {
            String value = null;

            if (theCell != null)
            {
                value = theCell.InnerText;
                if (theCell.DataType != null)
                {
                    switch (theCell.DataType.Value)
                    {
                        case CellValues.SharedString:

                            // For shared strings, look up the value in the
                            // shared strings table.
                            var stringTable =
                                _wbPart.GetPartsOfType<SharedStringTablePart>()
                                .FirstOrDefault();

                            // If the shared string table is missing, something 
                            // is wrong. Return the index that is in
                            // the cell. Otherwise, look up the correct text in 
                            // the table.
                            if (stringTable != null)
                            {
                                var index = int.Parse(value);
                                value = stringTable.SharedStringTable.ElementAt(index).InnerText;
                            }
                            break;

                        case CellValues.Boolean:
                            switch (value)
                            {
                                case "0":
                                    value = "FALSE";
                                    break;
                                default:
                                    value = "TRUE";
                                    break;
                            }
                            break;
                        case CellValues.String:
                            if (theCell.CellFormula != null)
                            {
                                value = theCell.CellValue.InnerText;
                            }
                            break;
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Given a cell name, parses the specified cell to get the column name.
        /// </summary>
        /// <param name="cellReference">Address of the cell (ie. B2)</param>
        /// <returns>Column Name (ie. B)</returns>
        public string GetColumnName(string cellReference)
        {
            // Create a regular expression to match the column name portion of the cell name.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellReference);

            return match.Value;
        }

        public void Dispose()
        {
            // close file
            // close Excel Workbook
            // and release all library refs
            if (_document!=null)
            {
                _document.Close();
                _document.Dispose();
                _document = null;
            }
        }

    }
}
