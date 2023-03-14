using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.ServiceProviders.NewVision.Models
{
    public class EventTicketData
    {
        public int Count { get; set; }
        public List<EventTicket> EventTickets { get; set; }
    }
}
