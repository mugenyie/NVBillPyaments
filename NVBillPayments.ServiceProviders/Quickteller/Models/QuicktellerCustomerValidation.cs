using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.ServiceProviders.Quickteller.Models
{
    public class QuicktellerCustomerValidation
    {
        public string requestReference { get; set; }
        public string customerId { get; set; }
        public string bankCbnCode { get; set; }
        public int amount { get; set; }
        public string customerMobile { get; set; }
        public string terminalId { get; set; }
        public string customerEmail { get; set; }
        public string paymentCode { get; set; }
    }
}
