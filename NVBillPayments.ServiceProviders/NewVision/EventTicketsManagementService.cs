using NVBillPayments.ServiceProviders.NewVision.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.ServiceProviders.NewVision
{
    public class EventTicketsManagementService : IEventTicketsManagementService
    {
        private IRestClient _restClient;
        private IRestRequest _restRequest;

        public EventTicketsManagementService()
        {
            _restClient = new RestClient("https://api.newvisionapp.com/v1/EventTicketManagement");
        }

        public async Task<List<EventTicket>> GetEventTicketsAsync(int offset, int limit)
        {
            _restRequest = new RestRequest($"EventTickets?offset={offset}&limit={limit}");
            var result = await _restClient.ExecuteAsync<EventTicketData>(_restRequest);
            return result.Data?.data;
        }

        public async Task<bool> GetTicketApproverAsync(string email, string password)
        {
            _restRequest = new RestRequest($"TicketApprover", Method.POST);
            _restRequest.AddJsonBody(new { email, password });
            var result = await _restClient.ExecuteAsync(_restRequest);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
                return true;
            else
                return false;
        }
    }
}
