using NVBillPayments.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Shared.ViewModels.Product
{
    public class AddProductVM
    {
        public AddProductVM()
        {
            CurrencyCode = "UGX";
        }

        public string ProductId { get; set; }
        public string ProductId_2 { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Grouping { get; set; }
        public string CategoryId { get; set; }
        public string ServiceProviderId { get; set; }
        public decimal Price { get; set; }
        public string CurrencyCode { get; set; }
        public string Volume { get; set; }
        public string Validity { get; set; }
        public SystemCategory SystemCategory { get; set; }
    }
}
