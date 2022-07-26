using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.ViewModels
{
    public class ValidatedCustomerReponseVM
    {
        public string customerName { get; set; }
        public string paymentItem { get; set; }
        public string customerId { get; set; }
        public int amount { get; set; }
        public int quantity { get; set; }
        public int surcharge { get; set; }
        public int excise { get; set; }
        public int totalAmount { get; set; }
        public string transactionRef { get; set; }
    }
}
