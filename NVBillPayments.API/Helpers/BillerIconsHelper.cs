using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NVBillPayments.API.Helpers
{
    public static class BillerIconsHelper
    {
        public static string GetCategoryIcon(int categoryId)
        {
            switch (categoryId)
            {
                case 4: return "/StaticFiles/categorys/calls.svg";
                case 5: return "/StaticFiles/categorys/tv.svg";
                case 6: return "/StaticFiles/categorys/internet.svg";
                case 7: return "/StaticFiles/categorys/corporate.svg";
                case 8: return "/StaticFiles/categorys/utilities.svg";
                case 11: return "/StaticFiles/categorys/momo.svg";
                default: return "/StaticFiles/placeholder.jpg";
            }
        }

        public static string GetBillerIcon(int billerId)
        {
            switch (billerId)
            {
                case 238: return "/StaticFiles/billers/airtime.png";
                case 252: return "/StaticFiles/billers/utl.png";
                case 433: return "/StaticFiles/billers/azam.png";
                case 215: return "/StaticFiles/billers/dstv.jpg";
                case 213: return "/StaticFiles/billers/startimes.png";
                case 258: return "/StaticFiles/billers/zuku.png";
                case 245: return "/StaticFiles/billers/airtel.png";
                case 246: return "/StaticFiles/billers/mtn.png";
                case 259: return "/StaticFiles/billers/roke.png";
                case 255: return "/StaticFiles/billers/smile.png";
                case 257: return "/StaticFiles/billers/fit-farmers.png";
                case 398: return "/StaticFiles/billers/kcca.png";
                case 327: return "/StaticFiles/billers/tugende.png";
                case 397: return "/StaticFiles/billers/ura.png";
                case 437: return "/StaticFiles/billers/umeme.png";
                case 249: return "/StaticFiles/billers/water.png";
                case 443: return "/StaticFiles/billers/airtel-money.jpg";
                case 283: return "/StaticFiles/billers/mtn-momo.jpeg";
                default: return "/StaticFiles/placeholder.jpg";
            }
        }
    }
}
