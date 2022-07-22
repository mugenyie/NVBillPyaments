using NVBillPayments.Shared.ViewModels.Transaction;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Shared.ViewModels.Product
{
    public class ServiceProviderExpandedVM
    {
        public string CompanyName { get; set; }
        public string LogoUrl { get; set; }
        public List<GroupingVM> Groupings { get; set; }
        public string SampleInput { get; set; }
    }

    public class GroupingVM
    {
        public string Name { get; set; }
        public List<SimpleProductVM> Products { get; set; }
    }
}
