using Microsoft.EntityFrameworkCore;
using NVBillPayments.Core;
using NVBillPayments.Core.Enums;
using NVBillPayments.Core.Interfaces;
using NVBillPayments.Core.Models;
using NVBillPayments.ServiceProviders.MTNUG;
using NVBillPayments.Services.Helpers;
using NVBillPayments.Shared.Extensions;
using NVBillPayments.Shared.ViewModels.Product;
using NVBillPayments.Shared.ViewModels.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Transaction> _transactionsRepository;
        private readonly IRepository<Product> _productsRepository;
        private readonly IRepository<ServiceProvider> _serviceProviderRepository;
        private readonly IRepository<Category> _productCategoriesRepository;
        private readonly IMTNService _mtnService;
        private readonly ICachingService _cachingService;

        public ProductService(ICachingService cachingService, IRepository<Transaction> transactionsRepository, IRepository<Product> productsRepository, IRepository<ServiceProvider> serviceProviderRepository, IRepository<Category> productCategoriesRepository, IMTNService mtnService)
        {
            _transactionsRepository = transactionsRepository;
            _productsRepository = productsRepository;
            _serviceProviderRepository = serviceProviderRepository;
            _productCategoriesRepository = productCategoriesRepository;
            _mtnService = mtnService;
            _cachingService = cachingService;
        }

        public Product AddProduct(AddProductVM pd)
        {
            string productDescription = pd.Description ?? $"{pd.Validity} {pd.Volume} for {Math.Round(pd.Price,0)}/-";
            var product = new Product
            {
                ProductId = pd.ProductId,
                ProductId_2 = pd.ProductId_2,
                Name = pd.Name ?? productDescription,
                Description = productDescription,
                CategoryId = new Guid(pd.CategoryId),
                ServiceProviderId = new Guid(pd.ServiceProviderId),
                Price = pd.Price,
                CurrencyCode = pd.CurrencyCode,
                Grouping = pd.Grouping,
                Volume = pd.Volume,
                IsActive = true,
                Validity = pd.Validity,
                SystemCategory = pd.SystemCategory,
                CreatedOnUTC = DateTime.UtcNow
            };

            _productsRepository.Add(product);
            _productsRepository.SaveChanges();
            return product;
        }

        public Category AddProductCategory(AddProductCategoryVM productCategory)
        {
            var productCat = new Category
            {
                Name = productCategory.Name.ToUpper(),
                IconUrl = productCategory.IconUrl,
                IsActive = true,
                CreatedOnUTC = DateTime.UtcNow
            };
            _productCategoriesRepository.Add(productCat);
            _productCategoriesRepository.SaveChanges();
            return productCat;
        }

        public ServiceProvider AddServiceProvider(AddServiceProviderVM sp)
        {
            var serviceProvider = new ServiceProvider
            {
                CompanyName = sp.CompanyName,
                ShortName = sp.ShortName.ToUpper(),
                LogoUrl = sp.LogoUrl,
                IsActive = true,
                CreatedOnUTC = DateTime.UtcNow
            };
            _serviceProviderRepository.Add(serviceProvider);
            _serviceProviderRepository.SaveChanges();
            return serviceProvider;
        }

        public List<CategoryVM> GetProductCategories(string email)
        {
            List<CategoryVM> categoryVMs = new List<CategoryVM>();
            var categories = _productCategoriesRepository.Query().Where(x => x.IsActive).ToList();
            categories.ForEach(c => categoryVMs.Add(new CategoryVM
            {
                Id = c.Id.ToString(),
                Name = c.Name,
                IconUrl = c.IconUrl,
                //RecentPurchases = GetRecentPurchasesAsync(email, c.Name).Result
            }));
            return categoryVMs;
        }

        public object GetServiceProviders(bool displayProducts)
        {
            /*List<ServiceProvidersProducts> serviceProvidersProducts = new List<ServiceProvidersProducts>();
            List<_Category> _Categories = new List<_Category>();
            List<_SubCategory> _SubCategories = new List<_SubCategory>();

            var products = _productsRepository.Query().Where(x => x.IsActive)
                .Include(x => x.Category)
                .Include(x => x.ServiceProvider);

            var serviceProviders = products.Select(x => x.ServiceProvider).Distinct().ToList();
            serviceProviders.ForEach(s =>
            {
                var category = products.Where(x => x.ServiceProvider.Id == s.Id).Select(x => x.Category);
                var subCategory = products.Select(x => x.Frequency).Distinct();

                serviceProvidersProducts.Add(new ServiceProvidersProducts
                {
                    Category = new List<_Category>() { }
                });
            });*/
            throw new NotImplementedException();
        }

        private async Task<List<SimpleTransactionsVM>> GetRecentPurchasesAsync(string email, string systemCategory)
        {
            string key = $"recentpurchases?category={systemCategory}?email={email}";
            var cacheData = await _cachingService.Get<List<SimpleTransactionsVM>>(key);
            if(cacheData == null)
            {
                List<SimpleTransactionsVM> recentPurchases = new List<SimpleTransactionsVM>();

                if (!string.IsNullOrEmpty(email))
                {
                    var purchases = _transactionsRepository.Query()
                        .Where(x => x.AccountEmail.ToLower().Equals(email.ToLower()))
                        .Where(x => x.SystemCategory.Equals(systemCategory))
                        .Where(x => x.TransactionStatus.Equals(TransactionStatus.SUCCESSFUL))
                        .OrderByDescending(x => x.CreatedOnUTC)
                        .Take(10)
                        .ToList();

                    recentPurchases = TransactionHelper.ToSimpleListView(purchases);
                }
                await _cachingService.Set(key, recentPurchases, 600);
                return recentPurchases;
            }
            else
            {
                return cacheData;
            }
        }

        public List<ServiceProviderExpandedVM> GetServiceProvidersByCategory(string CategoryId, bool displayProducts = true, string email = "")
        {
            if (CategoryId.Equals("9bfa5d82-ca49-4229-cb07-08d90b45ff6a",StringComparison.OrdinalIgnoreCase))
            {
                var grouping = new List<GroupingVM>
                {
                    new GroupingVM
                    {
                        Name = "Airtime",
                        Products = new List<SimpleProductVM>
                        {
                            new SimpleProductVM
                            {
                                Name = "TopUp Now",
                                ProductId = "AIRTIME",
                                UserInputAmount = true
                            }
                        }
                    }
                };

                var serviceProvider = new ServiceProviderExpandedVM
                {
                    CompanyName = "MTN, Airtel",
                    LogoUrl = "https://newvision-media.s3.amazonaws.com/cms/34b8159b-86f8-4374-8416-5d88f2bfb935.jpg",
                    SampleInput = "0781234567",
                    Groupings = grouping
                };

                return new List<ServiceProviderExpandedVM>
                {
                    serviceProvider
                };
            }

            List<ServiceProviderExpandedVM> serviceProvidersVM = new List<ServiceProviderExpandedVM>();

            var serviceProvidersProducts = _productsRepository.Query()
                .Where(x => x.CategoryId.ToString().Equals(CategoryId))
                .Where(x => x.IsActive)
                .Include(x => x.Category)
                .Include(x => x.ServiceProvider);

            var serviceProviders = serviceProvidersProducts.Select(x => x.ServiceProvider).Distinct().ToList();
            serviceProviders.ForEach(sp =>
            {
                List<GroupingVM> groupings = new List<GroupingVM>();
                
                List<string> GroupingNames = serviceProvidersProducts
                .Where(x => x.IsActive)
                .Where(x => x.ServiceProvider == sp)
                .Select(x => x.Grouping).Distinct().ToList();

                GroupingNames.ForEach(grp =>
                {
                    List<SimpleProductVM> products = new List<SimpleProductVM>();
                    var groupProducts = serviceProvidersProducts
                    .Where(x => x.IsActive)
                    .Where(x => x.Grouping.Equals(grp))
                    .Where(x => x.ServiceProvider.Equals(sp))
                    .OrderBy(x => x.Name)
                    .ToList();

                    groupProducts.ForEach(pd =>
                    {
                        products.Add(new SimpleProductVM
                        {
                            ProductId = pd.ProductId,
                            Name = pd.Name,
                            UserInputAmount = pd.UserInputAmount,
                            IsActive = pd.IsActive
                        });
                    });
                    groupings.Add(new GroupingVM
                    {
                        Name = grp,
                        Products = products
                    });
                });

                serviceProvidersVM.Add(new ServiceProviderExpandedVM
                {
                    CompanyName = sp.CompanyName,
                    LogoUrl = sp.LogoUrl,
                    SampleInput = sp.SampleInput,
                    Groupings = groupings
                });
            });
            return serviceProvidersVM;
        }

        public List<object> GetProductsByServiceProvider(string ServiceProviderId)
        {
            var products = _productsRepository.Query().Where(x => x.ServiceProvider.ToString().Equals(ServiceProviderId)).ToList();
            return null;
        }

        public List<object> GetProductsByCategory(string CategoryId)
        {
            throw new NotImplementedException();
        }

        public Product Get(string productId)
        {
            var product = _productsRepository.Query()
                .Where(x => x.ProductId.Equals(productId))
                .Include(x => x.ServiceProvider)
                .Include(x => x.Category)
                .FirstOrDefault();
            return product;
        }

        public ProductDetailVM GetThirdPartyProductDetail(string productId, string beneficiaryMSIDN, string transactionId, bool production = false)
        {
            ProductDetailVM productDetail = new ProductDetailVM();
            var product = _productsRepository.Query().Where(x => x.ProductId == productId)
                .Include(x => x.ServiceProvider)
                .FirstOrDefault();

            if ((bool)(product?.ServiceProvider?.ShortName.Contains("mtn", StringComparison.OrdinalIgnoreCase)))
            {
                try
                {
                    if (ProductHelper.IsValidSubscriber(beneficiaryMSIDN, "mtn"))
                    {
                        var productInfo = _mtnService.GetBundlePrice(beneficiaryMSIDN, productId, transactionId, production);
                        productDetail.Name = productInfo.data.name;
                        productDetail.Description = productInfo.data.name;
                        productDetail.Amount = productInfo.data.amount;
                        productDetail.ProductId = productInfo.data.id;
                    }
                    else
                    {
                        productDetail.Name = product.Name;
                        productDetail.Description = "";
                        productDetail.Amount = 0;
                        productDetail.ProductId = productId;
                        productDetail.Errored = true;
                        productDetail.ErrorMessage = $"Invalid Network Subsriber {beneficiaryMSIDN}";
                    }
                }
                catch (Exception exp)
                {
                    productDetail.Name = product.Name;
                    productDetail.Description = "";
                    productDetail.Amount = 0;
                    productDetail.ProductId = productId;
                    productDetail.Errored = true;
                    productDetail.ErrorMessage = "Error Querying Product Information, Try Again Later";
                }
            }
            else
            {
                productDetail.Name = product.Name;
                productDetail.Description = product.Description;
                productDetail.Amount = product.Price;
                productDetail.ProductId = productId;
            }

            return productDetail;
        }
    }
}
