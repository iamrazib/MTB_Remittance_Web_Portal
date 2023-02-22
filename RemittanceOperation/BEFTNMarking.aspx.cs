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
    public partial class BEFTNMarking : System.Web.UI.Page
    {
        string userId = "", loggedUserName = "";
        static Manager mg = new Manager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
                userId = Session[CSessionName.S_CURRENT_USERID].ToString();
                loggedUserName = Session[CSessionName.S_CURRENT_USER_FULL_NAME].ToString();
            }
            else
            {
                Response.Redirect("Login.aspx");
            }            
        }

        protected void btnBEFTNMarkSuccess_Click(object sender, EventArgs e)
        {
            string pinNo = txtBEFTNSuccessMarkPINNo.Text.Trim();
            lblMarkSuccessMsg.Text = "";
            string remarks = txtSuccessRemarks.Text; 

            if (!pinNo.Equals(""))
            {
                try
                {
                    int rc = mg.BEFTNMarkedSuccessManually(pinNo, remarks, userId);
                    lblMarkSuccessMsg.Text = "Pin: " + pinNo + " -> Mark Success. Table Modified: " + rc;
                }
                catch (Exception exy) {   }
            }
            else
            {
                lblMarkSuccessMsg.Text = "";
            }
        }

        protected void btnBEFTNMarkCancel_Click(object sender, EventArgs e)
        {
            string pinNo = txtBEFTNCancelMarkPINNo.Text.Trim();
            lblMarkCancelMsg.Text = "";
            string remarks = txtCancelledRemarks.Text;

            if (!pinNo.Equals(""))
            {
                try
                {
                    int rc = mg.BEFTNMarkedCancelledManually(pinNo, remarks, userId);
                    lblMarkCancelMsg.Text = "Pin: " + pinNo + " -> Mark Cancelled. Table Modified: " + rc;
                }
                catch (Exception exy) {  }
            }
            else
            {
                lblMarkCancelMsg.Text = "";
            }
        }

        protected void btnRippleEFTMarkCancel_Click(object sender, EventArgs e)
        {
            string pinNo = txtRippleEFTCancelMarkPINNo.Text.Trim();
            lblRippleEFTMarkCancelMsg.Text = "";
            string remarks = txtRippleEFTCancelledRemarks.Text;

            if (!pinNo.Equals(""))
            {
                try
                {
                    int rc = mg.BEFTNMarkedCancelRippleTxnManually(pinNo, remarks, userId);
                    lblMarkCancelMsg.Text = "Pin: " + pinNo + " -> Mark Cancelled. Table Modified: " + rc;

                    DataTable dtEftRecInfo = mg.GetEFTTxnInfoByAutoId(pinNo);

                    bool pmntRetResp = mg.InsertIntoBEFTPaymentReturnTable(dtEftRecInfo, loggedUserName);
                    lblRippleEFTMarkCancelMsg.Text = "BEFTN Return Table: " + pmntRetResp;

                    bool statusLogResp = mg.InsertIntoBEFTStatusLogTable(dtEftRecInfo, loggedUserName);
                    lblRippleEFTMarkCancelMsg.Text = "Status Log Table: " + pmntRetResp;

                    bool statusUpdResp = mg.UpdateBEFTStatus(dtEftRecInfo, loggedUserName);
                    lblRippleEFTMarkCancelMsg.Text = "BEFTN Status Update: " + pmntRetResp;
                }
                catch (Exception exy) { }
            }
            else
            {
                lblRippleEFTMarkCancelMsg.Text = "";
            }
        }

        protected void btnBEFTNMarkSuccessBulk_Click(object sender, EventArgs e)
        {
            string[] lines = Regex.Split(textBoxRefNoBulkSuccess.Text, "\n");
            string pinNo = "";

            lblMarkSuccessMsgBulk.Text = "";
            string remarks = txtSuccessRemarksBulk.Text; 

            for (int i = 0; i <= lines.GetUpperBound(0); i++)
            {
                pinNo = lines[i].ToString().Trim();
                if (!pinNo.Equals(""))
                {
                    try
                    {
                        int rc = mg.BEFTNMarkedSuccessManually(pinNo, remarks, userId);
                        lblMarkSuccessMsgBulk.Text = "Pin: " + pinNo + " -> Mark Success. ";
                    }
                    catch (Exception exy)
                    {                        
                    }
                }

            }//for end

            lblMarkSuccessMsgBulk.Text = "DONE";
        }

        protected void btnBEFTNMarkCancelBulk_Click(object sender, EventArgs e)
        {
            string[] lines = Regex.Split(txtBEFTNCancelMarkPINNoBulk.Text, "\n");
            string pinNo = "";

            lblMarkCancelMsgBulk.Text = "";
            string remarks = txtCancelledRemarksBulk.Text;

            for (int i = 0; i <= lines.GetUpperBound(0); i++)
            {
                pinNo = lines[i].ToString().Trim();
                if (!pinNo.Equals(""))
                {
                    try
                    {
                        int rc = mg.BEFTNMarkedCancelledManually(pinNo, remarks, userId);
                        lblMarkCancelMsg.Text = "Pin: " + pinNo + " -> Mark Cancelled. Table Modified: " + rc;
                    }
                    catch (Exception exy)
                    {                        
                    }
                }

            } // for end

            lblMarkCancelMsgBulk.Text = "DONE";
        }

        protected void btnBEFTNIncentiveMarkSuccessBulk_Click(object sender, EventArgs e)
        {
            string[] lines = Regex.Split(textBoxRefNoBulkIncentiveSuccess.Text, "\n");
            string pinNo = "";

            string remarks = txtSuccessRemarksIncentiveBulk.Text;

            for (int i = 0; i <= lines.GetUpperBound(0); i++)
            {
                pinNo = lines[i].ToString().Trim();
                if (!pinNo.Equals(""))
                {
                    try
                    {
                        mg.BEFTNIncentiveMarkedSuccessManually(pinNo, remarks, userId);
                        lblMarkSuccessIncentiveMsgBulk.Text = "Pin: " + pinNo + " -> Mark Success. ";
                    }
                    catch (Exception exy)
                    {
                    }
                }

            }//for end

            lblMarkSuccessIncentiveMsgBulk.Text = "DONE";
        }
    }
}