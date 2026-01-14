using DO.VIVICARE.Reporter;
using System;

namespace DO.VIVICARE.Document.Valorizzazione
{
    [DocumentReference(Name = "Valorizzazione", Description = "Esito del processo di valorizzazione ADI Lazio Alta intensità", RowStart = 2)]
    public class Valorizzazione : BaseDocument
    {
        #region Member

        #region ANAG
        [DocumentMemberReference(Column = "A", Position = 1, FieldName = "PatientName")]
        public string PatientName { get; set; }

        [DocumentMemberReference(Column = "B", Position = 2, FieldName = "FiscalCode")]
        public string FiscalCode { get; set; }

        [DocumentMemberReference(Column = "C", Position = 3, FieldName = "ASL")]
        public string ASL { get; set; }

        [DocumentMemberReference(Column = "D", Position = 4, FieldName = "District")]
        public string District { get; set; }

        [DocumentMemberReference(Column = "E", Position = 5, FieldName = "ActivityDate", Format = "dd/MM/yyyy")]
        public DateTime ActivityDate { get; set; }

        [DocumentMemberReference(Column = "F", Position = 6, FieldName = "TotalValue")]
        public decimal TotalValue { get; set; }

        [DocumentMemberReference(Column = "G", Position = 7, FieldName = "NoDiscountValue")]
        public decimal NoDiscountValue { get; set; }

        [DocumentMemberReference(Column = "H", Position = 8, FieldName = "Discount")]
        public decimal Discount { get; set; }
        #endregion

        #region PACCHETTI BASE
        [DocumentMemberReference(Column = "I", Position = 9, FieldName = "BasePacketNumber")]
        public decimal BasePacketNumber { get; set; }

        [DocumentMemberReference(Column = "J", Position = 10, FieldName = "BasePacketValue")]
        public decimal BasePacketValue { get; set; }

        [DocumentMemberReference(Column = "K", Position = 11, FieldName = "BasePacketTotal")]
        public decimal BasePacketTotal { get; set; }
        #endregion

        #region PACCHETTI SOLLIEVO
        [DocumentMemberReference(Column = "L", Position = 12, FieldName = "ReliefPacketNumber")]
        public decimal ReliefPacketNumber { get; set; }

        [DocumentMemberReference(Column = "M", Position = 13, FieldName = "ReliefPacketValue")]
        public decimal ReliefPacketValue { get; set; }

        [DocumentMemberReference(Column = "N", Position = 14, FieldName = "ReliefPacketTotal")]
        public decimal ReliefPacketTotal { get; set; }
        #endregion

        #region ORE INFERMIERISTICHE
        [DocumentMemberReference(Column = "O", Position = 15, FieldName = "HourInfNumber")]
        public decimal HourInfNumber { get; set; }

        [DocumentMemberReference(Column = "P", Position = 16, FieldName = "HourInfValue")]
        public decimal HourInfValue { get; set; }

        [DocumentMemberReference(Column = "Q", Position = 17, FieldName = "HourInfTotal")]
        public decimal HourInfTotal { get; set; }
        #endregion

        #region ORE RIABILITAZIONE
        [DocumentMemberReference(Column = "R", Position = 18, FieldName = "HourRehabNumber")]
        public decimal HourRehabNumber { get; set; }

        [DocumentMemberReference(Column = "S", Position = 19, FieldName = "HourRehabValue")]
        public decimal HourRehabValue { get; set; }

        [DocumentMemberReference(Column = "T", Position = 20, FieldName = "HourRehabTotal")]
        public decimal HourRehabTotal { get; set; }
        #endregion

        #region ORE OSS
        [DocumentMemberReference(Column = "U", Position = 21, FieldName = "HourOssNumber")]
        public decimal HourOssNumber { get; set; }

        [DocumentMemberReference(Column = "V", Position = 22, FieldName = "HourOssValue")]
        public decimal HourOssValue { get; set; }

        [DocumentMemberReference(Column = "W", Position = 23, FieldName = "HourOssTotal")]
        public decimal HourOssTotal { get; set; }
        #endregion

        #region ORE PSICOLOGO
        [DocumentMemberReference(Column = "X", Position = 24, FieldName = "HourPsyNumber")]
        public decimal HourPsyNumber { get; set; }

        [DocumentMemberReference(Column = "Y", Position = 25, FieldName = "HourPsyValue")]
        public decimal HourPsyValue { get; set; }

        [DocumentMemberReference(Column = "Z", Position = 26, FieldName = "HourPsyTotal")]
        public decimal HourPsyTotal { get; set; }
        #endregion

