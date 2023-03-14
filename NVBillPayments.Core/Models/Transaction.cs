using NVBillPayments.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NVBillPayments.Core.Models
{
    public class Transaction
    {
        public Transaction()
        {
            TransactionId = Guid.NewGuid();
            CreatedOnUTC = DateTime.UtcNow;
            CurrencyCode = "UGX";
            Cashback = 0;
            AmountCharged = 0;
            CountryCode = 256;
            OrderStatus = OrderStatus.PENDING;
            OrderStatusMsg = OrderStatus.PENDING.ToString();
            PaymentStatus = PaymentStatus.PENDING;
            PaymentStatusMsg = PaymentStatus.PENDING.ToString();
            TransactionStatus = TransactionStatus.PENDING;
            TransactionStatusMessage = "Transaction is being processed";
            IsExpired = false;
        }

        [Key]
        public Guid TransactionId { get; set; }
        public string ExternalUserId { get; set; }

        public string AccountMSISDN { get; set; }
        public string AccountName { get; set; }
        public string AccountEmail { get; set; }

        public string SponsorMSISDN { get; set; }
        public string BeneficiaryMSISDN { get; set; }

        public string ProductId { get; set; }
        public string ProductDescription { get; set; }
        public string SystemCategory { get; set; }
        public string ServiceProviderId { get; set; }
        public string PaymentProviderId { get; set; }

        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentStatusMsg { get; set; }

        public OrderStatus OrderStatus { get; set; }
        public string OrderStatusMsg { get; set; }

        public TransactionStatus TransactionStatus { get; set; }

        public DateTime CreatedOnUTC { get; set; }
        public DateTime ModifiedOnUTC { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public string PaymentProviderReponseMetaData { get; set; }
        public string ServiceProviderHTTPResponseStatusCode { get; set; }
        public string ServiceProviderResponseMetaData { get; set; }

        public string CurrencyCode { get; set; }
        public int CountryCode { get; set; }

        [Column(TypeName = "decimal(15,3)")]
        public decimal ProductValue { get; set; } // Product Value from List
        public string ProductValidity { get; set; }

        public float Cashback { get; set; }

        [Column(TypeName = "decimal(15,3)")]
        public decimal AmountToCharge { get; set; }
        [Column(TypeName = "decimal(15,3)")]
        public decimal AmountCharged { get; set; }

        public string TransactionStatusMessage { get; set; }
        public string TechnicalStatusMessage { get; set; }
        public string PaymentReference { get; set; }
        public string OrderReference { get; set; }
        public string MetaData { get; set; }
        public bool IsExpired { get; set; }
        public string QRCodeUrl { get; set; }
        public string CallbackURL { get; set; }
        public bool IsCallbackInvoked { get; set; }
        public string CallbackResponse { get; set; }
    }
}
