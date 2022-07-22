using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillpayments.WebUI.Models
{
    public class ValidateCustomer
    {
        public string productCode { get; set; }
        public string customerfield1 { get; set; }
        public int amount { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
    }
}
