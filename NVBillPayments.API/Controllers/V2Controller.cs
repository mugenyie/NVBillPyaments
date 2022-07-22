using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NVBillPayments.API.Attributes;
using NVBillPayments.API.Helpers;
using NVBillPayments.API.ViewModels;
using NVBillPayments.Core.Interfaces;
using NVBillPayments.Core.Models;
using NVBillPayments.ServiceProviders.Quickteller;
using NVBillPayments.ServiceProviders.Quickteller.Models;
using NVBillPayments.Shared.Enums;
using NVBillPayments.Shared.ViewModels.Product;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace NVBillPayments.API.Controllers
{
    [Route("v2")]
    [ApiController]
    public class V2Controller : ControllerBase
    {
        private readonly IQuicktellerService _quicktellerService;
        private readonly ITransactionService _transactionService;
        private readonly ICachingService _cachingService;

        public V2Controller(IQuicktellerService quicktellerService, ICachingService cachingService, ITransactionService transactionService)
        {
            _quicktellerService = quicktellerService;
            _cachingService = cachingService;
            _transactionService = transactionService;
        }

        [HttpGet]
        [Route("Categories")]
        public IActionResult FetchCategories()
        {
            string _billersFilePath = Path.Combine(AppContext.BaseDirectory, @"StaticFiles", $"quickteller.json");

            string quickteller_json_string = "";

            using (var stream = System.IO.File.OpenRead(_billersFilePath))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    quickteller_json_string = reader.ReadToEnd();
                }
            }

            QuickTellerSimpleVM quickteller = JsonConvert.DeserializeObject<QuickTellerSimpleVM>(quickteller_json_string);

            string BaseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
            string IconPlaceHolder = "/StaticFiles/placeholder.jpg";
            quickteller.categorys.ForEach(x =>
            {
                x.IconUrl = BaseUrl + x.IconUrl;
                x.billers.ForEach(b =>
                {
                    string billerIcon = string.IsNullOrEmpty(b.IconUrl) ? IconPlaceHolder : b.IconUrl;
                    b.IconUrl = BaseUrl + billerIcon;
                });
            });

            return Ok(quickteller);
        }

        [HttpGet]
        [Route("Billers")]
        public IActionResult FetchCategories(string billerId)
        {
            string _billersFilePath = Path.Combine(AppContext.BaseDirectory, @"StaticFiles", $"quickteller.json");

            string quickteller_json_string = "";

            using (var stream = System.IO.File.OpenRead(_billersFilePath))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    quickteller_json_string = reader.ReadToEnd();
                }
            }

            QuicktellerVM quickteller = JsonConvert.DeserializeObject<QuicktellerVM>(quickteller_json_string);

            var billers = quickteller.categorys.SelectMany(x => x.billers).ToList();

            var biller = billers.Where(x => x.id == billerId).FirstOrDefault();

            if(biller != null)
            {
                biller.paymentitems.ForEach(p =>
                {
                    StringWriter billerName = new StringWriter();
                    HttpUtility.HtmlDecode(p.name, billerName);
                    p.name = billerName.ToString();

                    p.amount = p.amount.Length > 3 ? p.amount.Substring(0, p.amount.Length - 2) : p.amount;
                });

                return Ok(biller);
            }
            else
            {
                return new NotFoundObjectResult("Biller Not Found");
            }
        }

        [HttpGet]
        [Route("Product")]
        public IActionResult FetchProductDetail(string productCode)
        {
            var paymentitem = GetProduct(productCode);
            return Ok(paymentitem);
        }

        private QuicktellerPaymentItemVM GetProduct(string productCode)
        {
            string _billersFilePath = Path.Combine(AppContext.BaseDirectory, @"StaticFiles", $"quickteller.json");

            string quickteller_json_string = "";

            using (var stream = System.IO.File.OpenRead(_billersFilePath))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    quickteller_json_string = reader.ReadToEnd();
                }
            }

            QuicktellerVM quickteller = JsonConvert.DeserializeObject<QuicktellerVM>(quickteller_json_string);

            var billers = quickteller.categorys.SelectMany(x => x.billers);

            var paymentitem = billers.SelectMany(x => x.paymentitems).Where(p => p.productCode == productCode).FirstOrDefault();

            StringWriter billerName = new StringWriter();
            HttpUtility.HtmlDecode(paymentitem.name, billerName);
            paymentitem.name = billerName.ToString();

            paymentitem.amount = CleanAmountString(paymentitem.amount);

            return paymentitem;
        }

        private string CleanAmountString(string amount)
        {
            return amount.Length > 3 ? amount.Substring(0, amount.Length - 2) : amount;
        }

        [HttpGet]
        [Route("DetailedCategories")]
        public IActionResult FetchBillers()
        {
            string _billersFilePath = Path.Combine(AppContext.BaseDirectory, @"StaticFiles", $"quickteller.json");

            string quickteller_json_string = "";

            using (var stream = System.IO.File.OpenRead(_billersFilePath))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    quickteller_json_string = reader.ReadToEnd();
                }
            }

            QuicktellerVM quickteller = JsonConvert.DeserializeObject<QuicktellerVM>(quickteller_json_string);

            string BaseUrl = $"{this.Request.Scheme}://{this.Request.Host}";
            string IconPlaceHolder = "/StaticFiles/placeholder.jpg";
            quickteller.categorys.ForEach(x =>
            {
                x.IconUrl = BaseUrl + x.IconUrl;
                x.billers.ForEach(b =>
                {
                    string billerIcon = string.IsNullOrEmpty(b.IconUrl) ? IconPlaceHolder : b.IconUrl;
                    b.IconUrl = BaseUrl + billerIcon;
                });
            });

            return Ok(quickteller);
        }

        [HttpPost]
        [Route("validatecustomer")]
        public async Task<IActionResult> ValidateCustomer([FromBody] ValidateCustomerVM customerVM)
        {
            ValidateCustomerRequest customerRequest = new ValidateCustomerRequest
            {
                ProductCode = customerVM.productCode,
                Amount = int.Parse(customerVM.amount),
                CustomerId = customerVM.customerfield1,
                PhoneNumber = customerVM?.phoneNumber?.ValidatePhoneNumber(),
                Email = customerVM.email,
                RequestReference = Guid.NewGuid().ToString("N").ToLower().Substring(0,12)
            };

            switch (customerVM.productCode)
            {
                case "1":
                case "2": //vision tickets
                    {
                        var paymentitem = GetProduct(customerVM.productCode);
                        string transactionRef = "custom-" + Guid.NewGuid().ToString();
                        ValidatedCustomerReponse customerReponse = new ValidatedCustomerReponse
                        {
                            amount = int.Parse(paymentitem.amount),
                            shortTransactionRef = transactionRef,
                            transactionRef = transactionRef,
                            customerName = customerVM.customerfield1,
                            paymentItem = paymentitem.name
                        };
                        ValidatedCustomerReponseVM customerReponseVM = new ValidatedCustomerReponseVM
                        {
                            customerName = customerVM.customerfield1,
                            paymentItem = paymentitem.name,
                            customerId = customerVM.customerfield1,
                            amount = int.Parse(customerVM.amount),
                            surcharge = 0,
                            excise = 0,
                            totalAmount = int.Parse(customerVM.amount),
                            transactionRef = transactionRef
                        };

                        TransactionReferenceVM transaction = new TransactionReferenceVM
                        {
                            serviceProvider = Shared.Enums.ServiceProvider.NEWVISION,
                            customerRequest = customerRequest,
                            customerReponse = customerReponse
                        };

                        await _cachingService.Set($"transactionRef-{customerReponse.transactionRef.ToLower()}", transaction, 86400);
                        return Ok(customerReponseVM);
                    }
                default:
                    {
                        var customerReponse = await _quicktellerService.ValidateCustomerAsync(customerRequest);
                        if (customerReponse?.transactionRef != null)
                        {
                            ValidatedCustomerReponseVM customerReponseVM = new ValidatedCustomerReponseVM
                            {
                                customerName = customerReponse?.customerName,
                                paymentItem = customerReponse?.paymentItem,
                                customerId = customerReponse?.customerId,
                                amount = customerReponse.isAmountFixed == "1" ? int.Parse(CleanAmountString(customerReponse.amount.ToString())) : customerReponse.amount,
                                surcharge = customerReponse.surcharge,
                                excise = customerReponse.excise,
                                totalAmount = customerReponse.isAmountFixed == "1" ? int.Parse(CleanAmountString(customerReponse.amount.ToString())) : customerReponse.amount,
                                transactionRef = customerReponse.transactionRef
                            };

                            TransactionReferenceVM transactionRef = new TransactionReferenceVM
                            {
                                serviceProvider = Shared.Enums.ServiceProvider.QUICKTELLER,
                                customerRequest = customerRequest,
                                customerReponse = customerReponse
                            };

                            await _cachingService.Set($"transactionRef-{customerReponse.transactionRef.ToLower()}", transactionRef, 86400);
                            return Ok(customerReponseVM);
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status503ServiceUnavailable, "Validation Service Currently Unavailable");
                        }
                    }
            }
        }

        [HttpGet]
        [Route("paymentmethods")]
        public IActionResult PaymentMethods()
        {
            List<PaymentMethod> methods = new List<PaymentMethod>
            {
                new PaymentMethod
                {
                    Id = "momo",
                    Name = "Pay with Mobile Money",
                    InputFields = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("PhoneNumber", "Pay with PhoneNumber")
                    }
                },
                new PaymentMethod
                {
                    Id = "card",
                    Name = "Pay with Visa/Mastercard",
                    InputFields = new List<KeyValuePair<string, string>>()
                },
            };

            return Ok(methods);
        }

        [HttpPost]
        [Route("paymentadvice")]
        public async Task<IActionResult> SendPaymentAdvice([FromBody] PaymentAdviceRequestVM paymentRequest)
        {
            string paymentLink = "";
            string sponsorId = paymentRequest?.SponsorId != null ? paymentRequest.SponsorId?.ValidatePhoneNumber() : "";

            var transactionRequest = await _cachingService.Get<TransactionReferenceVM>($"transactionRef-{paymentRequest.TransactionReference.ToLower()}");
            if (transactionRequest != null)
            {
                PaymentAdviceRequest paymentAdviceRequest = new PaymentAdviceRequest
                {
                    Amount = transactionRequest.customerReponse.amount,
                    PaymentCode = transactionRequest.customerRequest.ProductCode,
                    CustomerId = transactionRequest.customerRequest.CustomerId,
                    PhoneNumber = transactionRequest.customerRequest.PhoneNumber,
                    RequestReference = transactionRequest.customerReponse.shortTransactionRef,
                    Email = transactionRequest.customerRequest.Email,
                    TransactionReference = transactionRequest.customerReponse.transactionRef
                };

                //record transaction take note of service provider as QUICKTELLER
                Transaction _transaction = new Transaction
                {
                    AccountEmail = transactionRequest.customerRequest.Email,
                    AccountMSISDN = transactionRequest.customerRequest.PhoneNumber,
                    AccountName = transactionRequest.customerReponse.customerName,
                    ProductId = paymentAdviceRequest.PaymentCode,
                    ProductDescription = transactionRequest.customerReponse.paymentItem,
                    ServiceProviderId = transactionRequest.serviceProvider.ToString(),
                    OrderReference = JsonConvert.SerializeObject(paymentAdviceRequest),
                    PaymentReference = paymentLink,
                    CreatedOnUTC = DateTime.UtcNow,
                    BeneficiaryMSISDN = paymentAdviceRequest.CustomerId,
                    AmountToCharge = paymentAdviceRequest.Amount,
                    SponsorMSISDN = sponsorId
                };

                var transaction = await _transactionService.SaveTransactionAsync(_transaction);
                if(transaction != null)
                {
                    if (paymentRequest.PaymentMethod.Equals("card"))
                    {
                        paymentLink = await _transactionService.CreateCardPaymentLinkV2(transaction, PaymentProvider.INTERSWITCH);
                    }
                    else if (paymentRequest.PaymentMethod.Equals("momo"))
                    {
                        if (string.IsNullOrEmpty(sponsorId))
                            return new BadRequestObjectResult("SponsorId must not be empty");
                        await _transactionService.InititateMobilePaymentCollectionAsync(transaction);
                    }
                }

                string PaymentMessage = "";
                switch (paymentRequest.PaymentMethod)
                {
                    case "momo":
                        PaymentMessage += "Please follow prompts on your mobile to complete payment";
                        break;
                    case "card":
                        PaymentMessage += "Complete payment with our secure card checkout";
                        break;
                }

                return StatusCode(StatusCodes.Status200OK, new { Message = PaymentMessage, PaymentLink = paymentLink });
            }
            else
            {
                return new NotFoundObjectResult("Transaction Reference Not Found");
            }
        }
    }
}
