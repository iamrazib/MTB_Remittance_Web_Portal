using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        string loggedUserEmail = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
                loggedUserEmail = Session[CSessionName.S_CURRENT_USER_EMAIL].ToString();
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                LoadUserIntoDDL();
            }
        }

        private void LoadUserIntoDDL()
        {
            ddlUserId.Items.Clear();
            ddlUserId.Items.Add("---- Select User ----");
            DataTable userlist = mg.GetUsersList();

            for (int rw = 0; rw < userlist.Rows.Count; rw++)
            {
                ddlUserId.Items.Add(userlist.Rows[rw][0].ToString());
            }
            ddlUserId.SelectedIndex = 0;
        }

        protected void btnUserReset_Click(object sender, EventArgs e)
        {
            if (ddlUserId.SelectedIndex != 0)
            {
                MailManager mailManager = new MailManager();

                string userRmCode = ddlUserId.SelectedItem.Text.Split('-')[0].Trim();
                DataTable dtUserInfo = mg.GetUserInfoByUserRMCode(userRmCode);

                bool stat = mg.ResetPassword(userRmCode);

                if (stat)
                {
                    string tomail = dtUserInfo.Rows[0]["UserEmail"].ToString();
                    string ccmail = "", bccmail = "", frommail = loggedUserEmail;
                    string subject = "Password Reset at RMS";
                    string emailbody = "Dear Sir/Madam,";
                    emailbody += "<br><br>Your Password has been reset at Remittance Management System. ( http://10.45.22.88/remittanceoperation/ )";
                    emailbody += "<br>Your RM Code: " + userRmCode;
                    emailbody += "<br>Password:  <b> 1 </b>";

                    emailbody += "<br><br>You should change default password with your own.";
                    emailbody += "<br><br><br>Thanks & Regards";
                    emailbody += "<br>RMS Admin";

                    bool mailstatus = mailManager.SendMail(frommail, tomail, ccmail, bccmail, subject, emailbody);

                    lblStatusMsg.Text = "Password Reset Successful for User: " + userRmCode;
                }
                else
                {
                    lblStatusMsg.Text = "ERROR in Password Reset for User: " + userRmCode;
                }
            }
            else
            {

            }
        }
    }
}