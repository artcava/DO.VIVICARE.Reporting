using DO.VIVICARE.Reporter;

namespace DO.VIVICARE.Document.Comuni
{
    [DocumentReference(FileName = "Comuni.xlsx", Name = "Comuni", Description = "Elenco dei comuni italiani")]
    public class Comuni : BaseDocument
    {
        [DocumentMemberReference(Column = "A", Position = 0)]
        public string Name { get; set; }

        [DocumentMemberReference(Column = "B", Position = 1)]
        public string Code { get; set; }
    }
}
