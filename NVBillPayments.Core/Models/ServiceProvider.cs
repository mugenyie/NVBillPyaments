using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Core.Models
{
    public class ServiceProvider
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string ShortName { get; set; }
        public string LogoUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime ModifiedOnUTC { get; set; }
        public string SampleInput { get; set; }
    }
}
