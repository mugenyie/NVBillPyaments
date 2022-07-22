using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Shared.ViewModels.Product
{
    public class SimpleProductVM
    {
        public string ProductId { get; set; }
        public bool UserInputAmount { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
