using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillpayments.WebUI.Models
{
    public class PostPaymentAdvice
    {
        public string transactionReference { get; set; }
        public string paymentMethod { get; set; }
        public string sponsorId { get; set; }
    }
}
