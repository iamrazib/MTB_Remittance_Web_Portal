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
    public partial class RoleConfigure : System.Web.UI.Page
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
                DisplayUserRoles();
            }
        }

        private void DisplayUserRoles()
        {
            DataTable dtUserRoles = mg.GetRMSUserRoleList();
            dgridViewUserRoles.DataSource = null;
            dgridViewUserRoles.DataSource = dtUserRoles;
            dgridViewUserRoles.DataBind();
        }

        protected void dgridViewUserRoles_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = dgridViewUserRoles.SelectedRow;

            txtSlNo.Text = row.Cells[1].Text;
            txtRoleId.Text = row.Cells[2].Text;
            txtRoleName.Text = row.Cells[3].Text;

            ddlRoleActive.SelectedIndex = ddlRoleActive.Items.IndexOf(ddlRoleActive.Items.FindByText(row.Cells[4].Text));
            lblUpdateSuccMsg.Text = "";
            lblUpdateErrorMsg.Text = "";
        }

        protected void btnUpdateRoleInfo_Click(object sender, EventArgs e)
        {
            int slNo = Convert.ToInt32(txtSlNo.Text);
            int roleId = Convert.ToInt32(txtRoleId.Text);
            string roleNm = txtRoleName.Text.Trim();
            int roleActivity = Convert.ToInt32(ddlRoleActive.SelectedValue);

            try
            {
                bool stats = mg.UpdateWebUserRole(slNo, roleId, roleNm, roleActivity);
                if (stats)
                {
                    lblUpdateSuccMsg.Text = "Role Updated Successfully...";
                    DisplayUserRoles();

                    txtSlNo.Text = "";
                    txtRoleId.Text = "";
                    txtRoleName.Text = "";
                    dgridViewUserRoles.SelectedIndex = -1;
                }
            }
            catch (Exception ex) { }
        }

        protected void btnSaveNewRole_Click(object sender, EventArgs e)
        {
            string rolename = txtNewRoleName.Text.Trim();

            try
            {
                bool stats = mg.SaveWebUserRole(rolename);
                if (stats)
                {
                    lblSaveSuccMsg.Text = "Role Saved Successfully...";
                    txtNewRoleName.Text = "";
                    DisplayUserRoles();
                }
                else
                {
                    lblSaveErrorMsg.Text = "ERROR !! Role Save Failed";
                    DisplayUserRoles();
                }
            }
            catch (Exception ex) { }
            
        }
    }
}