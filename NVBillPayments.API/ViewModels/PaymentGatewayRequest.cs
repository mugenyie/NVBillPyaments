using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.ViewModels
{
    public class PaymentGatewayRequest
    {
        [Required]
        public string CustomerFullName { get; set; }
        [Required]
        public string CustomerId { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerMSISDN { get; set; }
        [Required]
        public string ProductCode { get; set; }
        [Required]
        public string ProductDescription { get; set; }
        [Required]
        public decimal AmountToDeduct { get; set; }
        [Required]
        public string PaymentMethod { get; set; } //card, momo
        public string PayWithMSISDN { get; set; }
        [Required]
        public string CurrencyCode { get; set; }
        [Required]
        public string CallbackURL { get; set; }
        public object MetaData { get; set; }
    }
}
