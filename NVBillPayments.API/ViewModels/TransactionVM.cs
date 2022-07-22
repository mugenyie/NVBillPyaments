using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.ViewModels
{
    public class TransactionVM
    {
        public string TransactionId { get; set; }
        public string AccountMSISDN { get; set; }
        public string AccountName { get; set; }
        public string AccountEmail { get; set; }
        public string SponsorMSISDN { get; set; }
        public string BeneficiaryMSISDN { get; set; }
        public string ProductDescription { get; set; }
        public string PaymentProvider { get; set; }
        public string PaymentStatus { get; set; }
        public string OrderStatus { get; set; }
        public string TransactionStatus { get; set; }
        public string CreatedOnUTC { get; set; }
        public decimal ProductValue { get; set; }
        public decimal AmountCharged { get; set; }
    }
}
