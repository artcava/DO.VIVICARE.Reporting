using DO.VIVICARE.Reporter;

namespace DO.VIVICARE.Document.Prestazioni
{
    [DocumentReference(Name = "Prestazioni", Description = "Elenco delle Prestazioni", RowStart = 2)]
    public class Prestazioni: BaseDocument
    {
        [DocumentMemberReference(Column = "A", Position = 1, FieldName = "HealthService")]
        public string HealthService { get; set; }

        [DocumentMemberReference(Column = "B", Position = 2, FieldName = "WorkType")]
        public string WorkType { get; set; }

        [DocumentMemberReference(Column = "C", Position = 2, FieldName = "HealthWorkerType")]
        public string HealthWorkerType { get; set; }
    }
}
