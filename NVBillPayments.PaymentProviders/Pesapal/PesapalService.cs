using NVBillPayments.Core.Enums;
using NVBillPayments.Core.Models;
using Pesapal.APIHelper;
using System;
using System.Collections.Generic;
using System.Linq;


namespace NVBillPayments.PaymentProviders.Pesapal
{
    public class PesapalService : IPesapalService
    {
        private readonly string _consumerKey = "ACQM/LE19q/l89MDeuk96aPAz23ee2Rf";
        private readonly string _consumerSecret = "82+nLQtc8hqhnjfEgXAGG6qPZr4=";
        private readonly string _pesapalCallBackUri = "https://transactions.newvisionapp.com/V1/Callback/pesapal/confirmation";
        private readonly string _pesapalQueryPaymentStatusUri = "https://www.pesapal.com/api/querypaymentstatus";
        private readonly string _pesapalPostUri = "https://www.pesapal.com/API/PostPesapalDirectOrderV4";

        public string GenerateCheckout(Transaction transaction)
        {
            return GetPesapalUrl(transaction);
        }

        public PaymentStatus CheckTransactionStatus(string pesapal_tracking_id, string reference)
        {
            Uri pesapalQueryPaymentStatusUri = new Uri(_pesapalQueryPaymentStatusUri);

            IBuilder builder = new APIPostParametersBuilder()
                .ConsumerKey(_consumerKey)
                .ConsumerSecret(_consumerSecret)
                .OAuthVersion(EOAuthVersion.VERSION1)
                .SignatureMethod(ESignatureMethod.HMACSHA1)
                .SimplePostHttpMethod(EHttpMethod.GET)
                .SimplePostBaseUri(pesapalQueryPaymentStatusUri);

            var helper = new APIHelper<IBuilder>(builder);

            string result = helper.PostGetQueryPaymentStatus(pesapal_tracking_id, reference);
            string[] resultParts = result.Split(new char[] { '=' });
            string paymentStatus = resultParts[1];

            if (paymentStatus.Equals("COMPLETED"))
                return PaymentStatus.SUCCESSFUL;
            else if (paymentStatus.Equals("FAILED"))
                return PaymentStatus.FAILED;
            else if (paymentStatus.Equals("INVALID"))
                return PaymentStatus.INVALID;
            else 
                return PaymentStatus.PENDING;
        }

        public PaymentStatus UpdateIpnTransactionStatus(string ipnType, string pesapal_tracking_id, string reference)
        {
            Uri pesapalQueryPaymentStatusUri = new Uri(_pesapalQueryPaymentStatusUri);

            IBuilder builder = new APIPostParametersBuilder()
                .ConsumerKey(_consumerKey)
                .ConsumerSecret(_consumerSecret)
                .OAuthVersion(EOAuthVersion.VERSION1)
                .SignatureMethod(ESignatureMethod.HMACSHA1)
                .SimplePostHttpMethod(EHttpMethod.GET)
                .SimplePostBaseUri(pesapalQueryPaymentStatusUri);

            var helper = new APIHelper<IBuilder>(builder);

            if (ipnType == "CHANGE")
            {
                // query pesapal for status >> format of the result is pesapal_response_data=<status>
                string result = helper.PostGetQueryPaymentStatus(pesapal_tracking_id, reference);
                string[] resultParts = result.Split(new char[] { '=' });
                string paymentStatus = resultParts[1]; /* Possible values:  
                           PENDING, COMPLETED, FAILED or INVALID*/

                if (paymentStatus.Equals("COMPLETED"))
                    return PaymentStatus.SUCCESSFUL;
                else if (paymentStatus.Equals("FAILED"))
                    return PaymentStatus.FAILED;
                else if (paymentStatus.Equals("INVALID"))
                    return PaymentStatus.INVALID;
                else if (paymentStatus.Equals("PENDING"))
                    return PaymentStatus.PENDING;
            }
            else
            {
                return PaymentStatus.PENDING;
            }

            return PaymentStatus.PENDING;
        }

        protected string GetPesapalUrl(Transaction transaction)
        {
            Uri pesapalPostUri = new Uri(_pesapalPostUri);
            Uri pesapalCallBackUri = new Uri(_pesapalCallBackUri);

            // Setup builder
            IBuilder builder = new APIPostParametersBuilderV2()
                .ConsumerKey(_consumerKey)
                .ConsumerSecret(_consumerSecret)
                .OAuthVersion(EOAuthVersion.VERSION1)
                .SignatureMethod(ESignatureMethod.HMACSHA1)
                .SimplePostHttpMethod(EHttpMethod.GET)
                .SimplePostBaseUri(pesapalPostUri)
                .OAuthCallBackUri(pesapalCallBackUri);

            // Initialize API helper
            APIHelper<IBuilder> helper = new APIHelper<IBuilder>(builder);
            // Populate line items

            var lineItems = new List<LineItem> { };

            // For each item purchased, add a lineItem. 
            // For example, if the user purchased 3 of Item A, add a line item as follows:
            var lineItem =
                new LineItem
                {
                    Particulars = transaction.ProductDescription,/* description of the item, example: Item A *,*/
                    UniqueId = transaction.ProductId,///* some unique id for the item */,
                    Quantity = 1,///* quantity (number of items) purchased, example: 3 */,
                    UnitCost = transaction.ProductValue,///* cost of the item (for 1 item) */
                 };

            lineItem.SubTotal = (lineItem.Quantity * lineItem.UnitCost);
            lineItems.Add(lineItem);
            // Do the same for additional items purchased
   
           // Compose the order
           PesapalDirectOrderInfo webOrder = new PesapalDirectOrderInfo()
           {
               Amount = (lineItems.Sum(x => x.SubTotal)).ToString(),
               Description = transaction.ProductDescription,//* [required] description of the purchase */,
               Type = "MERCHANT",
               Currency = "UGX",
               Reference = transaction.TransactionId.ToString(),//* [required] a unique id, example: an order number */,
               Email = transaction.AccountEmail,//* [either user email or phone number is required]  
               LineItems = lineItems
           };

            // Post the order to PesaPal, which upon successful verification, 
            // will return the string containing the url to load in the iframe
            return helper.PostGetPesapalDirectOrderUrl(webOrder);
        }
    }
}
