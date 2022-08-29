using Microsoft.AspNetCore.Mvc;
using NVBillpayments.WebUI.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillpayments.WebUI.Controllers
{
    public class ReceiptController : Controller
    {
        private IRestClient _restClient;
        private IRestRequest _restRequest;
        public ReceiptController()
        {
            _restClient = new RestClient("https://transactions-api-production.newvisionapp.com");
        }

        public async Task<IActionResult> DetailAsync(string transactionid)
        {
            _restRequest = new RestRequest($"/V1/Orders/{transactionid}");
            var reponse = await _restClient.ExecuteAsync<TransactionDetailVM>(_restRequest);
            return View(reponse.Data);
        }

        [HttpGet]
        [Route("invalidate")]
        public async Task<IActionResult> invalidate(string id, string passphrase)
        {
            if (passphrase.Equals(Environment.GetEnvironmentVariable("BILLPAYMENT_QR_PASSPHRASE")))
            {
                _restRequest = new RestRequest($"/V1/Orders/MarkExpired?transactionId={id}");
                var reponse = await _restClient.ExecuteAsync<string>(_restRequest);
                return Ok(new { message = reponse.Data });
            }
            else
            {
                return Ok(new { message = "Invalid Passphrase" });
            }
            
        }
    }
}
