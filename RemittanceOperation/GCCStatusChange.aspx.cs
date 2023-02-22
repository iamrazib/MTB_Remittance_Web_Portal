using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using RemittanceOperation.GCCServiceClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class GCCStatusChange : System.Web.UI.Page
    {
        static IGCCService gcclient = new GCCServiceClient.GCCServiceClient();
        static Manager mg = new Manager();
        string exh = "GCC";

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
                lblStatusProcessReceivedTxn.Text = "";
                lblUpdateBEFTNStatusToGCCAlreadySent.Text = "";
                lblUpdateBEFTNStatusAtMtbEnd.Text = "";
                lblTransactionIssue.Text = "";
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string refNo = textBoxRefNo.Text.Trim();
            lblTransactionIssue.Text = "";

            if (!String.IsNullOrEmpty(refNo))
            {
                string whereClause = " WHERE [TransactionNo]='" + refNo + "'";                
                DataTable aDTable = mg.GetIndividualTxnByWhereClause(exh, whereClause);
                dataGridViewTxnSearch.DataSource = null;
                dataGridViewTxnSearch.DataSource = aDTable;
                dataGridViewTxnSearch.DataBind();

                string accountNum = aDTable.Rows[0]["BankAccountNo"].ToString();
                string routingNum = aDTable.Rows[0]["BankBranchCode"].ToString();

                if (accountNum.Length > 17)
                {
                    lblTransactionIssue.Text = "Invalid Account Number Length. Account Length=" + accountNum.Length;
                }
                else if(!mg.IsRoutingNumberAlreadyExists(routingNum))
                {
                    lblTransactionIssue.Text = "Invalid Routing Number, Please Check";
                }
            }
        }

        protected void btnProcessReceivedTxn_Click(object sender, EventArgs e)
        {
            string txnStat = "";
            string refNo = textBoxRefNo.Text.Trim();

            if (!String.IsNullOrEmpty(refNo))
            {
                string whereClause = " WHERE [TransactionNo]='" + refNo + "'";
                DataTable aDTable = mg.GetIndividualTxnByWhereClause(exh, whereClause);

                if (aDTable.Rows.Count > 0)
                {
                    txnStat = aDTable.Rows[0]["TxnStatus"].ToString();

                    if (txnStat.Equals("RECEIVED"))
                    {
                        try
                        {
                            ProcessTransResponse procTranResp = gcclient.ProcessBankDepositTxn(Utility.GCCSecurityCode, refNo);
                            mg.UpdateTxnStatusIntoGCCTable(refNo, "Processed", "", Utility.downloadUser); 
                            lblStatusProcessReceivedTxn.Text = "Status Updated...";
                            lblStatusProcessReceivedTxn.ForeColor = Color.Green;
                        }
                        catch (Exception ex)
                        { }
                    }
                    else
                    {
                        lblStatusProcessReceivedTxn.Text = "Status Cannot Update....";
                        lblStatusProcessReceivedTxn.ForeColor = Color.Red;
                    }
                }
                else
                {
                    lblStatusProcessReceivedTxn.Text = "No Txn Found";
                    lblStatusProcessReceivedTxn.ForeColor = Color.Red;
                }
            }
        }

        protected void btnUpdateBEFTNStatusToGCCAlreadySent_Click(object sender, EventArgs e)
        {
            string payMode = "";
            string refNo = textBoxRefNo.Text.Trim();

            if (!String.IsNullOrEmpty(refNo))
            {
                string whereClause = " WHERE [TransactionNo]='" + refNo + "'";
                DataTable aDTable = mg.GetIndividualTxnByWhereClause(exh, whereClause);
                
                if (aDTable.Rows.Count > 0)
                {
                    payMode = aDTable.Rows[0]["PaymentMode"].ToString();

                    if (payMode.Equals("BEFTN"))
                    {
                        mg.InsertAutoFetchLog(Utility.GCCUserId, "ProcessBEFTNTxn", "Before GCC UpdateProcess Status: " + " refNo=" + refNo);

                        UpdateBankDepositResponse updateProcsStatusResp = gcclient.UpdateBankDepositTxnToPaid(Utility.GCCSecurityCode, refNo);

                        if (updateProcsStatusResp.ResponseCode.Equals("001") && updateProcsStatusResp.Successful.ToLower().Equals("true"))
                        {
                            mg.InsertAutoFetchLog(Utility.GCCUserId, "ProcessBEFTNTxn", refNo + ", GCC UpdateProcessStatusToPaid, RespCode="
                                  + updateProcsStatusResp.ResponseCode + ", Status=" + updateProcsStatusResp.Status + ", Message=" + updateProcsStatusResp.ResponseMessage + ", Successful=" + updateProcsStatusResp.Successful);

                            mg.UpdateTxnStatusIntoGCCTable(refNo, "Paid", "", Utility.downloadUser);
                            mg.InsertAutoFetchLog(Utility.GCCUserId, "ProcessBEFTNTxn", "RefNo=" + refNo + ", BEFTN Txn Update at DB Complete..");

                            lblUpdateBEFTNStatusToGCCAlreadySent.Text = "BEFTN Status Updated....";
                            lblUpdateBEFTNStatusToGCCAlreadySent.ForeColor = Color.Green;
                        }
                        else
                        {
                            mg.InsertAutoFetchLog(Utility.GCCUserId, "ProcessBEFTNTxn", refNo + ", GCC UpdateProcess Status ERROR!!! , RespCode="
                              + updateProcsStatusResp.ResponseCode + ", Message=" + updateProcsStatusResp.ResponseMessage);

                            lblUpdateBEFTNStatusToGCCAlreadySent.Text = "BEFTN Status Update ERROR !!!" + ", Message=" + updateProcsStatusResp.ResponseMessage;
                            lblUpdateBEFTNStatusToGCCAlreadySent.ForeColor = Color.Red;
                        }
                    }//if BEFTN
                }
                else
                {
                    lblUpdateBEFTNStatusToGCCAlreadySent.Text = "No Txn Found";
                    lblUpdateBEFTNStatusToGCCAlreadySent.ForeColor = Color.Red;
                }
            }
        }

        protected void btnUpdateBEFTNStatusAtMtbEnd_Click(object sender, EventArgs e)
        {
            string refNo = textBoxRefNo.Text.Trim();
            mg.UpdateTxnStatusIntoGCCTable(refNo, "Paid", "", Utility.downloadUser);
            lblUpdateBEFTNStatusAtMtbEnd.Text = "BEFTN Status Updated....";
            lblUpdateBEFTNStatusAtMtbEnd.ForeColor = Color.Green;
        }
    }
}