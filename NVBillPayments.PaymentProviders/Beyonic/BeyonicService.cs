using NVBillPayments.PaymentProviders.Beyonic.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.PaymentProviders.Beyonic
{
    public class BeyonicService : IBeyonicService
    {
        private readonly string BaseURL = "https://api.beyonic.com/api";
        private const string API_KEY = "31b710fb5a43c6d2765cc043b81958d86e667b77";
        private IRestClient _restClient;
        private IRestRequest _restRequest;

        public BeyonicService()
        {
            _restClient = new RestClient(BaseURL);
        }

        public CollectionResponse InitiateCollection(CollectionRequest collectionRequest)
        {
            _restRequest = new RestRequest($"collectionrequests", Method.POST);
            _restRequest.AddJsonBody(collectionRequest);
            _restRequest.AddHeader("Authorization", $"Token {API_KEY}");
            var response = _restClient.Execute<CollectionResponse>(_restRequest);
            return response.Data;
        }

        public PaymentResponse InitiatePayment(PaymentRequest paymentRequest)
        {
            _restRequest = new RestRequest($"payments", Method.POST);
            _restRequest.AddJsonBody(paymentRequest);
            _restRequest.AddHeader("Authorization", $"Token {API_KEY}");
            var response = _restClient.Execute<PaymentResponse>(_restRequest);
            return response.Data;
        }
    }
}
