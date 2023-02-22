using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class BulkTxnCheck : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static string txnType = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
            }
            else
            {
                Response.Redirect("Login.aspx");//CSessionName.F_LOGIN_PAGE);
            }

            if (!IsPostBack)
            {
                txnType = "";
            }
        }

        protected void btnCheckBeftnTable_Click(object sender, EventArgs e)
        {
            string[] lines = Regex.Split(textBoxRefNo.Text, "\n");
            string refno = "";
            DataTable dtTemp = new DataTable();
            DataTable notFoundRecordDt = CreateDataTableNotFound();
            DataTable foundRecordDt = CreateDataTableFoundRecord();
            DataRow drow;

            txnType = "BEFTN";

            for (int i = 0; i <= lines.GetUpperBound(0); i++)
            {
                refno = lines[i].ToString().Trim();
                if (!refno.Equals(""))
                {
                    dtTemp = mg.GetDataByRefNo(refno, "BEFTN");

                    if (dtTemp.Rows.Count > 0)
                    {
                        for (int rown = 0; rown < dtTemp.Rows.Count; rown++)
                        {
                            drow = foundRecordDt.NewRow();

                            drow["Exch"] = dtTemp.Rows[rown][0];
                            drow["RefNo"] = dtTemp.Rows[rown][1];
                            drow["TxnType"] = dtTemp.Rows[rown][14];
                            drow["Status"] = dtTemp.Rows[rown][2];
                            drow["Remarks"] = dtTemp.Rows[rown][13];
                            drow["Sender"] = dtTemp.Rows[rown][3];
                            drow["Beneficiary"] = dtTemp.Rows[rown][4];
                            drow["Bank"] = dtTemp.Rows[rown][5];
                            drow["Branch"] = dtTemp.Rows[rown][6];
                            drow["RoutingNo"] = dtTemp.Rows[rown][7];
                            drow["AccountNo"] = dtTemp.Rows[rown][8];
                            drow["Amount"] = dtTemp.Rows[rown][9];
                            drow["RequestTime"] = dtTemp.Rows[rown][10];
                            drow["ProcessBy"] = dtTemp.Rows[rown][11];
                            drow["ProcessTime"] = dtTemp.Rows[rown][12];

                            foundRecordDt.Rows.Add(drow);
                            lblOnProgs.Text = "processing -> " + dtTemp.Rows[rown][1];
                        }
                        /*
                        drow = foundRecordDt.NewRow();

                        drow["Exch"] = dtTemp.Rows[0][0];
                        drow["RefNo"] = dtTemp.Rows[0][1];
                        drow["TxnType"] = dtTemp.Rows[0][14];
                        drow["Status"] = dtTemp.Rows[0][2];
                        drow["Remarks"] = dtTemp.Rows[0][13];
                        drow["Sender"] = dtTemp.Rows[0][3];
                        drow["Beneficiary"] = dtTemp.Rows[0][4];
                        drow["Bank"] = dtTemp.Rows[0][5];
                        drow["Branch"] = dtTemp.Rows[0][6];
                        drow["RoutingNo"] = dtTemp.Rows[0][7];
                        drow["AccountNo"] = dtTemp.Rows[0][8];
                        drow["Amount"] = dtTemp.Rows[0][9];
                        drow["RequestTime"] = dtTemp.Rows[0][10];
                        drow["ProcessBy"] = dtTemp.Rows[0][11];
                        drow["ProcessTime"] = dtTemp.Rows[0][12];

                        foundRecordDt.Rows.Add(drow);*/
                        //lblOnProgs.Text = "processing -> " + dtTemp.Rows[0][1];
                    }
                    else
                    {
                        drow = notFoundRecordDt.NewRow();
                        drow["RefNo"] = refno;
                        notFoundRecordDt.Rows.Add(drow);
                    }
                }

            }

            lblOnProgs.Text = "";
            dgviewFoundRecord.DataSource = null;
            dgviewFoundRecord.DataSource = foundRecordDt;
            dgviewFoundRecord.DataBind();
            lblFoundRecordNo.Text = "Total Records: " + foundRecordDt.Rows.Count;

            dgviewNotFoundRecord.DataSource = null;
            dgviewNotFoundRecord.DataSource = notFoundRecordDt;
            dgviewNotFoundRecord.DataBind();
            lblNotFoundRecordNo.Text = "Total Records: " + notFoundRecordDt.Rows.Count;
        }

        private DataTable CreateDataTableNotFound()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("RefNo");
            return dt;
        }

        private DataTable CreateDataTableFoundRecord()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Exch");
            dt.Columns.Add("RefNo");
            dt.Columns.Add("TxnType");
            dt.Columns.Add("Status");
            dt.Columns.Add("Remarks");
            dt.Columns.Add("Sender");
            dt.Columns.Add("Beneficiary");
            dt.Columns.Add("Bank");
            dt.Columns.Add("Branch");
            dt.Columns.Add("RoutingNo");
            dt.Columns.Add("AccountNo");
            dt.Columns.Add("Amount");
            dt.Columns.Add("RequestTime");
            dt.Columns.Add("ProcessBy");
            dt.Columns.Add("ProcessTime");
            return dt;
        }

        protected void btnCheckMtbAcTable_Click(object sender, EventArgs e)
        {
            string[] lines = Regex.Split(textBoxRefNo.Text, "\n");
            string refno = "";
            DataTable dtTemp = new DataTable();
            DataTable notFoundRecordDt = CreateDataTableNotFound();
            DataTable foundRecordDt = CreateDataTableMtbAcFoundRecord();
            DataRow drow;

            txnType = "MTB A/C";
            
            for (int i = 0; i <= lines.GetUpperBound(0); i++)
            {
                refno = lines[i].ToString().Trim();
                if (!refno.Equals(""))
                {
                    dtTemp = mg.GetDataByRefNo(refno, "MTBAC");

                    if (dtTemp.Rows.Count > 0)
                    {
                        drow = foundRecordDt.NewRow();

                        drow["Exch"] = dtTemp.Rows[0][0];
                        drow["RefNo"] = dtTemp.Rows[0][1];
                        drow["Status"] = dtTemp.Rows[0][2];
                        drow["TxnCode"] = dtTemp.Rows[0][3];
                        drow["FromAccount"] = dtTemp.Rows[0][4];
                        drow["AccountNo"] = dtTemp.Rows[0][5];
                        drow["Amount"] = dtTemp.Rows[0][6];
                        drow["BeneficiaryName"] = dtTemp.Rows[0][7];
                        drow["TxnDate"] = dtTemp.Rows[0][8];
                        drow["SenderName"] = dtTemp.Rows[0][9];
                        //drow["SenderAddress"] = dtTemp.Rows[0][10];
                        drow["IsSuccess"] = dtTemp.Rows[0][10];
                        drow["SenderCountry"] = dtTemp.Rows[0][11];
                        drow["IncentivStat"] = dtTemp.Rows[0][12];
                        drow["InctvProcessTime"] = dtTemp.Rows[0][13];

                        foundRecordDt.Rows.Add(drow);
                    }
                    else
                    {
                        drow = notFoundRecordDt.NewRow();
                        drow["RefNo"] = refno;
                        notFoundRecordDt.Rows.Add(drow);
                    }
                }

            }

            dgviewFoundRecord.DataSource = null;
            dgviewFoundRecord.DataSource = foundRecordDt;
            dgviewFoundRecord.DataBind();
            lblFoundRecordNo.Text = "Total Records: " + foundRecordDt.Rows.Count;

            dgviewNotFoundRecord.DataSource = null;
            dgviewNotFoundRecord.DataSource = notFoundRecordDt;
            dgviewNotFoundRecord.DataBind();
            lblNotFoundRecordNo.Text = "Total Records: " + notFoundRecordDt.Rows.Count;
        }

        private DataTable CreateDataTableMtbAcFoundRecord()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Exch");
            dt.Columns.Add("RefNo");
            dt.Columns.Add("Status");
            dt.Columns.Add("TxnCode");
            dt.Columns.Add("FromAccount");
            dt.Columns.Add("AccountNo");
            dt.Columns.Add("Amount");
            dt.Columns.Add("BeneficiaryName");
            dt.Columns.Add("TxnDate");
            dt.Columns.Add("SenderName");
            dt.Columns.Add("SenderAddress");
            dt.Columns.Add("IsSuccess");
            dt.Columns.Add("SenderCountry");
            dt.Columns.Add("IncentivStat");
            dt.Columns.Add("InctvProcessTime");
            
            return dt;
        }

        protected void btnCheckBkashTable_Click(object sender, EventArgs e)
        {
            string[] lines = Regex.Split(textBoxRefNo.Text, "\n");
            string refno = "";
            DataTable dtTemp = new DataTable();
            DataTable notFoundRecordDt = CreateDataTableNotFound();
            DataTable foundRecordDt = CreateDataTableBkashFoundRecord();
            DataRow drow;
            txnType = "bKash Regular";

            /*
             Exch, RefNo, Status, Receiver, Amount, Sender, bKashMsg, bKashCode, RecvTime, ProcessTime, BkashResp,  CBSValueDate                
             */

            for (int i = 0; i <= lines.GetUpperBound(0); i++)
            {
                refno = lines[i].ToString().Trim();
                if (!refno.Equals(""))
                {
                    dtTemp = mg.GetDataByRefNo(refno, "BKASH");

                    if (dtTemp.Rows.Count > 0)
                    {
                        drow = foundRecordDt.NewRow();

                        drow["Exch"] = dtTemp.Rows[0][0];
                        drow["RefNo"] = dtTemp.Rows[0][1];
                        drow["Status"] = dtTemp.Rows[0][2];
                        drow["Receiver"] = dtTemp.Rows[0][3];
                        drow["Amount"] = dtTemp.Rows[0][4];
                        drow["Sender"] = dtTemp.Rows[0][5];
                        drow["bKashMsg"] = dtTemp.Rows[0][6];
                        drow["bKashCode"] = dtTemp.Rows[0][7];
                        drow["RecvTime"] = dtTemp.Rows[0][8];
                        drow["ProcessTime"] = dtTemp.Rows[0][9];
                        drow["BkashResp"] = dtTemp.Rows[0][10];
                        drow["CBSValueDate"] = dtTemp.Rows[0][11];
                        foundRecordDt.Rows.Add(drow);
                    }
                    else
                    {
                        drow = notFoundRecordDt.NewRow();
                        drow["RefNo"] = refno;
                        notFoundRecordDt.Rows.Add(drow);
                    }
                }
            }

            dgviewFoundRecord.DataSource = null;
            dgviewFoundRecord.DataSource = foundRecordDt;
            dgviewFoundRecord.DataBind();
            lblFoundRecordNo.Text = "Total Records: " + foundRecordDt.Rows.Count;

            dgviewNotFoundRecord.DataSource = null;
            dgviewNotFoundRecord.DataSource = notFoundRecordDt;
            dgviewNotFoundRecord.DataBind();
            lblNotFoundRecordNo.Text = "Total Records: " + notFoundRecordDt.Rows.Count;
        }

        private DataTable CreateDataTableBkashFoundRecord()
        {
            /*  Exch, RefNo, Status, Receiver, Amount, Sender, bKashMsg, bKashCode, RecvTime, ProcessTime, BkashResp,  CBSValueDate   */
            
            DataTable dt = new DataTable();
            dt.Columns.Add("Exch");
            dt.Columns.Add("RefNo");
            dt.Columns.Add("Status");            
            dt.Columns.Add("Receiver");
            dt.Columns.Add("Amount");
            dt.Columns.Add("Sender");
            dt.Columns.Add("bKashMsg");
            dt.Columns.Add("bKashCode");
            dt.Columns.Add("RecvTime");
            dt.Columns.Add("ProcessTime");
            dt.Columns.Add("BkashResp");
            dt.Columns.Add("CBSValueDate");
            return dt;
        }

        protected void btnCheckCashTable_Click(object sender, EventArgs e)
        {
            string[] lines = Regex.Split(textBoxRefNo.Text, "\n");
            string refno = "";
            DataTable dtTemp = new DataTable();
            DataTable notFoundRecordDt = CreateDataTableNotFound();
            DataTable foundRecordDt = CreateDataTableCashFoundRecord();
            DataRow drow;
            txnType = "Cash Txn";

            for (int i = 0; i <= lines.GetUpperBound(0); i++)
            {
                refno = lines[i].ToString().Trim();
                if (!refno.Equals(""))
                {
                    dtTemp = mg.GetDataByRefNo(refno, "CASH");

                    if (dtTemp.Rows.Count > 0)
                    {
                        drow = foundRecordDt.NewRow();

                        drow["Exch"] = dtTemp.Rows[0][0];
                        drow["RefNo"] = dtTemp.Rows[0][1];
                        drow["Status"] = dtTemp.Rows[0][2];
                        drow["TxnCode"] = dtTemp.Rows[0][3];
                        drow["SenderName"] = dtTemp.Rows[0][4];
                        drow["BeneficiaryName"] = dtTemp.Rows[0][5];
                        drow["Amount"] = dtTemp.Rows[0][6];
                        drow["RequestTime"] = dtTemp.Rows[0][7];
                        drow["ProcessDate"] = dtTemp.Rows[0][8];
                        drow["ProcessUser"] = dtTemp.Rows[0][9];
                        foundRecordDt.Rows.Add(drow);
                    }
                    else
                    {
                        drow = notFoundRecordDt.NewRow();
                        drow["RefNo"] = refno;
                        notFoundRecordDt.Rows.Add(drow);
                    }
                }
            }

            dgviewFoundRecord.DataSource = null;
            dgviewFoundRecord.DataSource = foundRecordDt;
            dgviewFoundRecord.DataBind();
            lblFoundRecordNo.Text = "Total Records: " + foundRecordDt.Rows.Count;

            dgviewNotFoundRecord.DataSource = null;
            dgviewNotFoundRecord.DataSource = notFoundRecordDt;
            dgviewNotFoundRecord.DataBind();
            lblNotFoundRecordNo.Text = "Total Records: " + notFoundRecordDt.Rows.Count;
        }

        private DataTable CreateDataTableCashFoundRecord()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Exch");
            dt.Columns.Add("RefNo");
            dt.Columns.Add("Status");
            dt.Columns.Add("TxnCode");
            dt.Columns.Add("SenderName");
            dt.Columns.Add("BeneficiaryName");
            dt.Columns.Add("Amount");
            dt.Columns.Add("RequestTime");
            dt.Columns.Add("ProcessDate");
            dt.Columns.Add("ProcessUser");
            return dt;
        }

        protected void btnCheckBulkBkashDirectTable_Click(object sender, EventArgs e)
        {
            string[] lines = Regex.Split(textBoxRefNo.Text, "\n");
            string refno = "";
            DataTable dtTemp = new DataTable();
            DataTable notFoundRecordDt = CreateDataTableNotFound();
            DataTable foundRecordDt = CreateDataTableBkashDirectFoundRecord();
            DataRow drow;
            txnType = "bKash Direct";

            for (int i = 0; i <= lines.GetUpperBound(0); i++)
            {
                refno = lines[i].ToString().Trim();
                if (!refno.Equals(""))
                {
                    dtTemp = mg.GetDataByRefNo(refno, "BKASHDIRECT");

                    if (dtTemp.Rows.Count > 0)
                    {
                        drow = foundRecordDt.NewRow();

                        drow["ID"] = dtTemp.Rows[0][0];
                        drow["Exch"] = dtTemp.Rows[0][1];
                        drow["RefNo"] = dtTemp.Rows[0][2];
                        drow["Status"] = dtTemp.Rows[0][3];
                        drow["AccNo"] = dtTemp.Rows[0][4];
                        drow["Amount"] = dtTemp.Rows[0][5];
                        drow["RequestTime"] = dtTemp.Rows[0][6];
                        drow["BkashAppr"] = dtTemp.Rows[0][7];
                        drow["BeneficiaryName"] = dtTemp.Rows[0][8];
                        drow["SenderName"] = dtTemp.Rows[0][9];
                        drow["SrcCountry"] = dtTemp.Rows[0][10];
                        drow["ProcStats"] = dtTemp.Rows[0][11];
                        foundRecordDt.Rows.Add(drow);
                    }
                    else
                    {
                        drow = notFoundRecordDt.NewRow();
                        drow["RefNo"] = refno;
                        notFoundRecordDt.Rows.Add(drow);
                    }
                }
            }

            dgviewFoundRecord.DataSource = null;
            dgviewFoundRecord.DataSource = foundRecordDt;
            dgviewFoundRecord.DataBind();
            lblFoundRecordNo.Text = "Total Records: " + foundRecordDt.Rows.Count;

            dgviewNotFoundRecord.DataSource = null;
            dgviewNotFoundRecord.DataSource = notFoundRecordDt;
            dgviewNotFoundRecord.DataBind();
            lblNotFoundRecordNo.Text = "Total Records: " + notFoundRecordDt.Rows.Count;
        }

        private DataTable CreateDataTableBkashDirectFoundRecord()
        {
            /*
             *  [ID], Exch, RefNo, Status, AccNo, Amount, RequestTime, BkashAppr, BeneficiaryName, SenderName, SrcCountry, ProcStats 
             */

            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Exch");
            dt.Columns.Add("RefNo");
            dt.Columns.Add("Status");
            dt.Columns.Add("AccNo");
            dt.Columns.Add("Amount");
            dt.Columns.Add("RequestTime");
            dt.Columns.Add("BkashAppr");
            dt.Columns.Add("BeneficiaryName");
            dt.Columns.Add("SenderName");
            dt.Columns.Add("SrcCountry");
            dt.Columns.Add("ProcStats");
            return dt;
        }

        protected void dgviewFoundRecord_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string statusVal = "";

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (txnType.Contains("BEFTN"))
                {
                    statusVal = e.Row.Cells[3].Text;
                    if (statusVal.Contains("Success"))
                    {
                        e.Row.Cells[3].BackColor = Color.FromName("#99ff99");
                    }
                    else
                    {
                        e.Row.Cells[3].BackColor = Color.FromName("yellow");
                    }
                }
                else if (txnType.Contains("MTB A/C"))
                {
                    statusVal = e.Row.Cells[2].Text;
                    if (statusVal.Contains("Success"))
                    {
                        e.Row.Cells[2].BackColor = Color.FromName("#99ff99");
                    }
                    else
                    {
                        e.Row.Cells[2].BackColor = Color.FromName("yellow");
                    }
                }
                else if (txnType.Contains("bKash Regular"))
                {
                    statusVal = e.Row.Cells[2].Text;
                    if (statusVal.Contains("Success"))
                    {
                        e.Row.Cells[2].BackColor = Color.FromName("#99ff99");
                    }
                    else
                    {
                        e.Row.Cells[2].BackColor = Color.FromName("yellow");
                    }
                }
                else if (txnType.Contains("bKash Direct"))
                {
                    statusVal = e.Row.Cells[3].Text;
                    if (statusVal.Contains("Success"))
                    {
                        e.Row.Cells[3].BackColor = Color.FromName("#99ff99");
                    }
                    else
                    {
                        e.Row.Cells[3].BackColor = Color.FromName("yellow");
                    }
                }
                else if (txnType.Contains("Cash Txn"))
                {
                }
                else
                {
                    statusVal = e.Row.Cells[2].Text;
                    if (statusVal.Contains("Disbursed"))
                    {
                        e.Row.Cells[2].BackColor = Color.FromName("#99ff99");
                    }
                    else
                    {
                        e.Row.Cells[2].BackColor = Color.FromName("yellow");
                    }
                }

            }
        }

    }
}