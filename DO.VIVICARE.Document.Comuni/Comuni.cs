using DO.VIVICARE.Reporter;

namespace DO.VIVICARE.Document.Comuni
{
    [DocumentReference(Name = "Comuni", Description = "Elenco dei comuni italiani", RowStart =2)]
    public class Comuni : BaseDocument
    {
        [DocumentMemberReference(Column = "A", Position = 1)]
        public string Name { get; set; }

        [DocumentMemberReference(Column = "B", Position = 2)]
        public string Code { get; set; }
    }
}
