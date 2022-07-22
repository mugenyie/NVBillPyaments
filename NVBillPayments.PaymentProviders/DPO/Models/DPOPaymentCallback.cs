using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace NVBillPayments.PaymentProviders.DPO.Models
{
	[XmlRoot(ElementName = "API3G")]
	public class DPOPaymentCallback
	{
		[XmlElement(ElementName = "Result")]
		public string Result { get; set; }
		[XmlElement(ElementName = "ResultExplanation")]
		public string ResultExplanation { get; set; }
		[XmlElement(ElementName = "TransactionToken")]
		public string TransactionToken { get; set; }
		[XmlElement(ElementName = "TransactionRef")]
		public string TransactionRef { get; set; }
		[XmlElement(ElementName = "CustomerName")]
		public string CustomerName { get; set; }
		[XmlElement(ElementName = "CustomerEmail")]
		public string CustomerEmail { get; set; }
		[XmlElement(ElementName = "CustomerCredit")]
		public string CustomerCredit { get; set; }
		[XmlElement(ElementName = "CustomerCreditType")]
		public string CustomerCreditType { get; set; }
		[XmlElement(ElementName = "TransactionApproval")]
		public string TransactionApproval { get; set; }
		[XmlElement(ElementName = "TransactionCurrency")]
		public string TransactionCurrency { get; set; }
		[XmlElement(ElementName = "TransactionAmount")]
		public string TransactionAmount { get; set; }
		[XmlElement(ElementName = "FraudAlert")]
		public string FraudAlert { get; set; }
		[XmlElement(ElementName = "FraudExplnation")]
		public string FraudExplnation { get; set; }
		[XmlElement(ElementName = "TransactionNetAmount")]
		public string TransactionNetAmount { get; set; }
		[XmlElement(ElementName = "TransactionSettlementDate")]
		public string TransactionSettlementDate { get; set; }
		[XmlElement(ElementName = "TransactionRollingReserveAmount")]
		public string TransactionRollingReserveAmount { get; set; }
		[XmlElement(ElementName = "TransactionRollingReserveDate")]
		public string TransactionRollingReserveDate { get; set; }
		[XmlElement(ElementName = "CustomerPhone")]
		public string CustomerPhone { get; set; }
		[XmlElement(ElementName = "CustomerCountry")]
		public string CustomerCountry { get; set; }
		[XmlElement(ElementName = "CustomerAddress")]
		public string CustomerAddress { get; set; }
		[XmlElement(ElementName = "CustomerCity")]
		public string CustomerCity { get; set; }
		[XmlElement(ElementName = "CustomerZip")]
		public string CustomerZip { get; set; }
		[XmlElement(ElementName = "MobilePaymentRequest")]
		public string MobilePaymentRequest { get; set; }
		[XmlElement(ElementName = "AccRef")]
		public string AccRef { get; set; }
	}
}
