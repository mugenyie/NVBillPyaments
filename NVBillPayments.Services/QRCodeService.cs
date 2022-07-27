using NVBillPayments.Core.Interfaces;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.Services
{
    public class QRCodeService : IQRCodeService
    {
        private const string BaseURL = "https://api.newvisionapp.com";
        private IRestClient _restClient;
        private IRestRequest _restRequest;

        public QRCodeService()
        {
            _restClient = new RestClient(BaseURL);
        }

        public async Task<string> GenerateQRCodeUploadURLAsync(string base64String, string transactionId)
        {
            FileUpload fileUpload = new FileUpload
            {
                Data = base64String,
                FileUniqueName = transactionId,
                Folder = "transaction_qr_code"
            };
            _restRequest = new RestRequest($"api/FileUpload", Method.POST);
            _restRequest.AddJsonBody(fileUpload);
            var result = await _restClient.ExecuteAsync<string>(_restRequest);
            return result.Data;
        }
    }

    public class FileUpload
    {
        public string Data { get; set; }
        public string FileUniqueName { get; set; }
        public string Folder { get; set; }
    }
}
