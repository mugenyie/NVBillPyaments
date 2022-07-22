using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.ViewModels
{
    public class PaymentAdviceRequestVM
    {
        public string TransactionReference { get; set; }
        public string PaymentMethod { get; set; } //card //momo
        public string SponsorId { get; set; }
    }
}
