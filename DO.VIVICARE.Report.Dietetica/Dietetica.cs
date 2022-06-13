using DO.VIVICARE.Reporter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DO.VIVICARE.Report.Dietetica
{
    /// <summary>
    /// 
    /// </summary>
    [ReportReference(Name = "Dietetica", Description = "Report inerente il consuntivo dietetica")]
    public class Dietetica : BaseReport
    {
        public Dietetica()
        {
            DocumentNames = new string[] { "ASST", "Comuni", "ZSDFatture", "Report16", "Report18", "Rendiconto", "MinSan", "Prezzi" };
        }

        public long ProgressiveNumber { get; set; }

        public override void CreateParameters()
        {
            Parameters.Clear();
            Parameters.Add(new ReportParameter
            {
                Description = "ASST",
                Name = "ASST",
                Type = "Document",
                DocumentName = "ASST",
                DocumentFieldText = "Description",
                DocumentFieldId = "IDSintesi",
                //DocumentValueText = string.Empty,
                DocumentValueId = string.Empty,
                ReturnValue = null
            });
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

        public override void LoadDocuments(bool withRecords = false)
        {
            Documents.Clear();
            foreach (var document in Manager.GetDocuments())
            {
                if (!DocumentNames.Contains(document.Attribute.Name)) continue;

                var list = Manager.Settings.GetDocumentValues(XMLSettings.LibraryType.Document, document.Attribute.Name);
                document.Document.SourceFilePath = "";
                if (list != null)
                {
                    document.Document.SourceFilePath = Path.Combine(Manager.Documents, list[0] + list[1]);
                }

                document.Document.AttributeName = document.Attribute.Name;
                if (withRecords)
                {
                    if (document.Attribute.Name == "Report16" || document.Attribute.Name == "Prezzi")
                    {
                        var objASST = GetParamValue("ASST");
                        if (objASST!=null)
                        {
                            var ASST = (BaseDocument)objASST;
                            var propField = ASST.GetType().GetProperty("IDSintesi");
                            int idSintesi = (int)propField.GetValue(ASST);
                            var filters = new List<FilterDocument>
                            {
                                new FilterDocument
                                {
                                    Column = document.Attribute.Name == "Report16" ? "N" : "E",
                                    Value = idSintesi.ToString()
                                }
                            };
                            document.Document.Filters = filters;
                        }
                    }
                    else if (document.Attribute.Name == "ZSDFatture")
                    {
                        var filters = new List<FilterDocument>();
                        var objASST = GetParamValue("ASST");
                        if (objASST != null)
                        {
                            var ASST = (BaseDocument)objASST;
                            var propField = ASST.GetType().GetProperty("SAPCode");
                            string SAPCode = (string)propField.GetValue(ASST);

                            filters.Add(new FilterDocument
                            {
                                Column = "A",
                                Value = SAPCode
                            });
                        }
                        filters.Add(new FilterDocument
                        {
                            Column = "G",
                            Value = "ANEN"
                        });
                        document.Document.Filters = filters;
                    }
                    document.Document.LoadRecords();
                }
                Documents.Add(document.Document);
            }
        }

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
            var nameFileWithoutExt = "Dietetica";
         
            try
            {
                #region Caricamento Documenti
                var documents = Documents;

                var doc = documents.Find(x => x.AttributeName == "ZSDFatture");
                if (doc == null)
                {
                    throw new Exception("ZSDFatture non trovato!");
                }
                var listZSDFatture = doc.Records;

                doc = documents.Find(x => x.AttributeName == "Report16");
                if (doc == null)
                {
                    throw new Exception("Report16 non trovato!");
                }
                var listReport16 = doc.Records;

                doc = documents.Find(x => x.AttributeName == "Report18");
                if (doc == null)
                {
                    throw new Exception("Report18 non trovato!");
                }
                var listReport18 = doc.Records;

                doc = documents.Find(x => x.AttributeName == "Comuni");
                if (doc == null)
                {
                    throw new Exception("Comuni non trovato!");
                }
                var listComuni = doc.Records;
                if (listComuni.Count() == 0)
                {
                    throw new Exception("Comuni non caricato!");
                }

                doc = documents.Find(x => x.AttributeName == "Rendiconto");
                if (doc == null)
                {
                    throw new Exception("Rendiconto non trovato!");
                }
                var listRendiconto = doc.Records;

                doc = documents.Find(x => x.AttributeName == "MinSan");
                if (doc == null)
                {
                    throw new Exception("MinSan non trovato!");
                }
                var listMinSan = doc.Records;
                if (listMinSan.Count() == 0)
                {
                    throw new Exception("MinSan non caricato!");
                }

                doc = documents.Find(x => x.AttributeName == "Prezzi");
                if (doc == null)
                {
                    throw new Exception("Prezzi non trovato!");
                }
                var listPrezzi = doc.Records;
                if (listPrezzi.Count() == 0)
                {
                    throw new Exception("Prezzi non caricato!");
                }
                #endregion

                #region Caricamento Parametri
                int idSintesi = 0;
                string SAPCode = "";
                int ATSCode = 0;
                int ASSTCode = 0;
                var objASST = GetParamValue("ASST");
                if (objASST != null)
                {
                    var ASST = (BaseDocument)objASST;
                    var propField = ASST.GetType().GetProperty("IDSintesi");
                    idSintesi = (int)propField.GetValue(ASST);
                    propField = ASST.GetType().GetProperty("SAPCode");
                    SAPCode = (string)propField.GetValue(ASST);
                    propField = ASST.GetType().GetProperty("ATSCode");
                    ATSCode = (int)propField.GetValue(ASST);
                    propField = ASST.GetType().GetProperty("ASSTCode");
                    ASSTCode = (int)propField.GetValue(ASST);
                }
                int year = 0;
                var objYear = GetParamValue("Year");
                if (objYear != null) year = (int)objYear;
                int month = 0;
                var objMonth = GetParamValue("Month");
                if (objMonth != null) month = (int)objMonth;

                nameFileWithoutExt = $"{nameReport}{ASSTCode}-{year:0000}{month:00}.{now:dd-MM-yyyy.HH.mm.ss}";

                ProgressiveNumber = 1;
                #endregion

                //fitrare FamilyCode per F0103, F0157, F0158, F0159

                var listReport16Filtered = listReport16.
                    Where((dynamic w) =>
                    w.ContractId == idSintesi &&
                    w.ErogationDate.Year == year &&
                    w.ErogationDate.Month == month &&
                    (w.FamilyCode == "F0103" || w.FamilyCode == "F0157" || w.FamilyCode == "F0158" || w.FamilyCode == "F0159")).ToList();
                List<Dietetica> reportFromReport16 = new List<Dietetica>();
                if (listReport16Filtered.Count()!=0)
                {
                    reportFromReport16 = listReport16Filtered.Select((dynamic r) => new {
                        REP16 = r,
                        REN = listRendiconto.Where((dynamic x) => x.ContractId == r.ContractId && x.FiscalCode == r.FiscalCode).FirstOrDefault(),
                        MINSAN = listMinSan.Where((dynamic m) => m.IDVivisol == r.ArticleCode).FirstOrDefault(),
                        PRZ = listPrezzi.Where((dynamic p) => p.IDVivisol == r.ArticleCode).FirstOrDefault(),
                        COM = listComuni.Where((dynamic c) => c.Name.ToUpper() == r.Town).FirstOrDefault()
                    }).
                    Select((dynamic ramp) => new Dietetica()
                    {
                        ATSCode = Manager.Left(ATSCode.ToString(), 3, ' '),
                        ASSTCode = ASSTCode.ToString("000000"),
                        Year = year.ToString("0000"),
                        Month = month.ToString("00"),
                        FiscalCode = ramp.REP16.FiscalCode,
                        Sex = Manager.SexCV(ramp.REP16.FiscalCode),
                        DateOfBirth = ramp.REP16.DateOfBirth.ToString("yyyyMMdd") /*Manager.DatCV(ramp.REP16.FiscalCode)*/,
                        ISTATCode = ramp.COM == null ? Manager.Space(6) : Manager.Left(ramp.COM.Code, 6, ' '),
                        UserHost = Manager.ErogaRSA(ramp.REP16.HostType),
                        PrescriptionNumber = Manager.Space(14),
                        DeliveryDate = ramp.REP16.ErogationDate.ToString("yyyyMMdd"),
                        TypeDescription = Manager.Left("ALIMENTINAD", 15, ' '),
                        Typology = "5",
                        MinsanCode = ramp.MINSAN == null ? Manager.Left(new string('9', 9), 30, ' ') : Manager.Left(ramp.MINSAN.ArtCode, 30, ' '),
                        MinsanDescription = Manager.Left(ramp.REP16.ArticleDescription, 30, ' '),
                        Manufacturer = ramp.REN == null ? Manager.Space(30) : Manager.Left(ramp.REN.CompanyName, 30, ' '),
                        PiecesPerPack = "001",
                        UnitOfMeasure = Manager.Left("PEZZO", 9, ' '),
                        Quantity = ramp.REP16.Quantity.ToString("0000"),
                        ManagementChannel = "4",
                        PurchaseAmount = ramp.PRZ == null ? new string('0', 12) : Manager.Amount("Report16", ramp.REP16, ramp.PRZ.Price),
                        ServiceChargeAmount = new string('0', 12),
                        RecordDestination = "N",
                        ID = Manager.NumProg(ProgressiveNumber++, year, month),
                        RepCode = Manager.Space(30),
                        CNDCode = Manager.Space(13),
                        FlagDM = "F",
                        Type = Manager.Space(1)
                    }).
                    ToList();
                }

                var errorUserHostReport16 = reportFromReport16.Where(r => r.UserHost == null);
                if (errorUserHostReport16 != null)
                {
                    foreach (var item in errorUserHostReport16)
                    {
                        list.Add(Tuple.Create("Report", "Elaborazione report dietetica", $"Errore Report16 ID {item.ID} : UserHost mancante"));
                    }
                }

                ResultRecords.AddRange(reportFromReport16);

                var listZSDFattureFiltered = listZSDFatture.Where((dynamic w) => w.Customer == SAPCode && w.ErogationDate.Year == year & w.ErogationDate.Month == month);
                List<Dietetica> reportFromZSDFatture = new List<Dietetica>();
                if (listZSDFattureFiltered.Count()!=0)
                {
                    reportFromZSDFatture = listZSDFattureFiltered.Select((dynamic f) => {
                        var istatCode = Manager.Space(6);
                        var userHost = Manager.Space(1);
                        DateTime dtmDateOfBirth = DateTime.MinValue;
                        var REP16 = listReport16.Where((dynamic r16) => r16.FiscalCode == f.FiscalCode).FirstOrDefault();
                        if (REP16 != null)
                        {
                            dtmDateOfBirth = REP16.DateOfBirth;
                            userHost = REP16.HostType;
                            var COM = listComuni.Where((dynamic c) => c.Name.ToUpper() == REP16.Town).FirstOrDefault();
                            if (COM != null)
                            {
                                istatCode = COM.Code;
                            }
                        }
                        else
                        {
                            var REP18 = listReport18.Where((dynamic r18) => r18.FiscalCode == f.FiscalCode && r18.AddressType == "1").FirstOrDefault();
                            if (REP18 != null)
                            {
                                var COM = listComuni.Where((dynamic c) => c.Name.ToUpper() == REP18.Town).FirstOrDefault();
                                if (COM != null)
                                {
                                    istatCode = COM.Code;
                                }
                            }
                        }
                        return new
                        {
                            ZSDF = f,
                            ISTATCode = istatCode,
                            UserHost = userHost,
                            DateOfBirth = dtmDateOfBirth
                        };
                    }).
                    Select((dynamic fa) => new Dietetica()
                    {
                        ATSCode = Manager.Left(ATSCode.ToString(), 3, ' '),
                        ASSTCode =ASSTCode.ToString("000000"),
                        Year = year.ToString("0000"),
                        Month = month.ToString("00"),
                        FiscalCode = fa.ZSDF.FiscalCode,
                        Sex = Manager.SexCV(fa.ZSDF.FiscalCode),
                        DateOfBirth = Manager.DatCV(fa.ZSDF.FiscalCode),
                        ISTATCode = Manager.Left(fa.ISTATCode, 6, ' '),
                        UserHost = Manager.ErogaRSA(fa.UserHost),
                        PrescriptionNumber = Manager.Space(14),
                        DeliveryDate = fa.ZSDF.ErogationDate.ToString("yyyyMMdd"),
                        TypeDescription = Manager.Left("SERVICENAD", 15, ' '),
                        Typology = "7",
                        MinsanCode = Manager.Left(new string('0', 9), 30, ' '),
                        MinsanDescription = Manager.Space(30),
                        Manufacturer = Manager.Space(30),
                        PiecesPerPack = "001",
                        UnitOfMeasure = Manager.Left("CMESE", 9, ' '),
                        Quantity = fa.ZSDF.Quantity.ToString("0000"),
                        ManagementChannel = "4",
                        PurchaseAmount = Manager.Amount("ZSDFatture", fa.ZSDF),
                        ServiceChargeAmount = new string('0', 12),
                        RecordDestination = "N",
                        ID = Manager.NumProg(ProgressiveNumber++, year, month),
                        RepCode = Manager.Space(30),
                        CNDCode = Manager.Space(13),
                        FlagDM = "F",
                        Type = Manager.Space(1)
                    }).
                    ToList();
                }
                var errorUserHostZSDFatture = reportFromZSDFatture.Where(r => r.UserHost == null);
                if (errorUserHostZSDFatture != null)
                {
                    foreach (var item in errorUserHostZSDFatture)
                    {
                        list.Add(Tuple.Create("Report", "Elaborazione report dietetica", $"Errore ZSDFatture ID {item.ID} : UserHost mancante"));
                    }
                }
                var errorDateOfBirthZSDFatture = reportFromZSDFatture.Where(r => r.DateOfBirth == null ? true : (int.Parse(r.DateOfBirth.Substring(0, 4)) - now.Year)>100);
                if (errorDateOfBirthZSDFatture != null)
                {
                    foreach (var item in errorDateOfBirthZSDFatture)
                    {
                        list.Add(Tuple.Create("Report", "Elaborazione report dietetica", $"Errore ZSDFatture ID {item.ID} : Data di nascita nulla o maggiore di 100"));
                    }
                }


                ResultRecords.AddRange(reportFromZSDFatture);
                
                Manager.CreateExcelFile(this, nameFileWithoutExt); //crea file excel xlsx
                Manager.CreateFile(this, nameFileWithoutExt); //crea file txt
                Manager.CreateFile(this, nameFileWithoutExt, true); //crea file csv
                
                var destinationFilePath = Path.Combine(Manager.Reports, $"{nameFileWithoutExt}.xlsx");
                Manager.Settings.UpdateReport(nameFileWithoutExt, nameReport, "xlsx", destinationFilePath, now, XMLSettings.ReportStatus.FileOK);
                Manager.Settings.Save();
            }
            catch (System.Exception ex)
            {
                list.Add(Tuple.Create("Report", "Elaborazione report dietetica", $"Errore interno : {ex.Message}"));
            }
            finally
            {
                WriteLog(list, nameFileWithoutExt);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Regenerate(string nameFileWithoutExt)
        {
            ResultRecords.Clear();
            var records = GetRecordsFromExcelFile(nameFileWithoutExt);

            var formattedRecords = records.Select((dynamic r) => new Dietetica()
            {
                ATSCode = Manager.Left(int.Parse(r.ATSCode).ToString(), 3, ' '),
                ASSTCode = int.Parse(r.ASSTCode).ToString("000000"),
                Year = int.Parse(r.Year).ToString("0000"),
                Month = int.Parse(r.Month).ToString("00"),
                FiscalCode = Manager.Left(r.FiscalCode, 16),
                Sex = r.Sex,
                DateOfBirth = Manager.ConvertDate(r.DateOfBirth).ToString("yyyyMMdd"),
                ISTATCode = Manager.Left(r.ISTATCode, 6, ' '),
                UserHost = r.UserHost,
                PrescriptionNumber = Manager.Space(14),
                DeliveryDate = Manager.ConvertDate(r.DeliveryDate).ToString("yyyyMMdd"),
                TypeDescription = Manager.Left(r.TypeDescription, 15, ' '),
                Typology = Manager.Left(r.Typology, 1, ' '),
                MinsanCode = Manager.Left(r.MinsanCode, 30, ' '),
                MinsanDescription = Manager.Left(r.MinsanDescription, 30, ' '),
                Manufacturer = Manager.Left(r.Manufacturer, 30, ' '),
                PiecesPerPack = int.Parse(r.PiecesPerPack).ToString("000"),
                UnitOfMeasure = Manager.Left(r.UnitOfMeasure, 9, ' '),
                Quantity = int.Parse(r.Quantity).ToString("0000"),
                ManagementChannel = ManagementChannel,
                PurchaseAmount = decimal.Parse(r.PurchaseAmount).ToString("0000000000.00").Substring(0, 10)+
                                 decimal.Parse(r.PurchaseAmount).ToString("0000000000.00").Substring(11, 2),
                ServiceChargeAmount = decimal.Parse(r.ServiceChargeAmount).ToString("0000000000.00").Substring(0, 10) +
                                 decimal.Parse(r.ServiceChargeAmount).ToString("0000000000.00").Substring(11, 2),
                RecordDestination = r.RecordDestination,
                ID = r.ID,
                RepCode = Manager.Left(r.RepCode, 30, ' '),
                CNDCode = Manager.Left(r.CNDCode, 13, ' '),
                FlagDM = r.FlagDM,
                Type = Manager.Left(r.Type, 1, ' ')
            }).ToList();

            ResultRecords.AddRange(formattedRecords);
            Manager.CreateFile(this, nameFileWithoutExt); //crea file txt
            Manager.CreateFile(this, nameFileWithoutExt, true); //crea file csv
        }

        #region Members

        //J(ZSD[A]:ATS[C; D])
        //J(R16[X]:ATS[C; D])
        [ReportMemberReference(Column = "A", Position = 1, ColumnName = "ATS", Length = 3, Required = true, FieldName = "ATSCode")]
        public string ATSCode { get; set; }

        //J(ZSD[A]:ATS[C; E])
        //J(R16[X]:ATS[C; E])
        [ReportMemberReference(Column = "B", Position = 2, ColumnName = "ASST", Length = 6, Required = true, FieldName = "ASSTCode")]
        public string ASSTCode { get; set; }

        //D(I)
        [ReportMemberReference(Column = "C", Position = 3, ColumnName = "Tipo record", Length = 1, FieldName = "PlanType")]
        public string PlanType { get { return "I"; } }

        //I(Inserisci anno)
        [ReportMemberReference(Column = "D", Position = 4, ColumnName = "Anno", Length = 4, FieldName = "Year")]
        public string Year { get; set; }

        //I(Inserisci mese)
        [ReportMemberReference(Column = "E", Position = 5, ColumnName = "Mese", Length = 2, FieldName = "Month")]
        public string Month { get; set; }

        //C(ZSD[AS])
        //C(R16[E])
        [ReportMemberReference(Column = "F", Position = 6, ColumnName = "Codice Fiscale", Length = 16, Required = true, FieldName = "FiscalCode")]
        public string FiscalCode { get; set; }

        //F(SexCV{ ZSD[AS]})
        //F(SexCV{ R16[E]})
        [ReportMemberReference(Column = "G", Position = 7, ColumnName = "Sesso", Length = 1, Required = true, FieldName = "Sex")]
        public string Sex { get; set; }

        //F(DatCV{ ZSD[AS]})
        //F(DatCV{ R16[E]})
        [ReportMemberReference(Column = "H", Position = 8, ColumnName = "Data di nascita", Length = 8, Required = true, FieldName = "DateOfBirth")]
        public string DateOfBirth { get; set; }

        //J(ZSD[?]:COM[A; B])
        //J(R16[K]:COM[A; B])
        [ReportMemberReference(Column = "I", Position = 9, ColumnName = "Comune residenza", Length = 6, Required = true, FieldName = "ISTATCode")]
        public string ISTATCode { get; set; }

        //F(ErogaRSA{ R16[T]})
        //F(ErogaRSA{ ZSD[?]})
        //ZSD FiscalCode in Report16 prelavare colonna T
        // vuoto altrimenti (ma segnalare errore)
        [ReportMemberReference(Column = "J", Position = 10, ColumnName = "Utente ospite RSA o RSD", Length = 1, Required = true, FieldName = "UserHost")]
        public string UserHost { get; set; }

        //D()
        [ReportMemberReference(Column = "K", Position = 11, ColumnName = "Numero Prescrizione", Length = 14, FieldName = "PrescriptionNumber")]
        public string PrescriptionNumber { get; set; }

        //C(R16[Q])
        //C(ZSD[AA])
        [ReportMemberReference(Column = "L", Position = 12, ColumnName = "Data erogazione", Length = 8, Required = true, FieldName = "DeliveryDate")]
        public string DeliveryDate { get; set; }

        //D(ALIMENTINAD)x R16
        //D(SERVICENAD)x ZSD
        [ReportMemberReference(Column = "M", Position = 13, ColumnName = "Descrizione tipologia", Length = 15, Required = true, FieldName = "TypeDescription")]
        public string TypeDescription { get; set; }

        //D(5)x R16
        //D(7)x ZSD
        [ReportMemberReference(Column = "N", Position = 14, ColumnName = "Tipologia", Length = 1, Required = true, FieldName = "Typology")]
        public string Typology { get; set; }

        //J(R16[AA]:ART[G;?])
        //D()x ZSD
        [ReportMemberReference(Column = "O", Position = 15, ColumnName = "Codice MINSAN", Length = 30, Required = true, FieldName = "MinsanCode")]
        public string MinsanCode { get; set; }

        //J(R16[AA]:ART[G;?])
        //D()x ZSD
        [ReportMemberReference(Column = "P", Position = 16, ColumnName = "Descrizione MINSAN", Length = 30, Required = true, FieldName = "MinsanDescription")]
        public string MinsanDescription { get; set; }

        //J(R16[AA]:ART[G;?])
        //D()x ZSD
        [ReportMemberReference(Column = "Q", Position = 17, ColumnName = "Descrizione produttore", Length = 30, FieldName = "Manufacturer")]
        public string Manufacturer { get; set; }

        //D(001)
        [ReportMemberReference(Column = "R", Position = 18, ColumnName = "Pezzi per confezione", Length = 3, Required = true, FieldName = "PiecesPerPack")]
        public string PiecesPerPack { get; set; }

        //D(PEZZO)x R16
        //D(CMESE)x ZSD
        [ReportMemberReference(Column = "S", Position = 19, ColumnName = "Unità di misura", Length = 9, Required = true, FieldName = "UnitOfMeasure")]
        public string UnitOfMeasure { get; set; }

        //Quantity 4 N
        //C(R16[AD])
        //C(ZSD[AN])
        [ReportMemberReference(Column = "T", Position = 20, ColumnName = "Quantità", Length = 4, Required = true, FieldName = "Quantity")]
        public string Quantity { get; set; }

        //D(4)
        [ReportMemberReference(Column = "U", Position = 21, ColumnName = "Canale di gestione", Length = 1, Required = true, FieldName = "ManagementChannel")]
        public string ManagementChannel { get; set; }

        //Purchase amount 12 N
        //F(Prezzo{ R16[R]})
        //F(Prezzo{ ZSD[AM]})
        [ReportMemberReference(Column = "V", Position = 22, ColumnName = "Importo acquisto", Length = 12, Required = true, FieldName = "PurchaseAmount")]
        public string PurchaseAmount { get; set; }

        //Service charge amount 12 N
        //D()
        [ReportMemberReference(Column = "W", Position = 23, ColumnName = "Importo onere di servizio", Length = 12, Required = true, FieldName = "ServiceChargeAmount")]
        public string ServiceChargeAmount { get; set; }

        //Record destination
        //D(N)
        [ReportMemberReference(Column = "X", Position = 24, ColumnName = "Destinazione del record", Length = 1, Required = true, FieldName = "RecordDestination")]
        public string RecordDestination { get; set; }

        //F(NumProg(I(ultimo numero progressivo)))
        [ReportMemberReference(Column = "Y", Position = 25, ColumnName = "ID", Length = 20, Required = true, FieldName = "ID")]
        public string ID { get; set; }

        //D()
        [ReportMemberReference(Column = "Z", Position = 26, ColumnName = "Codice repertorio", Length = 30, FieldName = "RepCode")]
        public string RepCode { get; set; }

        //D()
        [ReportMemberReference(Column = "AA", Position = 27, ColumnName = "Codice CND", Length = 13, FieldName = "CNDCode")]
        public string CNDCode { get; set; }

        //Flag DM
        //D(F)
        [ReportMemberReference(Column = "AB", Position = 28, ColumnName = "Flag DM", Length = 1, FieldName = "FlagDM")]
        public string FlagDM { get; set; }

        //D()
        [ReportMemberReference(Column = "AC", Position = 29, ColumnName = "Tipo", Length = 1, FieldName = "Type")]
        public string Type { get; set; }


        #endregion
    }
}
