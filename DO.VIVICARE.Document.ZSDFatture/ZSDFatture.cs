using DO.VIVICARE.Reporter;
using System;

namespace DO.VIVICARE.Document.ZSDFatture
{
    [DocumentReference(Name = "ZSDFatture", Description = "Fle delle fatture", RowStart =2)]
    public class ZSDFatture:BaseDocument
    {
        [DocumentMemberReference(Column = "A", Position = 1)]
        public string Customer { get; set; }

        [DocumentMemberReference(Column = "AA", Position = 27)]
        public DateTime ErogationDate { get; set; }

        [DocumentMemberReference(Column = "AM", Position = 39)]
        public decimal Price { get; set; }

        [DocumentMemberReference(Column = "AN", Position = 40)]
        public decimal Quantity { get; set; }

        [DocumentMemberReference(Column = "AS", Position = 45)]
        public string FiscalCode { get; set; }
    }
}
