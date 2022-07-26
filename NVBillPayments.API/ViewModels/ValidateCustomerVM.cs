using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.ViewModels
{
    public class ValidateCustomerVM
    {
        public string productCode { get; set; }
        public string customerfield1 { get; set; }
        public string amount { get; set; }
        public int quantity { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
    }
}
