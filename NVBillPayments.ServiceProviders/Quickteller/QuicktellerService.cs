using NVBillPayments.ServiceProviders.Quickteller.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace NVBillPayments.ServiceProviders.Quickteller
{
    public class QuicktellerService : IQuicktellerService
    {
        private const string API_HOST = "https://services.interswitchug.com";
        private const string QUICKTELLER_URL = "/api/v1/quickteller/";
        private const string SVA_URL = "/api/v1A/svapayments/";
        private const string CLIENT_ID = "IKIAF029F081CC4306F3941504BC7DB7C01C4219BE79";
        private const string CLIENT_SECRET = "k63Xp07iscU63B0W2uaU2izz9VNZiTgWyLIU3SCwkg7gmrX3GH8uKvDRJm2nHakN";
        private const string TERMINAL_ID = "3NVG0001";
        private const string BANK_CBN_CODE = "100";
        private const string REQUEST_REFERENCE_PREFIX = "NVG";

        #region sandbox
        //private const string API_HOST = "https://sandbox.interswitch.io";
        //private const string QUICKTELLER_URL = "/uatapi/api/v1/quickteller/";
        //private const string SVA_URL = "/uatapi/api/v1A/svapayments/";
        //private const string CLIENT_ID = "IKIA794905AF56402FB3948B99E0F770AE8B8BFD284E";
        //private const string CLIENT_SECRET = "ovbg/L/i8+eMrY41x0oz2O9XXpve1zWuzRoCV27jsIwaX+br9BPoMxzvDLV1E9Au";
        //private const string TERMINAL_ID = "3MCS0001";
        //private const string BANK_CBN_CODE = "044";
        //private const string REQUEST_REFERENCE_PREFIX = "MCSH";
        #endregion sandbox


        private IRestClient _restClient;
        private IRestRequest _restRequest;

        public QuicktellerService()
        {
            _restClient = new RestClient(API_HOST)
            {
                //RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
        }

        public async Task<QuicktellerCategorys> GetCategoriesAsync()
        {
            string resourceURL = $"{QUICKTELLER_URL}categorys";
            _restRequest = new RestRequest($"{resourceURL}");

            _restRequest = GenerateRequestHeaders(_restRequest, resourceURL);

            var restResponse = await _restClient.ExecuteAsync<QuicktellerCategorys>(_restRequest);
            return restResponse.Data;
        }

        public async Task<QuicktellerBillers> GetBillersAsync()
        {
            string resourceURL = $"{QUICKTELLER_URL}billers";
            _restRequest = new RestRequest($"{resourceURL}");

            _restRequest = GenerateRequestHeaders(_restRequest, resourceURL);

            var restResponse = await _restClient.ExecuteAsync<QuicktellerBillers>(_restRequest);
            return restResponse.Data;
        }

        public async Task<QuicktellerPaymentItems> GetBillerPaymentItemsAsync(string billerId)
        {
            string resourceURL = $"{QUICKTELLER_URL}billers/{billerId}/paymentitems";
            _restRequest = new RestRequest($"{resourceURL}");

            _restRequest = GenerateRequestHeaders(_restRequest, resourceURL);

            var restResponse = await _restClient.ExecuteAsync<QuicktellerPaymentItems>(_restRequest);
            return restResponse.Data;
        }

        public async Task<ValidatedCustomerReponse> ValidateCustomerAsync(ValidateCustomerRequest validateCustomer)
        {
            string resourceURL = $"{SVA_URL}validateCustomer";
            string requestRef = REQUEST_REFERENCE_PREFIX + validateCustomer.RequestReference;

            QuicktellerCustomerValidation customerReference = new QuicktellerCustomerValidation
            {
                bankCbnCode = BANK_CBN_CODE,
                terminalId = TERMINAL_ID,
                amount = validateCustomer.Amount,
                customerEmail = validateCustomer.Email,
                customerId = validateCustomer.CustomerId,
                customerMobile = validateCustomer.PhoneNumber,
                paymentCode = validateCustomer.ProductCode,
                requestReference = requestRef
            };

            _restRequest = new RestRequest(resourceURL, Method.POST);
            _restRequest = GenerateRequestHeaders(_restRequest, resourceURL, "POST");
            _restRequest.AddJsonBody(customerReference);
            var restResponse = await _restClient.ExecuteAsync<ValidatedCustomerReponse>(_restRequest);
            return restResponse.Data;
        }

        public async Task<IRestResponse<PaymentAdviceResponse>> SendPaymentAdviceAsync(PaymentAdviceRequest paymentAdvice)
        {
            string resourceURL = $"{SVA_URL}sendAdviceRequest";
            var additionalParameters = paymentAdvice.Amount+"00"+TERMINAL_ID+ paymentAdvice.RequestReference+paymentAdvice.CustomerId + paymentAdvice.PaymentCode;

            SendPaymentNotificationRequest paymentRequest = new SendPaymentNotificationRequest
            {
                bankCbnCode = BANK_CBN_CODE,
                terminalId = TERMINAL_ID,
                amount = int.Parse(paymentAdvice.Amount + "00"),
                customerEmail = paymentAdvice.Email,
                customerId = paymentAdvice.CustomerId,
                customerMobile = paymentAdvice.PhoneNumber,
                paymentCode = paymentAdvice.PaymentCode,
                requestReference = paymentAdvice.RequestReference,
                transactionRef = paymentAdvice.TransactionReference
            };

            _restRequest = new RestRequest(resourceURL, Method.POST);
            _restRequest = GenerateRequestHeaders(_restRequest, resourceURL, "POST", additionalParameters);
            _restRequest.AddJsonBody(paymentRequest);
            var restResponse = await _restClient.ExecuteAsync<PaymentAdviceResponse>(_restRequest);
            return restResponse;
        }

        public async Task<TransactionInquiryResponse> TransactionInquiryAsync(string transactionRef)
        {
            string resourceURL = $"{SVA_URL}transactions/{transactionRef}";
            _restRequest = new RestRequest(resourceURL);

            _restRequest = GenerateRequestHeaders(_restRequest, resourceURL);

            var restResponse = await _restClient.ExecuteAsync<TransactionInquiryResponse>(_restRequest);
            return restResponse.Data;
        }

        public async Task<object> BalanceInquiry(int inquiryType)
        {
            string resourceURL = $"{SVA_URL}terminal/balance/{inquiryType}/{TERMINAL_ID}";
            _restRequest = new RestRequest(resourceURL);

            _restRequest = GenerateRequestHeaders(_restRequest, resourceURL);

            var restResponse = await _restClient.ExecuteAsync<object>(_restRequest);
            return restResponse.Data;
        }

        #region helper methods
        private IRestRequest GenerateRequestHeaders(IRestRequest _restRequest, string resourceURL,string httpMethod = "GET", string parameters = "")
        {
            var dateTimeOffset = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            string clientIDBase64 = Base64Encode(CLIENT_ID);
            string nonce = GenerateNonce();

            string encodedUrl = HttpUtility.UrlEncode(API_HOST + resourceURL);

            Regex reg = new Regex(@"%[a-f0-9]{2}");
            encodedUrl = reg.Replace(encodedUrl, m => m.Value.ToUpperInvariant());

            var signatureCipher = httpMethod + '&' + encodedUrl + '&' + dateTimeOffset + '&' + nonce + '&' +
            CLIENT_ID + '&' + CLIENT_SECRET;

            if (!string.IsNullOrEmpty(parameters))
                signatureCipher += '&' + parameters;

            _restRequest.AddHeader("Authorization", $"InterswitchAuth {clientIDBase64}");
            _restRequest.AddHeader("Timestamp", $"{dateTimeOffset}");
            _restRequest.AddHeader("Nonce", $"{nonce}");
            _restRequest.AddHeader("Signature", $"{ComputeSha256Hash(signatureCipher)}");
            _restRequest.AddHeader("SignatureMethod", "sha256");
            _restRequest.AddHeader("TerminalId", $"{TERMINAL_ID}");
            return _restRequest;
        }

        private string GenerateNonce()
        {
            var _random = new Random();
            var radomNum = _random.Next(10000, 99999) + _random.Next(10000, 99999) + _random.Next(10000, 99999) + _random.Next(10000, 99999) + _random.Next(10000, 99999) + _random.Next(10000, 99999) + _random.Next(10000, 99999);
            return radomNum.ToString();
        }

        private string ComputeSha256Hash(string rawData)
        {
            using SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return Convert.ToBase64String(bytes);
        }

        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        #endregion helper methods
    }
}
