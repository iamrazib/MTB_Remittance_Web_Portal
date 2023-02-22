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
    public partial class MenuConfigure : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        string roleName = "";
        string permittedRole = "ADMIN";

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
                DisplayUserMenu();
                BindDropDownList(ddlMenuParent, "MenuTitle", "MenuId", "-- Please Select --");
                BindDropDownListNewRecord(ddlNewMenuParent, "MenuTitle", "MenuId", "-- Please Select --");
            }
        }

        private void BindDropDownListNewRecord(DropDownList ddlNewMenuParent, string text, string value, string defaultText)
        {
            ddlNewMenuParent.DataSource = mg.GetRMSParentMenuList();
            ddlNewMenuParent.DataTextField = text;
            ddlNewMenuParent.DataValueField = value;
            ddlNewMenuParent.DataBind();
            ddlNewMenuParent.Items.Insert(0, new ListItem(defaultText, "-1"));
            ddlNewMenuParent.Items.Insert(1, new ListItem("New Parent", "-2"));
        }

        private void BindDropDownList(DropDownList ddl, string text, string value, string defaultText)
        {
            ddl.DataSource = mg.GetRMSParentMenuList();
            ddl.DataTextField = text;
            ddl.DataValueField = value;
            ddl.DataBind();
            //ddl.Items.Insert(0, new ListItem(defaultText, "0"));
        }

        private void DisplayUserMenu()
        {
            DataTable dtUserMenus = mg.GetRMSUserMenuList();
            dgridViewUserMenus.DataSource = null;
            dgridViewUserMenus.DataSource = dtUserMenus;
            dgridViewUserMenus.DataBind();
        }

        protected void dgridViewUserMenus_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = dgridViewUserMenus.SelectedRow;

            txtSlNo.Text = row.Cells[1].Text;
            txtMenuId.Text = row.Cells[2].Text;
            txtMenuTitle.Text = row.Cells[3].Text;
            txtMenuUrl.Text = row.Cells[4].Text;
            txtInternalSlNo.Text = row.Cells[7].Text;
            
            ddlMenuActive.SelectedIndex = ddlMenuActive.Items.IndexOf(ddlMenuActive.Items.FindByText(row.Cells[6].Text));
            ddlMenuParent.SelectedIndex = ddlMenuParent.Items.IndexOf(ddlMenuParent.Items.FindByText(row.Cells[5].Text.ToString().Trim()));

            lblUpdateSuccMsg.Text = "";
            lblUpdateErrorMsg.Text = "";
        }

        protected void btnUpdateMenuInfo_Click(object sender, EventArgs e)
        {
            if (!roleName.Equals(permittedRole))
            {
                lblUserAuthorizationMsg.Text = "You are NOT Authorized to take any action in this screen !!!";
                lblUserAuthorizationMsg.ForeColor = Color.Red;
            }
            else
            {
                lblUserAuthorizationMsg.Text = "";
                int slNo = Convert.ToInt32(txtSlNo.Text);
                int menuId = Convert.ToInt32(txtMenuId.Text);

                int parentValue = Convert.ToInt32(ddlMenuParent.SelectedValue);
                //string sit = ddlMenuParent.SelectedItem.Text;
                //string siv = ddlMenuParent.SelectedItem.Value;
                //string st = ddlMenuParent.Text;

                string menuTitle = txtMenuTitle.Text;
                string menuUrl = txtMenuUrl.Text;
                int menuActivity = Convert.ToInt32(ddlMenuActive.SelectedValue);
                int internalSlNo = Convert.ToInt32(txtInternalSlNo.Text);

                try
                {
                    bool stats = mg.UpdateWebUserMenu(slNo, menuId, menuTitle, menuUrl, parentValue, menuActivity, internalSlNo);
                    if (stats)
                    {
                        lblUpdateSuccMsg.Text = "Menu Updated Successfully...";
                        DisplayUserMenu();

                        txtSlNo.Text = "";
                        txtMenuId.Text = "";
                        txtMenuTitle.Text = "";
                        txtMenuUrl.Text = "";
                        txtInternalSlNo.Text = "";

                        dgridViewUserMenus.SelectedIndex = -1;
                    }
                }
                catch (Exception ex) { }
            }
        }

        protected void btnSaveNewMenu_Click(object sender, EventArgs e)
        {
            if (!roleName.Equals(permittedRole))
            {
                lblUserAuthorizationMsg.Text = "You are NOT Authorized to take any action in this screen !!!";
                lblUserAuthorizationMsg.ForeColor = Color.Red;
            }
            else
            {
                lblUserAuthorizationMsg.Text = "";
                string menuTitle = txtNewMenuTitle.Text;
                string menuUrl = txtNewMenuUrl.Text;
                int parentValue = Convert.ToInt32(ddlNewMenuParent.SelectedValue);
                int menuActivity = Convert.ToInt32(ddlNewMenuActive.SelectedValue);

                if (parentValue != -1)
                {
                    if ((parentValue == -2) && (!menuUrl.Equals("#")))
                    {
                        lblSaveErrorMsg.Text = "New Parent Menu URL should be # ";
                    }
                    else
                    {
                        try
                        {
                            bool stats = mg.SaveWebUserMenu(menuTitle, menuUrl, parentValue, menuActivity);
                            if (stats)
                            {
                                lblSaveSuccMsg.Text = "Menu Saved Successfully...";
                                txtNewMenuTitle.Text = "";
                                txtNewMenuUrl.Text = "";
                                DisplayUserMenu();
                                BindDropDownList(ddlMenuParent, "MenuTitle", "MenuId", "-- Please Select --");
                                BindDropDownListNewRecord(ddlNewMenuParent, "MenuTitle", "MenuId", "-- Please Select --");
                            }
                            else
                            {
                                lblSaveErrorMsg.Text = "ERROR !! Menu Save Failed";
                                DisplayUserMenu();
                            }
                        }
                        catch (Exception ex) { }
                    }
                }
                else
                {
                    lblSaveErrorMsg.Text = "Please Select Parent Menu";
                }
            }
        }

    }
}