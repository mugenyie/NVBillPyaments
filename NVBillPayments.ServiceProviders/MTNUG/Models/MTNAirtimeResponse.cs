using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace NVBillPayments.ServiceProviders.MTNUG.Models
{
	[XmlRoot(ElementName = "agiml")]
	public class MTNAirtimeResponse
    {
		[XmlElement(ElementName = "header")]
		public Header Header { get; set; }
		[XmlElement(ElementName = "response")]
		public Response Response { get; set; }
	}

	[XmlRoot(ElementName = "header")]
	public class Header
	{
		[XmlElement(ElementName = "responsetype")]
		public string Responsetype { get; set; }
	}

	[XmlRoot(ElementName = "details")]
	public class Details
	{
		[XmlElement(ElementName = "strategy")]
		public string Strategy { get; set; }
		[XmlElement(ElementName = "description")]
		public string Description { get; set; }
		[XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Xsi { get; set; }
		[XmlAttribute(AttributeName = "java", Namespace = "http://www.w3.org/2000/xmlns/")]
		public string Java { get; set; }
		[XmlAttribute(AttributeName = "id")]
		public string Id { get; set; }
		[XmlAttribute(AttributeName = "tax")]
		public string Tax { get; set; }
		[XmlAttribute(AttributeName = "tax-amount")]
		public string Taxamount { get; set; }
		[XmlAttribute(AttributeName = "amount")]
		public string Amount { get; set; }
		[XmlAttribute(AttributeName = "config-item-id")]
		public string Configitemid { get; set; }
		[XmlAttribute(AttributeName = "priority")]
		public string Priority { get; set; }
		[XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
		public string Type { get; set; }
	}

	[XmlRoot(ElementName = "taxrecord")]
	public class Taxrecord
	{
		[XmlElement(ElementName = "details")]
		public List<object> Details { get; set; }
		[XmlElement(ElementName = "code")]
		public string Code { get; set; }
		[XmlElement(ElementName = "date-calculated")]
		public string Datecalculated { get; set; }
		[XmlElement(ElementName = "internal-precision")]
		public string Internalprecision { get; set; }
		[XmlElement(ElementName = "description")]
		public string Description { get; set; }
		[XmlAttribute(AttributeName = "serial")]
		public string Serial { get; set; }
		[XmlAttribute(AttributeName = "tax-amount")]
		public string Taxamount { get; set; }
		[XmlAttribute(AttributeName = "amount")]
		public string Amount { get; set; }
		[XmlAttribute(AttributeName = "precision")]
		public string Precision { get; set; }
		[XmlAttribute(AttributeName = "config-id")]
		public string Configid { get; set; }
		[XmlAttribute(AttributeName = "precise-total")]
		public string Precisetotal { get; set; }
		[XmlAttribute(AttributeName = "total")]
		public string Total { get; set; }
		[XmlAttribute(AttributeName = "precise-amount")]
		public string Preciseamount { get; set; }
		[XmlAttribute(AttributeName = "precise-tax-amount")]
		public string Precisetaxamount { get; set; }
	}

	[XmlRoot(ElementName = "physicaladdress")]
	public class Physicaladdress
	{
		[XmlElement(ElementName = "country")]
		public string Country { get; set; }
		[XmlElement(ElementName = "state")]
		public string State { get; set; }
		[XmlElement(ElementName = "city")]
		public string City { get; set; }
	}

	[XmlRoot(ElementName = "response")]
	public class Response
	{
		[XmlElement(ElementName = "taxrecord")]
		public Taxrecord Taxrecord { get; set; }
		[XmlElement(ElementName = "physicaladdress")]
		public Physicaladdress Physicaladdress { get; set; }
		[XmlElement(ElementName = "resultcode")]
		public string Resultcode { get; set; }
		[XmlElement(ElementName = "value")]
		public string Value { get; set; }
		[XmlElement(ElementName = "taxvalue")]
		public string Taxvalue { get; set; }
		[XmlElement(ElementName = "timestamp")]
		public string Timestamp { get; set; }
		[XmlElement(ElementName = "account")]
		public string Account { get; set; }
		[XmlElement(ElementName = "transno")]
		public string Transno { get; set; }
		[XmlElement(ElementName = "expirydate")]
		public string Expirydate { get; set; }
		[XmlElement(ElementName = "agentcode")]
		public string Agentcode { get; set; }
		[XmlElement(ElementName = "agentid")]
		public string Agentid { get; set; }
		[XmlElement(ElementName = "agenttransno")]
		public string Agenttransno { get; set; }
		[XmlElement(ElementName = "resultdescription")]
		public string Resultdescription { get; set; }
		[XmlElement(ElementName = "product_description")]
		public string Product_description { get; set; }
		[XmlElement(ElementName = "topupvalue")]
		public string Topupvalue { get; set; }
		[XmlElement(ElementName = "accountvalue")]
		public string Accountvalue { get; set; }
		[XmlElement(ElementName = "walletbalance")]
		public string Walletbalance { get; set; }
		[XmlElement(ElementName = "errordata")]
		public string Errordata { get; set; }
		[XmlElement(ElementName = "flowControlCode")]
		public string FlowControlCode { get; set; }
		[XmlElement(ElementName = "tradingname")]
		public string Tradingname { get; set; }
		[XmlElement(ElementName = "recieptno")]
		public string Recieptno { get; set; }
		[XmlElement(ElementName = "agentname")]
		public string Agentname { get; set; }
		[XmlElement(ElementName = "abn")]
		public string Abn { get; set; }
		[XmlElement(ElementName = "acn")]
		public string Acn { get; set; }
		[XmlElement(ElementName = "tradingabn")]
		public string Tradingabn { get; set; }
	}
}
