using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.PaymentProviders.Beyonic.Models.Callback
{
    public class BeyonicPaymentCallback
    {
        public Hook hook { get; set; }
        public int @event { get; set; }
        public Data data { get; set; }
    }

    public class Hook
    {
        public int id { get; set; }
        public string @event { get; set; }
        public string target { get; set; }
    }

    public class Metadata
    {
        public string nv_transactionId { get; set; }
    }

    public class Data
    {
        public string amount { get; set; }
        public Metadata metadata { get; set; }
        public string status { get; set; }
        public CollectionRequestCallback collection_request { get; set; }
    }

    public class CollectionRequestCallback
    {
        public Metadata metadata { get; set; }
    }
}
