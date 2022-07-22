using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Shared.ViewModels.Payment
{
    public class FlutterwaveCallbackResponse
    {
        public string @event { get; set; }
        public ResponseData data { get; set; }
        public string eventtype { get; set; }
    }

    public class ResponseDataCustomer
    {
        public int id { get; set; }
        public string name { get; set; }
        public string phone_number { get; set; }
        public string email { get; set; }
        public DateTime created_at { get; set; }
    }

    public class ResponseData
    {
        public int id { get; set; }
        public string tx_ref { get; set; }
        public string flw_ref { get; set; }
        public string device_fingerprint { get; set; }
        public int amount { get; set; }
        public string currency { get; set; }
        public int charged_amount { get; set; }
        public int app_fee { get; set; }
        public int merchant_fee { get; set; }
        public string processor_response { get; set; }
        public string auth_model { get; set; }
        public string ip { get; set; }
        public string narration { get; set; }
        public string status { get; set; }
        public string payment_type { get; set; }
        public DateTime created_at { get; set; }
        public int account_id { get; set; }
        public ResponseDataCustomer customer { get; set; }
    }
}
