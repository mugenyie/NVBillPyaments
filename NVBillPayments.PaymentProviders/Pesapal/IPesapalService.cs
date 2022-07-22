using NVBillPayments.Core.Enums;
using NVBillPayments.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.PaymentProviders.Pesapal
{
    public interface IPesapalService
    {
        string GenerateCheckout(Transaction transaction);
        PaymentStatus CheckTransactionStatus(string pesapal_tracking_id, string reference);
        PaymentStatus UpdateIpnTransactionStatus(string ipnType, string pesapal_tracking_id, string reference);
    }
}