        #region PRELIEVI
        [DocumentMemberReference(Column = "AA", Position = 27, FieldName = "SampleNumber")]
        public decimal SampleNumber { get; set; }

        [DocumentMemberReference(Column = "AB", Position = 28, FieldName = "SampleValue")]
        public decimal SampleValue { get; set; }

        [DocumentMemberReference(Column = "AC", Position = 29, FieldName = "SampleTotal")]
        public decimal SampleTotal { get; set; }
        #endregion

        #region TRASPORTI CON INF
        [DocumentMemberReference(Column = "AD", Position = 30, FieldName = "TransInfNumber")]
        public decimal TransInfNumber { get; set; }

        [DocumentMemberReference(Column = "AE", Position = 31, FieldName = "TransInfValue")]
        public decimal TransInfValue { get; set; }

        [DocumentMemberReference(Column = "AF", Position = 32, FieldName = "TransInfTotal")]
        public decimal TransInfTotal { get; set; }
        #endregion

        #region TRASPORTI CON MED
        [DocumentMemberReference(Column = "AG", Position = 33, FieldName = "TransDocNumber")]
        public decimal TransDocNumber { get; set; }

        [DocumentMemberReference(Column = "AH", Position = 34, FieldName = "TransDocValue")]
        public decimal TransDocValue { get; set; }

        [DocumentMemberReference(Column = "AI", Position = 35, FieldName = "TransDocTotal")]
        public decimal TransDocTotal { get; set; }
        #endregion

        #region ACCESSI SPECIALISTICI
        [DocumentMemberReference(Column = "AJ", Position = 36, FieldName = "SpecialistAccessNumber")]
        public decimal SpecialistAccessNumber { get; set; }

        [DocumentMemberReference(Column = "AK", Position = 37, FieldName = "SpecialistAccessValue")]
        public decimal SpecialistAccessValue { get; set; }

        [DocumentMemberReference(Column = "AL", Position = 38, FieldName = "SpecialistAccessTotal")]
        public decimal SpecialistAccessTotal { get; set; }
        #endregion

        #region RX DOMICILIARE
        [DocumentMemberReference(Column = "AM", Position = 39, FieldName = "RXDomNumber")]
        public decimal RXDomNumber { get; set; }

        [DocumentMemberReference(Column = "AN", Position = 40, FieldName = "RXDomValue")]
        public decimal RXDomValue { get; set; }

        [DocumentMemberReference(Column = "AO", Position = 41, FieldName = "RXDomTotal")]
        public decimal RXDomTotal { get; set; }
        #endregion

        #region ECOGRAFIA DOMICILIARE
        [DocumentMemberReference(Column = "AP", Position = 42, FieldName = "EcoDomNumber")]
        public decimal EcoDomNumber { get; set; }

        [DocumentMemberReference(Column = "AQ", Position = 43, FieldName = "EcoDomValue")]
        public decimal EcoDomValue { get; set; }

        [DocumentMemberReference(Column = "AR", Position = 44, FieldName = "EcoDomTotal")]
        public decimal EcoDomTotal { get; set; }
        #endregion

        #region EMOGAS DOMICILIARE
        [DocumentMemberReference(Column = "AS", Position = 45, FieldName = "EmoGasDomNumber")]
        public decimal EmoGasDomNumber { get; set; }

        [DocumentMemberReference(Column = "AT", Position = 46, FieldName = "EmoGasDomValue")]
        public decimal EmoGasDomValue { get; set; }

        [DocumentMemberReference(Column = "AU", Position = 47, FieldName = "EmoGasDomTotal")]
        public decimal EmoGasDomTotal { get; set; }
        #endregion

        #region EMOGAS DOMICILIARE SOLO PRELIEVO E TRASP.
        [DocumentMemberReference(Column = "AV", Position = 48, FieldName = "EmoGasLabDomNumber")]
        public decimal EmoGasLabDomNumber { get; set; }

        [DocumentMemberReference(Column = "AW", Position = 49, FieldName = "EmoGasLabDomValue")]
        public decimal EmoGasLabDomValue { get; set; }

        [DocumentMemberReference(Column = "AX", Position = 50, FieldName = "EmoGasLabDomTotal")]
        public decimal EmoGasLabDomTotal { get; set; }
        #endregion

        #region EMOTRASFUSIONE DOMICILIARE
        [DocumentMemberReference(Column = "AY", Position = 51, FieldName = "EmoTrasfDomNumber")]
        public decimal EmoTrasfDomNumber { get; set; }

        [DocumentMemberReference(Column = "AZ", Position = 52, FieldName = "EmoTrasfDomValue")]
        public decimal EmoTrasfDomValue { get; set; }

