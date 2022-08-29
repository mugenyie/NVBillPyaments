using NVBillPayments.Core.Interfaces;
using NVBillPayments.Core.Models;
using NVBillPayments.Services.Helpers;
using NVBillPayments.Services.Models;
using NVBillPayments.Shared.Helpers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.Services
{
    public class NotificationService : INotificationService
    {
        private const string BaseURL = "https://api.newvisionapp.com";
        private IRestClient _restClient;
        private IRestRequest _restRequest;

        private readonly IQRCodeService _qrCodeService;

        public NotificationService(IQRCodeService qrCodeService)
        {
            _restClient = new RestClient(BaseURL);
            _qrCodeService = qrCodeService;
        }

        public async Task SendEmailAsync(string Title, string email, string message, string username)
        {
            EmailNotification emailNotification = new EmailNotification
            {
                from = "noreply@newvision.co.ug",
                to = email,
                subject = Title,
                textBody = message
            };
            _restRequest = new RestRequest($"v1/Notification/sendemail", Method.POST);
            _restRequest.AddJsonBody(emailNotification);
            await _restClient.ExecuteAsync(_restRequest);
        }

        public async Task SendInAppAsync(string Title, string email, string message)
        {
            InAppNotification emailNotification = new InAppNotification
            {
                email = email,
                body = message,
                title = Title,
                type = "billpayment",
                imageUrl = "https://newvision-media.s3.amazonaws.com/cms/14aa2e3d-0e8e-4bbd-b3ff-95b788222d36.png",
                tag = "billpayment",
                options = new Dictionary<string,string>()
                {
                    { "notification_type", "billpayment"}
                }
            };
            _restRequest = new RestRequest($"v1/Notification/individual", Method.POST);
            _restRequest.AddJsonBody(emailNotification);
            await _restClient.ExecuteAsync(_restRequest);
        }

        public async Task<(string, string)> GenerateTransactionEmailTemplateAsync(Transaction transaction)
        {
            string qrCodeUrl = "";
            string validationUrl = $"https://billpayments-web.newvisionapp.com/receipt/detail?transactionid={transaction.TransactionId}";
            string qrCodebase64String = QRCodeHelper.Generate(validationUrl);
            try
            {
                qrCodeUrl = await _qrCodeService.GenerateQRCodeUploadURLAsync(qrCodebase64String, transaction.TransactionId.ToString());
            }
            catch (Exception exp)
            {

            }

            string emailMessage = TransactionEmailTemplate.Generate(transaction.AccountName, transaction.TransactionId.ToString(), qrCodeUrl, transaction.CreatedOnUTC.ToShortDateString(), transaction.ProductDescription, $"UGX {Math.Round(transaction.AmountCharged, 0)}/=");
            return (qrCodeUrl, emailMessage);
        }
    }
}
