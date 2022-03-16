using DO.VIVICARE.Reporter;
using System;

namespace DO.VIVICARE.Document.ADIAltaIntensita
{
    [DocumentReference(Name = "ADIAltaIntensita", Description = "ADI Alta Intensità (Lazio)", RowStart = 2)]
    public class ADIAltaIntensita:BaseDocument
    {
        [DocumentMemberReference(Column = "C", Position = 3, FieldName = "Patient")]
        public string Patient { get; set; }

        [DocumentMemberReference(Column = "D", Position = 4, FieldName = "ASL")]
        public string ASL { get; set; }

        [DocumentMemberReference(Column = "E", Position = 5, FieldName = "District")]
        public string District { get; set; }

        [DocumentMemberReference(Column = "F", Position = 6, FieldName = "Date")]
        public DateTime Date { get; set; }

        [DocumentMemberReference(Column = "J", Position = 10, FieldName = "HealthWorker")]
        public string HealthWorker { get; set; }

        [DocumentMemberReference(Column = "K", Position = 11, FieldName = "Duration")]
        public decimal Duration { get; set; }

        [DocumentMemberReference(Column = "L", Position = 12, FieldName = "Activity")]
        public string Activity { get; set; }

        public string NameKey { get { return HealthWorker.Replace(" ", string.Empty); } }
    }
}
