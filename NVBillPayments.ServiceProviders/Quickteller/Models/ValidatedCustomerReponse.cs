using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.ServiceProviders.Quickteller.Models
{

    public class ValidatedCustomerReponse
    {
        public string customerName { get; set; }
        public string responseCode { get; set; }
        public string biller { get; set; }
        public int excise { get; set; }
        public bool surchargeType { get; set; }
        public string transactionRef { get; set; }
        public string isAmountFixed { get; set; }
        public int surcharge { get; set; }
        public int paymentItemId { get; set; }
        public int amount { get; set; }
        public string alternateCustomerId { get; set; }
        public string shortTransactionRef { get; set; }
        public int balance { get; set; }
        public string paymentItem { get; set; }
        public string collectionsAccountNumber { get; set; }
        public string balanceNarration { get; set; }
        public string customerId { get; set; }
        public string narration { get; set; }
        public string collectionsAccountType { get; set; }
        public string balanceType { get; set; }
        public bool displayBalance { get; set; }
    }
}
