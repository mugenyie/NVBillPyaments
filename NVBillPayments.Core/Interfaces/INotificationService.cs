using NVBillPayments.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.Core.Interfaces
{
    public interface INotificationService
    {
        Task SendInAppAsync(string Title, string email, string message);
        Task SendEmailAsync(string Title, string email, string message, string username = "User");
        Task<(string, string)> GenerateTransactionEmailTemplateAsync(Transaction transaction);
    }
}
