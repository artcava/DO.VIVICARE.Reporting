using DO.VIVICARE.Reporting;
using System;

namespace DO.VIVICARE.Document.Report16
{
    [DocumentReference(FileName = "Report16.xlsx", Name = "Report16", Description = "Una descriione del File")]
    public class Report16 : BaseDocument
    {
        [DocumentMemberReference(Column = "E", Position = 4)]
        public string FiscalCode { get; set; }

        [DocumentMemberReference(Column = "K", Position = 10)]
        public string Town { get; set; }

        [DocumentMemberReference(Column = "T", Position = 19)]
        public string HostType { get; set; }

        [DocumentMemberReference(Column = "Q", Position = 16)]
        public DateTime ErogationDate { get; set; }

        [DocumentMemberReference(Column = "AA", Position = 26)]
        public string ArticleCode { get; set; }

        [DocumentMemberReference(Column = "AD", Position = 29)]
        public decimal Quantity { get; set; }
    }
}
