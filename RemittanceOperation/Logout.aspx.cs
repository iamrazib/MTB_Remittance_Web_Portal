using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class Logout : System.Web.UI.Page
    {
        static Manager mg = new Manager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
                mg.UpdateLogoutTime(Session[CSessionName.S_CURRENT_USERID].ToString());

                Session[CSessionName.S_CURRENT_USER_RM] = null;
                Session[CSessionName.S_LOGIN_TIME] = null;
                Session[CSessionName.S_SESSION_ID] = null;
                Session[CSessionName.S_CURRENT_USERID] = null;
                Session[CSessionName.S_CURRENT_USER_FULL_NAME] = null;
                Session[CSessionName.S_CURRENT_USER_EMAIL] = null;
                //Response.Redirect(CSessionName.F_LOGIN_PAGE);                              

                Response.Redirect("Login.aspx");
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
    }
}