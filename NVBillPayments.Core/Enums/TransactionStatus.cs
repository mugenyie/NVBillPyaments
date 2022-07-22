using NVBillPayments.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Core.Enums
{
    public enum TransactionStatus
    {
        PENDING = Status.PENDING_STATUS_CODE,
        SUCCESSFUL = Status.SUCCESS_STATUS_CODE,
        FAILED = Status.FAILURE_STATUS_CODE,
        DENIED = Status.DENIED_STATUS_CODE,
        UNDEFINED = Status.UNDFINED_STATUS_CODE
    }
}
