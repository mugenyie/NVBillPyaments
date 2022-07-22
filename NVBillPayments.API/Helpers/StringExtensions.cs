using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.API.Helpers
{
    public static class StringExtensions
    {
        public static string ValidatePhoneNumber(this string _phoneNumber)
        {
            string phoneNumber = _phoneNumber;
            if (_phoneNumber.StartsWith("0"))
                phoneNumber = "256"+_phoneNumber.Substring(1);
            else if(_phoneNumber.StartsWith("+"))
                phoneNumber = _phoneNumber.Substring(1);

            if (phoneNumber.Length != 12)
                throw new Exception("Invalid Phone Number");

            return phoneNumber;
        }

        public static string DecodeBase64(this string value)
        {
            var valueBytes = System.Convert.FromBase64String(value);
            return Encoding.UTF8.GetString(valueBytes);
        }

        public static string EncodeBase64(this string value)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(valueBytes);
        }
    }
}
