using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Services.Models
{
    public class EmailNotification
    {
        public string from { get; set; }
        public string to { get; set; }
        public string subject { get; set; }
        public string textBody { get; set; }
        public List<string> cc { get; set; }
        public List<string> bcc { get; set; }
        public List<string> attachments { get; set; }
    }
}
