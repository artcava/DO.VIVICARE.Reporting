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
        ///// <summary>
        ///// 
        ///// </summary>
        //public override void Execute()
        //{
        //    var list = new List<Tuple<string, string, string>>();
        //    var nameReport = "unkown";
        //    var ua = (ReportReferenceAttribute)this.GetType().GetCustomAttribute(typeof(ReportReferenceAttribute));
        //    if (ua != null)
        //    {
        //        nameReport = ua.Name;
        //    }
        //    var now = DateTime.Now;
        //    var nameFileWithoutExt = "Valorizzazione";

        //    try
        //    {
        //        var documents = Documents;

        //        var doc = documents.Find(x => x.AttributeName == "ADIAltaIntensita");
        //        if (doc == null)
        //        {
        //            throw new Exception("ADIAltaIntensita non trovato!");
        //        }
        //        var listADIAltaIntensita = doc.Records;

        //        doc = documents.Find(x => x.AttributeName == "LazioHealthWorker");
        //        if (doc == null)
        //        {
        //            throw new Exception("LazioHealthWorker non trovato!");
        //        }
        //        var listLazioHealthWorker = doc.Records;

        //        int year = 0;
        //        var objYear = GetParamValue("Year");
        //        if (objYear != null) year = (int)objYear;
        //        int month = 0;
        //        var objMonth = GetParamValue("Month");
        //        if (objMonth != null) month = (int)objMonth;

        //        nameFileWithoutExt = $"{nameReport}-{year:0000}{month:00}.{now:dd-MM-yyyy.HH.mm.ss}";

        //        List<Valorizzazione> reportValorizzazione = new List<Valorizzazione>();

        //        foreach(dynamic adi in listADIAltaIntensita)
        //        {
        //            // Creo o utilizzo una riga esistente di valorizzazione
        //            var val = reportValorizzazione.Where(r => r.PatientName == adi.Patient && r.ASL == adi.ASL && r.District == adi.District && r.ActivityDate == adi.Date).FirstOrDefault();
        //            if (val == null)
        //            {
        //                val = new Valorizzazione { PatientName = adi.Patient, ASL = adi.ASL, District = adi.District, ActivityDate = adi.Date };
        //                reportValorizzazione.Add(val);
        //            }

        //            // Operatore Sanitario, per individuare il WorkType
        //            dynamic oss = listLazioHealthWorker.Where((dynamic os) => os.NameKey == adi.NameKey).FirstOrDefault();
        //            if (oss == null) continue;

        //            // Prelievi e Trasporti sono identificati con un WorkType specifico
        //            switch (((string)adi.Activity).ToUpper())
        //            {
        //                case "PRELIEVO EMATICO (ADI ALTA INTENSITA’)":
        //                case "PRELIEVO EMATICO (ADI ALTA INTENSITA' - Frosinone)":
        //                case "PRELIEVO VENOSO (SIAT ASL FROSINONE)":
        //                    oss.WorkType = "PRELIEVO";
        //                    break;
        //                case "SERVIZIO TRASPORTO SANITARIO (ADI ALTA INTENSITA’)":
        //                    switch (((string)oss.WorkType).ToUpper())
        //                    {
        //                        case "ANESTESISTA":
        //                        case "MEDICO CHIRURGO":
        //                            oss.WorkType = "TRASPORTO CON MEDICO";
        //                            break;
        //                        default:
        //                            oss.WorkType = "TRASPORTO CON INFERMIERE";
        //                            break;
        //                    }
        //                    break;
        //            }

        //            var duration = ((adi.Duration >= 1) ? adi.Duration : 1);

        //            // In base alla durata e al WorkType definisco i pacchetti
        //            switch (((string)oss.WorkType).ToUpper())
        //            {
        //                case "ANESTESISTA":
        //                    val.AccessAneNumber += 1;
        //                    break;
        //                case "FISIOTERAPISTA":
        //                    val.HourFktNumberTotal += duration / 4;
        //                    break;
        //                case "INFERMIERE":
        //                    switch (adi.Activity)
        //                    {
        //                        case "ACCESSO OSS (ADI ALTA INTENSITA' - FROSINONE)":
        //                        case "ACCESSO OSS (ADI ALTA INTENSITA’)":
        //                        case "ASSIST. (OSS) PAZIENTE ALTA COMPLES (ADI ALTA INTENSITA’)":
        //                        case "ASSIST. (OSS) PAZIENTE ALTA COMPLES (SIAT ASL FROSINONE)":
        //                        case "ASSIST. (OSS) PAZIENTE ALTA COMPLES (SIAT ASL ROMA 6)":
        //                        case "ASSIST. (OSS) PAZIENTE ALTA COMPLES-H (SIAT ASL ROMA 2)":
        //                            val.HourOssNumber+= duration / 5;
        //                            break;
        //                        default:
        //                            val.HourInfNumber += duration / 4;
        //                            break;
        //                    }
        //                    break;
        //                case "INFERMIERE PEDIATRICO":
        //                    val.HourInfNumber += duration / 4;
        //                    break;
        //                case "LOGOPEDISTA":
        //                    val.HourLogNumberTotal += duration / 4;
        //                    break;
        //                case "MEDICO CHIRURGO":
        //                    val.AccessChiNumber += duration / 4;
        //                    break;
        //                case "OPERATORE SOCIO-SANITARIO":
        //                    val.HourOssNumber += duration / 5;
        //                    break;
        //                case "PSICOLOGO":
        //                    val.HourPsiNumberTotal += duration;
        //                    break;
        //                case "TERAPISTA DELLA NEURO E PSICOMOTRICITA' DELL'ETA' EVOLUTIVA":
        //                    val.HourTpnNumberTotal += duration/4;
        //                    break;
        //                case "TERAPISTA OCCUPAZIONALE":
        //                    val.HourTerNumberTotal += duration / 4;
        //                    break;
        //                case "PRELIEVO":
        //                    val.SampleNumber += 1;
        //                    break;
        //                case "TRASPORTO CON INFERMIERE":
        //                    val.TransportNurseNumber += 1;
        //                    break;
        //                case "TRASPORTO CON MEDICO":
        //                    val.TransportDoctorNumber += 1;
        //                    break;
        //            }
        //        }

        //        foreach(var val in reportValorizzazione)
        //        {
        //            double _pacchettiInfermieristici = val.HourInfNumber;
        //            double _pacchettiRiabilitativi = val.HourFktNumberTotal + val.HourLogNumberTotal + val.HourTpnNumberTotal + val.HourTerNumberTotal;
        //            double _pacchettiOSS = val.HourOssNumber;

        //            val.BasePacketValue = 120;
        //            val.ReliefPacketValue = 108;
        //            val.SpecialistAccessNumber = val.AccessAneNumber + val.AccessChiNumber;
        //            val.SpecialistAccessValue = (val.SpecialistAccessNumber > 2) ? 120 : 0;

        //            if (_pacchettiRiabilitativi < 1)
        //            {
        //                if ((_pacchettiRiabilitativi + _pacchettiInfermieristici) < 1)
        //                {
        //                    val.HourInfValue = 30;
        //                    val.HourRehabValue = 30;
        //                    val.TotalValue += (30 * (_pacchettiRiabilitativi + _pacchettiInfermieristici) * 4);
        //                    val.HourInfNumber += _pacchettiInfermieristici * 4;
        //                    val.HourRehabNumber += _pacchettiRiabilitativi * 4;
        //                }
        //                else
        //                {
        //                    val.HourInfValue = 27;
        //                    val.HourRehabValue = 27;
        //                    val.BasePacketNumber++;
        //                    val.ReliefPacketNumber += (int)Math.Truncate(_pacchettiInfermieristici + _pacchettiRiabilitativi - 1);
        //                    val.TotalValue += 120 + val.ReliefPacketNumber * 108 + (((_pacchettiInfermieristici + _pacchettiRiabilitativi) - (int)Math.Truncate(_pacchettiInfermieristici + _pacchettiRiabilitativi)) * 27 * 4);
        //                    val.HourInfNumber = ((_pacchettiInfermieristici + _pacchettiRiabilitativi) - (int)Math.Truncate(_pacchettiInfermieristici + _pacchettiRiabilitativi)) * 4;
        //                }
        //            }
        //            else
        //            {
        //                val.HourInfValue = 27;
        //                val.HourRehabValue = 27;
        //                val.TotalValue += 120 + ((_pacchettiRiabilitativi - 1) * 27 * 4) + ((int)Math.Truncate(_pacchettiInfermieristici) * 108) + ((_pacchettiInfermieristici - (int)Math.Truncate(_pacchettiInfermieristici)) * 27 * 4);
        //                val.ReliefPacketNumber += (int)Math.Truncate(_pacchettiInfermieristici);
        //                val.BasePacketNumber++;
        //                val.HourRehabNumber += ((_pacchettiRiabilitativi - (int)Math.Truncate(_pacchettiRiabilitativi)) * 4);
        //                val.HourInfNumber += ((_pacchettiInfermieristici - (int)Math.Truncate(_pacchettiInfermieristici)) * 4);
        //            }

        //            //Valorizzazione pacchetto di ore OSS + PSICOLOGO + PRELIEVI
        //            val.TotalValue += (_pacchettiOSS * 21.6 * 5) + val.SampleNumber * 14;
        //            val.HourOssValue = 21.6;
        //            val.SampleValue = 14;
        //            val.ReliefPacketNumber += (int)Math.Truncate(_pacchettiOSS);
        //            val.HourOssNumber = (_pacchettiOSS - (int)Math.Truncate(_pacchettiOSS)) * 5;

        //            //Valorizzazione attività Psicologo
        //            val.TotalValue += Math.Round(val.HourPsiNumberTotal) * 60;
        //            val.HourPsiNumberTotal = Math.Round(val.HourPsiNumberTotal);
        //            val.HourPsyValue = 60;

        //            //Valorizzazione trasporti in ambulanza assistiti
        //            val.TotalValue += (val.TransInfNumber * 62) + (val.TransDocNumber * 104);
        //            val.TransInfNumber = 62;
        //            val.TransDocNumber = 104;
        //        }

        //        foreach(var val in reportValorizzazione)
        //        {
        //            double _valoreBaseDaScontare = (val.BasePacketNumber * val.BasePacketValue) +
        //                (val.ReliefPacketNumber * val.ReliefPacketValue) +
        //                (val.HourInfNumber * val.HourInfValue) +
        //                (val.HourRehabNumber * val.HourRehabValue) +
        //                (val.HourOssNumber * val.HourOssValue);
                    
        //            double _valoreAddizionaleExtra = (val.HourPsiNumberTotal * val.HourPsyValue) +
        //                (val.SampleNumber * val.SampleValue) +
        //                (val.TransInfNumber * val.TransportNurseValue) +
        //                (val.TransDocNumber * val.TransportDoctorValue) +
        //                (val.SpecialistAccessNumber * val.SpecialistAccessValue);

        //            if (_valoreBaseDaScontare > 300)
        //            {
        //                val.Discount = (_valoreBaseDaScontare - 300) * 0.1;
        //                val.NoDiscountValue = _valoreBaseDaScontare + _valoreAddizionaleExtra;
        //                val.TotalValue = (_valoreBaseDaScontare + _valoreAddizionaleExtra) - val.Discount;
        //            }

        //            val.BasePacketTotal = val.BasePacketNumber * val.BasePacketValue;
        //            val.ReliefPacketTotal = val.ReliefPacketNumber * val.ReliefPacketValue;

        //            val.HourInfNumberTotal *= 4;
        //            val.HourFktNumberTotal *= 4;
        //            val.HourLogNumberTotal *= 4;
        //            val.HourTpnNumberTotal *= 4;
        //            val.HourTerNumberTotal *= 4;
        //            val.HourOssNumbertotal *= 5;
        //        }


        //        ResultRecords.AddRange(reportValorizzazione);

        //        Manager.CreateExcelFile(this, nameFileWithoutExt); //crea file excel xlsx
        //        Manager.CreateFile(this, nameFileWithoutExt); //crea file txt
        //        Manager.CreateFile(this, nameFileWithoutExt, true); //crea file csv

        //        var destinationFilePath = Path.Combine(Manager.Reports, $"{nameFileWithoutExt}.xlsx");
        //        Manager.Settings.UpdateReport(nameFileWithoutExt, nameReport, "xlsx", destinationFilePath, now, XMLSettings.ReportStatus.FileOK);
        //        Manager.Settings.Save();
        //    }
        //    catch (Exception ex)
        //    {
        //        list.Add(Tuple.Create("Report", "Elaborazione report Valorizzazione", $"Errore interno : {ex.Message}"));
        //    }
        //    finally
        //    {
        //        WriteLog(list, nameFileWithoutExt);
        //    }
        //}
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

                foreach (dynamic adi in listADIAltaIntensita)
                {
                    // Creo o utilizzo una riga esistente di valorizzazione
                    var val = reportValorizzazione.Where(r => r.PatientName == adi.Patient && r.ASL == adi.ASL && r.District == adi.District && r.ActivityDate == adi.Date).FirstOrDefault();
                    if (val == null)
                    {
                        val = new Valorizzazione { PatientName = adi.Patient, ASL = adi.ASL, District = adi.District, ActivityDate = adi.Date };
                        reportValorizzazione.Add(val);
                    }

                    // Operatore Sanitario, per individuare il WorkType
                    dynamic oss = listLazioHealthWorker.Where((dynamic os) => os.NameKey == adi.NameKey).FirstOrDefault();
                    if (oss == null) continue;

                    // Prelievi e Trasporti sono identificati con un WorkType specifico
                    switch (((string)adi.Activity).ToUpper())
                    {
                        case "PRELIEVO EMATICO (ADI ALTA INTENSITA’)":
                        case "PRELIEVO EMATICO (ADI ALTA INTENSITA' - Frosinone)":
                        case "PRELIEVO VENOSO (SIAT ASL FROSINONE)":
                            oss.WorkType = "PRELIEVO";
                            break;
                        case "SERVIZIO TRASPORTO SANITARIO (ADI ALTA INTENSITA’)":
                            switch (((string)oss.WorkType).ToUpper())
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

                    decimal duration = NumerizeDuration(adi.Duration);

                    duration = ((duration >= 1) ? duration : 1);
                    //duration = NumerizeDuration(duration);

                    // In base alla durata e al WorkType definisco i pacchetti
                    switch (((string)oss.WorkType).ToUpper())
                    {
                        case "ANESTESISTA":
                            val.AccessAneNumber += 1;
                            break;
                        case "FISIOTERAPISTA":
                            val.HourFktNumberTotal += duration;
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
                                    val.HourOssNumber += duration;
                                    break;
                                default:
                                    val.HourInfNumber += duration;
                                    break;
                            }
                            break;
                        case "INFERMIERE PEDIATRICO":
                            val.HourInfNumber += duration;
                            break;
                        case "LOGOPEDISTA":
                            val.HourLogNumberTotal += duration;
                            break;
                        case "MEDICO CHIRURGO":
                            val.AccessChiNumber += duration;
                            break;
                        case "OPERATORE SOCIO-SANITARIO":
                            val.HourOssNumber += duration;
                            break;
                        case "PSICOLOGO":
                            val.HourPsiNumberTotal += duration;
                            break;
                        case "TERAPISTA DELLA NEURO E PSICOMOTRICITA' DELL'ETA' EVOLUTIVA":
                            val.HourTpnNumberTotal += duration;
                            break;
                        case "TERAPISTA OCCUPAZIONALE":
                            val.HourTerNumberTotal += duration;
                            break;
                        case "PRELIEVO":
                            val.SampleNumber += 1;
                            break;
                        case "TRASPORTO CON INFERMIERE":
                            val.TransInfNumber += 1;
                            break;
                        case "TRASPORTO CON MEDICO":
                            val.TransDocNumber += 1;
                            break;
                    }
                }

                // Gestione pacchetti
                foreach (var val in reportValorizzazione)
                {
                    val.HourInfNumberTotal = val.HourInfNumber;
                    val.HourOssNumbertotal = val.HourOssNumber;
                    decimal _pacchettiRiabilitativi = val.HourFktNumberTotal + val.HourLogNumberTotal + val.HourTpnNumberTotal + val.HourTerNumberTotal;

                    val.BasePacketValue = 120;
                    val.ReliefPacketValue = 108;
                    val.SpecialistAccessNumber = val.AccessAneNumber + val.AccessChiNumber;
                    val.SpecialistAccessValue = (val.SpecialistAccessNumber > 2) ? 120 : 0;

                    val.HourInfValue = 27;
                    val.HourRehabValue = 27;


                    if((_pacchettiRiabilitativi + val.HourInfNumberTotal) < 4) // non ci sono pacchetti
                    {
                        val.HourInfValue = 30;
                        val.HourRehabValue = 30;
                        val.HourInfNumber = val.HourInfNumberTotal;
                        val.HourRehabNumber = _pacchettiRiabilitativi;
                    }
                    else
                    {
                        val.BasePacketNumber = 1;

                        switch (val.ASL)
                        {
                            case "ASL ROMA 5":
                            case "ASL ROMA 6":
                            case "ASL FROSINONE":
                                val.HourInfValue = 30;
                                val.HourRehabValue = 30;
                                break;
                        }

                        var rest = (_pacchettiRiabilitativi + val.HourInfNumberTotal - 4);
                        while (rest >= 4)
                        {
                            val.ReliefPacketNumber += 1;
                            rest -= 4;
                        }

                        var infrest = val.HourInfNumberTotal % 4;
                        var rehabrest = _pacchettiRiabilitativi % 4;
                        if (rest >= infrest)
                        {
                            val.HourInfNumber = infrest;
                            val.HourRehabNumber = rest - infrest;
                        }
                        else
                        {
                            val.HourInfNumber = rest;
                            val.HourRehabNumber = 0;
                        }
                    }

                    val.TotalValue += (val.BasePacketNumber * 120) + (val.ReliefPacketNumber * 108) + (val.HourInfNumber * val.HourInfValue) + (val.HourRehabNumber * val.HourRehabValue);

                    //Valorizzazione pacchetto di ore OSS + PRELIEVI
                    val.TotalValue += (val.HourOssNumbertotal * 21.6M) + val.SampleNumber * 14;
                    val.HourOssValue = 21.6M;
                    val.SampleValue = 14;
                    val.ReliefPacketNumber += (int)Math.Truncate(val.HourOssNumbertotal / 5);
                    val.HourOssNumber = val.HourOssNumbertotal - ((int)Math.Truncate(val.HourOssNumbertotal / 5) * 5);

                    //Valorizzazione attività Psicologo
                    val.HourPsyValue = 60;
                    val.HourPsyNumber = val.HourPsiNumberTotal = Math.Round(val.HourPsiNumberTotal);
                    val.TotalValue += val.HourPsiNumberTotal * val.HourPsyValue;

                    //Valorizzazione trasporti in ambulanza assistiti
                    val.TransInfValue = 62;
                    val.TransDocValue = 104;
                    //val.TransInfTotal = val.TransInfNumber * val.TransInfValue;
                    val.TransDocTotal = val.TransDocNumber * val.TransDocValue;
                    //val.TotalValue += val.TransInfTotal + val.TransDocTotal;
                    val.TotalValue += val.TransDocTotal;
                }

                foreach (var val in reportValorizzazione)
                {
                    decimal _valoreBaseDaScontare = (val.BasePacketNumber * val.BasePacketValue) +
                        (val.ReliefPacketNumber * val.ReliefPacketValue) +
                        (val.HourInfNumber * val.HourInfValue) +
                        (val.HourRehabNumber * val.HourRehabValue) +
                        (val.HourOssNumber * val.HourOssValue);

                    decimal _valoreAddizionaleExtra = (val.HourPsiNumberTotal * val.HourPsyValue) +
                        (val.SampleNumber * val.SampleValue) +
                        (val.TransDocNumber * val.TransDocValue) +
                        (val.SpecialistAccessNumber * val.SpecialistAccessValue);

                    val.Discount = (_valoreBaseDaScontare > 300) ? (_valoreBaseDaScontare - 300) * 0.1M : 0;
                    val.NoDiscountValue = _valoreBaseDaScontare + _valoreAddizionaleExtra;
                    val.TotalValue = val.NoDiscountValue - val.Discount;

                    val.BasePacketTotal = val.BasePacketNumber * val.BasePacketValue;
                    val.ReliefPacketTotal = val.ReliefPacketNumber * val.ReliefPacketValue;

                    val.HourInfTotal = val.HourInfNumber * val.HourInfValue;
                    val.HourRehabTotal = val.HourRehabNumber * val.HourRehabValue;
                    val.HourOssTotal = val.HourOssNumber * val.HourOssValue;
                    val.HourPsyTotal = val.HourPsyNumber * val.HourPsyValue;
                    val.SampleTotal = val.SampleNumber * val.SampleValue;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        private decimal NumerizeDuration(TimeSpan duration)
        {
            var intpart = duration.Hours;
            var decimalpart = duration.Minutes;
            if (decimalpart == 0) return intpart;
            return intpart + (decimal)(decimalpart / 6 * 10) / 100;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="duration"></param>
        /// <returns></returns>
        private TimeSpan HourizeDuration(decimal duration)
        {
            var intpart = (int)Math.Truncate(duration);
            var decimalpart = (duration - intpart) * 100;
            if (decimalpart == 0) return new TimeSpan(intpart, 0, 0);
            return new TimeSpan(intpart, (int)decimalpart * 6 / 10, 0);
        }

        #region Member
        [ReportMemberReference(Column = "A", Position = 1, ColumnName = "NOME PAZIENTE", Length = 50, Required = true, FieldName = "PatientName")]
        public string PatientName { get; set; }

        [ReportMemberReference(Column = "B", Position = 2, ColumnName = "AZIENDA SANITARIA", Length = 50, Required = true, FieldName = "ASL")]
        public string ASL { get; set; }

        [ReportMemberReference(Column = "C", Position = 3, ColumnName = "DISTRETTO", Length = 50, FieldName = "District")]
        public string District { get; set; }

        [ReportMemberReference(Column = "D", Position = 4, ColumnName = "DATA ATTIVITA'", FieldName = "ActivityDate", Format ="dd/MM/yyyy")]
        public DateTime ActivityDate { get; set; }

        [ReportMemberReference(Column = "E", Position = 5, ColumnName = "VALORE TOTALE CALCOLATO", FieldName = "TotalValue")]
        public decimal TotalValue { get; set; }

        [ReportMemberReference(Column = "F", Position = 6, ColumnName = "VALORE NON SCONTATO", FieldName = "NoDiscountValue")]
        public decimal NoDiscountValue { get; set; }

        [ReportMemberReference(Column = "G", Position = 7, ColumnName = "SCONTO", FieldName = "Discount")]
        public decimal Discount { get; set; }

        #region PACCHETTI BASE
        [ReportMemberReference(Column = "H", Position = 8, ColumnName = "NUMERO PACCHETTI BASE", FieldName = "BasePacketNumber")]
        public decimal BasePacketNumber { get; set; }

        [ReportMemberReference(Column = "I", Position = 9, ColumnName = "VALORE PACCHETTO BASE", FieldName = "BasePacketValue")]
        public decimal BasePacketValue { get; set; }

        [ReportMemberReference(Column = "J", Position = 10, ColumnName = "TOTALE PACCHETTO BASE", FieldName = "BasePacketTotal")]
        public decimal BasePacketTotal { get; set; }
        #endregion

        #region PACCHETTI SOLLIEVO
        [ReportMemberReference(Column = "K", Position = 11, ColumnName = "NUMERO PACCHETTI SOLLIEVO", FieldName = "ReliefPacketNumber")]
        public decimal ReliefPacketNumber { get; set; }

        [ReportMemberReference(Column = "L", Position = 12, ColumnName = "VALORE PACCHETTO SOLLIEVO", FieldName = "ReliefPacketValue")]
        public decimal ReliefPacketValue { get; set; }

        [ReportMemberReference(Column = "M", Position = 13, ColumnName = "TOTALE PACCHETTO SOLLIEVO", FieldName = "ReliefPacketTotal")]
        public decimal ReliefPacketTotal { get; set; }
        #endregion

        #region ORE INFERMIERISTICHE
        [ReportMemberReference(Column = "N", Position = 14, ColumnName = "NUMERO ORE INF.", FieldName = "HourInfNumber")]
        public decimal HourInfNumber { get; set; }

        [ReportMemberReference(Column = "O", Position = 15, ColumnName = "VALORE ORE INF.", FieldName = "HourInfValue")]
        public decimal HourInfValue { get; set; }

        [ReportMemberReference(Column = "P", Position = 16, ColumnName = "TOTALE ORE INF.", FieldName = "HourInfTotal")]
        public decimal HourInfTotal { get; set; }
        #endregion

        #region ORE RIABILITAZIONE
        [ReportMemberReference(Column = "Q", Position = 17, ColumnName = "NUMERO ORE RIAB. (FKT/LOGO/TPNEE/TO)", FieldName = "HourRehabNumber")]
        public decimal HourRehabNumber { get; set; }
        
        [ReportMemberReference(Column = "R", Position = 18, ColumnName = "VALORE ORE RIAB.", FieldName = "HourRehabValue")]
        public decimal HourRehabValue { get; set; }

        [ReportMemberReference(Column = "S", Position = 19, ColumnName = "TOTALE ORE RIAB.", FieldName = "HourRehabTotal")]
        public decimal HourRehabTotal { get; set; }
        #endregion

        #region ORE OSS
        [ReportMemberReference(Column = "T", Position = 20, ColumnName = "NUMERO ORE OSS", FieldName = "HourOssNumber")]
        public decimal HourOssNumber { get; set; }
        
        [ReportMemberReference(Column = "U", Position = 21, ColumnName = "VALORE ORE OSS", FieldName = "HourOssValue")]
        public decimal HourOssValue { get; set; }

        [ReportMemberReference(Column = "V", Position = 22, ColumnName = "TOTALE VALORE ORE OSS", FieldName = "HourOssTotal")]
        public decimal HourOssTotal { get; set; }
        #endregion

        #region ORE PSICOLOGO
        [ReportMemberReference(Column = "W", Position = 23, ColumnName = "NUMERO ORE PSICOLOGO", FieldName = "HourPsyNumber")]
        public decimal HourPsyNumber { get; set; }
        
        [ReportMemberReference(Column = "X", Position = 24, ColumnName = "VALORE ORE PSICOLOGO", FieldName = "HourPsyValue")]
        public decimal HourPsyValue { get; set; }

        [ReportMemberReference(Column = "Y", Position = 25, ColumnName = "TOTALE ORE PSICOLOGO", FieldName = "HourPsyTotal")]
        public decimal HourPsyTotal { get; set; }
        #endregion

        #region PRELIEVI
        [ReportMemberReference(Column = "Z", Position = 26, ColumnName = "NUMERO PRELIEVI", FieldName = "SampleNumber")]
        public decimal SampleNumber { get; set; }
        
        [ReportMemberReference(Column = "AA", Position = 27, ColumnName = "VALORE PRELIEVO", FieldName = "SampleValue")]
        public decimal SampleValue { get; set; }

        [ReportMemberReference(Column = "AB", Position = 28, ColumnName = "TOTALE PRELIEVI", FieldName = "SampleTotal")]
        public decimal SampleTotal { get; set; }
        #endregion

        #region TRASPORTI CON INF
        [ReportMemberReference(Column = "AC", Position = 29, ColumnName = "NUMERO TRASPORTI CON INF", FieldName = "TransInfNumber")]
        public decimal TransInfNumber { get; set; }
        
        [ReportMemberReference(Column = "AD", Position = 30, ColumnName = "VALORE TRASPORTO CON INF", FieldName = "TransInfValue")]
        public decimal TransInfValue { get; set; }

        [ReportMemberReference(Column = "AE", Position = 31, ColumnName = "TOTALE TRASPORTO CON INF", FieldName = "TransInfTotal")]
        public decimal TransInfTotal { get; set; }
        #endregion

        #region TRASPORTI CON MED
        [ReportMemberReference(Column = "AF", Position = 32, ColumnName = "NUMERO TRASPORTI CON MED", FieldName = "TransDocNumber")]
        public decimal TransDocNumber { get; set; }
        
        [ReportMemberReference(Column = "AG", Position = 33, ColumnName = "VALORE TRASPORTO CON MED", FieldName = "TransDocValue")]
        public decimal TransDocValue { get; set; }

        [ReportMemberReference(Column = "AH", Position = 34, ColumnName = "TOTALE TRASPORTO CON MED", FieldName = "TransDocTotal")]
        public decimal TransDocTotal { get; set; }
        #endregion

        #region ACCESSI SPECIALISTICI
        [ReportMemberReference(Column = "AI", Position = 35, ColumnName = "NUMERO ACCESSI SPECIALISTICI", FieldName = "SpecialistAccessNumber")]
        public decimal SpecialistAccessNumber { get; set; }
        
        [ReportMemberReference(Column = "AJ", Position = 36, ColumnName = "VALORE ACCESSI SPECIALISTICI", FieldName = "SpecialistAccessValue")]
        public decimal SpecialistAccessValue { get; set; }

        [ReportMemberReference(Column = "AK", Position = 37, ColumnName = "TOTALE ACCESSI SPECIALISTICI", FieldName = "SpecialistAccessTotal")]
        public decimal SpecialistAccessTotal { get; set; }
        #endregion

        #region ORE TOTALI
        [ReportMemberReference(Column = "AL", Position = 38, ColumnName = "NR ORE INF TOTALI", FieldName = "HourInfNumberTotal")]
        public decimal HourInfNumberTotal { get; set; }
        
        [ReportMemberReference(Column = "AM", Position = 39, ColumnName = "Nr ORE FKT TOTALI", FieldName = "HourFktNumberTotal")]
        public decimal HourFktNumberTotal { get; set; }
        
        [ReportMemberReference(Column = "AN", Position = 40, ColumnName = "NR ORE LOGOPEDISTA TOTALI", FieldName = "HourLogNumberTotal")]
        public decimal HourLogNumberTotal { get; set; }
        
        [ReportMemberReference(Column = "AO", Position = 41, ColumnName = "Nr ORE TPNEE TOTALI", FieldName = "HourTpnNumberTotal")]
        public decimal HourTpnNumberTotal { get; set; }
        
        [ReportMemberReference(Column = "AP", Position = 42, ColumnName = "NR ORE TO TOTALI", FieldName = "HourTerNumberTotal")]
        public decimal HourTerNumberTotal { get; set; }
        
        [ReportMemberReference(Column = "AQ", Position = 43, ColumnName = "Nr ORE PSICOLOGO TOTALI", FieldName = "HourPsiNumberTotal")]
        public decimal HourPsiNumberTotal { get; set; }
        
        [ReportMemberReference(Column = "AR", Position = 44, ColumnName = "NR ORE OSS TOTALI", FieldName = "HourOssNumbertotal")]
        public decimal HourOssNumbertotal { get; set; }
        #endregion

        #region NUMERO ACCESSI, PRELIEVI, TRASPORTI
        [ReportMemberReference(Column = "AS", Position = 45, ColumnName = "NR ACCESSI ANESTESISTA TOTALI", FieldName = "AccessAneNumber")]
        public decimal AccessAneNumber { get; set; }
        
        [ReportMemberReference(Column = "AT", Position = 46, ColumnName = "NR ACCESSI MEDICO CHIRURGO TOTALI", FieldName = "AccessChiNumber")]
        public decimal AccessChiNumber { get; set; }
        
        [ReportMemberReference(Column = "AU", Position = 47, ColumnName = "NR PRELIEVI", FieldName = "SampleNumber2")]
        public decimal SampleNumber2 { get; set; }
        
        //[ReportMemberReference(Column = "AV", Position = 48, ColumnName = "NR TRASPORTI INF.", FieldName = "TransInfNumber", IsDouble = true)]
        //public double TransInfNumber { get; set; }
        
        //[ReportMemberReference(Column = "AW", Position = 49, ColumnName = "NR TRASPORTI MED.", FieldName = "TransDocNumber", IsDouble = true)]
        //public double TransDocNumber { get; set; }
        #endregion

        #endregion
    }
}
