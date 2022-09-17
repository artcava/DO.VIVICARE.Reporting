using DO.VIVICARE.Reporter;
using System;

namespace DO.VIVICARE.Document.ADIBassaIntensita
{
    [DocumentReference(Name = "ADIBassaIntensita", Description = "ADI Bassa Intensità (Lazio)", RowStart = 2)]
    public class ADIBassaIntensita : BaseDocument
    {
        [DocumentMemberReference(Column = "C", Position = 3, FieldName = "FiscalCode")]
        public string FiscalCode { get; set; }

        [DocumentMemberReference(Column = "D", Position = 4, FieldName = "Patient")]
        public string Patient { get; set; }

        [DocumentMemberReference(Column = "E", Position = 5, FieldName = "ASL")]
        public string ASL { get; set; }

        [DocumentMemberReference(Column = "F", Position = 6, FieldName = "District")]
        public string District { get; set; }

        [DocumentMemberReference(Column = "H", Position = 8, FieldName = "Date", Format = "dd/MM/yyyy")]
        public DateTime Date { get; set; }

        [DocumentMemberReference(Column = "L", Position = 12, FieldName = "HealthWorker")]
        public string HealthWorker { get; set; }

        [DocumentMemberReference(Column = "O", Position = 15, FieldName = "Duration", Format = "hh\\:mm")]
        public TimeSpan Duration { get; set; }

        [DocumentMemberReference(Column = "S", Position = 19, FieldName = "Activity")]
        public string Activity { get; set; }

        public string NameKey { get { return HealthWorker.Replace(" ", string.Empty).ToUpper(); } }
    }
}
