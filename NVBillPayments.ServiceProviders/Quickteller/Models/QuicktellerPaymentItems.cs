using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.ServiceProviders.Quickteller.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Paymentitem
    {
        public string categoryid { get; set; }
        public string billerid { get; set; }
        public bool isAmountFixed { get; set; }
        public string paymentitemid { get; set; }
        public string paymentitemname { get; set; }
        public string amount { get; set; }
        public string billerType { get; set; }
        public string code { get; set; }
        public string currencyCode { get; set; }
        public string currencySymbol { get; set; }
        public string itemCurrencySymbol { get; set; }
        public string sortOrder { get; set; }
        public string pictureId { get; set; }
        public string paymentCode { get; set; }
    }

    public class QuicktellerPaymentItems
    {
        public List<Paymentitem> paymentitems { get; set; }
    }


}
