using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Services.Models
{
    public class InAppNotification
    {
        public string email { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public string tag { get; set; }
        public string imageUrl { get; set; }
        public string type { get; set; }
        public Dictionary<string, string> options { get; set; }
    }
}
