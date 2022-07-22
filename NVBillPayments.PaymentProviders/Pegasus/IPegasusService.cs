using NVBillPayments.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.PaymentProviders.Pegasus
{
    public interface IPegasusService
    {
        string GeneratePaymentLink(Transaction transaction);
        Task<string> GetTransactionStatusAsync(string transactionId);
    }
}
