using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillpayments.WebUI.Models
{
    public class TransactionDetailVM
    {
        public string transactionId { get; set; }
        public string accountMSISDN { get; set; }
        public string accountName { get; set; }
        public string accountEmail { get; set; }
        public string sponsorMSISDN { get; set; }
        public string beneficiaryMSISDN { get; set; }
        public string productDescription { get; set; }
        public string paymentProvider { get; set; }
        public string paymentStatus { get; set; }
        public string orderStatus { get; set; }
        public string transactionStatus { get; set; }
        public string createdOnUTC { get; set; }
        public int productValue { get; set; }
        public int amountCharged { get; set; }
        public bool isExpired { get; set; }
    }
}
