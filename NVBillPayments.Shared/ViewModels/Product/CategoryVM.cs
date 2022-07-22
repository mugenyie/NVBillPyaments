using NVBillPayments.Shared.ViewModels.Transaction;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Shared.ViewModels.Product
{
    public class CategoryVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public List<SimpleTransactionsVM> RecentPurchases { get; set; }
    }
}
