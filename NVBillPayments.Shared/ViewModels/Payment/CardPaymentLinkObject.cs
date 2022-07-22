using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Shared.ViewModels.Payment
{
    public class CardPaymentLinkObject
    {
        public string Link { get; set; }
        public string Response { get; set; }
        public string Token { get; set; }
    }
}
