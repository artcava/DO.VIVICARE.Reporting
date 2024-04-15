using DocumentFormat.OpenXml;
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
using FontSize = DocumentFormat.OpenXml.Spreadsheet.FontSize;

namespace DO.VIVICARE.Reporter
{
    public class ExcelManager : IDisposable
    {
        protected MemoryStream msExcel;

        SpreadsheetDocument _document;
        WorkbookPart _wbPart;
        Sheets _sheets;
        Sheet _sheet;
        SheetData _sheetData;
        WorkbookStylesPart _wbStylesPart;

        String[] _columnRefs;
        /// <summary>
        /// 
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="className"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="sheetName"></param>
        /// <returns></returns>
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
                Sheets sheets = _document.WorkbookPart.Workbook.AppendChild<Sheets>(new Sheets());

                // Append a new worksheet and associate it with the workbook.
                Sheet sheet = new Sheet()
                {
                    Id = _document.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = sheetName
                };
                sheets.Append(sheet);

                // Retrieve a reference to the workbook part.
                _wbPart = _document.WorkbookPart;
                _wbStylesPart = _wbPart.AddNewPart<WorkbookStylesPart>();
                GenerateWorkbookStylesPartContent();

                // get the first (or named) sheet of workbook
                _sheetData = sheetData;

