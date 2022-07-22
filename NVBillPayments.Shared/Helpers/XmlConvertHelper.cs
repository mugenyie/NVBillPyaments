using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace NVBillPayments.Shared.Helpers
{
    public class XmlConvertHelper
    {
        public static T FromXmlString<T>(string input)
        {
            try
            {
                return ParseXmlString<T>(input);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
                //throw new XmlConvertException(exception.Message, exception);
            }
        }

        public static T ParseXmlString<T>(string input)
        {
            // trim whitespace characters
            input = input.Trim().Trim(new char[] { '\0', '\r', '\n' });

            var stringReader = new StringReader(input);
            var xmlSerializer = new XmlSerializer(typeof(T));

            var deserializedOject = xmlSerializer.Deserialize(stringReader);

            return (T)deserializedOject;
        }

        public static string ToXmlString<T>(object input, bool removeXmlDeclaration = false, bool removeNameSpaces = false)
        {
            if (input == null)
                throw new NullReferenceException("Cannot convert null object");

            if (input.GetType() != typeof(T))
                throw new ArgumentException("Type mismatch");

            // create XML string
            var stringWriter = new StringWriter();
            var xmlWriter = XmlWriter.Create(stringWriter);
            var xmlSerializer = new XmlSerializer(typeof(T));
            xmlSerializer.Serialize(xmlWriter, input);
            xmlWriter.Close();

            var xmlString = stringWriter.ToString();
            stringWriter.Close();

            if (removeXmlDeclaration)
            {
                xmlString = RemoveXmlDeclration(xmlString);
            }

            if (removeNameSpaces)
            {
                xmlString = RemoveAllNamespaces(xmlString);
            }

            return xmlString;
        }

        public static string ToXmlString(object input, bool removeXmlDeclaration = true, bool removeNameSpaces = true)
        {
            if (input == null)
                throw new NullReferenceException("Cannot convert null object");

            var xmlString = string.Empty;

            // create XML string
            var stringWriter = new StringWriter();
            var xmlWriter = XmlWriter.Create(stringWriter);
            var xmlSerializer = new XmlSerializer(input.GetType());
            xmlSerializer.Serialize(xmlWriter, input);
            xmlWriter.Close();

            xmlString = stringWriter.ToString();
            stringWriter.Close();

            if (removeXmlDeclaration)
            {
                xmlString = RemoveXmlDeclration(xmlString);
            }

            if (removeNameSpaces)
            {
                xmlString = RemoveAllNamespaces(xmlString);
            }

            return xmlString;
        }

        public static string RemoveAllNamespaces(string xmlString, bool disableFormatting = true)
        {
            XElement xmlElementWithoutNs = RemoveAllNamespaces(XElement.Parse(xmlString));

            return xmlElementWithoutNs.ToString(disableFormatting ? SaveOptions.DisableFormatting : SaveOptions.None);
        }

        private static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                XElement xElement = new XElement(xmlDocument.Name.LocalName);
                xElement.Value = xmlDocument.Value;

                foreach (XAttribute attribute in xmlDocument.Attributes())
                    xElement.Add(attribute);

                return xElement;
            }

            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(xElem => RemoveAllNamespaces(xElem)));
        }

        private static string RemoveXmlDeclration(string input)
        {
            var xmlString = string.Empty;

            var stringWriter = new StringWriter();

            // convert XML to XML document object
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(input);

            // remove XML declaration
            foreach (XmlNode node in xmlDocument)
            {
                if (node.NodeType == XmlNodeType.XmlDeclaration)
                {
                    xmlDocument.RemoveChild(node);
                }
            }

            // remove first child attributes
            xmlDocument.FirstChild.Attributes.RemoveAll();

            // convert XML document back to string
            stringWriter = new StringWriter();
            var xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlDocument.WriteTo(xmlTextWriter);

            xmlString = stringWriter.ToString();
            stringWriter.Close();

            return xmlString;
        }
    }
}
