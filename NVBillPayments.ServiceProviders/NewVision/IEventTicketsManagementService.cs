using NVBillPayments.ServiceProviders.NewVision.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.ServiceProviders.NewVision
{
    public interface IEventTicketsManagementService
    {
        Task<List<EventTicket>> GetEventTicketsAsync(int offset, int limit);
        Task<bool> GetTicketApproverAsync(string email, string password);
    }
}
