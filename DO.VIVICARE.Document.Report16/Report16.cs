using DO.VIVICARE.Reporter;
using System;

namespace DO.VIVICARE.Document.Report16
{    [DocumentReference(Name = "Report16", Description = "Lista degli articoli somministrati ai pazienti", RowStart =3)]
    public class Report16 : BaseDocument
    {
        [DocumentMemberReference(Column = "E", Position = 5, FieldName = "FiscalCode")]
        public string FiscalCode { get; set; }

        [DocumentMemberReference(Column = "H", Position = 8, FieldName = "DateOfBirth")]
        public DateTime DateOfBirth { get; set; }

        [DocumentMemberReference(Column = "K", Position = 11, FieldName = "Town")]
        public string Town { get; set; }

        [DocumentMemberReference(Column = "N", Position = 14, FieldName = "ContractId")]
        public int ContractId { get; set; }

        [DocumentMemberReference(Column = "T", Position = 20, FieldName = "HostType")]
        public string HostType { get; set; }

        [DocumentMemberReference(Column = "Q", Position = 17, FieldName = "ErogationDate")]
        public DateTime ErogationDate { get; set; }

        [DocumentMemberReference(Column = "X", Position = 24, FieldName = "ERPCode")]
        public string ERPCode { get; set; }

        [DocumentMemberReference(Column = "AA", Position = 27, FieldName = "ArticleCode")]
        public string ArticleCode { get; set; }

        [DocumentMemberReference(Column = "AB", Position = 28, FieldName = "ArticleDescription")]
        public string ArticleDescription { get; set; }

        [DocumentMemberReference(Column = "AD", Position = 30, FieldName = "Quantity")]
        public decimal Quantity { get; set; }

        //colonna AM position 39 Famiglia fitrare FamilyCode per F0103, F0157, F0158, F0159
        //FamilyCode
        [DocumentMemberReference(Column = "AM", Position = 39, FieldName = "FamilyCode")]
        public string FamilyCode { get; set; }
    }
}
