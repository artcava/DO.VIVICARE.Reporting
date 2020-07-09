using DO.VIVICARE.Reporter;

namespace DO.VIVICARE.Document.Report18
{
    [DocumentReference(Name = "Report18", Description = "Una descrizione del file", RowStart =2)]
    public class Report18 : BaseDocument
    {
        [DocumentMemberReference(Column = "D", Position = 4, FieldName = "Town")]
        public string Town { get; set; }

        [DocumentMemberReference(Column = "H", Position = 8, FieldName = "AddressType")]
        public string AddressType { get; set; }

        [DocumentMemberReference(Column = "N", Position = 14, FieldName = "FiscalCode")]
        public string FiscalCode { get; set; }
    }
}
