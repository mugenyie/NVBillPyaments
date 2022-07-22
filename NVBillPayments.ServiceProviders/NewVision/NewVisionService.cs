using Microsoft.Extensions.DependencyInjection;
using NVBillPayments.Core.Enums;
using NVBillPayments.Core.Interfaces;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.ServiceProviders.NewVision
{
    public class NewVisionService : INewVisionService
    {
        private readonly IServiceProvider _services;
        private IRestClient _restClient;
        private IRestRequest _restRequest;

        public NewVisionService(IServiceProvider services)
        {
            _services = services;
            _restClient = new RestClient("https://transactions-api.newvisionapp.com");
        }

        public async Task AcknowledgeCollection(string transactionId)
        {
            _restRequest = new RestRequest($"v1/Callback/newvision/products", Method.POST);
            _restRequest.AddJsonBody(new { TransactionId = transactionId });
            await _restClient.ExecuteAsync(_restRequest);
        }
    }
}
