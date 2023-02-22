using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using RemittanceOperation.ICTCServiceClient;
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
    public partial class CancelInstantCashTxn : System.Web.UI.Page
    {
        string roleName = "", UserType = "";
        static Manager mg = new Manager();
        static CTCServiceClient ictcclient = new ICTCServiceClient.CTCServiceClient();

        protected void Page_Load(object sender, EventArgs e)
        {
            lblErrorMsg.Text = "";

            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
                //roleName = Session[CSessionName.S_ROLE_NAME].ToString();
                UserType = Session[CSessionName.S_FILE_PROCESS_USER_TYPE].ToString();

                if (!UserType.Equals("SuperAdmin"))
                {
                    if (!UserType.Equals("Authorizer"))
                    {
                        lblUserAuthorizationMsg.Text = "You are NOT Authorized to take any action in this screen !!!";
                        lblUserAuthorizationMsg.ForeColor = Color.Red;
                    }
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

        }

        protected void btnSearchTxn_Click(object sender, EventArgs e)
        {
            string refNo = textBoxRefNo.Text.Trim();
            DataTable aDTable = new DataTable();

            string whereClause = " WHERE [ICTC_NUMBER]='" + refNo + "'";

            aDTable = mg.GetIndividualTxnByWhereClause("ICTC", whereClause);
            if (aDTable.Rows.Count < 1)
            {
                lblErrorMsg.Text = "No Data Found";
            }

            dataGridViewTxnSearch.DataSource = null;
            dataGridViewTxnSearch.DataSource = aDTable;
            dataGridViewTxnSearch.DataBind();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if(!textBoxRemarks.Text.Trim().Equals(""))
            {

            }
            else
            {
                lblErrorMsg.Text = "Please provide Remarks";
            }
        }
    }
}