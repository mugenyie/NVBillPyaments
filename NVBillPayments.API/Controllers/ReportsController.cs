using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NVBillPayments.API.ViewModels.Reports;
using NVBillPayments.Core;
using NVBillPayments.Core.Enums;
using NVBillPayments.Core.Models;
using NVBillPayments.ServiceProviders.MTNUG.Models;
using NVBillPayments.Shared.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace NVBillPayments.API.Controllers
{
    [Route("V1/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private readonly IRepository<Transaction> _transactionsRepository;

        public ReportsController(IRepository<Transaction> transactionsRepository)
        {
            _transactionsRepository = transactionsRepository;
        }

        #region MTNBundles
        [HttpGet]
        [Route("mtn-bundles")]
        public IActionResult GetMTNBundlesReports(DateTime currentDate, DateTime previousDate)
        {
            List<MTNBundlesReportViewModel> mtnBundlesReport = new List<MTNBundlesReportViewModel>();

            var transactions = _transactionsRepository.Query()
                .Where(x => x.SystemCategory.Equals(SystemCategory.BUNDLES.ToString()))
                .Where(x => x.ServiceProviderHTTPResponseStatusCode != null)
                .Where(x => x.CreatedOnUTC <= currentDate)
                .Where(x => x.CreatedOnUTC >= previousDate)
                .OrderByDescending(x => x.CreatedOnUTC)
                .ToList();

            transactions.ForEach(x =>
            {
                MTNActivateBundleResponseSuccess successfullResponseData = new MTNActivateBundleResponseSuccess();
                MTNActivateBundleResponseFailure FailureResponseData = new MTNActivateBundleResponseFailure();
                try
                {
                    successfullResponseData = JsonConvert.DeserializeObject<MTNActivateBundleResponseSuccess>(x.ServiceProviderResponseMetaData);
                    FailureResponseData = JsonConvert.DeserializeObject<MTNActivateBundleResponseFailure>(x.ServiceProviderResponseMetaData);
                }catch(Exception exp)
                {

                }

                mtnBundlesReport.Add(new MTNBundlesReportViewModel
                {
                    TransactionId = x.TransactionId.ToString(),
                    HTTPResponseStatusCode = x.ServiceProviderHTTPResponseStatusCode,
                    RequestDateTimeEAT = x.CreatedOnUTC.AddHours(3).ToString("dd/MM/yyyy HH:mm:ss"),
                    BeneficiaryMSISDN = x.BeneficiaryMSISDN,
                    BundlePrice = x.ProductValue,
                    ActivationChannel = $"NewVision {x.CreatedBy}",
                    SubscriptionId = x.ProductId,
                    SubscriptionName = x.ProductDescription,
                    AmountDeducted = successfullResponseData?.amountCharged ?? 0,
                    BundleValidity = x?.ProductValidity,
                    ReponseBodyStatusCode = successfullResponseData?.statusCode ?? FailureResponseData?.status,
                    RequestStatus = successfullResponseData?.statusCode != null ? "SUCCESS" : FailureResponseData?.status != null ? "FAILED" : ""
                });
            });

            return Ok(new { currentDate, previousDate, data = mtnBundlesReport });
        }
        #endregion MTNBundles

        #region Transactionlist
        [HttpGet]
        [Route("transactions/date")]
        public IActionResult GetTransactions(DateTime currentDate, DateTime previousDate)
        {
            var transactions = _transactionsRepository.Query()
                .Where(x => x.CreatedOnUTC.Date <= currentDate)
                .Where(x => x.CreatedOnUTC.Date >= previousDate)
                .OrderByDescending(x => x.CreatedOnUTC)
                .ToList();

            return Ok(
                new { 
                    count = transactions.Count(),
                    from = currentDate,
                    to = previousDate,
                    data = transactions
                });
        }
        #endregion Transactionlist
    }
}
