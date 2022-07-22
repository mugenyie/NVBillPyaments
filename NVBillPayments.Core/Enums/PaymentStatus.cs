using NVBillPayments.Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Core.Enums
{
    public enum PaymentStatus
    {
        PENDING = Status.PENDING_STATUS_CODE,
        SUCCESSFUL = Status.SUCCESS_STATUS_CODE,
        FAILED = Status.FAILURE_STATUS_CODE,
        REVERSED = Status.REVERSED_STATUS_CODE,
        INVALID = Status.INVALID_STATUS_CODE
    }
}
