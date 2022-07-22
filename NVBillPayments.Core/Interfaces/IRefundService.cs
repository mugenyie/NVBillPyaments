using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Core.Interfaces
{
    public interface IRefundService
    {
        bool RefundTransaction(string transactionId);
    }
}
