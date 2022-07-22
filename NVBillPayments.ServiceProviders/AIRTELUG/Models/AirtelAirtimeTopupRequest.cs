using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace NVBillPayments.ServiceProviders.AIRTELUG.Models
{
    public sealed class AirtelAirtimeTopupRequest
    {
        public TopupRequest topupRequest { get; set; }
        public Credentials credentials { get; set; }
    }

    public class TopupRequest
    {
        public string PhoneNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public string TransactionId { get; set; }

        public string Type { get; set; }
        public decimal Amount { get; set; }
        public int Selector { get; set; }
    }

    public class Credentials
    {
        public string _retailerMSISDN { get; set; }
        public string _retailerPIN { get; set; }
        public string _retailerEXTCODE { get; set; }
        public string _login { get; set; }
        public string _password { get; set; }
        public string _requestGatewayCode { get; set; }
        public string _requestGatewayType { get; set; }
        public string _servicePort { get; set; }
        public string _sourceType { get; set; }
    }
}
