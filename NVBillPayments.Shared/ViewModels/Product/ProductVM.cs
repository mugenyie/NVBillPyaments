using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Shared.ViewModels.Product
{
    public class ProductVM
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public decimal Price { get; set; }
        public string CurrencyCode { get; set; }
        public string Volume { get; set; }
        public string Frequency { get; set; }
    }
}