                _sheets = sheets;

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isEditable"></param>
        /// <param name="filePath"></param>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="colIndex"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        public bool AddSheet(string sheetName, bool current = true)
        {
            try
            {
                    // Add a blank WorksheetPart.
                    WorksheetPart newWorksheetPart = _document.WorkbookPart.AddNewPart<WorksheetPart>();

                    SheetData sheetData = new SheetData();
                    newWorksheetPart.Worksheet = new Worksheet(sheetData);

                    //Sheets sheets = spreadSheet.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                    string relationshipId = _document.WorkbookPart.GetIdOfPart(newWorksheetPart);

                    // Get a unique ID for the new worksheet.
                    uint sheetId = 1;
                    if (_sheets.Elements<Sheet>().Count() > 0)
                    {
                        sheetId = _sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                    }

                    // Append the new worksheet and associate it with the workbook.
                    Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
                    _sheets.Append(sheet);
                    if (current) _sheetData = sheetData;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cells"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public bool AddRow(List<Cell> cells, UInt32Value rowIndex )
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="totals"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        public bool AddTotals(Dictionary<ReportMemberReferenceAttribute, decimal> totals, UInt32Value rowIndex)
        {
            try
            {
                Row row = new Row() { RowIndex = rowIndex };

                foreach (var col in totals.Keys)
                {
                    if (!col.HaveSum && !col.HaveText) continue;

                    var cell = new Cell
                    {
                        CellReference = $"{col.Column}{rowIndex}",
                        StyleIndex = col.TotalStyleIndex,
                        CellValue = new CellValue(col.HaveSum ? totals[col].ToString() : col.TextForSum),
                        DataType = CellValues.String
                    };
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        /// <returns></returns>
        public String GetCellValueByIndex(int rowIndex, int colIndex)
        {
            // just for test
            //if (rowIndex==2 && colIndex==4)
            //{
            //    string value = GetCellValueByIndex(rowIndex, colIndex, false);
            //}
            return GetCellValueByIndex(rowIndex, colIndex, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="colIndex"></param>
        /// <param name="cellNameIfNotFound"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellAddress"></param>
        /// <param name="cellNameIfNotFound"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="theCell"></param>
        /// <returns></returns>
        public String GetCellValue(Cell theCell)
        {
            String value = null;

            if (theCell != null)
            {
                value = theCell.InnerText;
                if (theCell.DataType != null)
                {
                    if(theCell.DataType.Value == CellValues.SharedString)
                    {
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
                    }
                    if (theCell.DataType.Value == CellValues.Boolean)
                    {
                        switch (value)
                        {
                            case "0":
                                value = "FALSE";
                                break;
                            default:
                                value = "TRUE";
                                break;
                        }
                    }
                    if (theCell.DataType.Value == CellValues.String)
                    {
                        if (theCell.CellFormula != null)
                        {
                            value = theCell.CellValue.InnerText;
                        }
                    }
                    #region Old code
                    //switch (theCell.DataType.Value)
                    //{
                    //    //case CellValues.SharedString:
                    //    case "s":

                    //        // For shared strings, look up the value in the
                    //        // shared strings table.
                    //        var stringTable =
                    //            _wbPart.GetPartsOfType<SharedStringTablePart>()
                    //            .FirstOrDefault();

                    //        // If the shared string table is missing, something 
                    //        // is wrong. Return the index that is in
                    //        // the cell. Otherwise, look up the correct text in 
                    //        // the table.
                    //        if (stringTable != null)
                    //        {
                    //            var index = int.Parse(value);
                    //            value = stringTable.SharedStringTable.ElementAt(index).InnerText;
                    //        }
                    //        break;

                    //    case CellValues.Boolean:
                    //        switch (value)
                    //        {
                    //            case "0":
                    //                value = "FALSE";
                    //                break;
                    //            default:
                    //                value = "TRUE";
                    //                break;
                    //        }
                    //        break;
                    //    case CellValues.String:
                    //        if (theCell.CellFormula != null)
                    //        {
                    //            value = theCell.CellValue.InnerText;
                    //        }
                    //        break;
                    //}
                    #endregion
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="workbookStylesPart1"></param>
        private void GenerateWorkbookStylesPartContent()
        {
            Stylesheet stylesheet1 = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

            Fonts fonts = new Fonts(
                new Font( // Index 0 - default
                    new FontSize() { Val = 11D },
                    new Color() { Theme = 1U },
                    new FontName() { Val = "Calibri" }
                ),
                new Font( // Index 1
                    new FontSize() { Val = 11D },
                    new Bold(),
                    new Color() { Theme = 1U },
                    new FontName() { Val = "Calibri" }
                ),
                new Font( // Index 2
                    new FontSize() { Val = 10D },
                    new Bold(),
                    new Color() { Rgb = "FFFFFF" },
                    new FontName() { Val = "Calibri" }
                ),
                new Font( // Index 3
                    new FontSize() { Val = 10D },
                    new Color() { Rgb = "FFFFFF" },
                    new FontName() { Val = "Calibri" }
                )
            );

            Fills fills = new Fills(
                new Fill(
                    new PatternFill()
                    {
                        PatternType = PatternValues.None
                    }
                ), // Index 0 - default
                new Fill(
                    new PatternFill()
                    {
                        PatternType = PatternValues.Gray125
                    }
                ), // Index 1 - default
                new Fill(
                    new PatternFill(new ForegroundColor { Rgb = new HexBinaryValue() { Value = "FF999999" } })
                    {
                        PatternType = PatternValues.Solid
                    }
                ), // Index 2 - Gray
                new Fill(
                    new PatternFill(new ForegroundColor { Rgb = "FFFF99" })
                    {
                        PatternType = PatternValues.Solid
                    }
                ), // Index 3 - header
                new Fill(
                    new PatternFill(new ForegroundColor { Rgb = "FF6600" })
                    {
                        PatternType = PatternValues.Solid
                    }
                ), // Index 5 - DarkFill
                new Fill(
                    new PatternFill(new ForegroundColor { Rgb = "333333" })
                    {
                        PatternType = PatternValues.Solid
                    }
                ), // Index 6 - BlueFill
                new Fill(
                    new PatternFill(new ForegroundColor { Rgb = "99CCFF" })
                    {
                        PatternType = PatternValues.Solid
                    }
                )
            );

            Borders borders = new Borders(
                new Border(), // index 0 default
                new Border( // index 1 black border
                    new LeftBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new RightBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new TopBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new BottomBorder(new Color() { Auto = true }) { Style = BorderStyleValues.Thin },
                    new DiagonalBorder())
            );

            CellStyleFormats cellStyleFormats1 = new CellStyleFormats() { Count = 1U };
            CellFormat cellFormat1 = new CellFormat() { NumberFormatId = 0U, FontId = 0U, FillId = 0U, BorderId = 0U };

            cellStyleFormats1.Append(cellFormat1);

            CellFormats cellFormats = new CellFormats(
                new CellFormat(), // default {StyleIndex = 0}
                new CellFormat { FontId = 0, FillId = 0, BorderId = 1, ApplyBorder = true }, // Header {StyleIndex = 1}
                new CellFormat { FontId = 2, FillId = 2, BorderId = 0, ApplyFont = true, ApplyFill = true }, // Gray header {StyleIndex = 2}
                new CellFormat { FontId = 0, FillId = 3, BorderId = 1, ApplyFill = true, ApplyBorder = true }, // Yellow cell {StyleIndex = 3}
                new CellFormat { FontId = 1, FillId = 0, BorderId = 1, ApplyFont = true, ApplyBorder = true }, // Header boldbordered {StyleIndex = 4}
                new CellFormat { FontId = 2, FillId = 4, BorderId = 0, ApplyFont = true, ApplyFill = true }, // Orange sum {StyleIndex = 5}
                new CellFormat { FontId = 3, FillId = 5, BorderId = 1, ApplyFont = true, ApplyFill = true, ApplyBorder = true }, // Dark cell {StyleIndex = 6}
                new CellFormat { FontId = 2, FillId = 5, BorderId = 1, ApplyFont = true, ApplyFill = true, ApplyBorder = true }, // Dark header {StyleIndex = 7}
                new CellFormat { FontId = 1, FillId = 6, BorderId = 1, ApplyFont = true, ApplyFill = true, ApplyBorder = true }, // Blue header {StyleIndex = 8}
                new CellFormat { FontId = 1, FillId = 6, BorderId = 0, ApplyFont = true, ApplyFill = true } // Blue sum {StyleIndex = 9}
            );

            CellStyles cellStyles1 = new CellStyles() { Count = 1U };
            CellStyle cellStyle1 = new CellStyle() { Name = "Normal", FormatId = 0U, BuiltinId = 0U };

            cellStyles1.Append(cellStyle1);
            DifferentialFormats differentialFormats1 = new DifferentialFormats() { Count = 0U };
            TableStyles tableStyles1 = new TableStyles() { Count = 0U, DefaultTableStyle = "TableStyleMedium2", DefaultPivotStyle = "PivotStyleLight16" };

            stylesheet1.Append(fonts);
            stylesheet1.Append(fills);
            stylesheet1.Append(borders);
            stylesheet1.Append(cellStyleFormats1);
            stylesheet1.Append(cellFormats);
            stylesheet1.Append(cellStyles1);
            stylesheet1.Append(differentialFormats1);
            stylesheet1.Append(tableStyles1);

            _wbStylesPart.Stylesheet = stylesheet1;
            _wbStylesPart.Stylesheet.Save();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            // close file
            // close Excel Workbook
            // and release all library refs
            if (_document!=null)
            {
                _document.Dispose();
                _document = null;
            }
        }

    }
}
