using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Services.Models
{
    public class EmailNotification
    {
        public Request request { get; set; }
        public Params @params { get; set; }
        public string to { get; set; }
        public string type { get; set; }
    }

    public class Request
    {
        public string type { get; set; }
        public string send_format { get; set; }
    }

    public class Params
    {
        public string BILLPAYMENT_TITLE { get; set; }
        public string NAME { get; set; }
        public string BILLPAYMENT_DESCRIPTION { get; set; }
    }
}
