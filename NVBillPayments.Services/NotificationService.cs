using NVBillPayments.Core.Interfaces;
using NVBillPayments.Services.Models;
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

        public NotificationService()
        {
            _restClient = new RestClient(BaseURL);
        }

        public async Task SendEmailAsync(string Title, string email, string message, string username)
        {
            EmailNotification emailNotification = new EmailNotification
            {
                type = "BILLPAYMENT_TRANSACTION",
                to = email,
                request = new Request
                {
                    type = "BILLPAYMENT_TRANSACTION",
                    send_format = "string"
                },
                @params = new Params
                {
                    NAME = username,
                    BILLPAYMENT_TITLE = Title,
                    BILLPAYMENT_DESCRIPTION = message
                }
            };
            _restRequest = new RestRequest($"v1/Notification/sendinstantemailnotification", Method.POST);
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
    }
}
