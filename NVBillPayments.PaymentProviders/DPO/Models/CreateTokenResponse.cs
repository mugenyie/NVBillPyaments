using System.Xml.Serialization;
using System.Collections.Generic;

namespace NVBillPayments.PaymentProviders.DPO.Models
{
	[XmlRoot(ElementName = "API3G")]
	public class CreateTokenResponse
	{
		[XmlElement(ElementName = "Result")]
		public string Result { get; set; }
		[XmlElement(ElementName = "ResultExplanation")]
		public string ResultExplanation { get; set; }
		[XmlElement(ElementName = "TransToken")]
		public string TransToken { get; set; }
		[XmlElement(ElementName = "TransRef")]
		public string TransRef { get; set; }
	}
}
