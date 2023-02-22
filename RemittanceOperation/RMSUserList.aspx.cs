using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
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
    public partial class RMSUserList : System.Web.UI.Page
    {
        string roleName = "";
        string permittedRole = "ADMIN";
        static Manager mg = new Manager();


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
                roleName = Session[CSessionName.S_ROLE_NAME].ToString();

                if (!roleName.Equals(permittedRole))
                {
                    lblUserAuthorizationMsg.Text = "You are NOT Authorized to take any action in this screen !!!";
                    lblUserAuthorizationMsg.ForeColor = Color.Red;
                }
                else
                {
                    lblUserAuthorizationMsg.Text = "";
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                DisplayUserList();
                BindDropDownList(ddlUserRole, "RoleName", "RoleId", "Select Role");
                BindDropDownList(ddlUserRoleNewUser, "RoleName", "RoleId", "Select Role");    
                //LoadUserRole();
                //LoadUserActivity();
            }
        }

        private void BindDropDownList(DropDownList ddl, string text, string value, string defaultText)
        {
            ddl.DataSource = mg.GetRMSUserRole();
            ddl.DataTextField = text;
            ddl.DataValueField = value;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem(defaultText, "0"));
        }

        private void LoadUserActivity()
        {
            
        }

        private void LoadUserRole()
        {
            DataTable dtRoles = mg.GetRMSUserRole();
            ddlUserRole.DataSource = null;
            ddlUserRole.DataSource = dtRoles;
            //ddlUserRole.DataTextField = dtRoles.Columns[1].ToString();
            //ddlUserRole.DataValueField = dtRoles.Columns[0].ToString();
            ddlUserRole.DataBind();
        }

        private void DisplayUserList()
        {
            DataTable dtUsers = mg.GetRMSUserList();
            dgridViewUserInfos.DataSource = null;
            dgridViewUserInfos.DataSource = dtUsers;
            dgridViewUserInfos.DataBind();
        }

        protected void dgridViewUserInfos_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = dgridViewUserInfos.SelectedRow;

            txtSlNo.Text = row.Cells[1].Text;
            txtUserId.Text = row.Cells[2].Text;
            txtUserName.Text = row.Cells[4].Text;
            txtUserEmail.Text = row.Cells[5].Text;
                        
            ddlUserRole.SelectedIndex = ddlUserRole.Items.IndexOf(ddlUserRole.Items.FindByText(row.Cells[6].Text.ToString().Trim()));            
            ddlUserActive.SelectedIndex = ddlUserActive.Items.IndexOf(ddlUserActive.Items.FindByText(row.Cells[7].Text));            
            ddlUserType.SelectedIndex = ddlUserType.Items.IndexOf(ddlUserType.Items.FindByText(row.Cells[8].Text));
            ddlIsMailRecv.SelectedIndex = ddlIsMailRecv.Items.IndexOf(ddlIsMailRecv.Items.FindByText(row.Cells[9].Text));
        }

        protected void btnUpdateUserInfo_Click(object sender, EventArgs e)
        {
            if (!roleName.Equals(permittedRole))
            {
                lblUserAuthorizationMsg.Text = "You are NOT Authorized to take any action in this screen !!!";
                lblUserAuthorizationMsg.ForeColor = Color.Red;
            }
            else
            {
                lblUserAuthorizationMsg.Text = "";

                string sl = txtSlNo.Text;
                string uId = txtUserId.Text;
                string uName = txtUserName.Text.Trim();
                string uMail = txtUserEmail.Text.Trim();
                string uRole = ddlUserRole.SelectedItem.Text;
                string uActv = ddlUserActive.Text;
                string uTyp = ddlUserType.Text;
                string mailRcv = ddlIsMailRecv.Text;

                try
                {
                    bool stats = mg.UpdateWebUserInfo(sl, uId, uName, uMail, uRole, uActv, uTyp, mailRcv);
                    if (stats)
                    {
                        lblUpdateSuccMsg.Text = "User Updated Successfully...";
                        ClearUpdateUserControl();
                        DisplayUserList();
                    }
                    else
                    {
                        lblUpdateErrorMsg.Text = "ERROR !! User Update Failed";
                        DisplayUserList();
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void ClearUpdateUserControl()
        {
            txtSlNo.Text = "";
            txtUserId.Text = "";
            txtUserName.Text = "";
            txtUserEmail.Text = "";
            ddlUserRole.SelectedIndex = 0;
        }

        protected void btnSaveUser_Click(object sender, EventArgs e)
        {
            lblSaveSuccMsg.Text = "";
            lblSaveErrorMsg.Text = "";

            if (!roleName.Equals(permittedRole))
            {
                lblUserAuthorizationMsg.Text = "You are NOT Authorized to take any action in this screen !!!";
                lblUserAuthorizationMsg.ForeColor = Color.Red;
            }
            else
            {
                lblUserAuthorizationMsg.Text = "";

                string uId = txtNewUserId.Text;
                string uRmCode = txtNewUserRM.Text;
                string uName = txtNewUserName.Text.Trim();
                string uMail = txtNewUserEmail.Text.Trim();
                string uRole = ddlUserRoleNewUser.SelectedItem.Text;
                string uActv = ddlUserActiveNewUser.Text;
                string uTyp = ddlUserTypeNewUser.Text;
                string mailRcv = ddlIsMailRecvNewUser.Text;

                try
                {
                    if (!mg.IsUserExistsAlready(uId))
                    {
                        bool stats = mg.SaveWebUserInfo(uId, uRmCode, uName, uMail, uRole, uActv, uTyp, mailRcv);
                        if (stats)
                        {
                            lblSaveSuccMsg.Text = "User Saved Successfully...";
                            ClearNewUserControl();
                            SendNewUserEmail(uId, uRmCode, uName, uMail);
                            DisplayUserList();
                        }
                        else
                        {
                            lblSaveErrorMsg.Text = "ERROR !! User Save Failed";
                            DisplayUserList();
                        }
                    }
                    else
                    {
                        lblSaveErrorMsg.Text = "User ALREADY Exists into System !!!";
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void SendNewUserEmail(string uId, string uRmCode, string uName, string uMail)
        {
            MailManager mailManager = new MailManager();
            string subject = "New User Creation at RMS";
            string emailbody = "Dear Sir/Madam,";
            emailbody += "<br><br>Your User has been created at Remittance Management System. ( http://10.45.22.88/remittanceoperation/Login.aspx )";
            emailbody += "<br><br>User Name: " + uName;
            emailbody += "<br>User ID: " + uId;
            emailbody += "<br>Password:  <b> 1 </b>";

            emailbody += "<br><br>You should change default password with your own.";
            emailbody += "<br><br><br>Thanks & Regards";
            emailbody += "<br>RMS Admin";

            bool mailstatus = mailManager.SendMail("automail@mutualtrustbank.com", uMail, "", "", subject, emailbody);
        }

        private void ClearNewUserControl()
        {
            txtNewUserId.Text = "";
            txtNewUserRM.Text = "";
            txtNewUserName.Text = "";
            txtNewUserEmail.Text = "";
            ddlUserRoleNewUser.SelectedIndex = 0;
        }


    }
}