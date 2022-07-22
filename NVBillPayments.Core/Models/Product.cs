using NVBillPayments.Core.Enums;
using NVBillPayments.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NVBillPayments.Core.Models
{
    public class Product
    {
        public Product()
        {
            CurrencyCode = "UGX";
        }

        [Key]
        public Guid Id { get; set; }
        public string ProductId { get; set; }
        public string ProductId_2 { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [ForeignKey("CategoryId")]
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }

        [ForeignKey("ServiceProviderId")]
        public Guid ServiceProviderId { get; set; }
        public ServiceProvider ServiceProvider { get; set; }

        [Column(TypeName = "decimal(15,3)")]
        public decimal Price { get; set; }
        public string CurrencyCode { get; set; }
        public string Volume { get; set; }
        public string Grouping { get; set; }
        public string Validity { get; set; }
        public SystemCategory SystemCategory { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedOnUTC { get; set; }
        public DateTime ModifiedOnUTC { get; set; }
        public bool FreeCharge { get; set; }
        public float CachbackPercentage { get; set; }
        public bool UserInputAmount { get; set; }
    }
}
