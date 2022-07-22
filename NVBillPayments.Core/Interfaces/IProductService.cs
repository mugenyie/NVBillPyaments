using NVBillPayments.Core.Models;
using NVBillPayments.Shared.ViewModels.Product;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.Core.Interfaces
{
    public interface IProductService
    {
        List<CategoryVM> GetProductCategories(string email);
        List<object> GetProductsByCategory(string CategoryId);
        List<object> GetProductsByServiceProvider(string ServiceProviderId);
        List<ServiceProviderExpandedVM> GetServiceProvidersByCategory(string CategoryId, bool displayProducts = true, string email = "");
        Category AddProductCategory(AddProductCategoryVM productCategory);
        ServiceProvider AddServiceProvider(AddServiceProviderVM serviceProvider);
        Product AddProduct(AddProductVM productObject);
        Product Get(string productId);
        ProductDetailVM GetThirdPartyProductDetail(string productId, string beneficiaryMSIDN, string transactionId, bool production = false);
        object GetServiceProviders(bool displayProducts);
    }
}
