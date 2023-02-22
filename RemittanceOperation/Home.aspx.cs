using RemittanceOperation.AppCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class Home : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
                //S_CURRENT_USER_RM.Text = Session[CSessionName.S_CURRENT_USER_RM].ToString();
                S_LOGIN_TIME.Text = Session[CSessionName.S_LOGIN_TIME].ToString();
                S_SESSION_ID.Text = Session[CSessionName.S_SESSION_ID].ToString();

                S_CURRENT_USERID.Text = Session[CSessionName.S_CURRENT_USERID].ToString();
                S_CURRENT_USER_FULL_NAME.Text = Session[CSessionName.S_CURRENT_USER_FULL_NAME].ToString();
                S_CURRENT_USER_EMAIL.Text = Session[CSessionName.S_CURRENT_USER_EMAIL].ToString();


                HtmlGenericControl SideBarDiv = (HtmlGenericControl)Master.FindControl("MasterPageSideBar");
                SideBarDiv.Visible = true;
            }
            else
            {
                Response.Redirect("Login.aspx");//CSessionName.F_LOGIN_PAGE);
            }
        }
    }
}