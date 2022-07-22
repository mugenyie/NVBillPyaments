using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.ServiceProviders.MTNUG.Models
{
    public class MTNBundlePrice
    {
        public string statusCode { get; set; }
        public string customerId { get; set; }
        public Data data { get; set; }
        public Links _links { get; set; }
    }

    public class Data
    {
        public string id { get; set; }
        public string name { get; set; }
        public string currency { get; set; }
        public int amount { get; set; }
        public object bundleCategory { get; set; }
        public string bundleType { get; set; }
        public string bundleValidity { get; set; }
    }

    public class Self
    {
        public string href { get; set; }
    }

    public class Links
    {
        public Self self { get; set; }
    }
}
