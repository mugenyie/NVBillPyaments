using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.ServiceProviders.NewVision.Models
{
    public class EventTicketData
    {
        public int count { get; set; }
        public List<EventTicket> data { get; set; }
    }
}
