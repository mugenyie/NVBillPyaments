using NVBillPayments.Core.Models;
using NVBillPayments.PaymentProviders.DPO.Models;
using NVBillPayments.Shared;
using NVBillPayments.Shared.Helpers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.PaymentProviders.DPO
{
    public class DPOService : IDPOService
    {
        private readonly string BaseURL = "https://secure.3gdirectpay.com/API/v6/";
        private readonly string CompanyToken = "9F416C11-127B-4DE2-AC7F-D5710E4C5E0A";
        private readonly string ServiceType = "5525";

        private readonly string EnvironmentURL;
        private readonly string CallBackURL;
        private readonly string RedirectURL = "https://www.newvisionapp.com";

        private IRestClient _restClient;
        private IRestRequest _restRequest;

        public DPOService()
        {
            _restClient = new RestClient(BaseURL);
            EnvironmentURL = ConfigurationConstants.ENVIRONMENT == "PRODUCTION" ? "https://transactions-api-production.newvisionapp.com" : "https://transactions-api-develop.newvisionapp.com";
            CallBackURL = $"{EnvironmentURL}/V1/Callback/dpo/payments";
        }

        #region direct DPO Call
        public async Task<IRestResponse> CreateTokenAsync(CreateTokenBody createTokenBody)
        {
            string xmlRequest = "" +
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<API3G>" +
                $"<CompanyToken>{CompanyToken}</CompanyToken>" +
                "<Request>createToken</Request>" +
                "<Transaction>" +
                $"<PaymentAmount>{createTokenBody.ChargeAmount}</PaymentAmount>" +
                $"<PaymentCurrency>{createTokenBody.Currency}</PaymentCurrency>" +
                $"<CompanyRef>{createTokenBody.TransactionId}</CompanyRef>" +
                $"<RedirectURL>{RedirectURL}</RedirectURL>" +
                $"<BackURL>{CallBackURL}</BackURL>" +
                "<CompanyRefUnique>1</CompanyRefUnique>" +
                "<PTL>3</PTL>" +
                "</Transaction>" +
                "<Services>" +
                  "<Service>" +
                    $"<ServiceType>{ServiceType}</ServiceType>" +
                    $"<ServiceDescription>{createTokenBody.ServiceDescription}</ServiceDescription>" +
                    $"<ServiceDate>{createTokenBody.DateTimeCreated:yyyy-MM-dd HH:mm:ss}</ServiceDate>" +
                  "</Service>" +
                "</Services>" +
                "</API3G>";
            _restRequest = new RestRequest(Method.POST);
            _restRequest.AddParameter("text/xml", xmlRequest, ParameterType.RequestBody);

            var restResponse = await _restClient.ExecuteAsync(_restRequest);
            return restResponse;
        }

        public void VerifyToken(string token)
        {
            /*string xmlRequest = "" +
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>"+
                "<API3G>" +
                  "<CompanyToken>57466282-EBD7-4ED5-B699-8659330A6996</CompanyToken>" +
                  "<Request>verifyToken</Request>" +
                  "<TransactionToken>72983CAC-5DB1-4C7F-BD88-352066B71592</TransactionToken>" +
                "</API3G>";*/
            throw new NotImplementedException();
        }

        public void RefundToken(Transaction transaction)
        {
            string xmlRequest = "" +
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<API3G>" +
                  "<Request>refundToken</Request>" +
                  "<CompanyToken>68B90B5E-25F6-4146-8AB1-CV4GA0C41A7F</CompanyToken>" +
                  "<TransactionToken>246757AE-4D4F-7763-BFF5-326704703102</TransactionToken>" +
                  "<refundAmount>1.2</refundAmount>" +
                  "<refundDetails>Refund discription</refundDetails>" +
                "</API3G>";
            xmlRequest += "";
            throw new NotImplementedException();
        }
        #endregion direct DPO Call
    }
}
