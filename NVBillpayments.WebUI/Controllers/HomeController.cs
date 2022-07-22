using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NVBillpayments.WebUI.Data;
using NVBillpayments.WebUI.Models;
using NVBillPayments.Core.Interfaces;
using NVBillPayments.Shared.ViewModels.Product;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NVBillpayments.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBillPaymentsService _billPaymentsService;
        private readonly ICachingService _cachingService;

        public HomeController(IBillPaymentsService billPaymentsService, ICachingService cachingService)
        {
            _billPaymentsService = billPaymentsService;
            _cachingService = cachingService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var results = await _billPaymentsService.FetchCategoriesAsync();
            return View(results);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
