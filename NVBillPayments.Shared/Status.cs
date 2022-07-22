using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Shared
{
    public class Status
    {
        public const int PENDING_STATUS_CODE  = 100;
        public const int SUCCESS_STATUS_CODE  = 000;
        public const int FAILURE_STATUS_CODE  = 111;
        public const int REVERSED_STATUS_CODE = 101;
        public const int DENIED_STATUS_CODE   = 102;
        public const int INVALID_STATUS_CODE  = 103;
        public const int UNDFINED_STATUS_CODE = 333;
    }
}