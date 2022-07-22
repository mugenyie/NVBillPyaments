using Newtonsoft.Json;
using NVBillpayments.WebUI.Models;
using NVBillPayments.Core.Interfaces;
using NVBillPayments.Shared.ViewModels.Product;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NVBillpayments.WebUI.Data
{
    public class BillPaymentsService : IBillPaymentsService
    {
        public RestClient _restClient;
        public HttpClient _client;
        private readonly ICachingService _cachingService;

        public BillPaymentsService(HttpClient client, ICachingService cachingService)
        {
            _client = client;
            _restClient = new RestClient("https://transactions-api-production.newvisionapp.com");
            _cachingService = cachingService;
        }

        public async Task<QuickTellerSimpleVM> FetchCategoriesAsync()
        {
            string key = $"cached_transaction_categories-";
            var cacheData = await _cachingService.Get<QuickTellerSimpleVM>(key);
            if (cacheData == null)
            {
                HttpResponseMessage response = await _client.GetAsync("/v2/Categories");
                response.EnsureSuccessStatusCode();
                var results = JsonConvert.DeserializeObject<QuickTellerSimpleVM>(await response.Content.ReadAsStringAsync());

                if (results.count > 0)
                    await _cachingService.Set(key, results, 1800);
                return results;
            }
            else
            {
                return cacheData;
            }
        }

        public async Task<QuicktellerBillerVM> FetchBillerDetails(string billerId)
        {
            string key = $"cached_biller_detail_{billerId}-";
            var cacheData = await _cachingService.Get<QuicktellerBillerVM>(key);
            if (cacheData == null)
            {
                HttpResponseMessage response = await _client.GetAsync($"/v2/Billers?billerId={billerId}");
                response.EnsureSuccessStatusCode();
                var results = JsonConvert.DeserializeObject<QuicktellerBillerVM>(await response.Content.ReadAsStringAsync());

                if (results != null)
                    await _cachingService.Set(key, results, 1800);
                return results;
            }
            else
            {
                return cacheData;
            }
        }

        public async Task<QuicktellerPaymentItemVM> FetchProductInfo(string productCode)
        {

            string key = $"cached_product_detail_{productCode}-";
            var cacheData = await _cachingService.Get<QuicktellerPaymentItemVM>(key);
            if (cacheData == null)
            {
                HttpResponseMessage response = await _client.GetAsync($"/v2/Product?productCode={productCode}");
                response.EnsureSuccessStatusCode();
                var results = JsonConvert.DeserializeObject<QuicktellerPaymentItemVM>(await response.Content.ReadAsStringAsync());

                if (results != null)
                    await _cachingService.Set(key, results, 1800);
                return results;
            }
            else
            {
                return cacheData;
            }
        }

        public async Task<PaymentAdviceReponse> PostPaymentAdvice(PostPaymentAdvice paymentAdvice)
        {
            var restRequest = new RestRequest("/v2/paymentadvice", Method.POST);
            restRequest.AddJsonBody(paymentAdvice);

            var response = await _restClient.ExecuteAsync<PaymentAdviceReponse>(restRequest);

            return response.Data;
        }

        public async Task<CustomerValidationResponse> ValidateCustomerAsync(ValidateCustomer validateCustomer)
        {
            var restRequest = new RestRequest("/v2/validatecustomer", Method.POST);
            restRequest.AddJsonBody(validateCustomer);

            var response = await _restClient.ExecuteAsync<CustomerValidationResponse>(restRequest);

            return response.Data;
        }
    }
}
