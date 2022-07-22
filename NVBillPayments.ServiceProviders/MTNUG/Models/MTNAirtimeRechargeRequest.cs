using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.ServiceProviders.MTNUG.Models
{
    public class MTNAirtimeRechargeRequest
    {
        public MTNAirtimeRechargeRequest()
        {
            Interface = "TOPUP";
            ProductCode = "MOBTOPUP";
        }

        public string Interface { get; set; }
        public string AgentCode { get; set; }
        public string AuthKey { get; set; }
        public bool Retry { get; set; }
        public string AgentTransNo { get; set; }
        public DateTime AgentTimeStamp { get; set; }
        public string Account { get; set; }
        public string ProductCode { get; set; }
        public decimal Value { get; set; }
        public string TerminalId { get; set; }
        public string Comments { get; set; }
    }
}