        [DocumentMemberReference(Column = "BA", Position = 53, FieldName = "EmoTrasfDomTotal")]
        public decimal EmoTrasfDomTotal { get; set; }
        #endregion

        #region CAMBIO CATETERE
        [DocumentMemberReference(Column = "BB", Position = 54, FieldName = "CatheterChangeNumber")]
        public decimal CatheterChangeNumber { get; set; }

        [DocumentMemberReference(Column = "BC", Position = 55, FieldName = "CatheterChangeValue")]
        public decimal CatheterChangeValue { get; set; }

        [DocumentMemberReference(Column = "BD", Position = 56, FieldName = "CatheterChangeTotal")]
        public decimal CatheterChangeTotal { get; set; }
        #endregion

        #region TAMPONI ANTIGENICI (COVID)
        [DocumentMemberReference(Column = "BE", Position = 57, FieldName = "SwabNumber")]
        public decimal SwabNumber { get; set; }

        [DocumentMemberReference(Column = "BF", Position = 58, FieldName = "SwabValue")]
        public decimal SwabValue { get; set; }

        [DocumentMemberReference(Column = "BG", Position = 59, FieldName = "SwabTotal")]
        public decimal SwabTotal { get; set; }
        #endregion

        #region PRESTAZIONI VACCINI (COVID)
        [DocumentMemberReference(Column = "BH", Position = 60, FieldName = "VaxNumber")]
        public decimal VaxNumber { get; set; }

        [DocumentMemberReference(Column = "BI", Position = 61, FieldName = "VaxValue")]
        public decimal VaxValue { get; set; }

        [DocumentMemberReference(Column = "BJ", Position = 62, FieldName = "VaxTotal")]
        public decimal VaxTotal { get; set; }
        #endregion

        #region GESTIONE KIT PER TELEMONITORAGGIO PAZ. COVID
        [DocumentMemberReference(Column = "BK", Position = 63, FieldName = "MonitoringKitNumber")]
        public decimal MonitoringKitNumber { get; set; }

        [DocumentMemberReference(Column = "BL", Position = 64, FieldName = "MonitoringKitValue")]
        public decimal MonitoringKitValue { get; set; }

        [DocumentMemberReference(Column = "BM", Position = 65, FieldName = "MonitoringKitTotal")]
        public decimal MonitoringKitTotal { get; set; }
        #endregion

        #region ORE TOTALI
        [DocumentMemberReference(Column = "BN", Position = 66, FieldName = "HourInfNumberTotal")]
        public decimal HourInfNumberTotal { get; set; }

        [DocumentMemberReference(Column = "BO", Position = 67, FieldName = "HourFktNumberTotal")]
        public decimal HourFktNumberTotal { get; set; }

        [DocumentMemberReference(Column = "BP", Position = 68, FieldName = "HourLogNumberTotal")]
        public decimal HourLogNumberTotal { get; set; }

        [DocumentMemberReference(Column = "BQ", Position = 69, FieldName = "HourTpnNumberTotal")]
        public decimal HourTpnNumberTotal { get; set; }

        [DocumentMemberReference(Column = "BR", Position = 70, FieldName = "HourTerNumberTotal")]
        public decimal HourTerNumberTotal { get; set; }

        [DocumentMemberReference(Column = "BS", Position = 71, FieldName = "HourPsiNumberTotal")]
        public decimal HourPsiNumberTotal { get; set; }

        [DocumentMemberReference(Column = "BT", Position = 72, FieldName = "HourOssNumbertotal")]
        public decimal HourOssNumbertotal { get; set; }
        #endregion

        #region NUMERO ACCESSI, PRELIEVI, TRASPORTI
        [DocumentMemberReference(Column = "BU", Position = 73, FieldName = "AccessAneNumber")]
        public decimal AccessAneNumber { get; set; }

        [DocumentMemberReference(Column = "BV", Position = 74, FieldName = "AccessChiNumber")]
        public decimal AccessChiNumber { get; set; }

        [DocumentMemberReference(Column = "BW", Position = 75, FieldName = "SampleNumber2")]
        public decimal SampleNumber2 { get; set; }

        [DocumentMemberReference(Column = "BX", Position = 76, FieldName = "TransInfNumber")]
        public decimal TransInfNumberTotal { get { return TransInfNumber; } }

        [DocumentMemberReference(Column = "BY", Position = 77, FieldName = "TransDocNumber")]
        public decimal TransDocNumberTotal { get { return TransDocNumber; } }
        #endregion

        #endregion
    }
}
