using NVBillPayments.Core.Models;
using NVBillPayments.Shared.Enums;
using NVBillPayments.Shared.ViewModels.Payment;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.Core.Interfaces
{
    public interface IPaymentService
    {
        object InitiateBeyonicMobileCollection(string SenderId, decimal amount, string transactionId);
        Task<CardPaymentLinkObject> CreateDPOCardPaymentLinkAsync(Transaction transaction);
        Task<CardPaymentLinkObject> InitiateFlutterwaveChargeCard(Transaction transaction);
    }
}
