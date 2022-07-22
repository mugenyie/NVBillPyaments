using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.ViewModels.Callbacks
{
    public class InterswitchCB
    {
        public string customerName { get; set; }
        public string externalReference { get; set; }
        public string initiatorAccount { get; set; }
        public string merchantId { get; set; }
        public string orderId { get; set; }
        public string paymentItem { get; set; }
        public string provider { get; set; }
        public string status { get; set; }
        public string statusMessage { get; set; }
        public string token { get; set; }
        public string transactionAmount { get; set; }
        public string transactionRef { get; set; }
    }
}
