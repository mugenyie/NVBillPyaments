﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NVBillPayments.API.Attributes;
using NVBillPayments.API.Helpers;
using NVBillPayments.API.ViewModels;
using NVBillPayments.Core.Enums;
using NVBillPayments.Core.Interfaces;
using NVBillPayments.Shared.Enums;
using NVBillPayments.Shared.ViewModels.Transaction;
using System;
using System.Threading.Tasks;

namespace NVBillPayments.API.Controllers
{
    [Route("V1/[controller]")]
    [ApiController]
    //[AuthKeys]
    public class OrdersController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ITransactionLogService _transactionLogService;
        private readonly IServiceProviderService _serviceProvider;

        public OrdersController(ITransactionService transactionService, ITransactionLogService transactionLogService, IServiceProviderService serviceProvider)
        {
            _transactionService = transactionService;
            _transactionLogService = transactionLogService;
            _serviceProvider = serviceProvider;
        }

        [HttpPost]
        public async Task<IActionResult> AddTransactionAsync(AddTransactionVM transaction)
        {
            try
            {
                if (transaction.PaymentMethod.Equals("card", StringComparison.OrdinalIgnoreCase) || transaction?.SponsorId == null)
                {
                    var _transaction = new AddTransactionVM
                    {
                        UserAccount = new UserAccount
                        {
                            Email = transaction.UserAccount.Email,
                            Name = transaction.UserAccount.Name,
                            PhoneNumber = transaction.UserAccount.PhoneNumber
                        },
                        Amount = transaction.Amount,
                        ProductId = transaction.ProductId,
                        PaymentMethod = "card",
                        BeneficiaryId = transaction.BeneficiaryId.ValidatePhoneNumber()
                    };

                    var _paymentSource = await _transactionService.CreateCardPaymentLink(_transaction, PaymentProvider.INTERSWITCH);
                    if (!string.IsNullOrEmpty(_paymentSource))
                        return Ok(_paymentSource);
                    else
                        return new BadRequestObjectResult("Error processing transaction");
                }
                if (transaction.PaymentMethod.Equals("pegasus", StringComparison.OrdinalIgnoreCase))
                {
                    var _transaction = new AddTransactionVM
                    {
                        UserAccount = new UserAccount
                        {
                            Email = transaction.UserAccount.Email,
                            Name = transaction.UserAccount.Name,
                            PhoneNumber = transaction.UserAccount.PhoneNumber
                        },
                        Amount = transaction.Amount,
                        ProductId = transaction.ProductId,
                        PaymentMethod = "card",
                        BeneficiaryId = transaction.BeneficiaryId.ValidatePhoneNumber()
                    };

                    var _paymentSource = await _transactionService.CreateCardPaymentLink(_transaction, PaymentProvider.PEGASUS);
                    if (!string.IsNullOrEmpty(_paymentSource))
                        return Ok(_paymentSource);
                    else
                        return new BadRequestObjectResult("Error processing transaction");
                }
                else
                {
                    var _transaction = new AddTransactionVM
                    {
                        UserAccount = new UserAccount
                        {
                            Email = transaction.UserAccount.Email,
                            Name = transaction.UserAccount.Name,
                            PhoneNumber = transaction.UserAccount.PhoneNumber
                        },
                        Amount = transaction.Amount,
                        ProductId = transaction.ProductId,
                        BeneficiaryId = transaction.BeneficiaryId.ValidatePhoneNumber(),
                        SponsorId = transaction.SponsorId.ValidatePhoneNumber()
                    };
                    //_transactionService.AddTransactionToQueueAsync(_transaction);
                    await _transactionService.ProcessTransactionAsync(_transaction);
                }
            }
            catch (Exception exp)
            {
                _transactionLogService.AddTransactionLogAsync("Error Processing Transaction", exp.Message);
                return new BadRequestObjectResult("Error Processing Transaction");
            }
            return Ok();
        }

        [HttpPost]
        [Route("ProcessQueueOrder")]
        public async Task<IActionResult> ProcessQueueTransactionAsync(QueueMessage queueMessage)
        {
            try
            {
                var transaction = JsonConvert.DeserializeObject<AddTransactionVM>(queueMessage.OrderInQueue);
                await _transactionService.ProcessTransactionAsync(transaction);
            }
            catch (Exception exp)
            {
                _transactionLogService.AddTransactionLogAsync("Error Processing Transaction", exp.Message);
            }
            return Ok();
        }

