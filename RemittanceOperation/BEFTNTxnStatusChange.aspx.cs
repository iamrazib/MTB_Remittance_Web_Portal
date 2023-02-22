using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class BEFTNTxnStatusChange : System.Web.UI.Page
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

            lblChangeStatusMainSuccessMsg.Text = "";
            lblChangeStatusIncentiveSuccessMsg.Text = "";
        }

        protected void btnCheckBeftnPrincipalTxn_Click(object sender, EventArgs e)
        {
            string[] lines = Regex.Split(textBoxRefNo.Text, "\n");
            string refno = "";
            DataTable dtTemp = new DataTable();

            DataTable foundRecordDt = CreateDTMainTxnFoundRecord();
            DataRow drow;

            for (int i = 0; i <= lines.GetUpperBound(0); i++)
            {
                refno = lines[i].ToString().Trim();
                if (!refno.Equals(""))
                {
                    dtTemp = mg.GetPendingDataByRefNoTxnType(refno, "Main");
                    if (dtTemp.Rows.Count > 0)
                    {
                        drow = foundRecordDt.NewRow();
                        //Exh, DebitAc, RefNo, TxnType, Status, Amount, BeneficiaryName, BeneficiaryAccountNo, BeneficiaryBankName, RoutingNo, RequestTime

                        drow["Exch"] = dtTemp.Rows[0][0];
                        drow["DebitAc"] = dtTemp.Rows[0][1];
                        drow["RefNo"] = dtTemp.Rows[0][2];
                        drow["TxnType"] = dtTemp.Rows[0][3];
                        drow["Status"] = dtTemp.Rows[0][4];
                        drow["Amount"] = dtTemp.Rows[0][5];
                        drow["Beneficiary"] = dtTemp.Rows[0][6];
                        drow["BeneAccountNo"] = dtTemp.Rows[0][7];
                        drow["Bank"] = dtTemp.Rows[0][8];
                        drow["RoutingNo"] = dtTemp.Rows[0][9];
                        drow["RequestTime"] = dtTemp.Rows[0][10];
                        
                        foundRecordDt.Rows.Add(drow);
                    }
                }
            }

            dgviewFoundRecord.DataSource = null;
            dgviewFoundRecord.DataSource = foundRecordDt;
            dgviewFoundRecord.DataBind();
            lblFoundRecordNo.Text = "Total Records: " + foundRecordDt.Rows.Count;
        }

        private DataTable CreateDTMainTxnFoundRecord()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Exch");
            dt.Columns.Add("DebitAc");
            dt.Columns.Add("RefNo");
            dt.Columns.Add("TxnType");
            dt.Columns.Add("Status");
            dt.Columns.Add("Amount");
            dt.Columns.Add("Beneficiary");
            dt.Columns.Add("BeneAccountNo");
            dt.Columns.Add("Bank");
            dt.Columns.Add("RoutingNo");
            dt.Columns.Add("RequestTime");
            return dt;
        }

        protected void btnChangeStatusPrincipalTxn_Click(object sender, EventArgs e)
        {
            string[] lines = Regex.Split(textBoxRefNo.Text, "\n");
            string pinNo = "";

            lblChangeStatusMainSuccessMsg.Text = "";

            for (int i = 0; i <= lines.GetUpperBound(0); i++)
            {
                pinNo = lines[i].ToString().Trim();
                if (!pinNo.Equals(""))
                {
                    try
                    {
                        bool rc = mg.ChangeBEFTNTxnStatusToReceived(pinNo, "Main");
                        lblChangeStatusMainSuccessMsg.Text = "DONE ...";
                    }
                    catch (Exception exy)
                    {
                    }
                }
            }
        }

        protected void btnCheckBeftnIncentive_Click(object sender, EventArgs e)
        {
            string[] lines = Regex.Split(textBoxRefNo.Text, "\n");
            string refno = "";
            DataTable dtTemp = new DataTable();

            DataTable foundRecordDt = CreateDTMainTxnFoundRecord();
            DataRow drow;

            for (int i = 0; i <= lines.GetUpperBound(0); i++)
            {
                refno = lines[i].ToString().Trim();
                if (!refno.Equals(""))
                {
                    dtTemp = mg.GetPendingDataByRefNoTxnType(refno, "Incentive");
                    if (dtTemp.Rows.Count > 0)
                    {
                        drow = foundRecordDt.NewRow();
                        //Exh, DebitAc, RefNo, TxnType, Status,Amount, BeneficiaryName, BeneficiaryAccountNo, BeneficiaryBankName, RoutingNo, RequestTime

                        drow["Exch"] = dtTemp.Rows[0][0];
                        drow["DebitAc"] = dtTemp.Rows[0][1];
                        drow["RefNo"] = dtTemp.Rows[0][2];
                        drow["TxnType"] = dtTemp.Rows[0][3];
                        drow["Status"] = dtTemp.Rows[0][4];
                        drow["Amount"] = dtTemp.Rows[0][5];
                        drow["Beneficiary"] = dtTemp.Rows[0][6];
                        drow["BeneAccountNo"] = dtTemp.Rows[0][7];
                        drow["Bank"] = dtTemp.Rows[0][8];
                        drow["RoutingNo"] = dtTemp.Rows[0][9];
                        drow["RequestTime"] = dtTemp.Rows[0][10];

                        foundRecordDt.Rows.Add(drow);
                    }
                }
            }

            dgviewFoundRecord.DataSource = null;
            dgviewFoundRecord.DataSource = foundRecordDt;
            dgviewFoundRecord.DataBind();
            lblFoundRecordNo.Text = "Total Records: " + foundRecordDt.Rows.Count;
        }

        protected void btnChangeStatusIncentiveTxn_Click(object sender, EventArgs e)
        {
            string[] lines = Regex.Split(textBoxRefNo.Text, "\n");
            string pinNo = "";

            lblChangeStatusMainSuccessMsg.Text = "";

            for (int i = 0; i <= lines.GetUpperBound(0); i++)
            {
                pinNo = lines[i].ToString().Trim();
                if (!pinNo.Equals(""))
                {
                    try
                    {
                        bool rc = mg.ChangeBEFTNTxnStatusToReceived(pinNo, "Incentive");
                        lblChangeStatusIncentiveSuccessMsg.Text = "DONE ...";
                    }
                    catch (Exception exy)
                    {
                    }
                }
            }
        }
    }
}