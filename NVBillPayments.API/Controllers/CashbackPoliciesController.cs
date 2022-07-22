using Microsoft.AspNetCore.Mvc;
using NVBillPayments.API.Attributes;
using NVBillPayments.API.ViewModels;
using NVBillPayments.Core;
using NVBillPayments.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.Controllers
{
    [Route("V1/[controller]")]
    [ApiController]
    [AuthKeys]
    public class CashbackPoliciesController : ControllerBase
    {
        private readonly IRepository<CashBackPolicy> _cashBackPolicy;

        public CashbackPoliciesController(IRepository<CashBackPolicy> cashBackPolicy)
        {
            _cashBackPolicy = cashBackPolicy;
        }

        [HttpPost]
        public IActionResult Index(AddCashbackPolicyVM cashbackPolicyVM)
        {
            var policy = new CashBackPolicy()
            {
                CreatedOnUTC = DateTime.UtcNow,
                Name = cashbackPolicyVM.Name,
                Description = cashbackPolicyVM.Description,
                PaymentMethod = cashbackPolicyVM.PaymentMethod,
                Percentage = cashbackPolicyVM.Percentage,
                SystemCategory = cashbackPolicyVM.systemCategory,
                IsActive = true
            };

            _cashBackPolicy.Add(policy);
            _cashBackPolicy.SaveChanges();

            return Ok();
        }
    }
}
