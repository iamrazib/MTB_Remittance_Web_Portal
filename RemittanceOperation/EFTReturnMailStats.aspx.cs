using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class EFTReturnMailStats : System.Web.UI.Page
    {
        static Manager mg = new Manager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
                
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                dtPickerReturnEmail.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                dtPickerReturnEmailResend.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

                LoadBEFTNReturnEmailExchList();
                ReturnResendMailStats.Text = "";

                btnReturnAutoMailSearch_Click(sender, e);
            }

        }

        private void LoadBEFTNReturnEmailExchList()
        {
            DataTable dtExchs = mg.GetBEFTNReturnEmailExchList();
            cbReturnEmailExchList.Items.Clear();
            cbReturnEmailExchList.Items.Add("--SELECT--");

            for (int rows = 0; rows < dtExchs.Rows.Count; rows++)
            {
                cbReturnEmailExchList.Items.Add(dtExchs.Rows[rows]["PartyId"] + " - " + dtExchs.Rows[rows]["ExchangeHouseName"]);
            }
            cbReturnEmailExchList.SelectedIndex = 0;
        }

        protected void btnReturnAutoMailSearch_Click(object sender, EventArgs e)
        {
            DateTime dateTime1;

            dateTime1 = DateTime.ParseExact(dtPickerReturnEmail.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);            
            string fromdt = dateTime1.ToString("yyyy-MM-dd");

            DataTable dtReturnEmailStats = mg.GetReturnEmailStatsByDate(fromdt);
            dataGridViewReturnMailSendInfo.DataSource = null;
            dataGridViewReturnMailSendInfo.DataSource = dtReturnEmailStats;
            dataGridViewReturnMailSendInfo.DataBind();

        }

        private DataTable CreateDataTableReturnMailStats()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ReturnDate");
            dt.Columns.Add("PartyId");
            dt.Columns.Add("Exchange");
            dt.Columns.Add("Amount");
            dt.Columns.Add("NoOfTxn");
            dt.Columns.Add("MailStat");
            dt.Columns.Add("MailSendDate");
            return dt;
        }

        protected void btnReturnEmailCheckStat_Click(object sender, EventArgs e)
        {
            DataRow drow;
            DataTable dtReturnMailStatsMerger = CreateDataTableReturnMailStats();

            DateTime dateTime1 = DateTime.ParseExact(dtPickerReturnEmailResend.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            string dtValue = dateTime1.ToString("yyyy-MM-dd");
            btnReturnMailResendToMTO.Enabled = true;

            if (cbReturnEmailExchList.SelectedIndex != 0)
            {
                int exhId = Convert.ToInt32(cbReturnEmailExchList.Text.Split('-')[0]);

                DataTable dtReturnInfoBEFTNTable = mg.GetReturnInfoFromBEFTNRequestTable(exhId, dtValue);
                DataTable dtReturnMailSendInfo = mg.GetReturnMailInfoFromBEFTNReturnEmailSendStatusTable(exhId, dtValue);

                for (int rw = 0; rw < dtReturnInfoBEFTNTable.Rows.Count; rw++)
                {
                    drow = dtReturnMailStatsMerger.NewRow();

                    drow["ReturnDate"] = dtReturnInfoBEFTNTable.Rows[rw]["ReturnDate"];
                    drow["PartyId"] = dtReturnInfoBEFTNTable.Rows[rw]["PartyId"];
                    drow["Exchange"] = dtReturnInfoBEFTNTable.Rows[rw]["Exch"];
                    drow["Amount"] = dtReturnInfoBEFTNTable.Rows[rw]["amt"];
                    drow["NoOfTxn"] = dtReturnInfoBEFTNTable.Rows[rw]["cnt"];

                    if (dtReturnMailSendInfo.Rows.Count > 0)
                    {
                        drow["MailStat"] = dtReturnMailSendInfo.Rows[rw]["MailStats"];
                        drow["MailSendDate"] = dtReturnMailSendInfo.Rows[rw]["MailSendDate"];
                    }
                    else
                    {
                        drow["MailStat"] = "";
                        drow["MailSendDate"] = "";
                    }

                    dtReturnMailStatsMerger.Rows.Add(drow);
                }

                dGridReturnMailSendStatus.DataSource = null;
                dGridReturnMailSendStatus.DataSource = dtReturnMailStatsMerger;
                dGridReturnMailSendStatus.DataBind();
            }
        }


        protected void btnReturnMailResendToMTO_Click(object sender, EventArgs e)
        {
            if (cbReturnEmailExchList.SelectedIndex != 0)
            {
                string partyId, userId, ToAddrs, ccAddrs, bccAddrs, exhouseName;
                bool mailSendStats = false, dbStat = false;
                int slNo = 0;

                DateTime dateTime1 = DateTime.ParseExact(dtPickerReturnEmailResend.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                string dtValue = dateTime1.ToString("yyyy-MM-dd");

                int exhId = Convert.ToInt32(cbReturnEmailExchList.Text.Split('-')[0]);                                
                DataTable dtReturnEmailStats = mg.GetReturnEmailSendStatsByDateAndExhId(exhId, dtValue);                               

                DataTable dtExhEmailInfo = mg.GetExhEmailInfoByPartyId(exhId);
                partyId = dtExhEmailInfo.Rows[0]["PartyId"].ToString();
                userId = dtExhEmailInfo.Rows[0]["UserId"].ToString();
                ToAddrs = dtExhEmailInfo.Rows[0]["ToAddress"].ToString();
                ccAddrs = dtExhEmailInfo.Rows[0]["CcAddress"].ToString();
                bccAddrs = Convert.ToString(dtExhEmailInfo.Rows[0]["BccAddress"]);
                exhouseName = dtExhEmailInfo.Rows[0]["ExchangeHouseName"].ToString();

                DataTable dtExhReturnTxn = mg.GetExhReturnTxnByPartyId(partyId, dtValue, exhouseName);

                if (dtExhReturnTxn.Rows.Count > 0)
                {
                    mailSendStats = SendMailToExh(dtExhReturnTxn, partyId, userId, ToAddrs, ccAddrs, bccAddrs, exhouseName);
                    
                    if (dtReturnEmailStats.Rows.Count > 0)
                    {
                        slNo = Convert.ToInt32(dtReturnEmailStats.Rows[0]["SL"]);
                        dbStat = mg.UpdateExhMailSendStatus(slNo, partyId, mailSendStats);
                    }
                    else
                    {
                        dbStat = mg.InsertExhMailSendStatus(partyId, userId, dtExhReturnTxn.Rows.Count, mailSendStats, dtValue);
                    }

                    ReturnResendMailStats.Text = "ExH: " + exhouseName + ", Mail Send: " + mailSendStats + ", DB Update: " + dbStat;

                    btnReturnAutoMailSearch_Click(sender, e);
                    btnReturnEmailCheckStat_Click(sender, e);
                }

            }
        }


        /*
        protected void btnReturnMailResendToMTO_Click(object sender, EventArgs e)
        {
            if (cbReturnEmailExchList.SelectedIndex != 0)
            {
                DateTime dateTime1 = DateTime.ParseExact(dtPickerReturnEmailResend.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                string dtValue = dateTime1.ToString("yyyy-MM-dd");

                int exhId = Convert.ToInt32(cbReturnEmailExchList.Text.Split('-')[0]);
                DataTable dtReturnEmailStats = mg.GetReturnEmailSendStatsByDateAndExhId(exhId, dtValue);

                string partyId, userId, ToAddrs, ccAddrs, bccAddrs, exhouseName;
                bool mailSendStats = false;

                int slNo = Convert.ToInt32(dtReturnEmailStats.Rows[0]["SL"]);
                int PartyId = Convert.ToInt32(dtReturnEmailStats.Rows[0]["PartyId"]);
                string returnDate = Convert.ToString(dtReturnEmailStats.Rows[0]["ReturnDate"]);

                DataTable dtExhEmailInfo = mg.GetExhEmailInfoByPartyId(PartyId);
                partyId = dtExhEmailInfo.Rows[0]["PartyId"].ToString();
                userId = dtExhEmailInfo.Rows[0]["UserId"].ToString();
                ToAddrs = dtExhEmailInfo.Rows[0]["ToAddress"].ToString();
                ccAddrs = dtExhEmailInfo.Rows[0]["CcAddress"].ToString();
                bccAddrs = Convert.ToString(dtExhEmailInfo.Rows[0]["BccAddress"]);
                exhouseName = dtExhEmailInfo.Rows[0]["ExchangeHouseName"].ToString();

                DataTable dtExhReturnTxn = mg.GetExhReturnTxnByPartyId(partyId, returnDate, exhouseName);
                if (dtExhReturnTxn.Rows.Count > 0)
                {
                    mailSendStats = SendMailToExh(dtExhReturnTxn, partyId, userId, ToAddrs, ccAddrs, bccAddrs, exhouseName);
                    //MessageBox.Show("Mail Send Status: " + mailSendStats);
                    bool updStat = mg.UpdateExhMailSendStatus(slNo, partyId, mailSendStats);
                    //MessageBox.Show("Update Status: " + updStat);

                    ReturnResendMailStats.Text = "Mail Send Status: " + mailSendStats + ", DB Update Status: " + updStat;

                    btnReturnAutoMailSearch_Click(sender, e);
                    btnReturnEmailCheckStat_Click(sender, e);
                }

            }
        }
        */

        private bool SendMailToExh(DataTable dtExhReturnTxn, string partyId, string userId, string ToAddrs, string ccAddrs, string bccAddrs, string exhouseName)
        {
            MailManager mailManager = new MailManager();

            string tomail = "", ccmail = "", bccmail = "";

            tomail = ToAddrs;
            ccmail = ccAddrs;
            bccmail = bccAddrs;

            if (ccmail == null || ccmail.Equals(""))
            {
                ccmail = "mtbremittance@mutualtrustbank.com";
            }
            else
            {
                ccmail += "; mtbremittance@mutualtrustbank.com";
            }

            string subject = "TRANSACTION REVERTED"; // : " + DateTime.Now.ToLongDateString();

            string emailbody = "";
            int slNo = 1;

            emailbody += "Dear Sir,<br><br>";
            emailbody += "Due to Beneficiary name & account differ Beneficiary bank have returned below transaction(s).<br>";
            emailbody += "We marked this transaction as cancelled & automatically credited to your NRT account.";
            
            //--- if ICTC or Universal  Then add below lines into mail body
            if (partyId.Equals("10035") || partyId.Equals("10045"))
            {
                emailbody += "<br><br>";
                emailbody += "Please advice us further whether these transactions should be amended or not.";
            }

            emailbody += "<br><br>";

            emailbody += "<style type=\"text/css\"> "
                + " .tg  {border-collapse:collapse;border-spacing:0;margin:0px auto;} "
                + " .tg td{border-color:black;border-style:solid;border-width:1px;font-family:Arial, sans-serif;font-size:14px; overflow:hidden;padding:10px 5px;word-break:normal;} "
                + " .tg th{border-color:black;border-style:solid;border-width:1px;font-family:Arial, sans-serif;font-size:14px; font-weight:normal;overflow:hidden;padding:10px 5px;word-break:normal;} "
                + " .tg .tg-1wig{font-weight:bold;text-align:left;vertical-align:top} "
                + " .tg .tg-0lax{text-align:left;vertical-align:top} "
                + " </style> "
                    + " <table class=\"tg\"> "
                    + " <thead> "
                    + " <tr> "
                       + " <th class=\"tg-1wig\">SL.</th> " 
                       + " <th class=\"tg-1wig\">TT PIN</th> "
                       + " <th class=\"tg-1wig\">Bank From</th> "
                       + " <th class=\"tg-1wig\">Receiver Account</th> "
                       + " <th class=\"tg-1wig\">Receiver Name</th> "
                       + " <th class=\"tg-1wig\">Amount</th> "
                       + " <th class=\"tg-1wig\">Exch Name</th> "
                       + " <th class=\"tg-1wig\">Request Date</th> "
                       + " <th class=\"tg-1wig\">Process Date</th> "
                       + " <th class=\"tg-1wig\">Return Date</th> "
                       + " <th class=\"tg-1wig\">Return Reason</th> "
                      + " </tr> "
                    + " </thead> "
                    + " <tbody> ";

            for (int rw = 0; rw < dtExhReturnTxn.Rows.Count; rw++)
            {
                emailbody += " <tr> "
                            + "<td class=\"tg-0lax\">" + slNo + "</td>"
                            + "<td class=\"tg-0lax\">" + dtExhReturnTxn.Rows[rw]["RefNo"].ToString() + "</td>"
                            + "<td class=\"tg-0lax\">" + dtExhReturnTxn.Rows[rw]["BeneficiaryBankName"].ToString() + "</td>"
                            + "<td class=\"tg-0lax\">" + dtExhReturnTxn.Rows[rw]["BeneficiaryAccountNo"].ToString() + "</td>"
                            + "<td class=\"tg-0lax\">" + dtExhReturnTxn.Rows[rw]["BeneficiaryName"].ToString() + "</td>"
                            + "<td class=\"tg-0lax\">" + Math.Round(Convert.ToDouble(dtExhReturnTxn.Rows[rw]["Amount"].ToString()), 2) + "</td>"
                            + "<td class=\"tg-0lax\">" + dtExhReturnTxn.Rows[rw]["ExchName"].ToString() + "</td>"
                            + "<td class=\"tg-0lax\">" + dtExhReturnTxn.Rows[rw]["RequestDate"].ToString() + "</td>"
                            + "<td class=\"tg-0lax\">" + dtExhReturnTxn.Rows[rw]["ProcessDate"].ToString() + "</td>"
                            + "<td class=\"tg-0lax\">" + dtExhReturnTxn.Rows[rw]["ReturnedDate"].ToString() + "</td>"
                            + "<td class=\"tg-0lax\">" + dtExhReturnTxn.Rows[rw]["ReturnedReason"].ToString() + "</td>"
                            + "</tr>";
                slNo++;
            }

            emailbody += " </tbody> ";
            emailbody += " </table>  ";

            emailbody += "<br><br>";
            emailbody += "Best Regards,<br>";
            emailbody += "MTB NRB Division";
            emailbody += "<br><br>";
            emailbody += "(<b>Note:</b> This is System generated email. For any query please send email to mtbremittance@mutualtrustbank.com )";

            bool mailstatus = mailManager.SendMail(tomail, ccmail, bccmail, subject, emailbody);
            return mailstatus;
        }

        protected void cbReturnEmailExchList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ReturnResendMailStats.Text = "";
        }

        protected void btnSendMailToAllExch_Click(object sender, EventArgs e)
        {
            DateTime dateTime1 = DateTime.ParseExact(dtPickerReturnEmail.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            string fromdt = dateTime1.ToString("yyyy-MM-dd");
            int exhId = 0;
            string partyId, userId, ToAddrs, ccAddrs, bccAddrs, exhouseName;
            string dtValue = "", mailStat = "";
            bool mailSendStats = false, dbStat = false;
            int slNo = 0;

            DataTable dtReturnEmailStats = mg.GetReturnEmailStatsByDate(fromdt);

            for (int ii = 0; ii < dtReturnEmailStats.Rows.Count; ii++)
            {
                exhId = Convert.ToInt32(dtReturnEmailStats.Rows[ii]["PartyId"]);
                dtValue = dtReturnEmailStats.Rows[ii]["ReturnDate"].ToString();
                mailStat = dtReturnEmailStats.Rows[ii]["MailStats"].ToString();

                DataTable dtReturnEmailSendStatus = mg.GetReturnEmailSendStatsByDateAndExhId(exhId, dtValue);  


                DataTable dtExhEmailInfo = mg.GetExhEmailInfoByPartyId(exhId);
                partyId = dtExhEmailInfo.Rows[0]["PartyId"].ToString();
                userId = dtExhEmailInfo.Rows[0]["UserId"].ToString();
                ToAddrs = dtExhEmailInfo.Rows[0]["ToAddress"].ToString();
                ccAddrs = dtExhEmailInfo.Rows[0]["CcAddress"].ToString();
                bccAddrs = Convert.ToString(dtExhEmailInfo.Rows[0]["BccAddress"]);
                exhouseName = dtExhEmailInfo.Rows[0]["ExchangeHouseName"].ToString();

                DataTable dtExhReturnTxn = mg.GetExhReturnTxnByPartyId(partyId, dtValue, exhouseName);

                if (dtExhReturnTxn.Rows.Count > 0 && mailStat.Equals("NO"))
                {
                    mailSendStats = SendMailToExh(dtExhReturnTxn, partyId, userId, ToAddrs, ccAddrs, bccAddrs, exhouseName);

                    if (dtReturnEmailSendStatus.Rows.Count > 0)
                    {
                        slNo = Convert.ToInt32(dtReturnEmailSendStatus.Rows[0]["SL"]);
                        dbStat = mg.UpdateExhMailSendStatus(slNo, partyId, mailSendStats);
                    }
                    else
                    {
                        dbStat = mg.InsertExhMailSendStatus(partyId, userId, dtExhReturnTxn.Rows.Count, mailSendStats, dtValue);
                    }

                    btnReturnAutoMailSearch_Click(sender, e);
                }

            }
        }

    }
}