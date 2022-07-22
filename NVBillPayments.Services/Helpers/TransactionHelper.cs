using NVBillPayments.Core.Models;
using NVBillPayments.Shared.ViewModels.Transaction;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Services.Helpers
{
    public static class TransactionHelper
    {
        public static List<SimpleTransactionsVM> ToSimpleListView(List<Transaction> transactions)
        {
            List<SimpleTransactionsVM> transactionsVm = new List<SimpleTransactionsVM>();
            transactions.ForEach(t =>
            {
                transactionsVm.Add(new SimpleTransactionsVM
                {
                    TransactionId = t.TransactionId.ToString().ToUpper(),
                    TransactionRef = (t.TransactionId.ToString().Substring(0, 3) + "-" + t.TransactionId.ToString()[^3..]).ToUpper(),
                    ProductId = t.ProductId,
                    ProductDescription = t.ProductDescription,
                    Category = t.SystemCategory,
                    AmountCharged = t.AmountCharged,
                    BeneficiaryId = t.BeneficiaryMSISDN,
                    SponsorId = t.SponsorMSISDN,
                    Status = t.TransactionStatus.ToString(),
                    TransactionStatusMessage = t.TransactionStatusMessage,
                    DateCreatedUTC = t.CreatedOnUTC.ToString("yyyy-MM-dd HH:mm:ss")
                });
            });
            return transactionsVm;
        }
    }
}
