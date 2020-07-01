using DO.VIVICARE.Reporter;
using System;

namespace DO.VIVICARE.Document.Report16
{    [DocumentReference(Name = "Report16", Description = "Lista degli articoli somministrati ai pazienti", RowStart =3)]
    public class Report16 : BaseDocument
    {
        [DocumentMemberReference(Column = "E", Position = 5)]
        public string FiscalCode { get; set; }

        [DocumentMemberReference(Column = "K", Position = 11)]
        public string Town { get; set; }

        //[DocumentMemberReference(Column = "R", Position = 18)]
        //public decimal Price { get; set; }

        [DocumentMemberReference(Column = "T", Position = 20)]
        public string HostType { get; set; }

        [DocumentMemberReference(Column = "Q", Position = 17)]
        public DateTime ErogationDate { get; set; }

        [DocumentMemberReference(Column = "X", Position = 24)]
        public string ERPCode { get; set; }

        [DocumentMemberReference(Column = "AA", Position = 27)]
        public string ArticleCode { get; set; }

        [DocumentMemberReference(Column = "AD", Position = 30)]
        public decimal Quantity { get; set; }
    }
}
