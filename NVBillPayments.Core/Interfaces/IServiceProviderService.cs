using NVBillPayments.Core.Models;
using NVBillPayments.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.Core.Interfaces
{
    public interface IServiceProviderService
    {
        Task ProcessOrderAsync(Transaction transaction, bool retry = false);
        object GetMTNBundlesTransactionStatus(Transaction transaction, bool production = false);
        Task<object> RetryMTNAirtimeRechargeAsync(Transaction transaction, bool production = false);
    }
}
