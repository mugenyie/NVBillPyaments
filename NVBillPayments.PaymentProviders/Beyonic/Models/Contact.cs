using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.PaymentProviders.Beyonic.Models
{
    public class Contact
    {
        public int id { get; set; }
        public int organization { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public object email { get; set; }
        public string phone_number { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public Metadata metadata { get; set; }
        public string phone_is_supported { get; set; }
        public string phone_is_mm_registered { get; set; }
        public string name_on_network { get; set; }
        public string name_matches_network_status { get; set; }
        public double name_matches_network_score { get; set; }
        public string network_name { get; set; }
        public DateTime created { get; set; }
        public int author { get; set; }
        public DateTime modified { get; set; }
        public int updated_by { get; set; }
        public object national_id { get; set; }
        public string national_id_type { get; set; }
    }
}
