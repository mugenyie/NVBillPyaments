using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.ServiceProviders.MTNUG.Models
{
    public class MTNAirtimeServerResponse
    {
        public int http_status_code { get; set; }
        public string http_status_code_desc { get; set; }
        public string response_status { get; set; }
        public string data { get; set; }
        public object error_msg { get; set; }
    }
}
