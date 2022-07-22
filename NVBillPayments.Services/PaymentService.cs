using Newtonsoft.Json;
using NVBillPayments.Core;
using NVBillPayments.Core.Enums;
using NVBillPayments.Core.Interfaces;
using NVBillPayments.Core.Models;
using NVBillPayments.PaymentProviders.Beyonic;
using NVBillPayments.PaymentProviders.Beyonic.Models;
using NVBillPayments.PaymentProviders.DPO;
using NVBillPayments.PaymentProviders.DPO.Models;
using NVBillPayments.PaymentProviders.Flutterwave;
using NVBillPayments.PaymentProviders.Flutterwave.Models;
using NVBillPayments.Shared.Enums;
using NVBillPayments.Shared.Helpers;
using NVBillPayments.Shared.ViewModels.Payment;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly string DPOPaymentURL = "https://secure.3gdirectpay.com/payv2.php?ID=";
        private readonly IBeyonicService _beyonicService;
        private readonly IDPOService _dpoService;
        private readonly IFlutterwaveService _flutterwaveService;

        public PaymentService(IBeyonicService beyonicService, IDPOService dpoService, IFlutterwaveService flutterwaveService)
        {
            _beyonicService = beyonicService;
            _dpoService = dpoService;
            _flutterwaveService = flutterwaveService;
        }

        public object InitiateBeyonicMobileCollection(string SenderId, decimal amount, string transactionId)
        {
            CollectionRequest collectionRequest = new CollectionRequest()
            {
                phonenumber = SenderId,
                amount = amount.ToString(),
                metadata = new Metadata
                {
                    nv_transactionId = transactionId
                },
                send_instructions = true,
                success_message = "Thanks {customer}, for your payment of amount {amount} through the Vision Group platform! Your transaction is being processed."
            };
            var response = _beyonicService.InitiateCollection(collectionRequest);
            return response;
        }

        public async Task<CardPaymentLinkObject> InitiateFlutterwaveChargeCard(Transaction transaction)
        {
            var response = await _flutterwaveService.InitiateChargeRequestAsync(transaction);
            CardPaymentLinkObject paymentLink = new CardPaymentLinkObject
            {
                Link = response.data.link,
                Response = JsonConvert.SerializeObject(response)
            };
            return paymentLink;
        }

        public async Task<CardPaymentLinkObject> CreateDPOCardPaymentLinkAsync(Transaction transaction)
        {
            CreateTokenBody createTokenBody = new CreateTokenBody
            {
                TransactionId = transaction.TransactionId.ToString(),
                ChargeAmount = transaction.ProductValue,
                DateTimeCreated = transaction.CreatedOnUTC,
                ServiceDescription = transaction.ProductDescription
            };
            var response = await _dpoService.CreateTokenAsync(createTokenBody);
            var objectData = new XMLStringHelper<CreateTokenResponse>().DeserializeXMLString(response.Content);
            CardPaymentLinkObject paymentLink = new CardPaymentLinkObject
            {
                Link = $"{DPOPaymentURL}{objectData.TransToken}",
                Response = response.Content,
                Token = objectData.TransToken
            };
            return paymentLink;
        }
    }
}
