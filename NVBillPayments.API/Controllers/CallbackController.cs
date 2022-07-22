using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NVBillPayments.API.Helpers;
using NVBillPayments.API.ViewModels.Callbacks;
using NVBillPayments.Core.Enums;
using NVBillPayments.Core.Interfaces;
using NVBillPayments.PaymentProviders.Beyonic.Models.Callback;
using NVBillPayments.PaymentProviders.DPO.Models;
using NVBillPayments.PaymentProviders.Flutterwave;
using NVBillPayments.PaymentProviders.Pesapal;
using NVBillPayments.ServiceProviders.NewVision;
using NVBillPayments.Shared.Helpers;
using NVBillPayments.Shared.ViewModels.Payment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.Controllers
{
    [Route("V1/[controller]")]
    [ApiController]
    public class CallbackController : ControllerBase
    {
        private readonly ITransactionLogService _transactionLogService;
        private readonly ITransactionService _transactionService;
        private readonly IPesapalService _pesapalService;
        private readonly IFlutterwaveService _flutterwaveService;
        private readonly JsonSerializerSettings _jsonSerialiserSettings;

        public CallbackController(ITransactionLogService transactionLogService, ITransactionService transactionService, IPesapalService pesapalService, IFlutterwaveService flutterwaveService)
        {
            _transactionLogService = transactionLogService;
            _transactionService = transactionService;
            _pesapalService = pesapalService;
            _flutterwaveService = flutterwaveService;
            _jsonSerialiserSettings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
        }

        [HttpPost]
        [Route("{ServiceProvider}/products")]
        public async Task<IActionResult> ServiceProviderCallbackAsync(string ServiceProvider)
        {
            string callbackData = new StreamReader(HttpContext.Request.Body).ReadToEndAsync().Result;
            switch (ServiceProvider.ToUpper())
            {
                case "NEWVISION":
                    {
                        _transactionLogService.AddTransactionLogAsync($"NEWVISION Callback", callbackData);
                        var dataObject = JsonConvert.DeserializeObject<NVOrderCallback>(callbackData);
                        await _transactionService.ProcessOrderCallbackAsync(dataObject.TransactionId, OrderStatus.SUCCESSFUL, "Successfuly collected funds");
                        return Ok("AIRTELUG");
                    }
                default:
                    return Ok();
            }
        }

        [HttpPost]
        [Route("{ServiceProvider}/bundles")]
        public IActionResult BundlesProvider(string ServiceProvider)
        {
            string callbackData = new StreamReader(HttpContext.Request.Body).ReadToEndAsync().Result;
            switch (ServiceProvider.ToUpper())
            {
                case "MTNUG":
                    {
                        _transactionLogService.AddTransactionLogAsync($"MTNUG bundles Callback", callbackData);
                        return Ok("MTNUG");
                    }
                case "AIRTELUG":
                    {
                        _transactionLogService.AddTransactionLogAsync($"AIRTELUG Callback", callbackData);
                        return Ok("AIRTELUG");
                    }
                default:
                    return Ok();
            }
        }

        [HttpGet]
        [Route("pegasus/confirmation")]
        public async Task<IActionResult> PegasusConfirmationAsync(string Status, string TranId, string VendorID, string Reason, string DigitalSignature, string PAYMENT_CHANNEL)
        {
            string requestString = $"?Status={Status}&TranID={TranId}&VendorID={VendorID}&Reason={Reason}&DigitalSignature={DigitalSignature}&PAYMENT_CHANNEL={PAYMENT_CHANNEL}";
            _transactionLogService.AddTransactionLogAsync($"Pegasus Confirmation", requestString);
            if (Status.ToUpper().Equals("SUCCESS"))
            {
                await _transactionService.ProcessSuccesfulPaymentCallback(VendorID, 0, "PEGASUS", requestString);
            }
            if (Status.ToUpper().Equals("FAILED"))
            {
                await _transactionService.ProcessFailedPayment(VendorID, "PEGASUS", requestString, "Payment Failed");
            }
            //return new ContentResult
            //{
            //    ContentType = "text/html; charset=utf-8",
            //    Content = $"<strong>Transaction ({VendorID}) {Status}</strong>"
            //};
            return Redirect("newvisionapp.com?status=complete");
        }

        [HttpGet]
        [Route("pesapal/confirmation")]
        public async Task<IActionResult> PesapalConfirmationAsync(string pesapal_transaction_tracking_id, string pesapal_merchant_reference)
        {
            string requestString = $"pesapal_transaction_tracking_id={pesapal_transaction_tracking_id}&pesapal_merchant_reference={pesapal_merchant_reference}";
            _transactionLogService.AddTransactionLogAsync($"PESAPAL CONFIRM ", requestString);
            try
            {
                var paymentStatus = _pesapalService.CheckTransactionStatus(pesapal_transaction_tracking_id, pesapal_merchant_reference);
                if (paymentStatus == PaymentStatus.SUCCESSFUL)
                {
                    await _transactionService.ProcessSuccesfulPaymentCallback(pesapal_merchant_reference, 0, "PESAPAL", requestString);
                    return new ContentResult
                    {
                        ContentType = "text/html; charset=utf-8",
                        Content = GenerateConfirmationPage(PaymentStatus.SUCCESSFUL)
                    };
                }
                else if(paymentStatus == PaymentStatus.FAILED)
                {
                    await _transactionService.ProcessFailedPayment(pesapal_merchant_reference, "PESAPAL", requestString, "Payment Failed");
                    return new ContentResult
                    {
                        ContentType = "text/html; charset=utf-8",
                        Content = GenerateConfirmationPage(PaymentStatus.FAILED)
                    };
                }else if(paymentStatus == PaymentStatus.INVALID)
                {
                    await _transactionService.ProcessFailedPayment(pesapal_merchant_reference, "PESAPAL", requestString, "Invalid Payment");
                    return new ContentResult
                    {
                        ContentType = "text/html; charset=utf-8",
                        Content = GenerateConfirmationPage(PaymentStatus.FAILED)
                    };
                }
                else
                {
                    return new ContentResult
                    {
                        ContentType = "text/html; charset=utf-8",
                        Content = GenerateConfirmationPage(PaymentStatus.PENDING)
                    };
                }
            }
            catch (Exception)
            {

            }
            return new ContentResult
            {
                ContentType = "text/html; charset=utf-8",
                Content = GenerateConfirmationPage(PaymentStatus.PENDING)
            };
        }

        #region confirmaitonHTML
        private string GenerateConfirmationPage(PaymentStatus paymentStatus)
        {
            string _circleColor = "";
            string _circleIcon = "";
            string _confirmationMessage = "";

            switch (paymentStatus)
            {
                case PaymentStatus.SUCCESSFUL:
                    {
                        _circleColor = "green";
                        _circleIcon = "✓";
                        _confirmationMessage = "Payment Sucessful";
                        goto Finish;
                    }
                case PaymentStatus.FAILED:
                    {
                        _circleColor = "red";
                        _circleIcon = "X";
                        _confirmationMessage = "Payment Failed";
                        goto Finish;
                    }
                default:
                    {
                        _circleColor = "grey";
                        _circleIcon = "−";
                        _confirmationMessage = "Payment Pending";
                        goto Finish;
                    }
            }
            
            Finish:
            string htmlConfirmation = "" +
                "<!DOCTYPE html><html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" /><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\"/><style>img { max-width: 100%; height: auto;}</style></head><body style=\"margin-top: 50%;text-align: center;font-family:sans-serif;font-weight: 800;\"><div> <div style=\" " +
                $"background-color: {_circleColor}; " +
                "color: white; border-radius: 50%; width: 80px; height: 80px; display: inline-block; text-align: center; vertical-align: middle; line-height: 80px;\"> " +
                $"<span style=\"font-size:50px;\">{_circleIcon}</span>" +
                $"</div><br /><span>{_confirmationMessage}</span>" +
                "<br /><br /><br /><a style=\"text-decoration: none;background-color: black;color: white;padding: 8px 20px 8px 20px;\" href=\"newvisionapp.com?status=complete\">VIEW TRANSACTION HISTORY</a></div></body></html>";
            return htmlConfirmation;
        }
        #endregion confirmaitonHTML

        [HttpGet]
        [Route("pesapal/payments")]
        public async Task<IActionResult> PesapalCallbackAsync(string pesapal_notification_type, string pesapal_transaction_tracking_id, string pesapal_merchant_reference)
        {
            string requestString = $"pesapal_notification_type={pesapal_notification_type}&pesapal_transaction_tracking_id={pesapal_transaction_tracking_id}&pesapal_merchant_reference={pesapal_merchant_reference}";
            _transactionLogService.AddTransactionLogAsync($"PESAPAL GET", requestString);
            try
            {
                var paymentStatus = _pesapalService.UpdateIpnTransactionStatus(pesapal_notification_type, pesapal_transaction_tracking_id, pesapal_merchant_reference);
                if (paymentStatus == PaymentStatus.SUCCESSFUL)
                {
                    await _transactionService.ProcessSuccesfulPaymentCallback(pesapal_merchant_reference, 0, "PESAPAL", requestString);
                    return Ok(requestString);
                }
                else if(paymentStatus == PaymentStatus.FAILED)
                {
                    await _transactionService.ProcessFailedPayment(pesapal_merchant_reference, "PESAPAL", requestString, "Payment Failed");
                    return Ok(requestString);
                }
                else if(paymentStatus == PaymentStatus.INVALID)
                {
                    await _transactionService.ProcessFailedPayment(pesapal_merchant_reference, "PESAPAL", requestString, "Invalid Payment");
                    return Ok(requestString);
                }
            }
            catch (Exception)
            {

            }
            return Ok();
        }

        //[HttpGet]
        //[Route("flutterwave/newvisionapp.com")]
        //public IActionResult FlutterwaveComplete(string status)
        //{
        //    return new ContentResult
        //    {
        //        ContentType = "text/html; charset=utf-8",
        //        Content = "<strong>COMPLETE!!</strong>"
        //    };
        //}

        [HttpGet]
        [Route("flutterwave/confirmation")]
        public async Task<IActionResult> FlutterwaveRedirect(string status, string tx_ref, string transaction_id)
        {
            string requestString = $"status={status}&tx_ref={tx_ref}&transaction_id={transaction_id}";
            _transactionLogService.AddTransactionLogAsync($"FLUTTERWAVE Confirm", requestString);
            
            if (status.Equals("failed"))
            {
                await _transactionService.ProcessFailedPayment(tx_ref, "FLUTTERWAVE", requestString, "Payment Failed");
                return Redirect("newvisionapp.com?status=complete");
            }
            if (status.Equals("cancelled"))
            {
                await _transactionService.ProcessFailedPayment(tx_ref, "FLUTTERWAVE", requestString, "Payment Canceled");
                return Redirect("newvisionapp.com?status=complete");
            }

            var flutterwavePayment = await _flutterwaveService.GetTransactionStatusAsync(transaction_id);
            if (flutterwavePayment.status.Equals("success"))
            {
                await _transactionService.ProcessSuccesfulPaymentCallback(tx_ref, 0, "FLUTTERWAVE", JsonConvert.SerializeObject(flutterwavePayment));
                return Redirect("newvisionapp.com?status=complete");
            }
            else if (flutterwavePayment.status.Equals("failed"))
            {
                await _transactionService.ProcessFailedPayment(tx_ref, "FLUTTERWAVE", JsonConvert.SerializeObject(flutterwavePayment), "Payment Failed");
                return Redirect("newvisionapp.com?status=complete");
            }
            else
            {
                return Redirect("newvisionapp.com?status=complete");
            }
        }

        [HttpGet]
        [Route("{processor}/newvisionapp.com")]
        public IActionResult CompleteProcess(string status)
        {
            return new ContentResult
            {
                ContentType = "text/html; charset=utf-8",
                Content = "<strong>COMPLETE!!</strong>"
            };
        }

        [HttpGet]
        [Route("interswitch/confirmation")]
        public async Task<IActionResult> InterswitchConfirmationAsync(string response)
        {
            var requestObjectString = response.DecodeBase64();
            try
            {
                var successObject = JsonConvert.DeserializeObject<InterswitchSuccessCB>(requestObjectString);
                var failureCallback = JsonConvert.DeserializeObject<InterswithFailureCB>(requestObjectString);
                if (successObject?.transactionRef != null)
                {
                    if (successObject.responseCode.Equals("00"))
                    {
                        string amountString = successObject.transactionAmount[0..^2];
                        await _transactionService.ProcessSuccesfulPaymentCallback(successObject.transactionRef, decimal.Parse(amountString), "INTERSWITCH", response);
                    }
                }
            }
            catch
            {

            }

            _transactionLogService.AddTransactionLogAsync($"Interswitch confirmation", requestObjectString);
            return Redirect("newvisionapp.com?status=complete");
        }

        [HttpPost]
        [Route("{ServiceProvider}/payments")]
        public async Task<IActionResult> PaymentsCallbackAsync(string ServiceProvider)
        {
            BeyonicPaymentCallback beyonicObject = new BeyonicPaymentCallback();
            string callbackRequestBody = new StreamReader(HttpContext.Request.Body).ReadToEndAsync().Result;

            switch (ServiceProvider.ToUpper())
            {
                case "BEYONIC":
                    {
                        try
                        {
                            beyonicObject = JsonConvert.DeserializeObject<BeyonicPaymentCallback>(callbackRequestBody, _jsonSerialiserSettings);
                        }
                        catch (Exception exp)
                        {
                        }

                        if(beyonicObject != null)
                            if (beyonicObject.hook.@event.Equals("collection.received"))
                            {
                                try
                                {
                                    await _transactionService.ProcessSuccesfulPaymentCallback(beyonicObject.data.collection_request.metadata.nv_transactionId, decimal.Parse(beyonicObject.data.amount), ServiceProvider.ToUpper(), callbackRequestBody);
                                }
                                catch (Exception exp)
                                {
                                    _transactionLogService.AddTransactionLogAsync($"BEYONIC Callback Error", exp.Message);
                                }
                            }
                            else if (beyonicObject.data.status.Equals("expired"))
                            {
                                try
                                {
                                    await _transactionService.ProcessFailedPayment(beyonicObject.data.metadata.nv_transactionId, "BEYONIC", callbackRequestBody, "Payment Collection Expired", beyonicObject.data.status);
                                }
                                catch (Exception exp)
                                {
                                    
                                }
                            }
                            else if (beyonicObject.data.status.Equals("successful"))
                            {
                                try
                                {
                                    await _transactionService.ProcessSuccesfulPaymentCallback(beyonicObject.data.metadata.nv_transactionId, decimal.Parse(beyonicObject.data.amount), ServiceProvider.ToUpper(), callbackRequestBody);
                                }
                                catch (Exception exp)
                                {

                                }
                            }
                        _transactionLogService.AddTransactionLogAsync($"BEYONIC Callback", callbackRequestBody);
                        return Ok("BEYONIC");
                    }
                case "DPO":
                    {
                        return Ok("DPO");
                    }
                case "INTERSWITCH":
                    {
                        var interswitchObject = JsonConvert.DeserializeObject<InterswitchCB>(callbackRequestBody);
                        if (interswitchObject.status.Equals("0"))
                        {
                            string amountString = interswitchObject.transactionAmount[0..^2];
                            await _transactionService.ProcessSuccesfulPaymentCallback(interswitchObject.transactionRef, decimal.Parse(amountString), ServiceProvider.ToUpper(), callbackRequestBody);
                            _transactionLogService.AddTransactionLogAsync($"INTERSWITCH Callback", callbackRequestBody);
                            return Ok(new { responseCode  = 0, responseMessage = "success" });
                        }
                        else
                        {
                            await _transactionService.ProcessFailedPayment(interswitchObject.transactionRef, ServiceProvider.ToUpper(), callbackRequestBody, interswitchObject.statusMessage, interswitchObject.statusMessage);
                            _transactionLogService.AddTransactionLogAsync($"INTERSWITCH Callback", callbackRequestBody);
                            return Ok(new { responseCode = 1, responseMessage = "fail" });
                        }
                    }
                case "FLUTTERWAVE":
                    {
                        _transactionLogService.AddTransactionLogAsync($"FLUTTERWAVE Callback", callbackRequestBody);
                        var objData = JsonConvert.DeserializeObject<FlutterwaveCallbackResponse>(callbackRequestBody);

                        if (objData?.data != null)
                        {
                            if(objData.data.status.Equals("successful"))
                                await _transactionService.ProcessSuccesfulPaymentCallback(objData.data.tx_ref, decimal.Parse(objData.data.amount.ToString()), ServiceProvider.ToUpper(), callbackRequestBody);
                            else if(objData.data.status.Equals("failed"))
                                await _transactionService.ProcessFailedPayment(objData.data.tx_ref, "FLUTTERWAVE", callbackRequestBody, objData.data.processor_response, objData.data.processor_response);
                        }
                        return Ok("FLUTTERWAVE");
                    }
                default:
                    return Ok();
            }
        }
    }
}
