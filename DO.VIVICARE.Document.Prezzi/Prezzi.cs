using DO.VIVICARE.Reporter;

namespace DO.VIVICARE.Document.Prezzi
{
    [DocumentReference(Name = "Prezzi", Description = "Elenco dei prezzi per articolo o servizio", RowStart = 2)]
    public class Prezzi : BaseDocument
    {
        [DocumentMemberReference(Column = "A", Position = 1, FieldName = "ASSTDescription")]
        public string ASSTDescription { get; set; }
        [DocumentMemberReference(Column = "B", Position = 2, FieldName = "SAPCode")]
        public string SAPCode { get; set; }
        [DocumentMemberReference(Column = "C", Position = 3, FieldName = "ATSCode")]
        public string ATSCode { get; set; }
        //ASSTCode filtrare per ASST
        [DocumentMemberReference(Column = "D", Position = 4, FieldName = "ASSTCode")]
        public string ASSTCode { get; set; }
        [DocumentMemberReference(Column = "E", Position = 5, FieldName = "IDSintesi")]
        public string IDSintesi { get; set; }
        [DocumentMemberReference(Column = "F", Position = 6, FieldName = "ArtDescription")]
        public string ArtDescription { get; set; }
        [DocumentMemberReference(Column = "G", Position = 7, FieldName = "IDVivisol")]
        public string IDVivisol { get; set; }
        [DocumentMemberReference(Column = "H", Position = 8, FieldName = "Price")]
        public decimal Price { get; set; }
    }
}
