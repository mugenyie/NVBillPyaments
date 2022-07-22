using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.ViewModels.Callbacks
{
    public class Error
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class InterswithFailureCB
    {
        public List<Error> errors { get; set; }
        public Error error { get; set; }
    }
}
