using DO.VIVICARE.Reporter;
using System;
using System.IO;
using System.Linq;

namespace DO.VIVICARE.Report.Dietetica
{
    /// <summary>
    /// 
    /// </summary>
    [ReportReference(Name = "Dietetica", Description = "Report inerente il consuntivo dietetica")]
    public class Dietetica : BaseReport
    {
        private int _year;
        private int _month;
        private long _lastProgressiveNumber;
        private string[] docs = new string[] { "ASST", "Comuni", "ZSDFatture", "Report16", "Report18", "MinSan", "Prezzi" };

        public void SetYear(int year)
        {
            _year = year;
        }

        public void SetMonth(int month)
        {
            _month = month;
        }

        public void SetLastProgressiveNumber(long lastProgressiveNumber)
        {
            _lastProgressiveNumber = lastProgressiveNumber;
        }

        public override void LoadDocuments(bool withRecords = false)
        {
            foreach (var document in Manager.GetDocuments())
            {
                if (!docs.Contains(document.Attribute.Name)) continue;

                //SourceFilePath recuperare da Settings
                var list = Manager.Settings.GetDocumentValues(XMLSettings.LibraryType.Document, document.Attribute.Name);
                document.Document.SourceFilePath = "";
                if (list != null)
                {
                    document.Document.SourceFilePath = Path.Combine(Manager.Documents, list[0] + list[1]);
                }

                document.Document.AttributeName = document.Attribute.Name;

                if (withRecords) document.Document.LoadRecords();
                Documents.Add(document.Document);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        //public Dietetica()
        //{
        //    //string[] docs = new string[] { "ASST", "Comuni", "Report16", "Report18", "ZSDFatture" };
        //    string[] docs = new string[] { "ASST", "Comuni", "ZSDFatture", "Report16", "Report18", "MinSan", "Prezzi" };
        //    //string[] docs = new string[] { "ZSDFatture" };
        //    foreach (var document in Manager.GetDocuments())
        //    {
        //        if (!docs.Contains(document.Attribute.Name)) continue;

        //        //SourceFilePath recuperare da Settings
        //        var list = Manager.Settings.GetDocumentValues(XMLSettings.LibraryType.Document, document.Attribute.Name);
        //        document.Document.SourceFilePath = "";
        //        if (list!=null)
        //        {
        //            if (list[2]!=null)
        //            {
        //                document.Document.SourceFilePath = list[2];
        //            }
        //        }
        //        document.Document.AttributeName = document.Attribute.Name;
        //        if (document.Document.LoadRecords())
        //        {
        //            Documents.Add(document.Document);
        //        }   
        //    }
        //}

        //public Dietetica(bool value)
        //{

        //}
        /// <summary>
        /// 
        /// </summary>
        public override void Execute()
        {
            //base.Execute();

            try
            {
                
                var documents = Documents;

                var doc = documents.Find(x => x.AttributeName == "ZSDFatture");
                if (doc == null)
                {
                    throw new Exception("ZSDFatture non trovato!");
                }
                var listZSDFatture = doc.Records;
                if (listZSDFatture == null)
                {
                    throw new Exception("ZSDFatture non caricato!");
                }
                doc = documents.Find(x => x.AttributeName == "Report16");
                if (doc == null)
                {
                    throw new Exception("Report16 non trovato!");
                }
                var listReport16 = doc.Records;
                if (listReport16 == null)
                {
                    throw new Exception("Report16 non caricato!");
                }
                doc = documents.Find(x => x.AttributeName == "Report18");
                if (doc == null)
                {
                    throw new Exception("Report18 non trovato!");
                }
                var listReport18 = doc.Records;
                if (listReport18 == null)
                {
                    throw new Exception("Report18 non caricato!");
                }
                doc = documents.Find(x => x.AttributeName == "ASST");
                if (doc == null)
                {
                    throw new Exception("ASST non trovato!");
                }
                var listASST = doc.Records;
                if (listASST == null)
                {
                    throw new Exception("ASST non caricato!");
                }
                doc = documents.Find(x => x.AttributeName == "Comuni");
                if (doc == null)
                {
                    throw new Exception("Comuni non trovato!");
                }
                var listComuni = doc.Records;
                if (listComuni == null)
                {
                    throw new Exception("Comuni non caricato!");
                }
                doc = documents.Find(x => x.AttributeName == "MinSan");
                if (doc == null)
                {
                    throw new Exception("MinSan non trovato!");
                }
                var listMinSan = doc.Records;
                if (listMinSan == null)
                {
                    throw new Exception("MinSan non caricato!");
                }
                doc = documents.Find(x => x.AttributeName == "Prezzi");
                if (doc == null)
                {
                    throw new Exception("Prezzi non trovato!");
                }
                var listPrezzi = doc.Records;
                if (listPrezzi == null)
                {
                    throw new Exception("Prezzi non caricato!");
                }
                var reportFromReport16 = listReport16.Select((dynamic r) => new {
                    REP16 = r,
                    ASST = listASST.Where((dynamic a) => System.Convert.ToInt64(a.SAPCode) == System.Convert.ToInt64(r.ERPCode)).FirstOrDefault(),
                    MINSAN = listMinSan.Where((dynamic m) => m.IDVivisol == r.ArticleCode).FirstOrDefault(),
                    PRZ = listPrezzi.Where((dynamic p) => p.IDVivisol == r.ArticleCode).FirstOrDefault(),
                    COM = listComuni.Where((dynamic c) => c.Name.ToUpper() == r.Town).FirstOrDefault()
                }). 
                    Select((dynamic ramp) => new Dietetica()
                    {
                        ATSCode = ramp.ASST==null? Manager.Space(3) : Manager.Left(ramp.ASST.ATSCode.ToString(), 3 ,' '),
                        ASSTCode = ramp.ASST == null ? Manager.Space(6) : Manager.Left(ramp.ASST.ASSTCode.ToString(), 6, ' '),
                        Year = _year.ToString("0000"),
                        Month = _month.ToString("00"),
                        FiscalCode = ramp.REP16.FiscalCode,
                        Sex = Manager.SexCV(ramp.REP16.FiscalCode),
                        DateOfBirth = Manager.DatCV(ramp.REP16.FiscalCode),
                        ISTATCode = ramp.COM==null? Manager.Space(6) : Manager.Left(ramp.COM.Code, 6, ' '),
                        UserHost = Manager.ErogaRSA(ramp.REP16.HostType),
                        PrescriptionNumber = Manager.Space(14),
                        DeliveryDate = ramp.REP16.ErogationDate.ToString("yyyyMMdd"),
                        TypeDescription = Manager.Left("ALIMENTINAD", 15, ' '),
                        Typology = "5",
                        MinsanCode = ramp.MINSAN==null? Manager.Space(30) : Manager.Left(ramp.MINSAN.ArtCode, 30, ' '),
                        MinsanDescription = ramp.MINSAN == null ? Manager.Space(30) : Manager.Left(ramp.MINSAN.ArtDescription, 30, ' '),
                        Manufacturer = ramp.MINSAN == null ? Manager.Space(30) : Manager.Left(ramp.MINSAN.Producer, 30, ' '),
                        PiecesPerPack = "001",
                        UnitOfMeasure = Manager.Left("PEZZO", 9, ' '),
                        Quantity = ramp.REP16.Quantity.ToString("0000"),
                        ManagementChannel = "4",
                        PurchaseAmount = ramp.PRZ==null? new string('0', 12):Manager.Amount("Report16", ramp.REP16, ramp.PRZ.Price),
                        ServiceChargeAmount = new string('0', 12),
                        RecordDestination = "N",
                        ID = Manager.NumProg(_lastProgressiveNumber, _year, _month),
                        RepCode = Manager.Space(30),
                        CNDCode = Manager.Space(13),
                        FlagDM = "F",
                        Type = Manager.Space(1)
                    }).
                    ToList();

                ResultRecords.AddRange(reportFromReport16);


                var reportFromZSDFatture = listZSDFatture.Select((dynamic f) => {
                    var istatCode = Manager.Space(6);
                    var REP16 = listReport16.Where((dynamic r16) => r16.FiscalCode == f.FiscalCode).FirstOrDefault();
                    if (REP16!=null)
                    {
                        var COM = listComuni.Where((dynamic c) => c.Name.ToUpper() == REP16.Town).FirstOrDefault();
                        if (COM!=null)
                        {
                            istatCode = COM.Code;
                        }
                    }
                    else
                    {
                        var REP18 = listReport18.Where((dynamic r18) => r18.FiscalCode == f.FiscalCode && r18.AddressType == "1").FirstOrDefault();
                        if (REP18!=null)
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
                        ASST = listASST.Where((dynamic a) => a.SAPCode == f.Customer).FirstOrDefault(),
                        ISTATCode = istatCode
                    };
                }).
                    Select((dynamic fa) => new Dietetica()
                    {
                        ATSCode = fa.ASST == null ? Manager.Space(3) : Manager.Left(fa.ASST.ATSCode.ToString(), 3, ' '),
                        ASSTCode = fa.ASST == null ? Manager.Space(6) : Manager.Left(fa.ASST.ASSTCode.ToString(), 6, ' '),
                        Year = _year.ToString("0000"),
                        Month = _month.ToString("00"),
                        FiscalCode = fa.ZSDF.FiscalCode,
                        Sex = Manager.SexCV(fa.ZSDF.FiscalCode),
                        DateOfBirth = Manager.DatCV(fa.ZSDF.FiscalCode),
                        ISTATCode = Manager.Left(fa.ISTATCode, 6, ' '),
                        UserHost = Manager.Space(1),
                        PrescriptionNumber = Manager.Space(14),
                        DeliveryDate = fa.ZSDF.ErogationDate.ToString("yyyyMMdd"),
                        TypeDescription = Manager.Left("SERVICENAD", 15, ' '),
                        Typology = "7",
                        MinsanCode = Manager.Space(30),
                        MinsanDescription = Manager.Space(30),
                        Manufacturer = Manager.Space(30),
                        PiecesPerPack = "001",
                        UnitOfMeasure = Manager.Left("CMESE", 9, ' '),
                        Quantity = fa.ZSDF.Quantity.ToString("0000"),
                        ManagementChannel = "4",
                        PurchaseAmount = Manager.Amount("ZSDFatture", fa.ZSDF),
                        ServiceChargeAmount = new string('0', 12),
                        RecordDestination = "N",
                        ID = Manager.NumProg(_lastProgressiveNumber, _year, _month),
                        RepCode = Manager.Space(30),
                        CNDCode = Manager.Space(13),
                        FlagDM = "F",
                        Type = Manager.Space(1)
                    }).
                    ToList();

                ResultRecords.AddRange(reportFromZSDFatture);

                Manager.CreateExcelFile(this); //crea file excel xlsx
                Manager.CreateFile(this); //crea file txt
                Manager.CreateFile(this, true); //crea file csv

            }
            catch (System.Exception ex)
            {
                throw;
            }
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
        [ReportMemberReference(Column = "H", Position = 8, ColumnName = "Data di nascita", Length = 8, IsDate = true, Required = true, FieldName = "DateOfBirth")]
        public string DateOfBirth { get; set; }

        //J(ZSD[?]:COM[A; B])
        //J(R16[K]:COM[A; B])
        [ReportMemberReference(Column = "I", Position = 9, ColumnName = "Comune residenza", Length = 6, Required = true, FieldName = "ISTATCode")]
        public string ISTATCode { get; set; }

        //F(ErogaRSA{ R16[T]})
        //F(ErogaRSA{ ZSD[?]})
        [ReportMemberReference(Column = "J", Position = 10, ColumnName = "Utente ospite RSA o RSD", Length = 1, Required = true, FieldName = "UserHost")]
        public string UserHost { get; set; }

        //D()
        [ReportMemberReference(Column = "K", Position = 11, ColumnName = "Numero Prescrizione", Length = 14, FieldName = "PrescriptionNumber")]
        public string PrescriptionNumber { get; set; }

        //C(R16[Q])
        //C(ZSD[AA])
        [ReportMemberReference(Column = "L", Position = 12, ColumnName = "Data erogazione", Length = 8, IsDate = true, Required = true, FieldName = "DeliveryDate")]
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
        [ReportMemberReference(Column = "V", Position = 22, ColumnName = "Importo acquisto", Length = 12, DecimalDigits = 2, Required = true, FieldName = "PurchaseAmount")]
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
