using DO.VIVICARE.Reporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DO.VIVICARE.Document.ZSDFatture
{
    [DocumentReference(FileName = "ZSDFatture.xlsx", Name = "ZSDFatture", Description = "Fle delle fatture")]
    public class ZSDFatture:BaseDocument
    {
        [DocumentMemberReference(Column = "A", Position = 0)]
        public string Customer { get; set; }

        [DocumentMemberReference(Column = "AS", Position = 44)]
        public string FiscalCode { get; set; }

        [DocumentMemberReference(Column = "AA", Position = 26)]
        public DateTime ErogationDate { get; set; }
    }
}
