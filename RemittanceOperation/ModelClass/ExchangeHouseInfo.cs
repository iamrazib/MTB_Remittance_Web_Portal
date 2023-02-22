using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RemittanceOperation.ModelClass
{
    public class ExchangeHouseInfo
    {
        public string slNo { get; set; }
        public string partyId { get; set; }
        public string userId { get; set; }
        public string exhType { get; set; } // 1- Wage, 2- Service
        public string exhName { get; set; }
        public string exhCountry { get; set; }
        public string companyType { get; set; }
        public string nrtAccount { get; set; }
        public string walletAccount { get; set; }
        public string usdAccount { get; set; }
        public string aedAccount { get; set; }

        public float nrtBalance { get; set; }
        public float walletBalance { get; set; }
        public float usdBalance { get; set; }
        public float aedBalance { get; set; }

        public string activeStat { get; set; } // 1- Active, 0- Inactive
        public string toMailAddr { get; set; }
        public string ccMailAddr { get; set; }
        public string apiorfile { get; set; }

        public string EftBdtRate { get; set; }
        public string BkashBdtRate { get; set; }
        public string CashBdtRate { get; set; }
        public string AcBdtRate { get; set; }
        public string EftUsdRate { get; set; }
        public string BkashUsdRate { get; set; }
        public string CashUsdRate { get; set; }
        public string AcUsdRate { get; set; }
        public string commissionCurrency { get; set; }
        public string exhShortName { get; set; }
        public string isCOC { get; set; } // Y- YES, N- NO
    }
}