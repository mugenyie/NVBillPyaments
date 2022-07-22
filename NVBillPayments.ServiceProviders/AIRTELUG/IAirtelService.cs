using NVBillPayments.ServiceProviders.AIRTELUG.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.ServiceProviders.AIRTELUG
{
    public interface IAirtelService
    {
        Task<AirtelAirtimeServerResponse> TopupAsync(TopupRequest topupRequest);
    }
}
