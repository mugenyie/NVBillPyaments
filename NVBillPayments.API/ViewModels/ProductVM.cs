using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.ViewModels
{
    public class ProductVM
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string Volume { get; set; }
        public string Frequency { get; set; }
    }
}
