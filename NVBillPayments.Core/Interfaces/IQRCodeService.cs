using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.Core.Interfaces
{
    public interface IQRCodeService
    {
        Task<string> GenerateQRCodeUploadURLAsync(string base64String, string transactionId);
    }
}
