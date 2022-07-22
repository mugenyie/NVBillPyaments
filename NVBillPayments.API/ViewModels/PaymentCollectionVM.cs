using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.ViewModels
{
    public class PaymentCollectionVM
    {
        public string CustomerId { get; set; }
        public decimal Amount { get; set; }
    }
}
