using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.ServiceProviders.Quickteller.Models
{
    public class ValidateCustomerRequest
    {
        public string ProductCode { get; set; }
        public string CustomerId { get; set; }
        public int Amount { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string RequestReference { get; set; }
    }
}
