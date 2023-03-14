using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NVBillPayments.API.Helpers;
using NVBillPayments.Core.Interfaces;
using NVBillPayments.ServiceProviders.NewVision;
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
        private readonly ICachingService _cachingService;
        private readonly IEventTicketsManagementService _eventTicketsManagementService;

        public QuicktellerController(IQuicktellerService quicktellerService, ICachingService cachingService, IEventTicketsManagementService eventTicketsManagementService)
        {
            _quicktellerService = quicktellerService;
            _cachingService = cachingService;
            _eventTicketsManagementService = eventTicketsManagementService;
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

        //Call every 10 mins to update utilties data 
        [HttpGet]
        [Route("JSON")]
        public async Task<IActionResult> FecthBillersJSON()
        {
            string paymentitems_cache_key = "quickteller_billers";

            string BaseUrl = $"{this.Request.Scheme}://{this.Request.Host}";

            List<QuicktellerCategoryVM> categoryVMs = new List<QuicktellerCategoryVM>();

            //fetch interswitch utilities
            await fetchQuickTellerData(categoryVMs, BaseUrl);

            //fetch event tickets
            await buildEventTicketsDataAsync(categoryVMs, BaseUrl);

            QuicktellerVM quickteller = new QuicktellerVM()
            {
                count = categoryVMs.Count,
                categorys = categoryVMs.OrderBy(x => x.id).ToList()
            };

            if(quickteller.count > 0)
            {
                await _cachingService.Set(paymentitems_cache_key, quickteller, 86400 * 30); //store for 30 days
            }

            return Ok(quickteller);
        }

        private async Task buildEventTicketsDataAsync(List<QuicktellerCategoryVM> categoryVMs, string BaseUrl)
        {
            string tickets_cache_key = "ticket_billers";
            var cachedTicketData = await _cachingService.Get<QuicktellerCategoryVM>(tickets_cache_key);
            if(cachedTicketData != null)
            {
                categoryVMs.Add(cachedTicketData);
            }
            else
            {
                string ticketCategoryId = "01";
                QuicktellerCategoryVM eventData = new QuicktellerCategoryVM
                {
                    id = ticketCategoryId,
                    name = "Tickets",
                    IconUrl = BaseUrl + "/StaticFiles/categorys/tickets.svg",
                    description = "Tickets & Events"
                };

                var eventTickets = await _eventTicketsManagementService.GetEventTicketsAsync(0, 100);
                List<QuicktellerBillerVM> ticketsData = new List<QuicktellerBillerVM>();
                if (eventTickets?.Count > 0)
                {
                    eventTickets.ForEach(eventTicket =>
                    {
                        string billerId = $"00{eventTicket.id}";

                        List<QuicktellerPaymentItemVM> ticketCategories = new List<QuicktellerPaymentItemVM>();

                        //add ticket catgeories
                        eventTicket.ticket_categories?.ForEach(c => ticketCategories.Add(new QuicktellerPaymentItemVM
                        {
                            productCode = c.id.ToString(),
                            billerId = billerId,
                            isAmountFixed = true,
                            name = c.name,
                            amount = c.amount.ToString()
                        }));

                        //add tickets along with categories
                        ticketsData.Add(new QuicktellerBillerVM
                        {
                            name = eventTicket.event_name,
                            categoryId = ticketCategoryId,
                            IconUrl = eventTicket.logo,
                            id = billerId,
                            customerfield1 = eventTicket.customer_field,
                            quantity = true,
                            paymentitems = ticketCategories
                        });
                    });
                }

                eventData.billers = ticketsData;
                
                if(eventData != null)
                {
                    categoryVMs.Add(eventData);
                    await _cachingService.Set(tickets_cache_key, eventData, 60 * 15); //15 mins
                }
            }
        }
    
        private async Task fetchQuickTellerData(List<QuicktellerCategoryVM> categoryVMs, string BaseUrl)
        {
            string quickteller_cache_key = "quickteller_billers_data";
            var cachedQuicktellerData = await _cachingService.Get<List<QuicktellerCategoryVM>>(quickteller_cache_key);
            if (cachedQuicktellerData != null)
            {
                categoryVMs.AddRange(cachedQuicktellerData);
            }
            else
            {
                List<QuicktellerCategoryVM> quicktellerCategories = new List<QuicktellerCategoryVM>();
                List<Biller> billers = new List<Biller>();
                var b = await _quicktellerService.GetBillersAsync();
                billers = b?.billers;

                List<string> categoryIds = new List<string> { "4", "5", "6", "7", "8" }; // remove 11, mobile money - 4, utl
                billers = billers.Where(b => categoryIds.Contains(b.categoryid)).ToList();

                List<QuicktellerBillerVM> billerVMs = new List<QuicktellerBillerVM>();

                foreach (var biller in billers)
                {
                    try
                    {
                        List<string> excludePaymentItems = new List<string> { "28310716", "4432361" };

                        List<string> excludeBillers = new List<string> { "250" };

                        if (excludeBillers.Contains(biller.billerid))
                            continue;

                        var paymentItems = await _quicktellerService.GetBillerPaymentItemsAsync(biller.billerid);
                        List<QuicktellerPaymentItemVM> paymentitemsVm = new List<QuicktellerPaymentItemVM>();
                        paymentItems.paymentitems?.ForEach(p =>
                        {
                            if (!excludePaymentItems.Contains(p.paymentCode))
                            {
                                paymentitemsVm.Add(new QuicktellerPaymentItemVM
                                {
                                    productCode = p.paymentCode,
                                    name = p.paymentitemname,
                                    amount = p.amount,
                                    isAmountFixed = p.isAmountFixed,
                                    billerId = p.billerid
                                });
                            }
                        });

                        billerVMs.Add(new QuicktellerBillerVM
                        {
                            id = biller.billerid,
                            categoryId = biller.categoryid,
                            IconUrl = BaseUrl + BillerIconsHelper.GetBillerIcon(int.Parse(biller.billerid)),
                            name = biller.billername,
                            customerfield1 = biller?.customerfield1 ?? "Account Number",
                            paymentitems = paymentitemsVm
                        });
                    }
                    catch (Exception exp)
                    {

                    }
                }


                categoryIds.ForEach(c =>
                {
                    try
                    {
                        string categoryName = billers.Where(b => b.categoryid == c).First().categoryname;
                        quicktellerCategories.Add(new QuicktellerCategoryVM
                        {
                            id = c,
                            name = categoryName,
                            IconUrl = BaseUrl + BillerIconsHelper.GetCategoryIcon(int.Parse(c)),
                            description = billers.Where(b => b.categoryid == c).First().categorydescription,
                            billers = billerVMs.Where(x => x.categoryId == c).ToList()
                        });
                    }
                    catch (Exception exp)
                    {

                    }
                });
                
                if(quicktellerCategories?.Count > 0)
                {
                    categoryVMs.AddRange(quicktellerCategories);
                    await _cachingService.Set(quickteller_cache_key, quicktellerCategories, 86400); //store for one day
                }
            }

        }
    }
}
