using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.ViewModels
{
    public class PaymentMethod
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<KeyValuePair<string, string>> InputFields { get; set; }
    }
}
