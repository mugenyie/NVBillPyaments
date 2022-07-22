using NVBillPayments.ServiceProviders.Quickteller.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.ServiceProviders.Quickteller
{
    public interface IQuicktellerService
    {
        Task<QuicktellerCategorys> GetCategoriesAsync();
        Task<QuicktellerBillers> GetBillersAsync();
        Task<QuicktellerPaymentItems> GetBillerPaymentItemsAsync(string billerId);
        Task<ValidatedCustomerReponse> ValidateCustomerAsync(ValidateCustomerRequest validateCustomer);
        Task<IRestResponse<PaymentAdviceResponse>> SendPaymentAdviceAsync(PaymentAdviceRequest paymentAdvice);
        Task<TransactionInquiryResponse> TransactionInquiryAsync(string transactionRef);
        Task<object> BalanceInquiry(int inquiryType);
    }
}
