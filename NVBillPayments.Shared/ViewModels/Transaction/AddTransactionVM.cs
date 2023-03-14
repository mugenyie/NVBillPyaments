using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Shared.ViewModels.Transaction
{
    public class AddTransactionVM
    {
        public AddTransactionVM()
        {
            PaymentMethod = "mobile"; // or card
        }

        public UserAccount UserAccount { get; set; }
        public string SponsorId { get; set; } // Senders Phone number with country code
        public string BeneficiaryId { get; set; } // Recipient Phone number with country code
        public string ProductId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string MetaData { get; set; }
    }

    public class UserAccount
    {
        public string userId { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
