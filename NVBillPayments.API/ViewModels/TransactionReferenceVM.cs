using NVBillPayments.ServiceProviders.Quickteller.Models;
using NVBillPayments.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.ViewModels
{
    public class TransactionReferenceVM
    {
        public ServiceProvider serviceProvider { get; set; } 
        public ValidateCustomerRequest customerRequest { get; set; }
        public ValidatedCustomerReponse customerReponse { get; set; }
    }
}
