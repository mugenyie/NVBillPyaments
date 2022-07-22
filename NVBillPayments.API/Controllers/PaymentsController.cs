using Microsoft.AspNetCore.Mvc;
using NVBillPayments.API.ViewModels;
using NVBillPayments.PaymentProviders.Interswitch;
using NVBillPayments.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.Controllers
{
    [Route("V1/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IInterswitchService _interswitchService;
        public PaymentsController(IInterswitchService interswitchService)
        {
            _interswitchService = interswitchService;
        }

        [HttpGet]
        [Route("Options")]
        public IActionResult GetPaymentOptions()
        {
            List<PaymentOptionVM> paymentOptions = new List<PaymentOptionVM>
            {
                //new PaymentOptionVM{Id = PaymentProvider.MOBILEMONEYUG.ToString().ToLower(), Name = "Pay with Mobile Money",ImageUrl = "", IsActive = true},
                //new PaymentOptionVM{Id = PaymentProvider.FLUTTERWAVE.ToString().ToLower(), Name = "Pay with card", ImageUrl = "", IsActive = true},
                new PaymentOptionVM{Id = PaymentProvider.INTERSWITCH.ToString().ToLower(),Name = "Interswitch", ImageUrl = "", IsActive = true},
                new PaymentOptionVM{Id = PaymentProvider.PEGASUS.ToString().ToLower(), Name = "Pegasus", ImageUrl = "", IsActive = true},
            };

            return Ok(paymentOptions);
        }

        [HttpGet]
        [Route("interswitch-status")]
        public async Task<IActionResult> GetInterswitchPaymentStatusAsync(string transactionID)
        {
            var result = await _interswitchService.GetTransactionStatusAsync(transactionID);
            return Ok(result);
        }
    }
}
