using NVBillPayments.Core.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NVBillPayments.PaymentProviders.Pegasus
{
    public class PegasusService : IPegasusService
    {
        private const string APIHOST = "https://test.pegasus.co.ug:8019/TestPegasusPaymentsGateway/Default.aspx";
        private const string Merchant = "NEW VISION PRINTING AND PUBLISHING COMPANY LIMITED";
        private const string MerchantID = "131641";
        private const string Username = "test";
        private const string Password = "test1234";
        private const string SecretKey = "T3rr16132016";

        private const string ReturnURL = "https://transactions.newvisionapp.com/V1/Callback/pegasus/confirmation";
        private const string HostedPaymentBaseURL = "https://pegpay.newvisionapp.com";

        private IRestClient _restClient;
        private IRestRequest _restRequest;

        public PegasusService()
        {
            _restClient = new RestClient(APIHOST);
        }

        public string GeneratePaymentLink(Transaction transaction)
        {
            string transactionId = transaction.TransactionId.ToString().ToLower();
            string productDesc = Regex.Replace(transaction.ProductDescription, "[^a-zA-Z0-9_]+", "_");
            string dataToSign = Username + MerchantID + transaction.AmountToCharge + productDesc + transaction.CurrencyCode + ReturnURL + transactionId;
            string DigitalSignatureHMAC = GenearetHMACSha256SignatureHash(dataToSign,SecretKey);
            string PasswordHMAC = GenearetHMACSha256SignatureHash(Password, SecretKey);

            return $"{HostedPaymentBaseURL}?VENDORCODE={Username}&PASSWORD={PasswordHMAC}&" +
                $"VENDOR_TRANID={transactionId}&ITEM_TOTAL={transaction.AmountToCharge}&ITEM_DESCRIPTION={productDesc}&" +
                $"CURRENCY={transaction.CurrencyCode}&DIGITAL_SIGNATURE={DigitalSignatureHMAC}&MERCHANTCODE={MerchantID}" +
                $"&RETURN_URL={ReturnURL}";
        }

        public async Task<string> GetTransactionStatusAsync(string transactionId)
        {
            string PasswordHMAC = GenearetHMACSha256SignatureHash(Password, SecretKey);
            _restRequest = new RestRequest($"/TestPegasusPaymentsGateway/QueryStatus.aspx?MerchantId={MerchantID}&VendorCode={Username}&Pswd={PasswordHMAC}&VendorTranId={transactionId}");
            var response = await _restClient.ExecuteAsync<string>(_restRequest);
            return response.Data;
        }

        public static string GenearetHMACSha256SignatureHash(string dataToSign, string key)
        {
            ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(key);
            byte[] messageBytes = encoding.GetBytes(dataToSign);
            using (HMACSHA256 hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                var result = prtByte(hashmessage);
                return result;
            }
        }

        public static string prtByte(byte[] b)
        {
            List<string> charList = new List<string>();
            for (var i = 0; i < b.Length; i++)
            {
                charList.Add(b[i].ToString("x2"));
            }
            return string.Join("", charList.ToArray());
        }
    }
}
