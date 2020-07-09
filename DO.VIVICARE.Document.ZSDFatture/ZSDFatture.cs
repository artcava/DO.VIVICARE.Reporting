using DO.VIVICARE.Reporter;
using System;

namespace DO.VIVICARE.Document.ZSDFatture
{
    [DocumentReference(Name = "ZSDFatture", Description = "File delle fatture", RowStart =2)]
    public class ZSDFatture:BaseDocument
    {
        [DocumentMemberReference(Column = "A", Position = 1, FieldName = "Customer")]
        public string Customer { get; set; }

        [DocumentMemberReference(Column = "AA", Position = 27, FieldName = "ErogationDate")]
        public DateTime ErogationDate { get; set; }

        [DocumentMemberReference(Column = "AL", Position = 38, FieldName = "VAT")]
        public string VAT { get; set; }

        [DocumentMemberReference(Column = "AM", Position = 39, FieldName = "Price")]
        public decimal Price { get; set; }

        [DocumentMemberReference(Column = "AN", Position = 40, FieldName = "Quantity")]
        public decimal Quantity { get; set; }

        [DocumentMemberReference(Column = "AS", Position = 45, FieldName = "FiscalCode")]
        public string FiscalCode { get; set; }
    }
}
