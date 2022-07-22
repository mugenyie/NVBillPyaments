using Newtonsoft.Json;
using NVBillPayments.ServiceProviders.MTNUG.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NVBillPayments.ServiceProviders.MTNUG
{
    public class MTNService : IMTNService
    {
        private readonly string BaseURL_Development = "https://staging-uganda.api.mtn.com";
        private readonly string BaseURL_Production = "https://uganda.api.mtn.com";
        private readonly string AirtmeRechargeURL = "http://mtntopup.newvisionapp.com";
        private const string PRODUCTION_API_KEY = "TZ9o6TLIMQxzFiGHrvAGKd7sHOoJmMgZ";
        private const string TEST_API_KEY = "BryN7CK7aFnJonHxiXfV7TAG3ESG2iZK";
        private IRestClient _restClientDataBundles_Development;
        private IRestClient _restClientDataBundles_Production;
        private IRestClient _restClientAirtime;
        private IRestRequest _restRequest;

        public MTNService()
        {
            _restClientDataBundles_Development = new RestClient(BaseURL_Development);
            _restClientDataBundles_Production = new RestClient(BaseURL_Production);
            _restClientAirtime = new RestClient(AirtmeRechargeURL);
        }

        public async Task<IRestResponse> ActivateBundleAsync(string sponsorMSISDN, string transactionId, MTNActivateBundle MTNActivateBundle, bool production = false)
        {
            IRestResponse response;
            if (production)
            {
                _restRequest = new RestRequest($"v2/customers/{sponsorMSISDN}/subscriptions", Method.POST);
                _restRequest.AddJsonBody(MTNActivateBundle);
                _restRequest.AddHeader("transactionId", transactionId);
                _restRequest.AddHeader("x-api-key", PRODUCTION_API_KEY);
                response = await _restClientDataBundles_Production.ExecuteAsync(_restRequest);
            }
            else
            {
                _restRequest = new RestRequest($"v2/customers/{sponsorMSISDN}/subscriptions", Method.POST);
                _restRequest.AddJsonBody(MTNActivateBundle);
                _restRequest.AddHeader("transactionId", transactionId);
                _restRequest.AddHeader("x-api-key", TEST_API_KEY);
                response = await _restClientDataBundles_Development.ExecuteAsync(_restRequest);
            }
            return response;
        }

        public MTNBundlePrice GetBundlePrice(string beneficiaryId, string subscriptionId, string transactionId, bool production = false)
        {
            IRestResponse<MTNBundlePrice> response;
            transactionId = Regex.Replace(transactionId, @"[^a-zA-Z0-9 -]", string.Empty);
            if (production)
            {
                _restRequest = new RestRequest($"v3/products/{beneficiaryId}/{subscriptionId}?transactionId={transactionId}", Method.GET);
                _restRequest.AddHeader("x-api-key", PRODUCTION_API_KEY);
                _restRequest.AddHeader("transactionId", transactionId);
                response = _restClientDataBundles_Production.Execute<MTNBundlePrice>(_restRequest);
            }
            else
            {
                _restRequest = new RestRequest($"v3/products/{beneficiaryId}/{subscriptionId}?transactionId={transactionId}", Method.GET);
                _restRequest.AddHeader("x-api-key", TEST_API_KEY);
                _restRequest.AddHeader("transactionId", transactionId);
                response = _restClientDataBundles_Development.Execute<MTNBundlePrice>(_restRequest);
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return response.Data;
                case HttpStatusCode.InternalServerError:
                    throw new Exception($"third party error -{response.Content}");
                default:
                    throw new Exception($"Error processing request -{response.StatusCode}-");
            }
        }

        public object GetTransactionStatus(string transactionId, string sponsorMSISDN, DateTime transactionTime, bool production = false)
        {
            IRestResponse response;
            if (production)
            {
                _restRequest = new RestRequest($"v2/customers/subscriptions/subscriptionId/status/{transactionId}?subscriptionProviderId=CIS&customerId={sponsorMSISDN}&transactionTime={transactionTime:yyyyMMdd}", Method.GET);
                _restRequest.AddHeader("x-api-key", PRODUCTION_API_KEY);
                _restRequest.AddHeader("transactionId", transactionId);
                response = _restClientDataBundles_Production.Execute(_restRequest);
            }
            else
            {
                _restRequest = new RestRequest($"v2/customers/subscriptions/subscriptionId/status/{transactionId}?subscriptionProviderId=CIS&customerId={sponsorMSISDN}&transactionTime={transactionTime:yyyyMMdd}", Method.GET);
                _restRequest.AddHeader("x-api-key", TEST_API_KEY);
                _restRequest.AddHeader("transactionId", transactionId);
                response = _restClientDataBundles_Development.Execute(_restRequest);
            }

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return response.Content;
                case HttpStatusCode.PreconditionFailed:
                    return null;
                case HttpStatusCode.NotFound:
                    return null;
                default:
                    return null;
            }
        }

        public async Task<IRestResponse<MTNAirtimeServerResponse>> RechargeAirtimeAsync(MTNAirtimeRechargeRequest rechargeRequest, bool production = false)
        {
            IRestResponse<MTNAirtimeServerResponse> response;
            if (production)
            {
                _restRequest = new RestRequest($"/V1/Recharge?production=true", Method.POST);
                _restRequest.AddJsonBody(rechargeRequest);
                var json = JsonConvert.SerializeObject(rechargeRequest);
                response = await _restClientAirtime.ExecuteAsync<MTNAirtimeServerResponse>(_restRequest);
            }
            else
            {
                _restRequest = new RestRequest($"/V1/Recharge?production=false", Method.POST);
                _restRequest.AddJsonBody(rechargeRequest);
                response = await _restClientAirtime.ExecuteAsync<MTNAirtimeServerResponse>(_restRequest);
            }
            return response;
        }
    }
}
