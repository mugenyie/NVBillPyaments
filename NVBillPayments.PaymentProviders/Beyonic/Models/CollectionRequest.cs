using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.PaymentProviders.Beyonic.Models
{
    public class CollectionRequest
    {
        public CollectionRequest()
        {
            currency = "UGX";
            expiry_date = "5 minutes";
        }

        public string phonenumber { get; set; }
        public string currency { get; set; }
        public string amount { get; set; }
        public Metadata metadata { get; set; }
        public string success_message { get; set; }
        public bool send_instructions { get; set; }
        public string expiry_date { get; set; }
    }
}
