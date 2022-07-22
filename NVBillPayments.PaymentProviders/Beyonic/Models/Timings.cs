using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.PaymentProviders.Beyonic.Models
{
    public class Timings
    {
        public int id { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public object processing_started { get; set; }
        public object instructions_sent { get; set; }
        public object collection_received { get; set; }
        public object collection_matched { get; set; }
        public object ipn_sent { get; set; }
        public int request { get; set; }
        public int author { get; set; }
        public int updated_by { get; set; }
    }
}
