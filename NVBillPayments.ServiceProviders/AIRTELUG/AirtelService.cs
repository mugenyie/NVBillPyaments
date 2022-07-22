using NVBillPayments.ServiceProviders.AIRTELUG.Models;
using NVBillPayments.Shared.Helpers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.ServiceProviders.AIRTELUG
{
    public class AirtelService : IAirtelService
    {
        private string _retailerMSISDN;
        private string _retailerPIN;
        private string _retailerEXTCODE;
        private string _authorization;
        private string _baseUrl;
        private string _certificatePath;
        private string _login;
        private string _password;
        private string _requestGatewayCode;
        private string _requestGatewayType;
        private IRestClient _restClient;
        private IRestRequest _restRequest;
        private string _servicePort;
        private string _sourceType;

        public AirtelService()
        {
            _baseUrl = "http://airteltopup.newvisionapp.com/api";
            _restClient = new RestClient(_baseUrl);
            _retailerMSISDN = "256753230498";
            _retailerPIN = "0511";
            _retailerEXTCODE = "30498";
            _login = "pretups";
            _password = "8dc86a7be04716effe073db1df838943";
            _requestGatewayCode = "NVPPC";
            _requestGatewayType = "EXTGW";
            _servicePort = "190";
            _sourceType = "EXT";
        }

        public async Task<AirtelAirtimeServerResponse> TopupAsync(TopupRequest topupRequest)
        {
            var request = new AirtelAirtimeTopupRequest
            {
                topupRequest = topupRequest,
                credentials = new Credentials
                {
                    _login = _login,
                    _password = _password,
                    _requestGatewayCode = _requestGatewayCode,
                    _requestGatewayType = _requestGatewayType,
                    _retailerEXTCODE = _retailerEXTCODE,
                    _retailerMSISDN = _retailerMSISDN,
                    _retailerPIN = _retailerPIN,
                    _servicePort = _servicePort,
                    _sourceType = _sourceType
                }
            };
            _restRequest = new RestRequest("TopUp", Method.POST);
            _restRequest.AddJsonBody(request);
            var restResponse = await _restClient.ExecuteAsync<AirtelAirtimeServerResponse>(_restRequest);
            return restResponse.Data;
        }
    }
}
