using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.ServiceProviders.MTNUG.Models
{
    public class MTNActivateBundleResponseFailure
    {
        public string status { get; set; }
        public string error { get; set; }
        public string message { get; set; }
        public string timestamp { get; set; }
        public string path { get; set; }
    }
}
