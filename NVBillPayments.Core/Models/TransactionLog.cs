using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NVBillPayments.Core.Models
{
    public class TransactionLog
    {
        [Key]
        public Guid LogId { get; set; }
        public string Title { get; set; }
        public string Metadata { get; set; }
        public DateTime CreatedOnUTC { get; set; }
    }
}
