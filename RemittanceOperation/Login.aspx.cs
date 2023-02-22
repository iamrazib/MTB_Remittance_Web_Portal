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
    public partial class Login : System.Web.UI.Page
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
                string encryptPass = Utility.HashSHA1Decryption(providedUsrPass);
                Guid Session_id = Guid.NewGuid();

                string userId = "", userName = "", userEmail = "", fileProcessUserType = "", IsMailReceive = "", roleName = "";
                string userMenuList = "";
                //int roleId = 0;

                bool passMatch = mg.isPasswordMatch(userRmCode, encryptPass, ref userId, ref userName, ref userEmail, ref fileProcessUserType,
                    ref IsMailReceive, ref roleName, ref userMenuList, Session_id.ToString());

                if (passMatch)
                {
                    Session[CSessionName.S_CURRENT_USER_RM] = userRmCode;
                    Session[CSessionName.S_CURRENT_USERID] = userId;
                    Session[CSessionName.S_CURRENT_USER_FULL_NAME] = userName;
                    Session[CSessionName.S_CURRENT_USER_EMAIL] = userEmail;

                    Session[CSessionName.S_LOGIN_TIME] = DateTime.Now;
                    Session[CSessionName.S_SESSION_ID] = Session_id.ToString();

                    Session[CSessionName.S_FILE_PROCESS_USER_TYPE] = fileProcessUserType;
                    Session[CSessionName.S_IS_MAIL_RECEIVE] = IsMailReceive;
                    Session[CSessionName.S_ROLE_NAME] = roleName;
                    Session[CSessionName.S_MENU_SESSION] = userMenuList;

                    Response.Redirect("Home.aspx");//CSessionName.F_HOME_PAGE);
                }
                else
                {
                    lblLoginMessage.Text = "Password Do Not Match, Please Try Again !!!";
                }
            }
        }
    }
}