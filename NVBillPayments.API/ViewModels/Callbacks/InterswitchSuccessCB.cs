using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.ViewModels.Callbacks
{
    public class InterswitchSuccessCB
    {
        public string fee { get; set; }
        public string transactionAmount { get; set; }
        public string transactionRef { get; set; }
        public string responseCode { get; set; }
        public string responseMessage { get; set; }
        public string terminalId { get; set; }
        public string approvalCode { get; set; }
        public string expiry { get; set; }
        public string panLast4Digits { get; set; }
        public string panFirst6Digits { get; set; }
        public string token { get; set; }
        public string orderId { get; set; }
        public string csCavv { get; set; }
        public string csCavvAlgorithm { get; set; }
        public string csCommerceIndicator { get; set; }
        public string csEciRaw { get; set; }
        public string csXid { get; set; }
        public string csParesStatus { get; set; }
        public string provider { get; set; }
    }
}
