using DO.VIVICARE.Reporter;
using System;

namespace DO.VIVICARE.Document.ADIAltaIntensita
{
    [DocumentReference(Name = "ADIAltaIntensita", Description = "ADI Alta Intensità (Lazio)", RowStart = 2)]
    public class ADIAltaIntensita : BaseDocument
    {
        [DocumentMemberReference(Column = "C", Position = 3, FieldName = "Patient")]
        public string Patient { get; set; }

        [DocumentMemberReference(Column = "D", Position = 4, FieldName = "ASL")]
        public string ASL { get; set; }

        [DocumentMemberReference(Column = "E", Position = 5, FieldName = "District")]
        public string District { get; set; }

        [DocumentMemberReference(Column = "G", Position = 7, FieldName = "Date", Format = "dd/MM/yyyy")]
        public DateTime Date { get; set; }

        [DocumentMemberReference(Column = "K", Position = 11, FieldName = "HealthWorker")]
        public string HealthWorker { get; set; }

        [DocumentMemberReference(Column = "M", Position = 13, FieldName = "Duration", Format = "hh\\:mm")]
        public TimeSpan Duration { get; set; }

        [DocumentMemberReference(Column = "O", Position = 15, FieldName = "Activity")]
        public string Activity { get; set; }

        public string NameKey { get { return HealthWorker.Replace(" ", string.Empty); } }
    }
}
