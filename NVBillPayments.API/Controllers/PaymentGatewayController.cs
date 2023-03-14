using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NVBillPayments.API.ViewModels;
using NVBillPayments.Core.Interfaces;
using NVBillPayments.Core.Models;
using NVBillPayments.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.Controllers
{
    [Route("V1/[controller]")]
    [ApiController]
    public class PaymentGatewayController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ICachingService _cachingService;

        public PaymentGatewayController(ICachingService cachingService, ITransactionService transactionService)
        {
            _cachingService = cachingService;
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<IActionResult> PostPaymentAsync(PaymentGatewayRequest paymentGatewayRequest)
        {
            string callbackURL = paymentGatewayRequest.CallbackURL.Trim();

            if (!paymentGatewayRequest.CurrencyCode.ToUpper().Equals("UGX"))
                return new BadRequestObjectResult(new { ErrorMessage = "Unsupported Currency format"});
            if(paymentGatewayRequest.PaymentMethod.Equals("momo") && string.IsNullOrEmpty(paymentGatewayRequest.PayWithMSISDN))
                return new BadRequestObjectResult(new { ErrorMessage = "No MSISDN provided for pay with momo" });
            if(!(paymentGatewayRequest.PaymentMethod.Equals("momo") || paymentGatewayRequest.PaymentMethod.Equals("card")))
                return new BadRequestObjectResult(new { ErrorMessage = "No valid payment method supplied" });
            if(string.IsNullOrEmpty(callbackURL))
                return new BadRequestObjectResult(new { ErrorMessage = "no CallbackURL supplied" });

            Transaction _transaction = new Transaction
            {
                ExternalUserId = paymentGatewayRequest.CustomerId,
                AccountEmail = paymentGatewayRequest.CustomerEmail,
                AccountMSISDN = paymentGatewayRequest.CustomerMSISDN,
                AccountName = paymentGatewayRequest.CustomerFullName,
                ProductId = $"VPG_{paymentGatewayRequest.ProductCode}",
                ProductDescription = paymentGatewayRequest.ProductDescription,
                ServiceProviderId = "NEWVISION",
                CreatedOnUTC = DateTime.UtcNow,
                AmountToCharge = paymentGatewayRequest.AmountToDeduct,
                SponsorMSISDN = paymentGatewayRequest?.PayWithMSISDN,
                CallbackURL = paymentGatewayRequest.CallbackURL,
                SystemCategory = SystemCategory.VISION_PAYMENT_GATEWAY.ToString()
            };

            if (paymentGatewayRequest.MetaData != null)
                _transaction.MetaData = paymentGatewayRequest.MetaData.ToString();

            var transaction = await _transactionService.SaveTransactionAsync(_transaction);
            if(transaction != null)
            {
                string paymentLink = "";
                if (paymentGatewayRequest.PaymentMethod.Equals("card"))
                {
                    paymentLink = await _transactionService.CreateCardPaymentLinkV2(transaction, PaymentProvider.INTERSWITCH);
                }
                else if (paymentGatewayRequest.PaymentMethod.Equals("momo"))
                {
                    await _transactionService.InititateMobilePaymentCollectionAsync(transaction);
                }
                else
                {
                    return new BadRequestObjectResult(new { ErrorMessage = "No payment method supplied" });
                }

                string PaymentMessage = "";
                switch (paymentGatewayRequest.PaymentMethod)
                {
                    case "momo":
                        PaymentMessage += "Please follow prompts on your mobile to complete payment";
                        break;
                    case "card":
                        PaymentMessage += "Complete payment with our secure card checkout";
                        break;
                }

                return StatusCode(StatusCodes.Status200OK, new {TransactionReference = transaction.TransactionId.ToString(), Message = PaymentMessage, PaymentLink = paymentLink });
            }
            else
            {
                return new BadRequestObjectResult(new { ErrorMessage = "Unable to create transaction request" });
            }
        }
    }
}
