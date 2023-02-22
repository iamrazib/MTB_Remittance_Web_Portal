using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RemittanceOperation.AppCode
{
    public class CConstants
    {
        public CConstants() { }

        public const string BB_IMPREST_GL = "2640100100200100";
        public const string BB_IMPREST_PARKING_GL = "1523100100100100";
        public const string BEFTN_OUTWARD_GL = "1310500800200100";

        public const string VAT_GL = "1521600100200100";
        public const string COMMISSION_GL = "3310300100100100";
        public const string COOP_FUND_GL = "1310500900100100";
        public const int COOP_FUND_PERCENTAGE = 5;

        public const string BKASH_COMMISSION_PAYABLE = "1310500600100100";

    }
}