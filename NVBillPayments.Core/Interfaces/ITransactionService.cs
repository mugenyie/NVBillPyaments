using NVBillPayments.Core.Enums;
using NVBillPayments.Core.Models;
using NVBillPayments.Shared.Enums;
using NVBillPayments.Shared.ViewModels.Transaction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.Core.Interfaces
{
    public interface ITransactionService
    {
        void AddTransactionToQueue(object message);
        Transaction GetById(string transactionId);
        Task<Transaction> SaveTransactionAsync(Transaction transaction);
        Task<string> CreateCardPaymentLinkV2(Transaction transaction, PaymentProvider paymentProvider);
        Task<Transaction> AddTransactionRecordAsync(AddTransactionVM transaction, Product product);
        Task<Transaction> ProcessTransactionAsync(AddTransactionVM transaction);
        Task ProcessSuccesfulPaymentCallback(string transactionId, decimal amountCharged, string paymentProviderId, string paymentMetaData);
        Task ProcessFailedPayment(string transactionId, string paymentProviderId, string paymentMetaData, string statusMessage, string technicalStatusMessage = "");
        Task ProcessOrderCallbackAsync(string transactionId, OrderStatus orderStatus, string serviceProviderMetaData);
        Task InititateMobilePaymentCollectionAsync(Transaction transaction);
        List<SimpleTransactionsVM> GetOrders(string email,string userId, string status, string category, int limit, int offset);
        List<SimpleTransactionsVM> GetRecommendedOrders(string email, string category, bool singlePerCategory = false, int limit=20, int offset=0);
        Task<object> GetThirdpartyTransactionStatusAsync(string transactionId);
        Task<string> CreateCardPaymentLink(AddTransactionVM transaction, PaymentProvider paymentProvider = PaymentProvider.FLUTTERWAVE);
        Task ProcessFreeChargeProductAsync(Transaction transactionRecord);
        Task<bool> MarkExpired(Transaction transaction);
    }
}
