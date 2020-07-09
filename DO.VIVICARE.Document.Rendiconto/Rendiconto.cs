using DO.VIVICARE.Reporter;

namespace DO.VIVICARE.Document.Rendiconto
{
    [DocumentReference(Name = "Rendiconto", Description = "Rendiconto prelievi da conto deposito", RowStart = 2)]
    public class Rendiconto:BaseDocument
    {
        [DocumentMemberReference(Column = "D", Position = 4, FieldName = "CompanyName")]
        public string CompanyName { get; set; }

        [DocumentMemberReference(Column = "G", Position = 7, FieldName = "ContractId")]
        public int ContractId { get; set; }

        [DocumentMemberReference(Column = "N", Position = 14, FieldName = "FiscalCode")]
        public string FiscalCode { get; set; }
    }
}
