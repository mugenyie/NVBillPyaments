using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.ServiceProviders.NewVision.Models
{
    public class EventTicket
    {
        public int id { get; set; }
        public string event_name { get; set; }
        public string logo { get; set; }
        public string customer_field { get; set; }
        public List<TicketCategory> ticket_categories { get; set; }
    }
}
