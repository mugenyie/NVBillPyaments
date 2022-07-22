using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Shared.ViewModels.Product
{
    public class ServiceProvidersProducts
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public List<_Category> Categories { get; set; }
    }

    public class _Category
    {
        public string Name { get; set; }
        public string MyProperty { get; set; }
        List<_SubCategory> SubCategories { get; set; }
    }

    public class _SubCategory
    {
        public List<SimpleProductVM> Products { get; set; }
    }
}
