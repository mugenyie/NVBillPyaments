using NVBillPayments.Core.Models;
using NVBillPayments.PaymentProviders.Flutterwave.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.PaymentProviders.Flutterwave
{
    public interface IFlutterwaveService
    {
        Task<FlutterwavePaymentResponse> InitiateChargeRequestAsync(Transaction transaction);
        Task<FlutterwavePaymentStatus> GetTransactionStatusAsync(string flutterwaveTransactionId);
    }
}
