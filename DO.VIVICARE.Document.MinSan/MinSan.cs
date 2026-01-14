using DO.VIVICARE.Reporter;

namespace DO.VIVICARE.Document.MinSan
{
    [DocumentReference(Name = "MinSan", Description = "Fle dei codici del Ministero della Sanità", RowStart = 2)]
    public class MinSan : BaseDocument
    {
        [DocumentMemberReference(Column = "A", Position = 1, FieldName = "Producer")]
        public string Producer { get; set; }
        [DocumentMemberReference(Column = "B", Position = 2, FieldName = "IDVivisol")]
        public string IDVivisol { get; set; }
        [DocumentMemberReference(Column = "C", Position = 3, FieldName = "ArtDescription")]
        public string ArtDescription { get; set; }
        [DocumentMemberReference(Column = "D", Position = 4, FieldName = "ArtCode")]
        public string ArtCode { get; set; }
    }
}
