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
    public partial class RoleMenuMapping : System.Web.UI.Page
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

            if(!IsPostBack)
            {
                LoadUserRole();
            }
        }

        private void LoadUserRole()
        {
            DataTable dtRoles = mg.GetRMSUserRole();
            ddlUserRole.Items.Clear();
            ddlUserRole.Items.Add("--SELECT--");

            for (int rows = 0; rows < dtRoles.Rows.Count; rows++)
            {
                ddlUserRole.Items.Add(dtRoles.Rows[rows]["RoleId"] + "-" + dtRoles.Rows[rows]["RoleName"]);
            }
            ddlUserRole.SelectedIndex = 0;
        }

        protected void btnSearchRoleMenu_Click(object sender, EventArgs e)
        {
            if (ddlUserRole.SelectedIndex != 0)
            {
                dgridViewRoleMenus.SelectedIndex = -1; // to deselect previously selected rows
                int roleId = Convert.ToInt32(ddlUserRole.Text.Split('-')[0]);

                DataTable dtMenuMappedList = mg.GetMappedUnmappedMenuListByRoleId(roleId);
                dgridViewRoleMenus.DataSource = null;
                dgridViewRoleMenus.DataSource = dtMenuMappedList;
                dgridViewRoleMenus.DataBind();
            }
        }

        protected void dgridViewRoleMenus_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = dgridViewRoleMenus.SelectedRow;

            txtSlNo.Text = row.Cells[1].Text;
            txtMenuId.Text = row.Cells[2].Text;
            txtMenuTitle.Text = row.Cells[4].Text;
            txtMenuUrl.Text = row.Cells[5].Text;
            txtMappingStatus.Text = row.Cells[6].Text;

            lblUpdateSuccMsg.Text = "";
            lblUpdateErrorMsg.Text = "";
            lblDeAssignUpdateSuccMsg.Text = "";
            lblDeAssignUpdateErrorMsg.Text = "";
        }

        protected void btnUpdateRoleMenuMapping_Click(object sender, EventArgs e)
        {
            try
            {
                int roleId = Convert.ToInt32(ddlUserRole.Text.Split('-')[0]);
                int menuId = Convert.ToInt32(txtMenuId.Text);

                bool stats = mg.AssignRoleMenu(roleId, menuId);
                if (stats)
                {
                    lblUpdateSuccMsg.Text = "Role-Menu Map Updated Successfully...";
                    btnSearchRoleMenu_Click(sender, e);

                    txtSlNo.Text = "";
                    txtMenuId.Text = "";
                    txtMenuTitle.Text = "";
                    txtMenuUrl.Text = "";
                    txtMappingStatus.Text = "";
                    dgridViewRoleMenus.SelectedIndex = -1;
                }
                else
                {
                    lblUpdateErrorMsg.Text = "ERROR !! Role-Menu Map Update Failed";
                    dgridViewRoleMenus.SelectedIndex = -1;
                    btnSearchRoleMenu_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                lblUpdateErrorMsg.Text = "ERROR !! Role-Menu Map Update Failed";
                dgridViewRoleMenus.SelectedIndex = -1;
                btnSearchRoleMenu_Click(sender, e);
            }
        }

        protected void btnDeAssignRoleMenuMapping_Click(object sender, EventArgs e)
        {
            try
            {
                int roleId = Convert.ToInt32(ddlUserRole.Text.Split('-')[0]);
                int menuId = Convert.ToInt32(txtMenuId.Text);

                bool stats = mg.DeAssignRoleMenu(roleId, menuId);
                if (stats)
                {
                    lblDeAssignUpdateSuccMsg.Text = "Role-Menu Map DeAssign Successfully...";
                    btnSearchRoleMenu_Click(sender, e);

                    txtSlNo.Text = "";
                    txtMenuId.Text = "";
                    txtMenuTitle.Text = "";
                    txtMenuUrl.Text = "";
                    txtMappingStatus.Text = "";

                    dgridViewRoleMenus.SelectedIndex = -1;
                }
                else
                {
                    lblDeAssignUpdateErrorMsg.Text = "ERROR !! Role-Menu Map DeAssign Failed";
                    dgridViewRoleMenus.SelectedIndex = -1;
                    btnSearchRoleMenu_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                lblDeAssignUpdateErrorMsg.Text = "ERROR !! Role-Menu Map DeAssign Failed";
                dgridViewRoleMenus.SelectedIndex = -1;
                btnSearchRoleMenu_Click(sender, e);
            }
        }


    }
}