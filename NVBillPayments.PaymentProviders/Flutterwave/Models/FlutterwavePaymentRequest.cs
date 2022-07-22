using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.PaymentProviders.Flutterwave.Models
{
    public class FlutterwavePaymentRequest
    {
        public string tx_ref { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string redirect_url { get; set; }
        public string payment_options { get; set; }
        public FlutterwavePaymentMeta meta { get; set; }
        public FlutterwaveCustomerRef customer { get; set; }
        public Customizations customizations { get; set; }
    }

    public class FlutterwavePaymentMeta
    {
        public string sponsor_user_id { get; set; }
        public string beneficiary_user_id { get; set; }
        public string product_description { get; set; }
    }

    public class FlutterwaveCustomerRef
    {
        public string email { get; set; }
        public string phonenumber { get; set; }
        public string name { get; set; }
    }

    public class Customizations
    {
        public Customizations()
        {
            logo = "https://newvision-media.s3.amazonaws.com/cms/e35c883a-3f68-4da6-8697-dedccceca027.png";
        }

        public string title { get; set; }
        public string description { get; set; }
        public string logo { get; set; }
    }
}
