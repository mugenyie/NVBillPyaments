using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NVBillpayments.WebUI.Data;
using NVBillpayments.WebUI.Models;
using NVBillPayments.Core.Interfaces;
using NVBillPayments.ServiceProviders.Quickteller.Models;
using NVBillPayments.Shared.ViewModels.Product;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NVBillpayments.WebUI.Controllers
{
    public class BillersController : Controller
    {
        private readonly IBillPaymentsService _billPaymentsService;

        public BillersController(IBillPaymentsService billPaymentsService)
        {
            _billPaymentsService = billPaymentsService;
        }

        public async Task<IActionResult> IndexAsync(string billerId)
        {
            var billersData = await _billPaymentsService.FetchBillerDetails(billerId);
            return View(billersData);
        }

        [HttpPost]
        public async Task<IActionResult> UserSelectAsync([FromForm] UserSelect UserSelect)
        {
            //display input field and selection
            var productData = await _billPaymentsService.FetchProductInfo(UserSelect.PaymentItemSelection);
            UserSelect.ProductData = productData;
            return View(UserSelect);
        }

        [HttpPost]
        public async Task<IActionResult> ValidateCustomerAsync([FromForm] UserSelect validateCustomer)
        {
            ValidateCustomer customer = new ValidateCustomer
            {
                customerfield1 = validateCustomer.CustomerField,
                productCode = validateCustomer.PaymentItemSelection,
                amount = validateCustomer.Amount,
                email = validateCustomer.Email,
                phoneNumber = validateCustomer.PhoneNumber
            };
            CustomerValidationResponse customerValidation = await _billPaymentsService.ValidateCustomerAsync(customer);
            ViewBag.PaymentBody = JsonConvert.SerializeObject(customerValidation);
            return View(customerValidation);
        }

        [HttpPost]
        public async Task<IActionResult> PostPaymentAdviceAsync([FromForm] PaymentBody payment)
        {
            
            StringWriter jsonString = new StringWriter();
            HttpUtility.HtmlDecode(payment.Json, jsonString);

            var jsonBody = JsonConvert.DeserializeObject<ValidatedCustomerReponse>(jsonString.ToString());

            PostPaymentAdvice paymentAdvice = new PostPaymentAdvice
            {
                transactionReference = jsonBody.transactionRef,
                paymentMethod = "card"
            };
            var reponse = await _billPaymentsService.PostPaymentAdvice(paymentAdvice);
            string redirectURI = reponse.paymentLink;
            ViewBag.PaymentLink = redirectURI;
            return View();
        }
    }

    public class PaymentBody
    {
        public string Json { get; set; }
    }
}
