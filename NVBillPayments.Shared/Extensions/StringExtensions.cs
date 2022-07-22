using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Shared.Extensions
{
    public static class StringExtensions
    {
        public static string CapitalizeString(this string str)
        {
            return new string(char.ToUpper(str[0]) + str[1..]);
        }
    }
}
