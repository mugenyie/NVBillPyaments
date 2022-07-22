using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Shared
{
    public static class ConfigurationConstants
    {
        public static readonly string ENVIRONMENT = Environment.GetEnvironmentVariable("NV_ENVIRONMENT"); // PRODUCTION / DEVELOPMENT
        public static readonly string DBCONNECTION = Environment.GetEnvironmentVariable("NV_BILLPAYMENTS_DBCONNECTION");
        public static readonly string RABBITMQ_URI = Environment.GetEnvironmentVariable("NV_RABBITMQ_URI");
        public static readonly string REDIS_URI = Environment.GetEnvironmentVariable("NV_REDIS_URI");
        public static readonly string API_AUTH_KEY = Environment.GetEnvironmentVariable("NV_API_AUTH_KEY");
        public static readonly string QUEUE_NAME = Environment.GetEnvironmentVariable("NV_QUEUE_NAME");
        public static readonly string LOGS_QUEUE = "nv_transaction_logs";
    }
}
