using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.ServiceProviders.MTNUG.Models
{
    public class MTNActivateBundle
    {
        public string subscriptionId { get; set; }
        public string subscriptionProviderId { get; set; }
        public string subscriptionName { get; set; }
        public string registrationChannel { get; set; }
        public string subscriptionPaymentSource { get; set; }
        public bool sendSMSNotification { get; set; }
        public string beneficiaryId { get; set; }
    }
}
