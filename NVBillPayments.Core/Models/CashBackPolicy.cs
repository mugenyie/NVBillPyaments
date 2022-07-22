using NVBillPayments.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NVBillPayments.Core.Models
{
    public class CashBackPolicy
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Percentage { get; set; }
        public SystemCategory SystemCategory { get; set; }
        public string PaymentMethod { get; set; } //card,mobile
        public bool IsActive { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime ModifiedOnUTC { get; set; }
    }
}
