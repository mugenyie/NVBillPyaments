using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.PaymentProviders.Flutterwave.Models
{
    public class FlutterwavePaymentResponseData
    {
        public string link { get; set; }
    }

    public class FlutterwavePaymentResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public FlutterwavePaymentResponseData data { get; set; }
    }
}
