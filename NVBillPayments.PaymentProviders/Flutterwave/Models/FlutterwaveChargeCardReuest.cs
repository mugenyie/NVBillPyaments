using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.PaymentProviders.Flutterwave.Models
{
    public class FlutterwaveChargeCardReuest
    {
        internal string transactionId;

        public string amount { get; internal set; }
        public string currency { get; internal set; }
        public string beneficiary_id { get; internal set; }
        public string sponsor_id { get; internal set; }
        public string productDescription { get; internal set; }
        public string user_email { get; internal set; }
        public string user_name { get; internal set; }
        public string phone_number { get; internal set; }
    }
}
