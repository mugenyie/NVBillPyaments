using Newtonsoft.Json;
using NVBillPayments.Core;
using NVBillPayments.Core.Enums;
using NVBillPayments.Core.Interfaces;
using NVBillPayments.Core.Models;
using NVBillPayments.ServiceProviders;
using NVBillPayments.ServiceProviders.AIRTELUG;
using NVBillPayments.ServiceProviders.AIRTELUG.Enums;
using NVBillPayments.ServiceProviders.AIRTELUG.Models;
using NVBillPayments.ServiceProviders.MTNUG;
using NVBillPayments.ServiceProviders.MTNUG.Models;
using NVBillPayments.ServiceProviders.NewVision;
using NVBillPayments.ServiceProviders.Quickteller;
using NVBillPayments.ServiceProviders.Quickteller.Models;
using NVBillPayments.Services.Helpers;
using NVBillPayments.Shared.Enums;
using NVBillPayments.Shared.Helpers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.Services
{
    public class ServiceProviderService : IServiceProviderService
    {
        private readonly IQRCodeService _qrCodeService;
        private readonly IQuicktellerService _quicktellerService;
        private readonly INewVisionService _newvisionService;
        private readonly IMTNService _mtnService;
        private readonly IAirtelService _airtelService;
        private readonly ITransactionLogService _transactionLogService;
        private readonly IRepository<Transaction> _transactionsRepository;
        private readonly INotificationService _notificationService;

        private const string SPONSORS_MSISDN = "256786516121";
        private const string TEST_SPONSORS_MSISDN = "256779999723";
        private const string MTNAirtime_AgentAuthorisationKey_Development = "Tu9FJCDQ7B";
        private const string MTNAirtime_AgentAuthorisationKey_Production = "MNlaV2kfw1";

        public ServiceProviderService(IQRCodeService qrCodeService, IQuicktellerService quicktellerService, INotificationService notificationService, INewVisionService newvisionService, IMTNService mtnService, IAirtelService airtelService, ITransactionLogService transactionLogService, IRepository<Transaction> transactionsRepository)
        {
            _qrCodeService = qrCodeService;
            _quicktellerService = quicktellerService;
            _newvisionService = newvisionService;
            _mtnService = mtnService;
            _airtelService = airtelService;
            _transactionLogService = transactionLogService;
            _transactionsRepository = transactionsRepository;
            _notificationService = notificationService;
        }

        public async Task ProcessOrderAsync(Transaction transaction, bool retry = false)
        {
            if (transaction.PaymentStatus == PaymentStatus.SUCCESSFUL)
            {
                switch (transaction.ServiceProviderId)
                {
                    case "QUICKTELLER":
                        {
                            try
                            {
                                await ProcessQuicktellerTransaction(transaction, retry);
                            }catch(Exception exp)
                            {

                            }
                            return;
                        }
                    case "MTNUG":
                        {
                            if (transaction.ProductId.Equals("MTNUG_AIRTIME"))
                            {
                                await ProcessMTNAirtimeAsync(transaction, true);
                            }
                            else
                            {
                                await ProcessMTNBundlesAysnc(transaction, true);
                            }
                            return;
                        }
                    case "MTNUG_TESTS":
                        {
                            if (transaction.ProductId.Equals("TEST_MTNUG_AIRTIME"))
                            {
                                await ProcessMTNAirtimeAsync(transaction, false);
                            }
                            else
                            {
                                await ProcessMTNBundlesAysnc(transaction, false);
                            }
                            return;
                        }
                    case "AIRTELUG":
                        {
                            if (transaction.ProductId.Equals("AIRTELUG_AIRTIME"))
                            {
                                await ProcessAirtelTransaction(transaction, AirtelTransactionType.AIRTIME, true);
                            }
                            else
                            {
                                await ProcessAirtelTransaction(transaction, AirtelTransactionType.BUNDLES, true);
                            }
                            return;
                        }
                    case "AIRTELUG_TESTS":
                        {
                            if (transaction.ProductId.Equals("AIRTELUG_AIRTIME"))
                            {
                                await ProcessAirtelTransaction(transaction, AirtelTransactionType.AIRTIME, false);
                            }
                            return;
                        }
                    case "NEWVISION":
                        {
                            await RecordSuccesfulTransactionAsync(transaction);
                            return;
                        }
                    default:
                        return;
                }
            }
        }

        private async Task RecordSuccesfulTransactionAsync(Transaction transaction)
        {
            var qrEmailTemplate = await _notificationService.GenerateTransactionEmailTemplateAsync(transaction);
            string customerMessage = $"{transaction.ProductDescription} for {transaction.BeneficiaryMSISDN}, {transaction.CurrencyCode} {Math.Round(transaction.AmountCharged,0)}";

            transaction.OrderStatus = OrderStatus.SUCCESSFUL;
            transaction.OrderStatusMsg = OrderStatus.SUCCESSFUL.ToString();
            transaction.TransactionStatus = TransactionStatus.SUCCESSFUL;
            transaction.TransactionStatusMessage = customerMessage;
            transaction.ModifiedBy = "Orders Processor";
            transaction.ModifiedOnUTC = DateTime.UtcNow;
            transaction.QRCodeUrl = qrEmailTemplate.Item1;
            _transactionsRepository.Update(transaction);
            await _transactionsRepository.SaveChangesAsync();
            _notificationService.SendInAppAsync($"Successful Transaction - {transaction.ProductDescription}", transaction.AccountEmail, customerMessage);
            _notificationService.SendEmailAsync(transaction.ProductDescription, transaction.AccountEmail, qrEmailTemplate.Item2, transaction.AccountName);
        }

        private async Task RecordFailedTransaction(Transaction transaction, string TechnicalErrorMessage, string CustomerFriendlyMessage)
        {
            transaction.TechnicalStatusMessage = TechnicalErrorMessage;
            transaction.TransactionStatusMessage = CustomerFriendlyMessage;
            transaction.OrderStatus = OrderStatus.FAILED;
            transaction.OrderStatusMsg = OrderStatus.FAILED.ToString();
            transaction.TransactionStatus = TransactionStatus.FAILED;
            transaction.ModifiedBy = "Orders Processor";
            transaction.ModifiedOnUTC = DateTime.UtcNow;
            _transactionsRepository.Update(transaction);
            await _transactionsRepository.SaveChangesAsync();
        }

        #region Quickteller
        private async Task ProcessQuicktellerTransaction(Transaction transaction, bool retry)
        {
            IRestResponse<PaymentAdviceResponse> response = null;
            PaymentAdviceRequest paymentAdvice = JsonConvert.DeserializeObject<PaymentAdviceRequest>(transaction.OrderReference);

            if (retry)
            {
                var transactionStatus = await _quicktellerService.TransactionInquiryAsync(paymentAdvice.RequestReference);

                switch (transactionStatus.responseCode)
                {
                    case "9000":
                        goto SUCCESS;
                    case "90009":
                        goto FINISH;
                }

                paymentAdvice.RequestReference = Guid.NewGuid().ToString("N").ToLower().Substring(0, 12);

                ValidateCustomerRequest customerRequest = new ValidateCustomerRequest
                {
                    ProductCode = paymentAdvice.PaymentCode,
                    Amount = paymentAdvice.Amount,
                    CustomerId = paymentAdvice.CustomerId,
                    PhoneNumber = paymentAdvice.PhoneNumber,
                    Email = paymentAdvice.Email,
                    RequestReference = paymentAdvice.RequestReference
                };

                var customerReponse = await _quicktellerService.ValidateCustomerAsync(customerRequest);

                if (customerReponse?.transactionRef != null)
                {
                    PaymentAdviceRequest paymentAdviceRequest = new PaymentAdviceRequest
                    {
                        Amount = customerReponse.amount,
                        PaymentCode = transaction.ProductId,
                        CustomerId = customerReponse.customerId,
                        PhoneNumber = paymentAdvice.PhoneNumber,
                        RequestReference = customerReponse.shortTransactionRef,
                        Email = paymentAdvice.Email,
                        TransactionReference = customerReponse.transactionRef
                    };
                    response = await _quicktellerService.SendPaymentAdviceAsync(paymentAdviceRequest);
                    transaction.OrderReference = JsonConvert.SerializeObject(paymentAdviceRequest);
                }
            }
            else
            {
                response = await _quicktellerService.SendPaymentAdviceAsync(paymentAdvice);
            }
            if(response != null)
            {
                transaction.ServiceProviderHTTPResponseStatusCode = response.StatusCode.ToString();
                transaction.ServiceProviderResponseMetaData = response.Content;

                if (response.Data.responseCode.Equals("9000"))
                    goto SUCCESS;
                //else
                //{
                //    await RecordFailedTransaction(transaction, response.Data.responseMessage, "Error activating service, contact help desk");
                //}
            }

            SUCCESS:
            await RecordSuccesfulTransactionAsync(transaction);
            FINISH:
            return;
        }
        #endregion Quickteller

        #region MTN
        private async Task ProcessMTNAirtimeAsync(Transaction transaction, bool production = false)
        {
            string SponsorMSISDN = production == true ? SPONSORS_MSISDN : TEST_SPONSORS_MSISDN;
            try
            {
                MTNAirtimeRechargeRequest rechargeRequest = new MTNAirtimeRechargeRequest
                {
                    Account = transaction.BeneficiaryMSISDN,
                    AgentCode = SponsorMSISDN,
                    AgentTimeStamp = DateTime.Now,
                    AgentTransNo = transaction.TransactionId.ToString(),
                    AuthKey = production == true ? MTNAirtime_AgentAuthorisationKey_Production : MTNAirtime_AgentAuthorisationKey_Development,
                    Comments = transaction.ProductDescription,
                    Retry = false,
                    Value = transaction.ProductValue,
                    TerminalId = production == true ? "MTNAirtimeRecharge_NewVision" : "MTNAirtimeRecharge_Test_NewVision"
                };

                var response = await _mtnService.RechargeAirtimeAsync(rechargeRequest, production);
                transaction.ServiceProviderHTTPResponseStatusCode = response.Data.http_status_code + " " + response.Data.http_status_code_desc;
                transaction.ServiceProviderResponseMetaData = response.Data.data;

                MTNAirtimeResponse AirtimeRechargeResponse = new XMLStringHelper<MTNAirtimeResponse>().DeserializeXMLString(response.Data.data);

                if (AirtimeRechargeResponse.Response.Resultcode.Equals("0"))
                {
                    await RecordSuccesfulTransactionAsync(transaction);
                }
                else
                {
                    await RecordFailedTransaction(transaction, AirtimeRechargeResponse.Response.Resultdescription, $"Unsuccesful {transaction.ProductDescription} for {transaction.BeneficiaryMSISDN} of amount {Math.Round(transaction.ProductValue, 0)}.");
                }
            }
            catch (Exception exp)
            {
                await RecordFailedTransaction(transaction, exp.Message, $"Error processing Airtime");
            }
        }

        private async Task ProcessMTNBundlesAysnc(Transaction transaction, bool production = false)
        {
            string SponsorMSISDN = production == true ? SPONSORS_MSISDN : TEST_SPONSORS_MSISDN;
            try
            {
                MTNActivateBundle activateBundle = new MTNActivateBundle
                {
                    beneficiaryId = transaction.BeneficiaryMSISDN,
                    subscriptionId = transaction.ProductId,
                    subscriptionName = transaction.ProductDescription,
                    subscriptionProviderId = "CIS",
                    subscriptionPaymentSource = "EVDS",
                    registrationChannel = production == true ? "Mobile App" : "Mobile App Test",
                    sendSMSNotification = false
                };
                var response = await _mtnService.ActivateBundleAsync(SponsorMSISDN, transaction.TransactionId.ToString(), activateBundle, production);

                transaction.ServiceProviderHTTPResponseStatusCode = (int)response.StatusCode + " " + response.StatusCode.ToString();
                transaction.ServiceProviderResponseMetaData = response.Content;

                var successfullResponseData = JsonConvert.DeserializeObject<MTNActivateBundleResponseSuccess>(response.Content);
                var FailureResponseData = JsonConvert.DeserializeObject<MTNActivateBundleResponseFailure>(response.Content);

                if (successfullResponseData?.statusCode == "0000")
                {
                    await RecordSuccesfulTransactionAsync(transaction);
                    return;
                }
                else if (FailureResponseData?.status != null)
                {
                    await RecordFailedTransaction(transaction, FailureResponseData.message, $"Unsuccesful {transaction.ProductDescription} for {transaction.BeneficiaryMSISDN}.");
                }
            }
            catch (Exception exp)
            {
                await RecordFailedTransaction(transaction, exp.Message, "Error Processing Bundles");
            }
        }

        public object GetMTNBundlesTransactionStatus(Transaction transaction, bool production = false)
        {
            string SponsorMSISDN = production == true ? SPONSORS_MSISDN : TEST_SPONSORS_MSISDN;
            return _mtnService.GetTransactionStatus(transaction.TransactionId.ToString().ToLower(), SponsorMSISDN, transaction.CreatedOnUTC);
        }

        public async Task<object> RetryMTNAirtimeRechargeAsync(Transaction transaction, bool production = false)
        {
            string SponsorMSISDN = production == true ? SPONSORS_MSISDN : TEST_SPONSORS_MSISDN;
            MTNAirtimeRechargeRequest rechargeRequest = new MTNAirtimeRechargeRequest
            {
                Account = transaction.BeneficiaryMSISDN,
                AgentCode = SponsorMSISDN,
                AgentTimeStamp = DateTime.Now,
                AgentTransNo = transaction.TransactionId.ToString(),
                AuthKey = production == true ? MTNAirtime_AgentAuthorisationKey_Production : MTNAirtime_AgentAuthorisationKey_Development,
                Comments = transaction.ProductDescription,
                Retry = true,
                Value = transaction.ProductValue,
                TerminalId = production == true ? "MTNAirtimeRecharge_NewVision" : "MTNAirtimeRecharge_Test_NewVision"
            };

            var result = await _mtnService.RechargeAirtimeAsync(rechargeRequest, production);
            return result;
        }
        #endregion MTN

        #region Airtel
        private async Task ProcessAirtelTransaction(Transaction transaction, AirtelTransactionType transactionType, bool production = false)
        {
            try
            {
                TopupRequest topupRequest = new TopupRequest();
                if(transactionType == AirtelTransactionType.AIRTIME)
                {
                    topupRequest = new TopupRequest
                    {
                        PhoneNumber = transaction.BeneficiaryMSISDN,
                        Amount = transaction.ProductValue,
                        CreatedOn = transaction.CreatedOnUTC,
                        TransactionId = transaction.TransactionId.ToString(),
                        Type = "EXRCTRFREQ",
                        Selector = 1
                    };
                }
                else if(transactionType == AirtelTransactionType.BUNDLES)
                {
                    var airtelMeta = JsonConvert.DeserializeObject<AirtelBundleMetaData>(transaction.MetaData);
                    topupRequest = new TopupRequest
                    {
                        PhoneNumber = transaction.BeneficiaryMSISDN,
                        Amount = airtelMeta.Amount,
                        CreatedOn = transaction.CreatedOnUTC,
                        TransactionId = transaction.TransactionId.ToString(),
                        Type = "VASEXTRFREQ",
                        Selector = airtelMeta.Selector
                    };
                }

                var response = await _airtelService.TopupAsync(topupRequest);

                transaction.ServiceProviderHTTPResponseStatusCode = response.http_status_code_desc;
                transaction.ServiceProviderResponseMetaData = response.data;

                AirtelAirtimeTopupResponse AirtimeRechargeResponse = new XMLStringHelper<AirtelAirtimeTopupResponse>().DeserializeXMLString(response.data);
                
                if (AirtimeRechargeResponse.ResponseCode == 200)
                {
                    await RecordSuccesfulTransactionAsync(transaction);
                }
                else
                {
                    await RecordFailedTransaction(transaction, AirtimeRechargeResponse.ResponseMessage, $"Unsuccesful {transaction.ProductDescription} for {transaction.BeneficiaryMSISDN} of amount {Math.Round(transaction.ProductValue, 0)}.");
                }
            }
            catch (Exception exp)
            {
                await RecordFailedTransaction(transaction, exp.Message, $"Error processing Airtel transaction");
            }
        }
        #endregion Airtel
    }
}
