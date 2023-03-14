using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Shared.ViewModels.Payment
{
    public class VPGCallback_V1
    {
        public string TransactionReference { get; set; }
        public string CustomerId { get; set; }
        public string ProductCode { get; set; }
        public object MetaData { get; set; }
        public string PaymentStatusCode { get; set; }
        public string Message { get; set; }
    }
}
