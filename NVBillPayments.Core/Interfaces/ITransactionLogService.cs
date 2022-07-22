using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.Core.Interfaces
{
    public interface ITransactionLogService
    {
        Task AddTransactionLogAsync(string Title, string transactionData);
        //void AddTransactionLogAsync(string Title, string Data);
    }
}
