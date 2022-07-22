using NVBillPayments.Core.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NVBillPayments.PaymentProviders.Interswitch
{
    public class InterswitchService : IInterswitchService
    {
        private const string APIHOST = "https://esb.interswitch-ke.com:18082";

        private IRestClient _restClient;
        private IRestRequest _restRequest;

        private const string CLIENT_ID = "IKIAF4C46A4AB9846761AD4E1B50E9743A40E20F48CF";
        private const string CLIENT_SECRET = "WF0gvQ6LnnFi652c6abBpnK7G/CE9Xcq8WDQaUvh2Zo=";
        private const string MERCHANT_ID = "NEWVIS0001";

        public InterswitchService()
        {
            _restClient = new RestClient(APIHOST);
        }

        public string GeneratePaymentLink(Transaction transaction)
        {
            string date = transaction.CreatedOnUTC.ToString("MM/dd/yyyy hh:mm:ss tt");
            string customerId = transaction.AccountEmail;
            string customerFirstName = transaction.AccountName;
            string customerSecondName = transaction.AccountName;
            string customerEmail = transaction.AccountEmail;
            string customerMobile = transaction.AccountMSISDN;
            string amount = ((int)transaction.AmountToCharge).ToString();
            string narration = transaction.ProductDescription;

            string url = $"https://interswitchpay.newvisionapp.com?nv_transactionId={transaction.TransactionId}&amount=" +
                $"{amount}&date={date}&" +
                $"customerId={customerId}&customerFirstName={customerFirstName}" +
                $"&customerSecondName={customerSecondName}&customerEmail={customerEmail}" +
                $"&customerMobile={customerMobile}&narration={narration}";

            return url.Replace(" ", "%20");
        }

        public async Task<object> GetTransactionStatusAsync(string transactionId)
        {

            string resourceURL = $"/api/v1/merchant/transactions/{transactionId}";
            _restRequest = new RestRequest($"{resourceURL}");

            _restRequest = GenerateRequestHeaders(_restRequest, resourceURL);

            var restResponse = await _restClient.ExecuteAsync<object>(_restRequest);
            return restResponse.Data;
        }

        private IRestRequest GenerateRequestHeaders(IRestRequest _restRequest, string resourceURL)
        {
            var dateTimeOffset = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            string clientIDBase64 = Base64Encode(CLIENT_ID);
            string nonce = GenerateNonce();
            
            string encodedUrl = HttpUtility.UrlEncode(APIHOST + resourceURL);

            var signatureCipher = "GET" + '&' + encodedUrl + '&' + dateTimeOffset + '&' + nonce + '&' +
            CLIENT_ID + '&' + CLIENT_SECRET;

            _restRequest.AddHeader("Authorization", $"InterswitchAuth {clientIDBase64}");
            _restRequest.AddHeader("Timestamp", $"{dateTimeOffset}");
            _restRequest.AddHeader("Nonce", $"{nonce}");
            _restRequest.AddHeader("Signature", $"{GenerateSHA1(signatureCipher)}");
            _restRequest.AddHeader("SignatureMethod", "SHA1");
            _restRequest.AddHeader("merchantId", $"{MERCHANT_ID}");
            return _restRequest;
        }

        private string GenerateNonce()
        {
            var _random = new Random();
            var radomNum = _random.Next(10000, 99999) + _random.Next(10000, 99999) + _random.Next(10000, 99999) + _random.Next(10000, 99999) + _random.Next(10000, 99999)+ _random.Next(10000, 99999)+ _random.Next(10000, 99999);
            return radomNum.ToString();
        }

        private string GenerateSHA1(string str)
        {
            UnicodeEncoding unicode = new UnicodeEncoding();
            Byte[] hash = new byte[28];
            Byte[] byProduct;
            SHA1 SH1 = new SHA1CryptoServiceProvider();

            byProduct = unicode.GetBytes(str);

            hash = SH1.ComputeHash(byProduct);

            return Convert.ToBase64String(hash);
        }

        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
