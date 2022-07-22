using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace NVBillPayments.Shared.Helpers
{
    public class XMLStringHelper<T>
    {
        public T DeserializeXMLString(string xmlString)
        {
            byte[] xmlDataByteArray = Encoding.UTF8.GetBytes(xmlString);
            MemoryStream stream = new MemoryStream(xmlDataByteArray);

            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stream);
        }
    }
}
