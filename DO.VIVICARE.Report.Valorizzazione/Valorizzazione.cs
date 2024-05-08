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
            DocumentNames = new string[] { "LazioHealthWorker", "ADIAltaIntensita", "Prestazioni" };
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

                doc = documents.Find(x => x.AttributeName == "Prestazioni");
                if (doc == null)
                {
                    throw new Exception("Prestazioni non trovato!");
                }
                var listPreastazioni = doc.Records;

                int year = 0;
                var objYear = GetParamValue("Year");
                if (objYear != null) year = (int)objYear;
                int month = 0;
                var objMonth = GetParamValue("Month");
                if (objMonth != null) month = (int)objMonth;

                nameFileWithoutExt = $"{nameReport}-{year:0000}{month:00}.{now:dd-MM-yyyy.HH.mm.ss}";

                List<Valorizzazione> reportValorizzazione = new List<Valorizzazione>();
                List<string> patients = new List<string>();

                foreach (dynamic adi in listADIAltaIntensita)
                {
                    // Creo o utilizzo una riga esistente di valorizzazione
                    var val = reportValorizzazione.Where(r => r.PatientName == adi.Patient && r.FiscalCode== adi.FiscalCode && r.ASL == adi.ASL && r.District == adi.District && r.ActivityDate == adi.Date).FirstOrDefault();
                    if (val == null)
                    {
                        val = new Valorizzazione { PatientName = adi.Patient, FiscalCode = adi.FiscalCode, ASL = adi.ASL, District = adi.District, ActivityDate = adi.Date };
                        reportValorizzazione.Add(val);
                        if (!patients.Contains(val.PatientName))
                            patients.Add(val.PatientName);
                    }

                    // Operatore Sanitario, per individuare il WorkType
                    dynamic oss = listLazioHealthWorker.Where((dynamic os) => os.NameKey == adi.NameKey).FirstOrDefault();
                    if (oss == null) continue;

                    var worktype = oss.WorkType;

                    #region Nuova gestione Prestazioni
                    var prestazioni = listPreastazioni.Where((dynamic hs) => ((string)hs.HealthService).ToUpper() == ((string)adi.Activity).ToUpper()).ToList();
                    switch (prestazioni.Count)
                    {
                        case 0:
                            break;
                        case 1:
                            if (string.IsNullOrEmpty(prestazioni.FirstOrDefault().WorkType)) continue;
                            worktype = prestazioni.FirstOrDefault().WorkType;
                            break;
                        default:
                            var prestazione = prestazioni.Where((dynamic hs) => ((string)hs.HealthWorkerType).ToUpper() == ((string)oss.WorkType).ToUpper()).FirstOrDefault();
                            if (prestazione == null)
                            {
                                prestazione = prestazioni.Where((dynamic hs) =>  string.IsNullOrEmpty(hs.HealthWorkerType)).FirstOrDefault();
                                if (prestazione == null) throw new Exception($"[{adi.Activity}] non ha un corrispettivo nel file delle Prestazioni!");
                            }
                            worktype = prestazione.WorkType;
                            break;
                    }
                    #endregion

                    // Prelievi e Trasporti sono identificati con un WorkType specifico
                    switch (((string)adi.Activity).ToUpper())
                    {
                        //case "PRELIEVO PER EMOTRASFUSIONI (ADI ALTA INTENSITA')":
                        //case "TELEMONITORAGGIO/TELEVISITA/TELECONSULTO (SIAT ASL FROSINONE)":
                        //    continue; // 2022/07/14 - Per queste attività non si conteggia nulla 
                        //case "PRELIEVO EMATICO (ADI ALTA INTENSITA')":
                        //case "PRELIEVO EMATICO (ADI ALTA INTENSITA’)":
                        //case "PRELIEVO VENOSO (ADI ALTA INTENSITA')":
                        //case "PRELIEVO ALTRO MATERIALE BIOLOGICO (ADI ALTA INTENSITA')":
                        //case "PRELIEVO EMATICO (ADI ALTA INTENSITA' - FROSINONE)":
                        //case "PRELIEVO VENOSO (SIAT ASL FROSINONE)":
                        //case "PRELIEVO EMATICO (ADI PRESTAZIONALE)":
                        //case "RACCOLTA DI UN CAMPIONE DI URINE (ADI ALTA INTENSITA')":
                        //case "RACCOLTA DI UN CAMPIONE DI URINE (ADI ALTA INTENSITA’)":
                        //    worktype = "PRELIEVO";
                        //    break;
                        //case "SERVIZIO TRASPORTO SANITARIO (ADI ALTA INTENSITA')":
                        //case "SERVIZIO TRASPORTO SANITARIO (ADI ALTA INTENSITA’)":
                        //case "SERVIZIO TRASPORTO (ADI ALTA INTENSITA')":
                        //case "SERVIZIO TRASPORTO (ADI ALTA INTENSITA’)":
                        //    switch (((string)oss.WorkType).ToUpper())
                        //    {
                        //        case "ANESTESISTA":
                        //        case "MEDICO CHIRURGO":
                        //            worktype = "TRASPORTO CON MEDICO";
                        //            break;
                        //        default:
                        //            worktype = "TRASPORTO CON INFERMIERE";
                        //            break;
                        //    }
                        //    break;
                        //    // Aggiunte il 14/06/2022 per difformità con l'originale
                        //case "SERVIZIO TRASPORTO AMBULANZE INFERM. (ADI ALTA INTENSITA’)":
                        //    worktype = "TRASPORTO CON INFERMIERE";
                        //    break;
                        //case "SERVIZIO TRASPORTO AMBULANZE MEDICO (ADI ALTA INTENSITA’)":
                        //    worktype = "TRASPORTO CON MEDICO";
                        //    break;
                        //    // Tutte aggiunte il 14/07/2022 per uniformità alle ASL
                        //case "RADIOGRAFIA DOMICILIARE CON REFERTAZIONE (ADI ALTA INTENSITA’)":
                        //case "RADIOGRAFIA DOMICILIARE CON REFERTAZIONE (ADI ALTA INTENSITA')":
                        //case "ESAMI DIAGNOSTICI RX (ADI ALTA INTENSITA’)":
                        //case "ESAMI DIAGNOSTICI RX (ADI ALTA INTENSITA')":
                        //    worktype = "RX DOMICILIARE";
                        //    break;
                        //case "ECOGRAFIA DOMICILIARE (ADI ALTA INTENSITA’)":
                        //case "ECOGRAFIA DOMICILIARE (ADI ALTA INTENSITA')":
                        //    worktype = "ECOGRAFIA DOMICILIARE";
                        //    break;
                        //case "EMOGASANALISI-PRELIEVO.ANALISI.REFERTO (ADI ALTA INTENSITA’)":
                        //case "EMOGASANALISI-PRELIEVO.ANALISI.REFERTO (ADI ALTA INTENSITA')":
                        //case "EMOGASANALISI-PRELIEVO.ANALISI.REFERTO (SIAT ASL FROSINONE)":
                        //    worktype = "EMOGAS DOMICILIARE";
                        //    break;
                        //case "EMOGASANALISI-PRELIEVO.TRASPORTO.LAB":
                        //    worktype = "EMOGAS DOMICILIARE SOLO PRELIEVO E TRASP";
                        //    break;
                        //case "EMOTRASFUSIONE (SEMP.) (ADI ALTA INTENSITA’)":
                        //case "EMOTRASFUSIONE (SEMP.) (ADI ALTA INTENSITA')":
                        //case "EMOTRASFUSIONE (SEMP.) (ADI PRESTAZIONALE)":
                        //case "EMOTRASFUSIONE (SEMP.) (SIAT ASL FROSINONE)":
                        //case "EMOTRASFUSIONE DOMICILIARE (ADI ALTA INTENSITA’)":
                        //case "EMOTRASFUSIONE DOMICILIARE (ADI ALTA INTENSITA')":
                        //case "EMOTRASFUSIONE DOMICILIARE (ADI PRESTAZIONALE)":
                        //case "EMOTRASFUSIONE-PREL.PROVE.RITIRO.SACCA (SIAT ASL ROMA 2)":
                        //    worktype = "EMOTRASFUSIONE DOMICILIARE";
                        //    break;
                        //case "SOSTITUZIONE DEL CATETERE VESCICALE (ADI ALTA INTENSITA’)":
                        //case "SOSTITUZIONE DEL CATETERE VESCICALE (ADI ALTA INTENSITA')":
                        //    worktype = "CAMBIO CATETERE";
                        //    break;
                        //case "TAMPONE RAPIDO (ADI ALTA INTENSITA’)":
                        //case "TAMPONE RAPIDO (ADI ALTA INTENSITA')":
                        //case "TEST ANTIGIENICO A DOMICILIO (SIAT ASL FROSINONE)":
                        //case "TEST ANTIGIENICO A DOMICILIO (SIAT ASL ROMA 2)":
                        //case "TAMPONE RAPIDO A DOMICILIO - SINGOLO (ADI ALTA INTENSITA’)":
                        //case "TAMPONE RAPIDO A DOMICILIO - SINGOLO (ADI ALTA INTENSITA')":
                        //case "TAMPONE ANTIGIENICO DOMICILIARE (ADI ALTA INTENSITA')":
                        //case "TAMPONE ANTIGENICO DOMICILIARE (ADI ALTA INTENSITA')":
                        //    worktype = "TAMPONI ANTIGENICI (COVID)";
                        //    break;
                        //case "VACCINAZIONE ANTI SARS-COV-2/COVID-19 (SIAT ASL FROSINONE)":
                        //case "VACCINAZIONE ANTI SARS-COV-2/COVID-19 (SIAT ASL ROMA 2)":
                        //case "VACCINAZIONE ANTI SARS-COV-2/COVID-19 (ADI ALTA INTENSITA’)":
                        //case "VACCINAZIONE ANTI SARS-COV-2/COVID-19 (ADI ALTA INTENSITA')":
                        //    worktype = "PRESTAZIONI VACCINI (COVID)";
                        //    break;
                        case "MONITORAGG. DOMICILIARE PAZIENTI COVID19 (ADI ALTA INTENSITA’)":
                        case "MONITORAGG. DOMICILIARE PAZIENTI COVID19 (ADI ALTA INTENSITA')":
                        case "MONITORAGG. DOMICILIARE PAZIENTI COVID19 (SIAT ASL ROMA 2)":
                            //case "MONITORAGG. DOMICILIARE PAZIENTI COVID19 (SIAT ASL FROSINONE)": // #1446 Non considerare per Frosinone
                            //worktype = "GESTIONE KIT PER TELEMONITORAGGIO PAZ. COVID"; // #1446 Mantenere le normali valorizzazioni
                            val.MonitoringKitNumber = 1; // #1446 Attribuire il costo aggiuntivo ma solo in presenza di pacchetto base da 120€. (vedi attribuzione pacchetto)
                            break;
                    }

                    decimal duration = NumerizeDuration(adi.Duration);

                    // In base alla durata e al WorkType definisco i pacchetti
                    switch (worktype.ToUpper())
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
                                case "ACCESSO OSS (ADI ALTA INTENSITA’ - FROSINONE)":
                                case "ACCESSO OSS (ADI ALTA INTENSITA’)":
                                case "ACCESSO OSS (ADI ALTA INTENSITA')":
                                case "ASSIST. (OSS) PAZIENTE ALTA COMPLES (ADI ALTA INTENSITA’)":
                                case "ASSIST. (OSS) PAZIENTE ALTA COMPLES (ADI ALTA INTENSITA')":
                                case "ASSIST. (OSS) PAZIENTE ALTA COMPLES (SIAT ASL FROSINONE)":
                                case "ASSIST. (OSS) PAZIENTE ALTA COMPLES (SIAT ASL ROMA 6)":
                                case "ASSIST. (OSS) PAZIENTE ALTA COMPLES-H (SIAT ASL ROMA 2)":
                                case "ATTIVITÀ DI SUPPORTO OSS (SIAT ASL FROSINONE)":
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
                            val.AccessChiNumber += 1;
                            break;
                        case "OPERATORE SOCIO-SANITARIO":
                        case "ASSISTENTE SANITARIO":
                            val.HourOssNumber += duration;
                            break;
                        case "PSICOLOGO":
                            val.HourPsiNumberTotal += duration;
                            break;
                        case "TERAPISTA DELLA NEURO E PSICOMOTRICITÀ DELL'ETÀ EVOLUTIVA":
                            val.HourTpnNumberTotal += duration;
                            break;
                        case "TERAPISTA OCCUPAZIONALE":
                            val.HourTerNumberTotal += duration;
                            break;
                        case "PRELIEVO":
                            val.SampleNumber = val.SampleNumber2 += 1;
                            break;
                        case "TRASPORTO CON INFERMIERE":
                        // Modifica 17/04/2024 su Tk #1506
                            val.TransInfNumberTotal += 1;
                            //val.TransInfNumber += 1;
                            break;
                        case "TRASPORTO CON MEDICO":
                            val.TransDocNumber += 1;
                            break;
                        // Tutte aggiunte il 14/07/2022 per uniformità alle ASL
                        case "RX DOMICILIARE":
                            val.RXDomNumber += 1;
                            break;
                        case "ECOGRAFIA DOMICILIARE":
                            val.EcoDomNumber += 1;
                            break;
                        case "EMOGAS DOMICILIARE":
                            val.EmoGasDomNumber += 1;
                            break;
                        case "EMOGAS DOMICILIARE SOLO PRELIEVO E TRASP":
                            val.EmoGasLabDomNumber += 1;
                            break;
                        case "EMOTRASFUSIONE DOMICILIARE":
                            val.EmoTrasfDomNumber += 1;
                            break;
                        case "CAMBIO CATETERE":
                            val.CatheterChangeNumber += 1;
                            break;
                        case "TAMPONI ANTIGENICI (COVID)":
                            val.SwabNumber += 1;
                            break;
                        case "PRESTAZIONI VACCINI (COVID)":
                            val.VaxNumber += 1;
                            break;
                            //case "GESTIONE KIT PER TELEMONITORAGGIO PAZ. COVID": // #1446 Da non gestire più singolarmente
                            //    val.MonitoringKitNumber += 1;
                            //    break;
                    }
                }

                //Arrotondamenti sulle attività a durata
                foreach(var val in reportValorizzazione)
                {
                    val.HourFktNumberTotal = RoundValue(val.HourFktNumberTotal);
                    val.HourOssNumber = RoundValue(val.HourOssNumber);
                    val.HourInfNumber = RoundValue(val.HourInfNumber);
                    val.HourLogNumberTotal = RoundValue(val.HourLogNumberTotal);
                    val.HourPsiNumberTotal = RoundValue(val.HourPsiNumberTotal);
                    val.HourTpnNumberTotal = RoundValue(val.HourTpnNumberTotal);
                    val.HourTerNumberTotal = RoundValue(val.HourTerNumberTotal);
                }

                // Gestione pacchetti
                foreach (var val in reportValorizzazione)
                {
                    val.HourInfNumberTotal = val.HourInfNumber;
                    val.HourOssNumbertotal = val.HourOssNumber;

                    decimal _pacchettiRiabilitativi = val.HourFktNumberTotal + val.HourLogNumberTotal + val.HourTpnNumberTotal + val.HourTerNumberTotal; // Prima era così

                    val.BasePacketValue = 120;
                    val.ReliefPacketValue = 108;

                    val.HourInfValue = 27;
                    val.HourRehabValue = 27;

                    val.HourInfNumber = val.HourInfNumberTotal;

                    if (val.ASL == "ASL FROSINONE")
                    {
                        val.HourRehabValue = 30;
                        val.HourRehabNumber = _pacchettiRiabilitativi;
                        if (val.HourInfNumber >= 4) // c'è il pacchetto base
                        {
                            val.BasePacketNumber = 1;
                            for (int i = 1; i <= 4; i++)
                            {
                                if (val.HourInfNumber > 0)
                                    val.HourInfNumber--;
                            }

                            while (val.HourInfNumber >= 4)
                            {
                                val.ReliefPacketNumber += 1;
                                val.HourInfNumber -= 4;
                            }
                        }
                        else
                        {
                            val.HourInfValue = 30;
                            val.MonitoringKitNumber = 0; // #1446 Non considerare se non c'è pacchetto base
                        }
                    }
                    else
                    {
                        if ((_pacchettiRiabilitativi + val.HourInfNumberTotal) < 4) // non ci sono pacchetti
                        {
                            val.HourInfValue = 30;
                            val.HourRehabValue = 30;
                            val.HourRehabNumber = _pacchettiRiabilitativi;
                            val.MonitoringKitNumber = 0; // #1446 Non considerare se non c'è pacchetto base
                        }
                        else
                        {
                            val.BasePacketNumber = 1;

                            switch (val.ASL)
                            {
                                case "ASL ROMA 2":
                                case "ASL ROMA 3":
                                case "ASL LATINA":
                                    // rehab priority
                                    for (int i = 1; i <= 4; i++)
                                    {
                                        if (_pacchettiRiabilitativi == 0)
                                            val.HourInfNumber--;
                                        else
                                            _pacchettiRiabilitativi--;
                                    }
                                    break;
                                case "ASL ROMA 5":
                                case "ASL ROMA 6":
                                    // inf priority
                                    val.HourRehabValue = 30; // Unica condizione particolare
                                    for (int i = 1; i <= 4; i++)
                                    {
                                        if (val.HourInfNumber == 0)
                                            _pacchettiRiabilitativi--;
                                        else
                                            val.HourInfNumber--;
                                    }
                                    break;
                            }

                            var rest = (_pacchettiRiabilitativi + val.HourInfNumber);
                            while (rest >= 4)
                            {
                                val.ReliefPacketNumber += 1;
                                rest -= 4;
                            }

                            var infrest = val.HourInfNumber % 4;

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
                    }
                    //Per Frosinone considerare solo ore infermieristiche per il pacchetto base

                    val.TotalValue += (val.BasePacketNumber * 120) + (val.ReliefPacketNumber * 108) + (val.HourInfNumber * val.HourInfValue) + (val.HourRehabNumber * val.HourRehabValue);

                    //Valorizzazione pacchetto di ore OSS + PRELIEVI
                    val.SampleValue = 14;
                    val.TotalValue += (val.HourOssNumbertotal * 21.6M) + val.SampleNumber * val.SampleValue;
                    val.HourOssValue = 21.6M;
                    val.ReliefPacketNumber += (int)Math.Truncate(val.HourOssNumbertotal / 5);
                    val.HourOssNumber = val.HourOssNumbertotal - ((int)Math.Truncate(val.HourOssNumbertotal / 5) * 5);

                    //Valorizzazione attività Psicologo
                    val.HourPsyValue = 60;
                    val.HourPsyNumber = val.HourPsiNumberTotal = Math.Round(val.HourPsiNumberTotal);
                    val.TotalValue += val.HourPsiNumberTotal * val.HourPsyValue;

                    //Valorizzazione trasporti in ambulanza assistiti
                    #region Modifica su Tk #1506
                    //val.TransInfValue = 62;
                    val.TransInfValue = 0;
                    val.TransInfTotal = val.TransInfNumber * val.TransInfValue;
                    val.TotalValue += val.TransInfTotal;
                    if (val.TransInfTotal < 1) val.TransInfValue = 0; //#1445
                    #endregion
                    val.TransDocValue = 106;
                    val.TransDocTotal = val.TransDocNumber * val.TransDocValue;
                    val.TotalValue += val.TransDocTotal;
                    if (val.TransDocTotal < 1) val.TransDocValue = 0; //#1445

                    // Tutte aggiunte il 14/07/2022 per uniformità alle ASL

                    //Valorizzazione RX DOMICILIARE
                    val.RXDomValue = 120;
                    val.RXDomTotal = val.RXDomNumber * val.RXDomValue;
                    val.TotalValue += val.RXDomTotal;
                    if (val.RXDomTotal < 1) val.RXDomValue = 0; //#1445

                    //Valorizzazione ECOGRAFIA DOMICILIARE
                    val.EcoDomValue = 140;
                    val.EcoDomTotal = val.EcoDomNumber * val.EcoDomValue;
                    val.TotalValue += val.EcoDomTotal;
                    if (val.EcoDomTotal < 1) val.EcoDomValue = 0; //#1445

                    //Valorizzazione EMOGAS DOMICILIARE
                    val.EmoGasDomValue = 90;
                    val.EmoGasDomTotal = val.EmoGasDomNumber * val.EmoGasDomValue;
                    val.TotalValue += val.EmoGasDomTotal;
                    if (val.EmoGasDomTotal < 1) val.EmoGasDomValue = 0; //#1445

                    //Valorizzazione EMOGAS DOMICILIARE SOLO PRELIEVO E TRASP
                    val.EmoGasLabDomValue = 30;
                    val.EmoGasLabDomTotal = val.EmoGasLabDomNumber * val.EmoGasLabDomValue;
                    val.TotalValue += val.EmoGasLabDomTotal;
                    if (val.EmoGasLabDomTotal < 1) val.EmoGasLabDomValue = 0; //#1445

                    //Valorizzazione EMOSTRASFUSIONE DOMICILIARE
                    val.EmoTrasfDomValue = (val.ASL == "ASL FROSINONE") ? 200 : 250;
                    val.EmoTrasfDomTotal = val.EmoTrasfDomNumber * val.EmoTrasfDomValue;
                    val.TotalValue += val.EmoTrasfDomTotal;
                    if (val.EmoTrasfDomTotal < 1) val.EmoTrasfDomValue = 0; //#1445

                    //Valorizzazione CAMBIO CATETERE
                    val.CatheterChangeValue = 22;
                    val.CatheterChangeTotal = val.CatheterChangeNumber * val.CatheterChangeValue;
                    val.TotalValue += val.CatheterChangeTotal;
                    if (val.CatheterChangeTotal < 1) val.CatheterChangeValue = 0; //#1445

                    //Valorizzazione TAMPONI ANTIGENICI (COVID)
                    val.SwabValue = 33;
                    val.SwabTotal = val.SwabNumber * val.SwabValue;
                    val.TotalValue += val.SwabTotal;
                    if (val.SwabTotal < 1) val.SwabValue = 0; //#1445

                    //Valorizzazione PRESTAZIONI VACCINI (COVID)
                    val.VaxValue = 44;
                    val.VaxTotal = val.VaxNumber * val.VaxValue;
                    val.TotalValue += val.VaxTotal;
                    if (val.VaxTotal < 1) val.VaxValue = 0; //#1445

                    //Valorizzazione GESTIONE KIT PER TELEMONITORAGGIO PAZ. COVID
                    val.MonitoringKitValue = 40;
                    val.MonitoringKitTotal = val.MonitoringKitNumber * val.MonitoringKitValue;
                    val.TotalValue += val.MonitoringKitTotal;
                    if (val.MonitoringKitTotal < 1) val.MonitoringKitValue = 0; //#1445
                }

                //Valorizzazione della quota accesso medici
                foreach (var patient in patients)
                {
                    int counter = 0;
                    var items = (from r in reportValorizzazione where r.PatientName == patient && r.AccessAneNumber+r.AccessChiNumber > 0 select r).ToList();
                    bool havepack = (from r in reportValorizzazione where r.PatientName == patient select r).Any(r => r.BasePacketNumber > 0);

                    foreach(var item in items)
                    {
                        var specnumber = item.AccessAneNumber + item.AccessChiNumber;
                        for (int i = 1; i <= specnumber; i++)
                        {
                            item.SpecialistAccessNumber += (counter < 2 && havepack) ? 0 : 1;
                            counter++;
                        }
                        if (item.SpecialistAccessNumber > 0)
                        {
                            item.SpecialistAccessValue = 120;
                            item.SpecialistAccessTotal = item.SpecialistAccessNumber * item.SpecialistAccessValue;
                        }
                    }
                }

                foreach (var val in reportValorizzazione)
                {
                    decimal _valoreBaseDaScontare = (val.BasePacketNumber * val.BasePacketValue) +
                        (val.ReliefPacketNumber * val.ReliefPacketValue) +
                        (val.HourInfNumber * val.HourInfValue) +
                        (val.HourRehabNumber * val.HourRehabValue) +
                        (val.HourOssNumber * val.HourOssValue);

                    decimal _valoreAddizionaleExtra = (val.HourPsiNumberTotal * val.HourPsyValue) + (val.SampleNumber * val.SampleValue) +
                        val.TransInfTotal + val.TransDocTotal + val.SpecialistAccessTotal +
                        val.RXDomTotal + val.EcoDomTotal + val.EmoGasDomTotal +
                        val.EmoGasLabDomTotal + val.EmoTrasfDomTotal + val.CatheterChangeTotal +
                        val.SwabTotal + val.VaxTotal + val.MonitoringKitTotal;

                    val.Discount = (_valoreBaseDaScontare > 300) ? (_valoreBaseDaScontare - 300) * 0.1M : 0;
                    val.NoDiscountValue = _valoreBaseDaScontare;
                    // Modificato perché il valore non scontato deve essere calcolato senza i costi extra
                    var grosstotal = _valoreBaseDaScontare + _valoreAddizionaleExtra;
                    val.TotalValue = grosstotal - val.Discount;

                    val.BasePacketTotal = val.BasePacketNumber * val.BasePacketValue;
                    if (val.BasePacketTotal == 0) val.BasePacketValue = 0;
                    val.ReliefPacketTotal = val.ReliefPacketNumber * val.ReliefPacketValue;
                    if (val.ReliefPacketTotal == 0) val.ReliefPacketValue = 0;

                    val.HourInfTotal = val.HourInfNumber * val.HourInfValue;
                    if (val.HourInfTotal == 0) val.HourInfValue = 0;
                    val.HourRehabTotal = val.HourRehabNumber * val.HourRehabValue;
                    if (val.HourRehabTotal == 0) val.HourRehabValue = 0;
                    val.HourOssTotal = val.HourOssNumber * val.HourOssValue;
                    if (val.HourOssTotal == 0) val.HourOssValue = 0;
                    val.HourPsyTotal = val.HourPsyNumber * val.HourPsyValue;
                    if (val.HourPsyTotal == 0) val.HourPsyValue = 0;
                    val.SampleTotal = val.SampleNumber * val.SampleValue;
                    if (val.SampleTotal == 0) val.SampleValue = 0;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private decimal RoundValue(decimal value)
        {
            return (value > 0) ? ((value >= 1) ? value : 1) : 0;
        }

        #region Member

        #region ANAG (A-H)
        [ReportMemberReference(Column = "A", Position = 1, ColumnName = "NOME PAZIENTE", Length = 50, Required = true, FieldName = "PatientName")]
        public string PatientName { get; set; }

        [ReportMemberReference(Column = "B", Position = 2, ColumnName = "CODICE FISCALE", Length = 50, Required = true, FieldName = "FiscalCode")]
        public string FiscalCode { get; set; }

        [ReportMemberReference(Column = "C", Position = 3, ColumnName = "AZIENDA SANITARIA", Length = 50, Required = true, FieldName = "ASL")]
        public string ASL { get; set; }

        [ReportMemberReference(Column = "D", Position = 4, ColumnName = "DISTRETTO", Length = 50, FieldName = "District")]
        public string District { get; set; }

        [ReportMemberReference(Column = "E", Position = 5, ColumnName = "DATA ATTIVITA'", FieldName = "ActivityDate", Format ="dd/MM/yyyy")]
        public DateTime ActivityDate { get; set; }

        [ReportMemberReference(Column = "F", Position = 6, ColumnName = "VALORE TOTALE CALCOLATO", FieldName = "TotalValue")]
        public decimal TotalValue { get; set; }

        [ReportMemberReference(Column = "G", Position = 7, ColumnName = "VALORE NON SCONTATO", FieldName = "NoDiscountValue")]
        public decimal NoDiscountValue { get; set; }

        [ReportMemberReference(Column = "H", Position = 8, ColumnName = "SCONTO", FieldName = "Discount")]
        public decimal Discount { get; set; }
        #endregion

        #region PACCHETTI BASE (I-K)
        [ReportMemberReference(Column = "I", Position = 9, ColumnName = "NUMERO PACCHETTI BASE", FieldName = "BasePacketNumber")]
        public decimal BasePacketNumber { get; set; }

        [ReportMemberReference(Column = "J", Position = 10, ColumnName = "VALORE PACCHETTO BASE", FieldName = "BasePacketValue")]
        public decimal BasePacketValue { get; set; }

        [ReportMemberReference(Column = "K", Position = 11, ColumnName = "TOTALE PACCHETTO BASE", FieldName = "BasePacketTotal")]
        public decimal BasePacketTotal { get; set; }
        #endregion

        #region PACCHETTI SOLLIEVO (L-N)
        [ReportMemberReference(Column = "L", Position = 12, ColumnName = "NUMERO PACCHETTI SOLLIEVO", FieldName = "ReliefPacketNumber")]
        public decimal ReliefPacketNumber { get; set; }

        [ReportMemberReference(Column = "M", Position = 13, ColumnName = "VALORE PACCHETTO SOLLIEVO", FieldName = "ReliefPacketValue")]
        public decimal ReliefPacketValue { get; set; }

        [ReportMemberReference(Column = "N", Position = 14, ColumnName = "TOTALE PACCHETTO SOLLIEVO", FieldName = "ReliefPacketTotal")]
        public decimal ReliefPacketTotal { get; set; }
        #endregion

        #region ORE INFERMIERISTICHE (O-Q)
        [ReportMemberReference(Column = "O", Position = 15, ColumnName = "NUMERO ORE INF.", FieldName = "HourInfNumber")]
        public decimal HourInfNumber { get; set; }

        [ReportMemberReference(Column = "P", Position = 16, ColumnName = "VALORE ORE INF.", FieldName = "HourInfValue")]
        public decimal HourInfValue { get; set; }

        [ReportMemberReference(Column = "Q", Position = 17, ColumnName = "TOTALE ORE INF.", FieldName = "HourInfTotal")]
        public decimal HourInfTotal { get; set; }
        #endregion

        #region ORE RIABILITAZIONE (R-T)
        [ReportMemberReference(Column = "R", Position = 18, ColumnName = "NUMERO ORE RIAB. (FKT/LOGO/TPNEE/TO)", FieldName = "HourRehabNumber")]
        public decimal HourRehabNumber { get; set; }
        
        [ReportMemberReference(Column = "S", Position = 19, ColumnName = "VALORE ORE RIAB.", FieldName = "HourRehabValue")]
        public decimal HourRehabValue { get; set; }

        [ReportMemberReference(Column = "T", Position = 20, ColumnName = "TOTALE ORE RIAB.", FieldName = "HourRehabTotal")]
        public decimal HourRehabTotal { get; set; }
        #endregion

        #region ORE OSS (U-W)
        [ReportMemberReference(Column = "U", Position = 21, ColumnName = "NUMERO ORE OSS", FieldName = "HourOssNumber")]
        public decimal HourOssNumber { get; set; }
        
        [ReportMemberReference(Column = "V", Position = 22, ColumnName = "VALORE ORE OSS", FieldName = "HourOssValue")]
        public decimal HourOssValue { get; set; }

        [ReportMemberReference(Column = "W", Position = 23, ColumnName = "TOTALE VALORE ORE OSS", FieldName = "HourOssTotal")]
        public decimal HourOssTotal { get; set; }
        #endregion

        #region ORE PSICOLOGO (X-Z)
        [ReportMemberReference(Column = "X", Position = 24, ColumnName = "NUMERO ORE PSICOLOGO", FieldName = "HourPsyNumber")]
        public decimal HourPsyNumber { get; set; }
        
        [ReportMemberReference(Column = "Y", Position = 25, ColumnName = "VALORE ORE PSICOLOGO", FieldName = "HourPsyValue")]
        public decimal HourPsyValue { get; set; }

        [ReportMemberReference(Column = "Z", Position = 26, ColumnName = "TOTALE ORE PSICOLOGO", FieldName = "HourPsyTotal")]
        public decimal HourPsyTotal { get; set; }
        #endregion

        #region PRELIEVI (AA-AC)
        [ReportMemberReference(Column = "AA", Position = 27, ColumnName = "NUMERO PRELIEVI", FieldName = "SampleNumber")]
        public decimal SampleNumber { get; set; }
        
        [ReportMemberReference(Column = "AB", Position = 28, ColumnName = "VALORE PRELIEVO", FieldName = "SampleValue")]
        public decimal SampleValue { get; set; }

        [ReportMemberReference(Column = "AC", Position = 29, ColumnName = "TOTALE PRELIEVI", FieldName = "SampleTotal")]
        public decimal SampleTotal { get; set; }
        #endregion

        #region TRASPORTI CON INF (AD-AF)
        [ReportMemberReference(Column = "AD", Position = 30, ColumnName = "NUMERO TRASPORTI CON INF", FieldName = "TransInfNumber")]
        public decimal TransInfNumber { get; set; }
        
        [ReportMemberReference(Column = "AE", Position = 31, ColumnName = "VALORE TRASPORTO CON INF", FieldName = "TransInfValue")]
        public decimal TransInfValue { get; set; }

        [ReportMemberReference(Column = "AF", Position = 32, ColumnName = "TOTALE TRASPORTO CON INF", FieldName = "TransInfTotal")]
        public decimal TransInfTotal { get; set; }
        #endregion

        #region TRASPORTI CON MED (AG-AI)
        [ReportMemberReference(Column = "AG", Position = 33, ColumnName = "NUMERO TRASPORTI CON MED", FieldName = "TransDocNumber")]
        public decimal TransDocNumber { get; set; }
        
        [ReportMemberReference(Column = "AH", Position = 34, ColumnName = "VALORE TRASPORTO CON MED", FieldName = "TransDocValue")]
        public decimal TransDocValue { get; set; }

        [ReportMemberReference(Column = "AI", Position = 35, ColumnName = "TOTALE TRASPORTO CON MED", FieldName = "TransDocTotal")]
        public decimal TransDocTotal { get; set; }
        #endregion

        #region ACCESSI SPECIALISTICI (AJ-AL)
        [ReportMemberReference(Column = "AJ", Position = 36, ColumnName = "NUMERO ACCESSI SPECIALISTICI", FieldName = "SpecialistAccessNumber")]
        public decimal SpecialistAccessNumber { get; set; }
        
        [ReportMemberReference(Column = "AK", Position = 37, ColumnName = "VALORE ACCESSI SPECIALISTICI", FieldName = "SpecialistAccessValue")]
        public decimal SpecialistAccessValue { get; set; }

        [ReportMemberReference(Column = "AL", Position = 38, ColumnName = "TOTALE ACCESSI SPECIALISTICI", FieldName = "SpecialistAccessTotal")]
        public decimal SpecialistAccessTotal { get; set; }
        #endregion

        #region RX DOMICILIARE (AM-AO)
        [ReportMemberReference(Column = "AM", Position = 39, ColumnName = "NUMERO RX DOMICILIARE", FieldName = "RXDomNumber")]
        public decimal RXDomNumber { get; set; }

        [ReportMemberReference(Column = "AN", Position = 40, ColumnName = "VALORE RX DOMICILIARE", FieldName = "RXDomValue")]
        public decimal RXDomValue { get; set; }

        [ReportMemberReference(Column = "AO", Position = 41, ColumnName = "TOTALE RX DOMICILIARE", FieldName = "RXDomTotal")]
        public decimal RXDomTotal { get; set; }
        #endregion

        #region ECOGRAFIA DOMICILIARE (AP-AR)
        [ReportMemberReference(Column = "AP", Position = 42, ColumnName = "NUMERO ECOGRAFIA DOMICILIARE", FieldName = "EcoDomNumber")]
        public decimal EcoDomNumber { get; set; }

        [ReportMemberReference(Column = "AQ", Position = 43, ColumnName = "VALORE ECOGRAFIA DOMICILIARE", FieldName = "EcoDomValue")]
        public decimal EcoDomValue { get; set; }

        [ReportMemberReference(Column = "AR", Position = 44, ColumnName = "TOTALE ECOGRAFIA DOMICILIARE", FieldName = "EcoDomTotal")]
        public decimal EcoDomTotal { get; set; }
        #endregion

        #region EMOGAS DOMICILIARE (AS-AU)
        [ReportMemberReference(Column = "AS", Position = 45, ColumnName = "NUMERO EMOGAS DOMICILIARE", FieldName = "EmoGasDomNumber")]
        public decimal EmoGasDomNumber { get; set; }

        [ReportMemberReference(Column = "AT", Position = 46, ColumnName = "VALORE EMOGAS DOMICILIARE", FieldName = "EmoGasDomValue")]
        public decimal EmoGasDomValue { get; set; }

        [ReportMemberReference(Column = "AU", Position = 47, ColumnName = "TOTALE EMOGAS DOMICILIARE", FieldName = "EmoGasDomTotal")]
        public decimal EmoGasDomTotal { get; set; }
        #endregion

        #region EMOGAS DOMICILIARE SOLO PRELIEVO E TRASP. (AV-AX)
        [ReportMemberReference(Column = "AV", Position = 48, ColumnName = "NUMERO EMOGAS DOMICILIARE SOLO PRELIEVO E TRASP.", FieldName = "EmoGasLabDomNumber")]
        public decimal EmoGasLabDomNumber { get; set; }

        [ReportMemberReference(Column = "AW", Position = 49, ColumnName = "VALORE EMOGAS DOMICILIARE SOLO PRELIEVO E TRASP.", FieldName = "EmoGasLabDomValue")]
        public decimal EmoGasLabDomValue { get; set; }

        [ReportMemberReference(Column = "AX", Position = 50, ColumnName = "TOTALE EMOGAS DOMICILIARE SOLO PRELIEVO E TRASP.", FieldName = "EmoGasLabDomTotal")]
        public decimal EmoGasLabDomTotal { get; set; }
        #endregion

        #region EMOTRASFUSIONE DOMICILIARE (AY-BA)
        [ReportMemberReference(Column = "AY", Position = 51, ColumnName = "NUMERO EMOTRASFUSIONE DOMICILIARE", FieldName = "EmoTrasfDomNumber")]
        public decimal EmoTrasfDomNumber { get; set; }

        [ReportMemberReference(Column = "AZ", Position = 52, ColumnName = "VALORE EMOTRASFUSIONE DOMICILIARE", FieldName = "EmoTrasfDomValue")]
        public decimal EmoTrasfDomValue { get; set; }

        [ReportMemberReference(Column = "BA", Position = 53, ColumnName = "TOTALE EMOTRASFUSIONE DOMICILIARE", FieldName = "EmoTrasfDomTotal")]
        public decimal EmoTrasfDomTotal { get; set; }
        #endregion

        #region CAMBIO CATETERE (BB-BD)
        [ReportMemberReference(Column = "BB", Position = 54, ColumnName = "NUMERO CAMBIO CATETERE", FieldName = "CatheterChangeNumber")]
        public decimal CatheterChangeNumber { get; set; }

        [ReportMemberReference(Column = "BC", Position = 55, ColumnName = "VALORE CAMBIO CATETERE", FieldName = "CatheterChangeValue")]
        public decimal CatheterChangeValue { get; set; }

        [ReportMemberReference(Column = "BD", Position = 56, ColumnName = "TOTALE CAMBIO CATETERE", FieldName = "CatheterChangeTotal")]
        public decimal CatheterChangeTotal { get; set; }
        #endregion

        #region TAMPONI ANTIGENICI (COVID) (BE-BG)
        [ReportMemberReference(Column = "BE", Position = 57, ColumnName = "NUMERO TAMPONI ANTIGENICI (COVID)", FieldName = "SwabNumber")]
        public decimal SwabNumber { get; set; }

        [ReportMemberReference(Column = "BF", Position = 58, ColumnName = "VALORE TAMPONI ANTIGENICI (COVID)", FieldName = "SwabValue")]
        public decimal SwabValue { get; set; }

        [ReportMemberReference(Column = "BG", Position = 59, ColumnName = "TOTALE TAMPONI ANTIGENICI (COVID)", FieldName = "SwabTotal")]
        public decimal SwabTotal { get; set; }
        #endregion

        #region PRESTAZIONI VACCINI (COVID) (BH-BJ)
        [ReportMemberReference(Column = "BH", Position = 60, ColumnName = "NUMERO PRESTAZIONI VACCINI (COVID)", FieldName = "VaxNumber")]
        public decimal VaxNumber { get; set; }

        [ReportMemberReference(Column = "BI", Position = 61, ColumnName = "VALORE PRESTAZIONI VACCINI (COVID)", FieldName = "VaxValue")]
        public decimal VaxValue { get; set; }

        [ReportMemberReference(Column = "BJ", Position = 62, ColumnName = "TOTALE PRESTAZIONI VACCINI (COVID)", FieldName = "VaxTotal")]
        public decimal VaxTotal { get; set; }
        #endregion

        #region GESTIONE KIT PER TELEMONITORAGGIO PAZ. COVID (BK-BM)
        [ReportMemberReference(Column = "BK", Position = 63, ColumnName = "NUMERO MAGGIOR. MONITORAGGIO PAZ. COVID", FieldName = "MonitoringKitNumber")]
        public decimal MonitoringKitNumber { get; set; }

        [ReportMemberReference(Column = "BL", Position = 64, ColumnName = "VALORE MAGGIOR. MONITORAGGIO PAZ. COVID", FieldName = "MonitoringKitValue")]
        public decimal MonitoringKitValue { get; set; }

        [ReportMemberReference(Column = "BM", Position = 65, ColumnName = "TOTALE MAGGIOR. MONITORAGGIO PAZ. COVID", FieldName = "MonitoringKitTotal")]
        public decimal MonitoringKitTotal { get; set; }
        #endregion

        #region ORE TOTALI (BN-BT)
        [ReportMemberReference(Column = "BN", Position = 66, ColumnName = "NR ORE INF TOTALI", FieldName = "HourInfNumberTotal")]
        public decimal HourInfNumberTotal { get; set; }
        
        [ReportMemberReference(Column = "BO", Position = 67, ColumnName = "Nr ORE FKT TOTALI", FieldName = "HourFktNumberTotal")]
        public decimal HourFktNumberTotal { get; set; }
        
        [ReportMemberReference(Column = "BP", Position = 68, ColumnName = "NR ORE LOGOPEDISTA TOTALI", FieldName = "HourLogNumberTotal")]
        public decimal HourLogNumberTotal { get; set; }
        
        [ReportMemberReference(Column = "BQ", Position = 69, ColumnName = "Nr ORE TPNEE TOTALI", FieldName = "HourTpnNumberTotal")]
        public decimal HourTpnNumberTotal { get; set; }
        
        [ReportMemberReference(Column = "BR", Position = 70, ColumnName = "NR ORE TO TOTALI", FieldName = "HourTerNumberTotal")]
        public decimal HourTerNumberTotal { get; set; }
        
        [ReportMemberReference(Column = "BS", Position = 71, ColumnName = "Nr ORE PSICOLOGO TOTALI", FieldName = "HourPsiNumberTotal")]
        public decimal HourPsiNumberTotal { get; set; }
        
        [ReportMemberReference(Column = "BT", Position = 72, ColumnName = "NR ORE OSS TOTALI", FieldName = "HourOssNumbertotal")]
        public decimal HourOssNumbertotal { get; set; }
        #endregion

        #region NUMERO ACCESSI, PRELIEVI, TRASPORTI (BU-BY)
        [ReportMemberReference(Column = "BU", Position = 73, ColumnName = "NR ACCESSI ANESTESISTA TOTALI", FieldName = "AccessAneNumber")]
        public decimal AccessAneNumber { get; set; }
        
        [ReportMemberReference(Column = "BV", Position = 74, ColumnName = "NR ACCESSI MEDICO CHIRURGO TOTALI", FieldName = "AccessChiNumber")]
        public decimal AccessChiNumber { get; set; }
        
        [ReportMemberReference(Column = "BW", Position = 75, ColumnName = "NR PRELIEVI", FieldName = "SampleNumber2")]
        public decimal SampleNumber2 { get; set; }

        [ReportMemberReference(Column = "BX", Position = 76, ColumnName = "NR TRASPORTI INF.", FieldName = "TransInfNumber")]
        public decimal TransInfNumberTotal { get; set; }

        [ReportMemberReference(Column = "BY", Position = 77, ColumnName = "NR TRASPORTI MED.", FieldName = "TransDocNumber")]
        public decimal TransDocNumberTotal { get { return TransDocNumber; } }
        #endregion

        #endregion
    }
}

/*
Da BN a BR trovo le ore da calcolare in pacchetto base (4 ore). Solo l'eccedenza (a multipli di 4 ore) deve confluire in Colonna L come pacchetto sollievo (riabilitazione).
Probabilmente il problema è dovuto ad una diversa imputazione della colonna ASL (C), nello switch alla riga 644 i case sono riportati come ASL ROMA n. mentre nel file caricato trovo ASL RMn.
Quindi lo switch non soddisfa alcun case di quelli proposti e quindi passa senza scontare le ore per i pacchetti base.

Colonne AD AE AF da valorizzare sempre a 0
Mantenere però il valore in BX
 */