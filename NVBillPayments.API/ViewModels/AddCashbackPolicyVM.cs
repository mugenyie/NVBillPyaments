using NVBillPayments.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.ViewModels
{
    public class AddCashbackPolicyVM
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float Percentage { get; set; }
        public SystemCategory systemCategory { get; set; }
        public string PaymentMethod { get; set; }
    }
}
