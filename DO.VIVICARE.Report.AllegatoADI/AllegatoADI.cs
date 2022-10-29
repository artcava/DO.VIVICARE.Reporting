using DO.VIVICARE.Reporter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DO.VIVICARE.Report.AllegatoADI
{
    [ReportReference(Name = "AllegatoADI", Description = "Allegato per Valorizzazione ADI Lazio")]
    public class AllegatoADI : BaseReport
    {
        /// <summary>
        /// 
        /// </summary>
        public AllegatoADI()
        {
            DocumentNames = new string[] { "Valorizzazione" };
            Phantom = new Patient();
        }
        /// <summary>
        /// 
        /// </summary>
        public override void CreateParameters()
        {
            Parameters.Clear();
            Parameters.Add(new ReportParameter
            {
                Description = "Anno",
                Name = "Year",
                Type = "Int",
                ReturnValue = null
            });
            Parameters.Add(new ReportParameter
            {
                Description = "Mese",
                Name = "Month",
                Type = "Int",
                ReturnValue = null
            });
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Execute()
        {
            var list = new List<Tuple<string, string, string>>();
            var nameReport = "unkown";
            var ua = (ReportReferenceAttribute)GetType().GetCustomAttribute(typeof(ReportReferenceAttribute));
            if (ua != null)
            {
                nameReport = ua.Name;
            }
            var now = DateTime.Now;
            var nameFileWithoutExt = "AllegatoADI";

            try
            {
                var documents = Documents;

                var doc = documents.Find(x => x.AttributeName == "Valorizzazione");
                if (doc == null)
                {
                    throw new Exception("Valorizzazione non trovato!");
                }
                var listValorizzazione = doc.Records;

                int year = 0;
                var objYear = GetParamValue("Year");
                if (objYear != null) year = (int)objYear;
                int month = 0;
                var objMonth = GetParamValue("Month");
                if (objMonth != null) month = (int)objMonth;

                DateTime start = new DateTime(year, month, 1);
                DateTime end = start.AddMonths(1).AddDays(-1);

                nameFileWithoutExt = $"{nameReport}-{year:0000}{month:00}.{now:dd-MM-yyyy.HH.mm.ss}";

                List<AllegatoADI> reportAllegatoADI = new List<AllegatoADI>();
                //List<string> patients = new List<string>();

                foreach (dynamic adi in listValorizzazione)
                {
                    if (adi.ASL != "ASL LATINA") continue;

                    var val = reportAllegatoADI.Where(r => (r.PatientName == adi.PatientName) 
                                                        && (r.District == adi.ASL + " " + adi.District)).FirstOrDefault();
                    if (val == null)
                    {
                        val = new AllegatoADI { SheetName = adi.PatientName, PatientName = adi.PatientName, District = adi.ASL + " " + adi.District };
                        reportAllegatoADI.Add(val);
                        //if (!patients.Contains(val.PatientName))
                        //    patients.Add(val.PatientName);

                        for (DateTime d = start; d <= end; d = d.AddDays(1))
                        {
                            Patient row = new Patient { PatientName = val.PatientName, District = val.District, ActivityDate = d };
                            val.SheetRecords.Add(row);
                        }

                    }

                    Patient sheetrow = val.SheetRecords.Where(s => ((Patient)s).PatientName == adi.PatientName && ((Patient)s).District == adi.ASL + " " + adi.District && ((Patient)s).ActivityDate == adi.ActivityDate).FirstOrDefault() as Patient;
                    if (sheetrow == null) continue;

                    sheetrow.HourFktNumberTotal += adi.HourFktNumberTotal + adi.HourTpnNumberTotal + adi.HourTerNumberTotal;
                    sheetrow.HourInfNumberTotal += adi.HourInfNumberTotal;
                    sheetrow.HourLogNumberTotal += adi.HourLogNumberTotal;
                    sheetrow.HourOssNumbertotal += adi.HourOssNumbertotal;
                    sheetrow.BasePrice = adi.BasePacketTotal;

                    sheetrow.AccessDocNumber += adi.AccessAneNumber + adi.AccessChiNumber;
                    sheetrow.AccessDocCost += adi.SpecialistAccessTotal;
                    sheetrow.DiagnosticCost += adi.RXDomTotal + adi.EcoDomTotal;
                    sheetrow.TransportCost += adi.TransInfTotal + adi.TransDocTotal;

                    //Se non c'è pacchetto base devo togliere il -4
                    if (sheetrow.BasePrice > 0)
                    {
                        sheetrow.BasePacketNumber = ((sheetrow.HourNumberTotal - 4) / 4) <= 0 ? 0 : ((sheetrow.HourNumberTotal - 4) / 4);
                        sheetrow.ReliefPacketNumber= ((sheetrow.HourNumberTotal - 4) / 4) >= 0 ? sheetrow.HourOssNumbertotal / 5 : ((sheetrow.HourNumberTotal + sheetrow.HourOssNumbertotal - 4) / 5) <= 0 ? 0 : (sheetrow.HourNumberTotal + sheetrow.HourOssNumbertotal - 4) / 5;
                    }
                    else
                    {
                        sheetrow.BasePacketNumber = (sheetrow.HourNumberTotal / 4) <= 0 ? 0 : (sheetrow.HourNumberTotal / 4);
                        sheetrow.ReliefPacketNumber = (sheetrow.HourNumberTotal / 4) >= 0 ? sheetrow.HourOssNumbertotal / 5 : ((sheetrow.HourNumberTotal + sheetrow.HourOssNumbertotal) / 5) <= 0 ? 0 : (sheetrow.HourNumberTotal + sheetrow.HourOssNumbertotal) / 5;
                    }

                    sheetrow.ReliefPacketTotal = adi.ReliefPacketTotal + adi.HourInfTotal + adi.HourRehabTotal + adi.HourOssTotal;

                    val.AidTotal += sheetrow.NetTotal;
                    val.ExtraMedicalVisit += sheetrow.AccessDocCost;
                    val.Diagnostic += sheetrow.DiagnosticCost;
                    val.Transport += sheetrow.TransportCost;
                }

                ResultRecords.AddRange(reportAllegatoADI);

                Manager.CreateExcelFile(this, nameFileWithoutExt); //crea file excel xlsx
                Manager.CreateFile(this, nameFileWithoutExt); //crea file txt
                Manager.CreateFile(this, nameFileWithoutExt, true); //crea file csv

                var destinationFilePath = Path.Combine(Manager.Reports, $"{nameFileWithoutExt}.xlsx");
                Manager.Settings.UpdateReport(nameFileWithoutExt, nameReport, "xlsx", destinationFilePath, now, XMLSettings.ReportStatus.FileOK);
                Manager.Settings.Save();

            }
            catch (Exception ex)
            {
                list.Add(Tuple.Create("Report", "Elaborazione report AllegatoADI", $"Errore interno : {ex.Message}"));
            }
            finally
            {
                WriteLog(list, nameFileWithoutExt);
            }
        }

        #region Member
        [ReportMemberReference(Column = "A", Position = 1, ColumnName = "DISTRETTO", Length = 50, FieldName = "District")]
        public string District { get; set; }

        [ReportMemberReference(Column = "B", Position = 2, ColumnName = "NOME PAZIENTE", Length = 50, Required = true, FieldName = "PatientName")]
        public string PatientName { get; set; }

        [ReportMemberReference(Column = "C", Position = 3, ColumnName = "TOTALE ASSISTENZA", FieldName = "AidTotal")]
        public decimal AidTotal { get; set; }

        [ReportMemberReference(Column = "D", Position = 4, ColumnName = "VISITE MEDICHE EXTRA", FieldName = "ExtraMedicalVisit")]
        public decimal ExtraMedicalVisit { get; set; }

        [ReportMemberReference(Column = "E", Position = 5, ColumnName = "DIAGNOSTICA'", FieldName = "Diagnostic")]
        public decimal Diagnostic { get; set; }

        [ReportMemberReference(Column = "F", Position = 6, ColumnName = "TRASPORTI", FieldName = "Transport")]
        public decimal Transport { get; set; }

        [ReportMemberReference(Column = "G", Position = 7, ColumnName = "TOTALE FATTURATO", FieldName = "InvoiceTotal")]
        public decimal InvoiceTotal { get { return AidTotal + ExtraMedicalVisit + Diagnostic + Transport; } }


        public class Patient:BaseSheet
        {
            [ReportMemberReference(Column = "A", Position = 1, ColumnName = "CONTRAENTE", Length = 50, FieldName = "District")]
            public string District { get; set; }

            [ReportMemberReference(Column = "B", Position = 2, ColumnName = "PAZIENTE", Length = 50, Required = true, FieldName = "PatientName")]
            public string PatientName { get; set; }

            [ReportMemberReference(Column = "C", Position = 3, ColumnName = "DATA", FieldName = "ActivityDate", Format = "dd/MM/yyyy")]
            public DateTime ActivityDate { get; set; }

            [ReportMemberReference(Column = "D", Position = 4, ColumnName = "ORE FKT", FieldName = "HourFktNumberTotal")]
            public decimal HourFktNumberTotal { get; set; }

            [ReportMemberReference(Column = "E", Position = 5, ColumnName = "ORE INF", FieldName = "HourInfNumberTotal")]
            public decimal HourInfNumberTotal { get; set; }

            [ReportMemberReference(Column = "F", Position = 6, ColumnName = "ORE LOGO", FieldName = "HourLogNumberTotal")]
            public decimal HourLogNumberTotal { get; set; }

            [ReportMemberReference(Column = "G", Position = 7, ColumnName = "FKT + INF + LOGO", FieldName = "HourNumberTotal")]
            public decimal HourNumberTotal { get { return HourFktNumberTotal + HourInfNumberTotal + HourLogNumberTotal; } }

            [ReportMemberReference(Column = "H", Position = 8, ColumnName = "ORE OSS", FieldName = "HourOssNumbertotal")]
            public decimal HourOssNumbertotal { get; set; }

            [ReportMemberReference(Column = "I", Position = 9, ColumnName = "BASE", FieldName = "BasePrice")]
            public decimal BasePrice { get; set; }

            [ReportMemberReference(Column = "J", Position = 10, ColumnName = "PACCHETTO SOLL INF", FieldName = "BasePacketNumber")]
            public decimal BasePacketNumber { get; set; }

            [ReportMemberReference(Column = "K", Position = 11, ColumnName = "PACCHETTO SOLL OSS", FieldName = "ReliefPacketNumber")]
            public decimal ReliefPacketNumber { get; set; }

            [ReportMemberReference(Column = "L", Position = 12, ColumnName = "TOTALE SOLLIEVO", FieldName = "ReliefPacketTotal")]
            public decimal ReliefPacketTotal { get; set; }

            [ReportMemberReference(Column = "M", Position = 13, ColumnName = "TOTALE FATTURATO", FieldName = "TotalValue")]
            public decimal TotalValue { get { return BasePrice + ReliefPacketTotal; } }

            [ReportMemberReference(Column = "N", Position = 14, ColumnName = "BASE SCONTO", FieldName = "DiscountBase")]
            public decimal DiscountBase { get { return (TotalValue > 300) ? TotalValue - 300 : 0; } }

            [ReportMemberReference(Column = "O", Position = 15, ColumnName = "€ 0,10", FieldName = "Discount")]
            public decimal Discount { get { return DiscountBase * 0.1M; } }

            [ReportMemberReference(Column = "P", Position = 16, ColumnName = "TOTALE NETTO", FieldName = "NetTotal")]
            public decimal NetTotal { get { return TotalValue-Discount; } }

            [ReportMemberReference(Column = "Q", Position = 17, ColumnName = "ACCESSI MEDICO", FieldName = "AccessDocNumber")]
            public decimal AccessDocNumber { get; set; }

            [ReportMemberReference(Column = "R", Position = 18, ColumnName = "COSTO VIS. MEDICHE EXTRA", FieldName = "AccessDocCost")]
            public decimal AccessDocCost { get; set; }

            [ReportMemberReference(Column = "S", Position = 19, ColumnName = "COSTO DIAGNOSTICA", FieldName = "DiagnosticCost")]
            public decimal DiagnosticCost { get; set; }

            [ReportMemberReference(Column = "T", Position = 20, ColumnName = "COSTO TRASPORTO", FieldName = "TransportCost")]
            public decimal TransportCost { get; set; }

            [ReportMemberReference(Column = "U", Position = 21, ColumnName = "TOTALE FATTURA", FieldName = "InvoiceTotal")]
            public decimal InvoiceTotal { get { return NetTotal + AccessDocCost + DiagnosticCost + TransportCost; } }
        }
        #endregion
    }
}
