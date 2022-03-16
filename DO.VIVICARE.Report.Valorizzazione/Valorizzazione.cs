using DO.VIVICARE.Reporter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DO.VIVICARE.Report.Valorizzazione
{
    /// <summary>
    /// 
    /// </summary>
    [ReportReference(Name = "Valorizzazione", Description = "Report Valorizzazione ADI Lazio")]
    public class Valorizzazione:BaseReport
    {
        /// <summary>
        /// 
        /// </summary>
        public Valorizzazione()
        {
            DocumentNames = new string[] { "LazioHealthWorker", "ADIAltaIntensita" };
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
            var ua = (ReportReferenceAttribute)this.GetType().GetCustomAttribute(typeof(ReportReferenceAttribute));
            if (ua != null)
            {
                nameReport = ua.Name;
            }
            var now = DateTime.Now;
            var nameFileWithoutExt = "Valorizzazione";

            try
            {
                var documents = Documents;

                var doc = documents.Find(x => x.AttributeName == "ADIAltaIntensita");
                if (doc == null)
                {
                    throw new Exception("ADIAltaIntensita non trovato!");
                }
                var listADIAltaIntensita = doc.Records;

                doc = documents.Find(x => x.AttributeName == "LazioHealthWorker");
                if (doc == null)
                {
                    throw new Exception("LazioHealthWorker non trovato!");
                }
                var listLazioHealthWorker = doc.Records;

                int year = 0;
                var objYear = GetParamValue("Year");
                if (objYear != null) year = (int)objYear;
                int month = 0;
                var objMonth = GetParamValue("Month");
                if (objMonth != null) month = (int)objMonth;

                nameFileWithoutExt = $"{nameReport}-{year:0000}{month:00}.{now:dd-MM-yyyy.HH.mm.ss}";

                List<Valorizzazione> reportValorizzazione = new List<Valorizzazione>();

                foreach(dynamic adi in listADIAltaIntensita)
                {
                    var val = reportValorizzazione.Where(r => r.PatientName == adi.Patient && r.ASL == adi.ASL && r.District == adi.District && r.ActivityDate == adi.Date).FirstOrDefault();
                    if (val == null)
                    {
                        val = new Valorizzazione { PatientName = adi.Patient, ASL = adi.ASL, District = adi.District, ActivityDate = adi.Date };
                        reportValorizzazione.Add(val);
                    }

                    // Operatore Sanitario
                    dynamic oss = listLazioHealthWorker.Where((dynamic os) => os.NameKey == adi.NameKey).FirstOrDefault();
                    if (oss == null) continue;

                    switch (adi.Activity)
                    {
                        case "PRELIEVO EMATICO (ADI ALTA INTENSITA’)":
                        case "PRELIEVO EMATICO (ADI ALTA INTENSITA' - Frosinone)":
                        case "PRELIEVO VENOSO (SIAT ASL FROSINONE)":
                            oss.WorkType = "PRELIEVO";
                            break;
                        case "SERVIZIO TRASPORTO SANITARIO (ADI ALTA INTENSITA’)":
                            switch (oss.WorkType)
                            {
                                case "ANESTESISTA":
                                case "MEDICO CHIRURGO":
                                    oss.WorkType = "TRASPORTO CON MEDICO";
                                    break;
                                default:
                                    oss.WorkType = "TRASPORTO CON INFERMIERE";
                                    break;
                            }
                            break;
                    }

                    var duration = ((adi.Duration >= 1) ? adi.Duration : 1);

                    switch (oss.WorkType)
                    {
                        case "ANESTESISTA":
                            val.AccessAneNumber += 1;
                            break;
                        case "FISIOTERAPISTA":
                            val.HourFktNumber += duration / 4;
                            break;
                        case "INFERMIERE":
                            switch (adi.Activity)
                            {
                                case "ACCESSO OSS (ADI ALTA INTENSITA' - FROSINONE)":
                                case "ACCESSO OSS (ADI ALTA INTENSITA’)":
                                case "ASSIST. (OSS) PAZIENTE ALTA COMPLES (ADI ALTA INTENSITA’)":
                                case "ASSIST. (OSS) PAZIENTE ALTA COMPLES (SIAT ASL FROSINONE)":
                                case "ASSIST. (OSS) PAZIENTE ALTA COMPLES (SIAT ASL ROMA 6)":
                                case "ASSIST. (OSS) PAZIENTE ALTA COMPLES-H (SIAT ASL ROMA 2)":
                                    val.HourOssNumber+= duration / 5;
                                    break;
                                default:
                                    val.HourInfNumber += duration / 4;
                                    break;
                            }
                            break;
                        case "INFERMIERE PEDIATRICO":
                            val.HourInfNumber += duration / 4;
                            break;
                        case "LOGOPEDISTA":
                            val.HourLogNumber += duration / 4;
                            break;
                        case "MEDICO CHIRURGO":
                            val.AccessChiNumber += duration / 4;
                            break;
                        case "OPERATORE SOCIO-SANITARIO":
                            val.HourOssNumber += duration / 5;
                            break;
                        case "PSICOLOGO":
                            val.HourPsiNumber += duration;
                            break;
                        case "TERAPISTA DELLA NEURO E PSICOMOTRICITA' DELL'ETA' EVOLUTIVA":
                            val.HourTpnNumber += duration/4;
                            break;
                        case "TERAPISTA OCCUPAZIONALE":
                            val.HourTerNumber += duration / 4;
                            break;
                        case "PRELIEVO":
                            val.SampleNumber += 1;
                            break;
                        case "TRASPORTO CON INFERMIERE":
                            val.TransportNurseNumber += 1;
                            break;
                        case "TRASPORTO CON MEDICO":
                            val.TransportDoctorNumber += 1;
                            break;
                    }
                }

                foreach(var val in reportValorizzazione)
                {
                    double _pacchettiInfermieristici = val.HourInfNumber;
                    double _pacchettiRiabilitativi = val.HourFktNumber + val.HourLogNumber + val.HourTpnNumber + val.HourTerNumber;
                    double _pacchettiOSS = val.HourOssNumber;

                    val.BasePacketValue = 120;
                    val.ReliefPacketValue = 108;
                    val.SpecialistAccessNumber = val.AccessAneNumber + val.AccessChiNumber;
                    val.SpecialistAccessValue = (val.SpecialistAccessNumber > 2) ? 120 : 0;

                    if (_pacchettiRiabilitativi < 1)
                    {
                        if ((_pacchettiRiabilitativi + _pacchettiInfermieristici) < 1)
                        {
                            val.FractHourInfValue = 30;
                            val.FractHourRehabValue = 30;
                            val.TotalValue += (30 * (_pacchettiRiabilitativi + _pacchettiInfermieristici) * 4);
                            val.FractHourInf += _pacchettiInfermieristici * 4;
                            val.FractHourRehab += _pacchettiRiabilitativi * 4;
                        }
                        else
                        {
                            val.FractHourInfValue = 27;
                            val.FractHourRehabValue = 27;
                            val.BasePacketNumber++;
                            val.ReliefPacketNumber += (int)Math.Truncate(_pacchettiInfermieristici + _pacchettiRiabilitativi - 1);
                            val.TotalValue += 120 + val.ReliefPacketNumber * 108 + (((_pacchettiInfermieristici + _pacchettiRiabilitativi) - (int)Math.Truncate(_pacchettiInfermieristici + _pacchettiRiabilitativi)) * 27 * 4);
                            val.FractHourInf = ((_pacchettiInfermieristici + _pacchettiRiabilitativi) - (int)Math.Truncate(_pacchettiInfermieristici + _pacchettiRiabilitativi)) * 4;
                        }
                    }
                    else
                    {
                        val.FractHourInfValue = 27;
                        val.FractHourRehabValue = 27;
                        val.TotalValue += 120 + ((_pacchettiRiabilitativi - 1) * 27 * 4) + ((int)Math.Truncate(_pacchettiInfermieristici) * 108) + ((_pacchettiInfermieristici - (int)Math.Truncate(_pacchettiInfermieristici)) * 27 * 4);
                        val.ReliefPacketNumber += (int)Math.Truncate(_pacchettiInfermieristici);
                        val.BasePacketNumber++;
                        val.FractHourRehab += ((_pacchettiRiabilitativi - (int)Math.Truncate(_pacchettiRiabilitativi)) * 4);
                        val.FractHourInf += ((_pacchettiInfermieristici - (int)Math.Truncate(_pacchettiInfermieristici)) * 4);
                    }

                    //Valorizzazione pacchetto di ore OSS + PSICOLOGO + PRELIEVI
                    val.TotalValue += (_pacchettiOSS * 21.6 * 5) + val.SampleNumber * 14;
                    val.FractHourOssValue = 21.6;
                    val.SampleValue = 14;
                    val.ReliefPacketNumber += (int)Math.Truncate(_pacchettiOSS);
                    val.FractHourOss = (_pacchettiOSS - (int)Math.Truncate(_pacchettiOSS)) * 5;

                    //Valorizzazione attività Psicologo
                    val.TotalValue += Math.Round(val.HourPsiNumber) * 60;
                    val.HourPsiNumber = Math.Round(val.HourPsiNumber);
                    val.FractHourPsyValue = 60;

                    //Valorizzazione trasporti in ambulanza assistiti
                    val.TotalValue += (val.TransInfNumber * 62) + (val.TransDocNumber * 104);
                    val.TransInfNumber = 62;
                    val.TransDocNumber = 104;
                }

                foreach(var val in reportValorizzazione)
                {
                    double _valoreBaseDaScontare = (val.BasePacketNumber * val.BasePacketValue) +
                        (val.ReliefPacketNumber * val.ReliefPacketValue) +
                        (val.FractHourInf * val.FractHourInfValue) +
                        (val.FractHourRehab * val.FractHourRehabValue) +
                        (val.FractHourOss * val.FractHourOssValue);
                    
                    double _valoreAddizionaleExtra = (val.HourPsiNumber * val.FractHourPsyValue) +
                        (val.SampleNumber * val.SampleValue) +
                        (val.TransInfNumber * val.TransportNurseValue) +
                        (val.TransDocNumber * val.TransportDoctorValue) +
                        (val.SpecialistAccessNumber * val.SpecialistAccessValue);

                    if (_valoreBaseDaScontare > 300)
                    {
                        val.Discount = (_valoreBaseDaScontare - 300) * 0.1;
                        val.TotalValue = (_valoreBaseDaScontare + _valoreAddizionaleExtra) - val.Discount;
                    }

                    val.HourInfNumber *= 4;
                    val.HourFktNumber *= 4;
                    val.HourLogNumber *= 4;
                    val.HourTpnNumber *= 4;
                    val.HourTerNumber *= 4;
                    val.HourOssNumber *= 5;
                }


                ResultRecords.AddRange(reportValorizzazione);

                Manager.CreateExcelFile(this, nameFileWithoutExt); //crea file excel xlsx
                Manager.CreateFile(this, nameFileWithoutExt); //crea file txt
                Manager.CreateFile(this, nameFileWithoutExt, true); //crea file csv

                var destinationFilePath = Path.Combine(Manager.Reports, $"{nameFileWithoutExt}.xlsx");
                Manager.Settings.UpdateReport(nameFileWithoutExt, nameReport, "xlsx", destinationFilePath, now, XMLSettings.ReportStatus.FileOK);
                Manager.Settings.Save();
            }
            catch (Exception ex)
            {
                list.Add(Tuple.Create("Report", "Elaborazione report Valorizzazione", $"Errore interno : {ex.Message}"));
            }
            finally
            {
                WriteLog(list, nameFileWithoutExt);
            }
        }

        #region Member
        [ReportMemberReference(Column = "A", Position = 1, ColumnName = "NOME PAZIENTE", Length = 50, Required = true, FieldName = "PatientName")]
        public string PatientName { get; set; }
        [ReportMemberReference(Column = "B", Position = 2, ColumnName = "AZIENDA SANITARIA", Length = 50, Required = true, FieldName = "ASL")]
        public string ASL { get; set; }
        [ReportMemberReference(Column = "C", Position = 3, ColumnName = "DISTRETTO", Length = 50, FieldName = "District")]
        public string District { get; set; }
        [ReportMemberReference(Column = "D", Position = 4, ColumnName = "DATA ATTIVITA'", FieldName = "ActivityDate")]
        public DateTime ActivityDate { get; set; }
        [ReportMemberReference(Column = "E", Position = 5, ColumnName = "VALORE TOTALE CALCOLATO", FieldName = "TotalValue")]
        public double TotalValue { get; set; }
        [ReportMemberReference(Column = "F", Position = 6, ColumnName = "VALORE NON SCONTATO", FieldName = "NoDiscountValue")]
        public double NoDiscountValue { get; set; }
        [ReportMemberReference(Column = "G", Position = 7, ColumnName = "SCONTO", FieldName = "Discount")]
        public double Discount { get; set; }
        [ReportMemberReference(Column = "H", Position = 8, ColumnName = "Nr PACCHETTI BASE", FieldName = "BasePacketNumber")]
        public double BasePacketNumber { get; set; }
        [ReportMemberReference(Column = "I", Position = 9, ColumnName = "VALORE PACCHETTO BASE", FieldName = "BasePacketValue")]
        public double BasePacketValue { get; set; }
        [ReportMemberReference(Column = "J", Position = 10, ColumnName = "Nr PACCHETTI SOLLIEVO", FieldName = "ReliefPacketNumber")]
        public double ReliefPacketNumber { get; set; }
        [ReportMemberReference(Column = "K", Position = 11, ColumnName = "VALORE PACCHETTO SOLLIEVO", FieldName = "ReliefPacketValue")]
        public double ReliefPacketValue { get; set; }
        [ReportMemberReference(Column = "L", Position = 12, ColumnName = "ORE FRAZIONARIE INF.", FieldName = "FractHourInf")]
        public double FractHourInf { get; set; }
        [ReportMemberReference(Column = "M", Position = 13, ColumnName = "VALORE ORE FRAZIONARIE INF.", FieldName = "FractHourInfValue")]
        public double FractHourInfValue { get; set; }
        [ReportMemberReference(Column = "N", Position = 14, ColumnName = "ORE FRAZIONARIE RIAB. (FKT/LOGO/TPNEE/TO)", FieldName = "FractHourRehab")]
        public double FractHourRehab { get; set; }
        [ReportMemberReference(Column = "O", Position = 15, ColumnName = "VALORE ORE FRAZIONARIE RIAB.", FieldName = "FractHourRehabValue")]
        public double FractHourRehabValue { get; set; }
        [ReportMemberReference(Column = "P", Position = 16, ColumnName = "ORE FRAZIONARIE OSS", FieldName = "FractHourOss")]
        public double FractHourOss { get; set; }
        [ReportMemberReference(Column = "Q", Position = 17, ColumnName = "VALORE ORE FRAZIONARIE OSS", FieldName = "FractHourOssValue")]
        public double FractHourOssValue { get; set; }
        [ReportMemberReference(Column = "R", Position = 18, ColumnName = "ORE PSICOLOGO", FieldName = "FractHourPsy")]
        public double FractHourPsy { get; set; }
        [ReportMemberReference(Column = "S", Position = 19, ColumnName = "VALORE ORE FRAZIONARIE PSICOLOGO", FieldName = "FractHourPsyValue")]
        public double FractHourPsyValue { get; set; }
        [ReportMemberReference(Column = "T", Position = 20, ColumnName = "NR PRELIEVI", FieldName = "SampleNumber")]
        public double SampleNumber { get; set; }
        [ReportMemberReference(Column = "U", Position = 21, ColumnName = "VALORE PRELIEVO", FieldName = "SampleValue")]
        public double SampleValue { get; set; }
        [ReportMemberReference(Column = "V", Position = 22, ColumnName = "NR TRASPORTI CON INF.", FieldName = "TransportNurseNumber")]
        public double TransportNurseNumber { get; set; }
        [ReportMemberReference(Column = "W", Position = 23, ColumnName = "VALORE TRASPORTO CON INF.", FieldName = "TransportNurseValue")]
        public double TransportNurseValue { get; set; }
        [ReportMemberReference(Column = "X", Position = 24, ColumnName = "NR TRASPORTI CON MED.", FieldName = "TransportDoctorNumber")]
        public double TransportDoctorNumber { get; set; }
        [ReportMemberReference(Column = "Y", Position = 25, ColumnName = "VALORE TRASPORTO CON MED.", FieldName = "TransportDoctorValue")]
        public double TransportDoctorValue { get; set; }
        [ReportMemberReference(Column = "Z", Position = 26, ColumnName = "NR ACCESSI SPECIALISTICI", FieldName = "SpecialistAccessNumber")]
        public double SpecialistAccessNumber { get; set; }
        [ReportMemberReference(Column = "AA", Position = 27, ColumnName = "VALORE ACCESSI SPECIALISTICI", FieldName = "SpecialistAccessValue")]
        public double SpecialistAccessValue { get; set; }
        [ReportMemberReference(Column = "AB", Position = 28, ColumnName = "NR ORE INF TOTALI", FieldName = "HourInfNumber")]
        public double HourInfNumber { get; set; }
        [ReportMemberReference(Column = "AC", Position = 29, ColumnName = "Nr ORE FKT TOTALI", FieldName = "HourFktNumber")]
        public double HourFktNumber { get; set; }
        [ReportMemberReference(Column = "AD", Position = 30, ColumnName = "NR ORE LOGOPEDISTA TOTALI", FieldName = "HourLogNumber")]
        public double HourLogNumber { get; set; }
        [ReportMemberReference(Column = "AE", Position = 31, ColumnName = "Nr ORE TPNEE TOTALI", FieldName = "HourTpnNumber")]
        public double HourTpnNumber { get; set; }
        [ReportMemberReference(Column = "AF", Position = 32, ColumnName = "NR ORE TERAPISTA OCCUPAZIONALE TOTALI", FieldName = "HourTerNumber")]
        public double HourTerNumber { get; set; }
        [ReportMemberReference(Column = "AG", Position = 33, ColumnName = "Nr Accessi ANESTESISTA TOTALI", FieldName = "AccessAneNumber")]
        public double AccessAneNumber { get; set; }
        [ReportMemberReference(Column = "AH", Position = 34, ColumnName = "NR Accessi MEDICO CHIRURGO TOTALI", FieldName = "AccessChiNumber")]
        public double AccessChiNumber { get; set; }
        [ReportMemberReference(Column = "AI", Position = 35, ColumnName = "Nr Ore PSICOLOGO TOTALI", FieldName = "HourPsiNumber")]
        public double HourPsiNumber { get; set; }
        [ReportMemberReference(Column = "AJ", Position = 36, ColumnName = "NR ORE OSS TOTALI", FieldName = "HourOssNumber")]
        public double HourOssNumber { get; set; }
        [ReportMemberReference(Column = "AK", Position = 37, ColumnName = "Nr Prelievi", FieldName = "SampleNumber2")]
        public double SampleNumber2 { get; set; }
        [ReportMemberReference(Column = "AL", Position = 38, ColumnName = "NR TRASPORTI INF.", FieldName = "TransInfNumber")]
        public double TransInfNumber { get; set; }
        [ReportMemberReference(Column = "AM", Position = 39, ColumnName = "NR TRASPORTI MED.", FieldName = "TransDocNumber")]
        public double TransDocNumber { get; set; }
        #endregion
    }
}
