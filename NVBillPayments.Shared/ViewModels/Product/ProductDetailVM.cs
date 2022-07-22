using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Shared.ViewModels.Product
{
    public class ProductDetailVM
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public bool Errored { get; set; }
        public string ErrorMessage { get; set; }
    }
}
