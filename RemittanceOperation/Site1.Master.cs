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
    public partial class Site1 : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
                S_CURRENT_USERID.Text = Session[CSessionName.S_CURRENT_USERID].ToString();

                MasterPageSideBar.InnerHtml = Session[CSessionName.S_MENU_SESSION].ToString();
            }
        }
    }
}