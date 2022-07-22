using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.PaymentProviders.DPO.Models
{
    public class CreateTokenBody
    {
        public CreateTokenBody()
        {
            Currency = "UGX";
        }
        public decimal ChargeAmount { get; set; }
        public string Currency { get; set; }
        public string TransactionId { get; set; }
        public string ServiceDescription { get; set; }
        public DateTime DateTimeCreated { get; set; }
    }
}
