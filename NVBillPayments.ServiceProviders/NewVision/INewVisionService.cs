using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NVBillPayments.ServiceProviders.NewVision
{
    public interface INewVisionService
    {
        Task AcknowledgeCollection(string transactionId);
    }
}
