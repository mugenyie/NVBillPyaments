using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.ServiceProviders.Quickteller.Models
{
    public class QuicktellerCategory
    {
        public string categoryid { get; set; }
        public string categoryname { get; set; }
        public string categorydescription { get; set; }
    }

    public class QuicktellerCategorys
    {
        public List<QuicktellerCategory> categorys { get; set; }
    }
}
