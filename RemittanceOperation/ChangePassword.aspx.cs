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
    public partial class ChangePassword : System.Web.UI.Page
    {
        static Manager mg = new Manager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {                
                S_CURRENT_USERID.Text = Session[CSessionName.S_CURRENT_USERID].ToString();             
            }
            else
            {
                Response.Redirect("Login.aspx");//CSessionName.F_LOGIN_PAGE);
            }
        }

        protected void btnChangePass_Click(object sender, EventArgs e)
        {
            string currPass = txtCurrPassword.Text.Trim();
            string newPass = txtNewPassword.Text.Trim();
            string confPass = txtConfPassword.Text.Trim();

            DataTable dtUserInfo = mg.GetUserInfoByUserId(S_CURRENT_USERID.Text);

            string encryptCurrPass = Utility.HashSHA1Decryption(currPass);

            if (!dtUserInfo.Rows[0]["UserPassword"].ToString().Equals(encryptCurrPass))
            {
                lblErrorMsg.Text = "Current Password Do Not Match, Please Try Again !!!";
            }
            else if(!newPass.Equals(confPass))
            {
                lblErrorMsg.Text = "New Password and Confirm Password Do Not Match !!!";
            }
            else
            {
                newPass = Utility.HashSHA1Decryption(newPass);
                currPass = Utility.HashSHA1Decryption(txtCurrPassword.Text.Trim());

                bool status = mg.ChangePassword(S_CURRENT_USERID.Text, currPass, newPass);
                if(status)
                {
                    Session[CSessionName.S_CURRENT_USER_RM] = null;
                    Session[CSessionName.S_LOGIN_TIME] = null;
                    Session[CSessionName.S_SESSION_ID] = null;
                    Session[CSessionName.S_CURRENT_USERID] = null;
                    Session[CSessionName.S_CURRENT_USER_FULL_NAME] = null;
                    Session[CSessionName.S_CURRENT_USER_EMAIL] = null;
                    Response.Redirect("Login.aspx");
                }
                else
                {
                    lblErrorMsg.Text = "Error in Password Change !!!";
                }
            }

        }
    }
}