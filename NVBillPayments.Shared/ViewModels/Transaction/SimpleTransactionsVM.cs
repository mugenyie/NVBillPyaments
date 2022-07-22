using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Shared.ViewModels.Transaction
{
    public class SimpleTransactionsVM
    {
        public string TransactionId { get; set; }
        public string TransactionRef { get; set; }
        public string ProductId { get; set; }
        public string ProductDescription { get; set; }
        public string Status { get; set; }
        public string TransactionStatusMessage { get; set; }
        public string Category { get; set; }
        public decimal AmountCharged { get; set; }
        public string BeneficiaryId { get; set; }
        public string SponsorId { get; set; }
        public string DateCreatedUTC { get; set; }
    }
}
