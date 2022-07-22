using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NVBillPayments.ServiceProviders.Quickteller;
using NVBillPayments.ServiceProviders.Quickteller.Models;
using NVBillPayments.Shared.ViewModels.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuicktellerController : ControllerBase
    {
        private readonly IQuicktellerService _quicktellerService;

        public QuicktellerController(IQuicktellerService quicktellerService)
        {
            _quicktellerService = quicktellerService;
        }

        [HttpGet]
        [Route("categories")]
        public async Task<IActionResult> GetCategoriesAsync()
        {
            var categories = await _quicktellerService.GetCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet]
        [Route("billers")]
        public async Task<IActionResult> GetBillersAsync()
        {
            var billers = await _quicktellerService.GetBillersAsync();
            return Ok(billers);
        }

        [HttpGet]
        [Route("paymentitems")]
        public async Task<IActionResult> GetBillersAsync(string billerId)
        {
            var billers = await _quicktellerService.GetBillerPaymentItemsAsync(billerId);
            return Ok(billers);
        }

        [HttpPost]
        [Route("validatecustomer")]
        public async Task<IActionResult> ValidateCustomer([FromBody] ValidateCustomerRequest validateCustomer)
        {
            var customerRef = await _quicktellerService.ValidateCustomerAsync(validateCustomer);
            return Ok(customerRef);
        }

        [HttpPost]
        [Route("paymentadvice")]
        public async Task<IActionResult> SendPaymentAdvice([FromBody] PaymentAdviceRequest paymentAdvice)
        {
            var paymentReponse = await _quicktellerService.SendPaymentAdviceAsync(paymentAdvice);
            return Ok(paymentReponse);
        }

        [HttpGet]
        [Route("transactioninquiry")]
        public async Task<IActionResult> TransactionInquiry(string transactionRef)
        {
            var transaction = await _quicktellerService.TransactionInquiryAsync(transactionRef);
            return Ok(transaction);
        }

        [HttpGet]
        [Route("balance")]
        public async Task<IActionResult> BalanceInquiry(int inquirytype)
        {
            var transaction = await _quicktellerService.BalanceInquiry(inquirytype);
            return Ok(transaction);
        }

        [HttpGet]
        [Route("JSON")]
        public async Task<IActionResult> FecthBillersJSON()
        {
            List<QuicktellerCategoryVM> categoryVMs = new List<QuicktellerCategoryVM>();
            List<Biller> billers = new List<Biller>();

            var b = await _quicktellerService.GetBillersAsync();
            billers = b?.billers;

            List<string> categoryIds = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" ,"13", "14" };
            billers = billers.Where(b => categoryIds.Contains(b.categoryid)).ToList();

            List<QuicktellerBillerVM> billerVMs = new List<QuicktellerBillerVM>();

            foreach (var biller in billers)
            {
                try
                {
                    var paymentItems = await _quicktellerService.GetBillerPaymentItemsAsync(biller.billerid);
                    List<QuicktellerPaymentItemVM> paymentitemsVm = new List<QuicktellerPaymentItemVM>();
                    paymentItems.paymentitems?.ForEach(p => paymentitemsVm.Add(new QuicktellerPaymentItemVM
                    {
                        productCode = p.paymentCode,
                        name = p.paymentitemname,
                        amount = p.amount,
                        isAmountFixed = p.isAmountFixed,
                        billerId = p.billerid
                    }));

                    billerVMs.Add(new QuicktellerBillerVM
                    {
                        id = biller.billerid,
                        categoryId = biller.categoryid,
                        name = biller.billername,
                        customerfield1 = biller.customerfield1,
                        paymentitems = paymentitemsVm
                    });
                }catch(Exception exp)
                {

                }
            }

            categoryIds.ForEach(c =>
            {
                try
                {
                    categoryVMs.Add(new QuicktellerCategoryVM
                    {
                        id = c,
                        name = billers.Where(b => b.categoryid == c).First().categoryname,
                        description = billers.Where(b => b.categoryid == c).First().categorydescription,
                        billers = billerVMs.Where(x => x.categoryId == c).ToList()
                    });
                }catch(Exception exp)
                {

                }
            });


            QuicktellerVM quickteller = new QuicktellerVM()
            {
                count = categoryVMs.Count,
                categorys = categoryVMs
            };

            var serialised_object = JsonConvert.SerializeObject(quickteller);

            //await _cachingService.Set(paymentitems_cache_key, quickteller, 3600);

            return Ok(quickteller);
        }
    }
}
