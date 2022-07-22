using System;
using System.Collections.Generic;
using System.Text;

namespace NVBillPayments.Shared.ViewModels.Product
{
    public class QuicktellerVM
    {
        public int count { get; set; }
        public List<QuicktellerCategoryVM> categorys { get; set; }
    }

    public class QuickTellerSimpleVM
    {
        public int count { get; set; }
        public List<QuicktellerSimpleCategoryVM> categorys { get; set; }
    }

    public class QuicktellerSimpleCategoryVM
    {
        public QuicktellerSimpleCategoryVM()
        {
            IconUrl = "https://static.thenounproject.com/png/77089-200.png";
        }

        public string id { get; set; }
        public string name { get; set; }
        public string IconUrl { get; set; }
        public string description { get; set; }
        public List<QuicktellerSimpleBillerVM> billers { get; set; }
    }

    public class QuicktellerSimpleBillerVM
    {
        public QuicktellerSimpleBillerVM()
        {
            IconUrl = "https://www.multichoice.com/media/1171/kisspng-dstv-multichoice-television-channel-supersport-high-end-decadent-strokes-5aee29bfb154805670399115255576957264.png";
        }

        public string id { get; set; }
        public string categoryId { get; set; }
        public string name { get; set; }
        public string IconUrl { get; set; }
    }

    public class QuicktellerCategoryVM
    {
        public QuicktellerCategoryVM()
        {
            IconUrl = "https://static.thenounproject.com/png/77089-200.png";
        }

        public string id { get; set; }
        public string name { get; set; }
        public string IconUrl { get; set; }
        public string description { get; set; }
        public List<QuicktellerBillerVM> billers { get; set; }
    }

    public class QuicktellerBillerVM
    {
        public QuicktellerBillerVM()
        {
            IconUrl = "https://www.multichoice.com/media/1171/kisspng-dstv-multichoice-television-channel-supersport-high-end-decadent-strokes-5aee29bfb154805670399115255576957264.png";
        }

        public string id { get; set; }
        public string categoryId { get; set; }
        public string name { get; set; }
        public string IconUrl { get; set; }
        public string customerfield1 { get; set; }
        public List<QuicktellerPaymentItemVM> paymentitems { get; set; }
    }

    public class QuicktellerPaymentItemVM 
    {
        public string productCode { get; set; }
        public string name { get; set; }
        public string amount { get; set; }
        public bool isAmountFixed { get; set; }
        public string billerId { get; set; }
    }
}
