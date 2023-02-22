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
    public partial class BEFTNTxnCheck : System.Web.UI.Page
    {
        static Manager mg = new Manager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
            }
            else
            {
                Response.Redirect("Login.aspx");//CSessionName.F_LOGIN_PAGE);
            }
        }

        protected void btnSearchBeftnTxn_Click(object sender, EventArgs e)
        {
            lblBeftnSearchMsg.Text = "";
            string refNo = txtBeftnRefNo.Text;

            if (!refNo.Equals(""))
            {
                DataTable dtBeftn = mg.GetBEFTNTxnByRefNo(refNo);
                dataGridViewBeftnTxn.DataSource = null;
                dataGridViewBeftnTxn.DataSource = dtBeftn;
                dataGridViewBeftnTxn.DataBind();

                if (dtBeftn.Rows.Count == 0)
                {
                    lblBeftnSearchMsg.Text = "No Data Found";
                    //lblBeftnSearchMsg.ForeColor = Color.Red;
                }
            }
            else
            {
                txtBeftnRefNo.Focus();
            }
        }
    }
}