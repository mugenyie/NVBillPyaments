using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NVBillPayments.Core.Interfaces;
using NVBillPayments.Services.Helpers;
using NVBillPayments.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IQRCodeService _qrCodeService;
        private readonly INotificationService _notificationService;

        public TestController(IQRCodeService qrCodeService, ITransactionService transactionService, INotificationService notificationService)
        {
            _transactionService = transactionService;
            _notificationService = notificationService;
            _qrCodeService = qrCodeService;
        }

        [HttpGet]
        [Route("SendEmail")]
        public async Task<IActionResult> SendEmailAsync(string transactionId)
        {
            var transaction = _transactionService.GetById(transactionId);

            var qrEmailTemplate = await _notificationService.GenerateTransactionEmailTemplateAsync(transaction);

            _notificationService.SendEmailAsync(transaction.ProductDescription, transaction.AccountEmail, qrEmailTemplate.Item2, transaction.AccountName);
            return Ok();
        }

        [HttpGet]
        [Route("TransactionQR")]
        public IActionResult GenerateQR(string transactionId)
        {
            string validationUrl = $"https://billpayments.newvisionapp.com/Transactions/{transactionId}";
            return Ok(QRCodeHelper.Generate(validationUrl));
        }
    }
}