        [HttpPost]
        [Route("RetryTransaction")]
        public async Task<IActionResult> RetryTransactionAsync(string transactionId)
        {
            try
            {
                var transaction = _transactionService.GetById(transactionId);
                if (transaction != null)
                {
                    if (transaction.PaymentStatus == PaymentStatus.SUCCESSFUL)
                        await _serviceProvider.ProcessOrderAsync(transaction, true);
                    else
                        await _transactionService.InititateMobilePaymentCollectionAsync(transaction);
                    return Ok();
                }
                else
                {
                    return new NotFoundObjectResult("Transaction Not Found!");
                }
            }catch(Exception exp)
            {
                return new BadRequestObjectResult(exp.Message);
            }
        }

        [HttpGet]
        [Route("full-detail/{TransactionId}")]
        public IActionResult GetFullDetail(string TransactionId)
        {
            var transaction = _transactionService.GetById(TransactionId);
            return Ok(transaction);
        }

        [HttpGet]
        [Route("{TransactionId}")]
        public IActionResult GetTRansactionInformation(string TransactionId)
        {
            var transaction = _transactionService.GetById(TransactionId);
            if(transaction != null)
            {
                TransactionVM transactionVM = new TransactionVM
                {
                    TransactionId = transaction.TransactionId.ToString().ToUpper(),
                    AccountMSISDN = transaction.AccountMSISDN,
                    AccountName = transaction.AccountName,
                    AccountEmail = transaction.AccountEmail,
                    SponsorMSISDN = transaction.SponsorMSISDN,
                    BeneficiaryMSISDN = transaction.BeneficiaryMSISDN,
                    ProductDescription = transaction.ProductDescription,
                    PaymentProvider = transaction.PaymentProviderId,
                    PaymentStatus = transaction.PaymentStatusMsg,
                    OrderStatus = transaction.OrderStatusMsg,
                    TransactionStatus = transaction.TransactionStatusMessage,
                    CreatedOnUTC = transaction.CreatedOnUTC.ToString("yyyy-MM-dd HH:mm:ss"),
                    ProductValue = transaction.ProductValue,
                    AmountCharged = transaction.AmountCharged,
                    IsExpired = transaction.IsExpired
                };
                return Ok(transactionVM);
            }
            else
            {
                return new NotFoundObjectResult($"Transaction -{TransactionId}- not found");
            }
        }

        [HttpGet]
        [Route("MarkExpired")]
        public IActionResult MarkExpired(string transactionId)
        {
            var transaction = _transactionService.GetById(transactionId);
            if(transaction != null)
            {
                var result = _transactionService.MarkExpired(transaction);
                return Ok("Success");
            }
            else
            {
                return Ok("Failed");
            }
        }

        [HttpGet]
        [Route("History")]
        public IActionResult GetOrdersHistory(string email, string userId, string status, string category, int limit = 10, int offset = 0)
        {
            var history = _transactionService.GetOrders(email, userId, status, category, limit, offset);
            return Ok(history);
        }

        [HttpGet]
        [Route("transaction-receipt")]
        public IActionResult getTransactionReceipt(string transactionId)
        {
            var transaction = _transactionService.GetById(transactionId);
            if(transaction != null)
            {
                return Ok(new
                {
                    TransactionRef = transaction.TransactionId.ToString(),
                    CustomerRef = transaction.AccountName,
                    Date = transaction.CreatedOnUTC.ToShortDateString(),
                    ProductDescription = transaction.ProductDescription,
                    AmountCharged = transaction.AmountCharged,
                    QRCodeUrl = transaction.QRCodeUrl ?? ""
                });
            }
            return new NotFoundObjectResult("Transaction not found");
        }

        [HttpGet]
        [Route("Recommended")]
        public IActionResult GetRecommended(string email, string category, bool singlePerCategory = false, int limit = 10, int offset = 0)
        {
            var history = _transactionService.GetRecommendedOrders(email, category, singlePerCategory, limit, offset);
            return Ok(history);
        }

        [HttpGet]
        [Route("ThirdpartyTransactionStatus")]
        public async Task<IActionResult> GetThirdpartyTransactionStatusAsync(string transactionId)
        {
            var status = await _transactionService.GetThirdpartyTransactionStatusAsync(transactionId);
            return Ok(status);
        }
    }
}
