using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace NVBillPayments.ServiceProviders.AIRTELUG.Models
{
    [XmlRoot("COMMAND")]
    public class AirtelAirtimeTopupResponse
    {
        [XmlIgnore]
        public AirtelAirtimeTopupRequest AirtimeTopupRequest { get; set; }

        public string ResponseContent { get; set; }

        [XmlElement("TYPE")]
        public string ResponseType { get; set; }

        [XmlElement("DATE")]
        public string CreatedOn { get; set; }

        [XmlElement("TXNSTATUS")]
        public int ResponseCode { get; set; }

        [XmlElement("TXNID")]
        public string ReferenceId { get; set; }

        [XmlElement("EXTREFNUM")]
        public string TransactionId { get; set; }

        [XmlElement("MESSAGE")]
        public string ResponseMessage { get; set; }
    }
}
