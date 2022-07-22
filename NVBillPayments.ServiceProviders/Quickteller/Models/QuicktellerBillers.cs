using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.ServiceProviders.Quickteller.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class PageFlowInfo
    {
        public bool allowRetry { get; set; }
        public string finishButtonName { get; set; }
        public bool performInquiry { get; set; }
        public string startPage { get; set; }
        public bool usesPaymentItems { get; set; }
    }

    public class Biller
    {
        public string categoryid { get; set; }
        public string categoryname { get; set; }
        public string categorydescription { get; set; }
        public string billerid { get; set; }
        public string billername { get; set; }
        public string customerfield1 { get; set; }
        public string customerfield2 { get; set; }
        public string supportemail { get; set; }
        public string paydirectProductId { get; set; }
        public string paydirectInstitutionId { get; set; }
        public string narration { get; set; }
        public string shortName { get; set; }
        public string surcharge { get; set; }
        public string currencyCode { get; set; }
        public string quickTellerSiteUrlName { get; set; }
        public string amountType { get; set; }
        public string currencySymbol { get; set; }
        public string customSectionUrl { get; set; }
        public string logoUrl { get; set; }
        public string networkId { get; set; }
        public string productCode { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public PageFlowInfo pageFlowInfo { get; set; }
    }

    public class QuicktellerBillers
    {
        public List<Biller> billers { get; set; }
    }


}
