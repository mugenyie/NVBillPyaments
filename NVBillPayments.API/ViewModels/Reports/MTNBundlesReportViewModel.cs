using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.ViewModels.Reports
{
    public class MTNBundlesReportViewModel
    {
        public string RequestDateTimeEAT { get; set; }
        public string TransactionId { get; set; }
        public string HTTPResponseStatusCode { get; set; }
        public string ReponseBodyStatusCode { get; set; }
        public string RequestStatus { get; set; }
        public string SubscriptionId { get; set; }
        public string SubscriptionName { get; set; }
        public decimal BundlePrice { get; set; }
        public int AmountDeducted { get; set; }
        public string BundleValidity { get; set; }
        public string BeneficiaryMSISDN { get; set; }
        public string ActivationChannel { get; set; }
    }
}
