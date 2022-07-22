using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.PaymentProviders.Beyonic.Models
{
    public class CollectionResponse
    {
        public int id { get; set; }
        public int organization { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string phonenumber { get; set; }
        public Contact contact { get; set; }
        public string reason { get; set; }
        public Metadata metadata { get; set; }
        public DateTime created { get; set; }
        public int author { get; set; }
        public DateTime modified { get; set; }
        public int updated_by { get; set; }
        public object collection { get; set; }
        public object account { get; set; }
        public string success_message { get; set; }
        public object instructions { get; set; }
        public bool send_instructions { get; set; }
        public bool instructions_sent { get; set; }
        public object subscription_settings { get; set; }
        public string status { get; set; }
        public string start_date { get; set; }
        public string error_message { get; set; }
        public string error_details { get; set; }
        public string expiry_date { get; set; }
        public int max_attempts { get; set; }
        public int retry_interval_minutes { get; set; }
        public int attempts { get; set; }
        public Timings timings { get; set; }
    }
}
