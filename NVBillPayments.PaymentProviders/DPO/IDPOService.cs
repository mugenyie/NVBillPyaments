using NVBillPayments.Core.Models;
using NVBillPayments.PaymentProviders.DPO.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.PaymentProviders.DPO
{
    public interface IDPOService
    {
        Task<IRestResponse> CreateTokenAsync(CreateTokenBody createTokenBody);
        void VerifyToken(string xmlRequest);
        void RefundToken(Transaction transaction);
    }
}
