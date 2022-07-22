using NVBillPayments.PaymentProviders.Beyonic.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.PaymentProviders.Beyonic
{
    public interface IBeyonicService
    {
        CollectionResponse InitiateCollection(CollectionRequest collectionRequest);
        PaymentResponse InitiatePayment(PaymentRequest paymentRequest);
    }
}
