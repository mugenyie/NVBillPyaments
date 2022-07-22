using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NVBillPayments.API.Attributes;
using NVBillPayments.API.Helpers;
using NVBillPayments.Core.Interfaces;
using NVBillPayments.Shared;
using NVBillPayments.Shared.ViewModels.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.Controllers
{
    [Route("V1/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ICachingService _cachingService;

        public ProductsController(IProductService productService, ICachingService cachingService)
        {
            _productService = productService;
            _cachingService = cachingService;
        }

        [HttpGet]
        [Route("Category")]
        public async Task<IActionResult> GetProductCategoriesAsync(string email)
        {
            string key = $"products/category?email={email}";
            var cacheData = await _cachingService.Get<List<CategoryVM>>(key);
            if (cacheData == null)
            {
                var results = _productService.GetProductCategories(email);
                await _cachingService.Set(key, results, 1800);
                return Ok(results);
            }
            else
            {
                return Ok(cacheData);
            }
        }

        [HttpGet]
        [Route("Category/{categoryId}")]
        public async Task<IActionResult> GetServiceProvidersAsync(string categoryId)
        {
            string key = $"products/category/{categoryId}___";
            var cacheData = await _cachingService.Get<List<ServiceProviderExpandedVM>>(key);
            if (cacheData == null)
            {
                var results = _productService.GetServiceProvidersByCategory(categoryId);
                await _cachingService.Set(key, results, 1800);
                return Ok(results);
            }
            else
            {
                return Ok(cacheData);
            }
        }

        [HttpPost]
        [Route("Category")]
        public IActionResult AddCategory(AddProductCategoryVM categoryVM)
        {
            var result = _productService.AddProductCategory(categoryVM);
            return Ok(result);
        }

        [HttpPost]
        [Route("ServiceProvider")]
        public IActionResult AddServiceProvider(AddServiceProviderVM AddServiceProviderViewModel)
        {
            var result = _productService.AddServiceProvider(AddServiceProviderViewModel);
            return Ok(result);
        }

        [HttpPost]
        public IActionResult AddProduct(AddProductVM AddProductViewModel)
        {
            var result = _productService.AddProduct(AddProductViewModel);
            return Ok(result);
        }

        [HttpGet]
        [Route("Info")]
        public async Task<IActionResult> GetProductDetailAsync(string productId, string beneficiaryMSIDN)
        {
            string customer_product_trackingId = $"product_info_{productId}";
            var cacheData = await _cachingService.Get<ProductDetailVM>(customer_product_trackingId);
            if (cacheData != null)
                return Ok(cacheData);
            else
            {
                //bool production = false;
                //if (ConfigurationConstants.ENVIRONMENT.Equals("PRODUCTION"))
                bool production = true;
                string beneficiary = beneficiaryMSIDN.ValidatePhoneNumber();
                try
                {
                    var result = _productService.GetThirdPartyProductDetail(productId, beneficiary,customer_product_trackingId, production);
                    if(!string.IsNullOrEmpty(result.ErrorMessage))
                        await _cachingService.Set(customer_product_trackingId, result, 3600 * 3);
                    return Ok(result);
                }
                catch (Exception)
                {
                    var productDetail = new ProductDetailVM()
                    {
                        Errored = true,
                        ErrorMessage = "Error Querying Product Information",
                        ProductId = productId
                    };
                    return Ok(productDetail);
                }
            }
        }
    }
}
