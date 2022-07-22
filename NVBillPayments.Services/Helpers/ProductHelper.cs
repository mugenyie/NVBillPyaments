using NVBillPayments.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Services.Helpers
{
    public static class ProductHelper
    {
        public static bool IsValidSubscriber(string beneficiaryId, string network)
        {
            switch (network)
            {
                case "mtn":
                    {
                        if (beneficiaryId.StartsWith("25676") || beneficiaryId.StartsWith("25677") || beneficiaryId.StartsWith("25678") || beneficiaryId.StartsWith("25631") || beneficiaryId.StartsWith("25639"))
                            return true;
                        else
                            return false;
                    }
                case "airtel":
                    {
                        if (beneficiaryId.StartsWith("25670") || beneficiaryId.StartsWith("25674") || beneficiaryId.StartsWith("25675") || beneficiaryId.StartsWith("25620"))
                            return true;
                        else
                            return false;
                    }
                default:
                    return true;
            }
        }
    }
}
