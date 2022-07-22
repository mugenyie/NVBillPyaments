using NVBillPayments.ServiceProviders.MTNUG.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.ServiceProviders.MTNUG
{
    public interface IMTNService
    {
        MTNBundlePrice GetBundlePrice(string beneficiaryId, string subscriptionId, string transactionId, bool production = false);
        Task<IRestResponse> ActivateBundleAsync(string sponsorMSISDN, string transactionId, MTNActivateBundle MTNActivateBundle, bool production = false);
        Task<IRestResponse<MTNAirtimeServerResponse>> RechargeAirtimeAsync(MTNAirtimeRechargeRequest rechargeRequest, bool production = false);
        object GetTransactionStatus(string transactionId, string sponsorMSISDN, DateTime transactionTime, bool production = false);
    }
}
