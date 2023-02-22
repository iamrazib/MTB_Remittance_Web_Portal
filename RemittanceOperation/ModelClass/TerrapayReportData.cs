using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RemittanceOperation.ModelClass
{
    public class TerrapayReportData
    {
        public int TxnNo { get; set; }
        public string ReceivedTime { get; set; }
        public string LastUpdateTime { get; set; }
        public string ConversationId { get; set; }
        public string PartnerTxnId { get; set; }
        public string MTOName { get; set; }
        public string SenderMSISDN { get; set; }
        public string SourceCountry { get; set; }
        public float Amount { get; set; }
        public string ReceiverWallet { get; set; }
        public string bKashTxnId { get; set; }
        public string ReceiverName { get; set; }
        public string SenderName { get; set; }
        public string ReportFromDate { get; set; }
        public string ReportToDate { get; set; }
        public int PartyId { get; set; }
        public string TxnMode { get; set; }
    }
}