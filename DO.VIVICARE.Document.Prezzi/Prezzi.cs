using DO.VIVICARE.Reporter;

namespace DO.VIVICARE.Document.Prezzi
{
    [DocumentReference(Name = "Prezzi", Description = "Elenco dei prezzi per articolo o servizio", RowStart = 2)]
    public class Prezzi: BaseDocument
    {
        [DocumentMemberReference(Column = "A", Position = 1)]
        public string ASSTDescription { get; set; }

        [DocumentMemberReference(Column = "B", Position = 2)]
        public string SAPCode { get; set; }
        [DocumentMemberReference(Column = "C", Position = 3)]
        public string ATSCode { get; set; }
        [DocumentMemberReference(Column = "D", Position = 4)]
        public string ASSTCode { get; set; }
        [DocumentMemberReference(Column = "E", Position = 5)]
        public string IDSintesi { get; set; }
        [DocumentMemberReference(Column = "F", Position = 6)]
        public string ArtDescription { get; set; }
        [DocumentMemberReference(Column = "G", Position = 7)]
        public string IDVivisol { get; set; }
        [DocumentMemberReference(Column = "H", Position = 8)]
        public decimal Price { get; set; }
    }
}
