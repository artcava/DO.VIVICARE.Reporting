using DO.VIVICARE.Reporter;

namespace DO.VIVICARE.Document.ASST
{
    [DocumentReference(FileName = "ASST.xlsx", Name = "ASST", Description = "Elenco delle Aziende Sanitarie")]
    public class ASST : BaseDocument
    {
        [DocumentMemberReference(Column = "B", Position = 1)]
        public string Description { get; set; }

        [DocumentMemberReference(Column = "C", Position = 2)]
        public int SAPCode { get; set; }

        [DocumentMemberReference(Column = "D", Position = 3)]
        public int ATSCode { get; set; }

        [DocumentMemberReference(Column = "E", Position = 4)]
        public int ASSTCode { get; set; }

        [DocumentMemberReference(Column = "F", Position = 5)]
        public int IDSintesi { get; set; }
    }
}
