using DO.VIVICARE.Reporter;

namespace DO.VIVICARE.Document.ASST
{
    [DocumentReference(Name = "ASST", Description = "Elenco delle Aziende Sanitarie", RowStart =2)]
    public class ASST : BaseDocument
    {
        [DocumentMemberReference(Column = "B", Position = 2, FieldName = "Description")]
        public string Description { get; set; }

        [DocumentMemberReference(Column = "C", Position = 3, FieldName = "SAPCode")]
        public string SAPCode { get; set; }

        [DocumentMemberReference(Column = "D", Position = 4, FieldName = "ATSCode")]
        public int ATSCode { get; set; }

        [DocumentMemberReference(Column = "E", Position = 5, FieldName = "ASSTCode")]
        public int ASSTCode { get; set; }

        [DocumentMemberReference(Column = "H", Position = 8, FieldName = "IDSintesi")]
        public int IDSintesi { get; set; }
    }
}
