using NVBillPayments.Shared.ViewModels.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillpayments.WebUI.Models
{
    public class UserSelect
    {
        public string CustomerField { get; set; }
        public string PaymentItemSelection { get; set; }
        public QuicktellerPaymentItemVM ProductData { get; set; }
        public int Amount { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
