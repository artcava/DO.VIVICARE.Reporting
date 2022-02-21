using DO.VIVICARE.Reporter;

namespace DO.VIVICARE.Document.LazioHealthWorker
{
    [DocumentReference(Name = "LazioHealthWorker", Description = "Operatori Sanitari del Lazio", RowStart = 2)]
    public class LazioHealthWorker: BaseDocument
    {
        [DocumentMemberReference(Column = "A", Position = 1, FieldName = "HealthWorker")]
        public string HealthWorker { get; set; }

        [DocumentMemberReference(Column = "B", Position = 2, FieldName = "WorkType")]
        public string WorkType { get; set; }
    }
}
