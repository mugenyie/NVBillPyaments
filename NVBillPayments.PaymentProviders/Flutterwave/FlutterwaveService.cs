using NVBillPayments.Core.Models;
using NVBillPayments.PaymentProviders.Flutterwave.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.PaymentProviders.Flutterwave
{
    public class FlutterwaveService : IFlutterwaveService
    {
        private const string APIKEY = "FLWSECK-faae5c96fe56722effb9c78ab3f6e84e-X";
        private const string APIHOST = "https://api.flutterwave.com";
        private readonly string RedirectURL = "https://transactions.newvisionapp.com/V1/Callback/flutterwave/confirmation";

        private IRestClient _restClient;
        private IRestRequest _restRequest;

        public FlutterwaveService()
        {
            _restClient = new RestClient(APIHOST);
        }

        public async Task<FlutterwavePaymentStatus> GetTransactionStatusAsync(string flutterwaveTransactionId)
        {
            _restRequest = new RestRequest($"/v3/transactions/{flutterwaveTransactionId}/verify");
            _restRequest.AddHeader("Authorization", $"Bearer {APIKEY}");

            var flutterResponse = await _restClient.ExecuteAsync<FlutterwavePaymentStatus>(_restRequest);
            return flutterResponse.Data;
        }

        public async Task<FlutterwavePaymentResponse> InitiateChargeRequestAsync(Transaction transaction)
        {
            _restRequest = new RestRequest("/v3/payments", Method.POST);
            _restRequest.AddHeader("Authorization", $"Bearer {APIKEY}");

            FlutterwavePaymentRequest flutterwavePayment = new FlutterwavePaymentRequest
            {
                tx_ref = transaction.TransactionId.ToString(),
                amount = transaction.AmountToCharge.ToString(),
                currency = transaction.CurrencyCode,
                redirect_url = RedirectURL,
                payment_options = "card",
                meta = new FlutterwavePaymentMeta
                {
                    beneficiary_user_id = transaction.BeneficiaryMSISDN,
                    sponsor_user_id = transaction.SponsorMSISDN ?? transaction.AccountEmail,
                    product_description = transaction.ProductDescription
                },
                customer = new FlutterwaveCustomerRef
                {
                    email = transaction.AccountEmail,
                    name = transaction.AccountName,
                    phonenumber = transaction.AccountMSISDN
                },
                customizations = new Customizations
                {
                    title = transaction.ProductDescription,
                }
            };

            _restRequest.AddJsonBody(flutterwavePayment);
            var flutterResponse = await _restClient.ExecuteAsync<FlutterwavePaymentResponse>(_restRequest);
            return flutterResponse.Data;
        }
    }
}
