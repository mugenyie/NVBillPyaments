using NVBillPayments.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.PaymentProviders.Interswitch
{
    public interface IInterswitchService
    {
        string GeneratePaymentLink(Transaction transaction);
        Task<object> GetTransactionStatusAsync(string transactionId);
    }
}
