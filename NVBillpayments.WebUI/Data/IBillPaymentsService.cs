using NVBillpayments.WebUI.Models;
using NVBillPayments.Shared.ViewModels.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillpayments.WebUI.Data
{
    public interface IBillPaymentsService
    {
        Task<QuickTellerSimpleVM> FetchCategoriesAsync();
        Task<QuicktellerBillerVM> FetchBillerDetails(string billerId);
        Task<QuicktellerPaymentItemVM> FetchProductInfo(string productCode);
        Task<CustomerValidationResponse> ValidateCustomerAsync(ValidateCustomer validateCustomer);
        Task<PaymentAdviceReponse> PostPaymentAdvice(PostPaymentAdvice paymentAdvice);
    }
}
