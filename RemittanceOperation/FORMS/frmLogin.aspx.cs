using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation.FORMS
{
    public partial class frmLogin : System.Web.UI.Page
    {
        static Manager mg = new Manager();


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
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
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (ddlUserId.SelectedIndex == 0)
            {
                lblLoginMessage.Text = "Please Select your user";
            }
            else
            {
                lblLoginMessage.Text = "";
                string userRmCode = ddlUserId.SelectedItem.Text;
                userRmCode = userRmCode.Split('-')[0].Trim();
                string providedUsrPass = txtUserPasswd.Text.Trim();

                string userId = "", userName = "", userEmail = "";

                bool passMatch = mg.isPasswordMatch(userRmCode, providedUsrPass, ref userId, ref userName, ref userEmail);

                if (passMatch)
                {
                    Guid Session_id = Guid.NewGuid();
                    Session[CSessionName.S_CURRENT_USER_RM] = userRmCode;
                    Session[CSessionName.S_CURRENT_USERID] = userId;
                    Session[CSessionName.S_CURRENT_USER_FULL_NAME] = userName;
                    Session[CSessionName.S_CURRENT_USER_EMAIL] = userEmail;

                    Session[CSessionName.S_LOGIN_TIME] = DateTime.Now;
                    Session[CSessionName.S_SESSION_ID] = Session_id.ToString();

                    Response.Redirect("FORMS/frmHome.aspx");//CSessionName.F_HOME_PAGE);
                }
                else
                {
                    lblLoginMessage.Text = "Password Do Not Match, Please Try Again !!!";
                }
            }
        }

    }
}