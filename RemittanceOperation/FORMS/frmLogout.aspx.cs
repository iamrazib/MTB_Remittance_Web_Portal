using RemittanceOperation.AppCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation.FORMS
{
    public partial class frmLogout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
                Session[CSessionName.S_CURRENT_USER_RM] = null;
                Session[CSessionName.S_LOGIN_TIME] = null;
                Session[CSessionName.S_SESSION_ID] = null;
                Session[CSessionName.S_CURRENT_USERID] = null;
                Session[CSessionName.S_CURRENT_USER_FULL_NAME] = null;
                Session[CSessionName.S_CURRENT_USER_EMAIL] = null;
                //Response.Redirect(CSessionName.F_LOGIN_PAGE);

                Response.Redirect("frmLogin.aspx");
            }
            else
            {
                Response.Redirect("frmLogin.aspx");
            }
        }
    }
}