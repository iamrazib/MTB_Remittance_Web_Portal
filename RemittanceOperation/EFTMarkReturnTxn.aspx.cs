using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class EFTMarkReturnTxn : System.Web.UI.Page
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
                dtPickerReturnDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                lblTxnCheckNoDataFound.Text = "";
                lblMarkReturnStatusMsg.Text = "";
            }
        }

        protected void btnSearchTxnCheck_Click(object sender, EventArgs e)
        {
            lblMarkReturnStatusMsg.Text = "";

            if (!txtBoxPinTxnCheck.Text.Trim().Equals(""))
            {
                DataTable aDataTable = mg.GetBEFTNDataFromNewSystem(txtBoxPinTxnCheck.Text.Trim());
                dGridViewTxnCheckOutput.DataSource = null;
                dGridViewTxnCheckOutput.DataSource = aDataTable;
                dGridViewTxnCheckOutput.DataBind();

                if (aDataTable.Rows.Count < 1)
                {
                    lblTxnCheckNoDataFound.Text = "NO DATA FOUND !!!";
                }
                else
                {
                    lblTxnCheckNoDataFound.Text = "";
                }
            }
        }

        protected void btnMarkPrincipalTxnReturn_Click(object sender, EventArgs e)
        {
            if (!txtBoxPinTxnCheck.Text.Trim().Equals(""))
            {
                DateTime dateTime1 = DateTime.ParseExact(dtPickerReturnDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);               
                string returnDt = dateTime1.ToString("yyyy-MM-dd");
                string retReason = txtReturnReason.Text.Trim();

                bool stat = mg.UpdateEFTReturnStatus(txtBoxPinTxnCheck.Text.Trim(), returnDt, retReason, "MAIN");
                if (stat)
                {
                    lblMarkReturnStatusMsg.Text = "Principal Return Status Update Successfully...";
                    lblMarkReturnStatusMsg.ForeColor = Color.Green;
                }
            }
        }

        protected void btnMarkIncentiveTxnReturn_Click(object sender, EventArgs e)
        {
            if (!txtBoxPinTxnCheck.Text.Trim().Equals(""))
            {
                DateTime dateTime1 = DateTime.ParseExact(dtPickerReturnDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                string returnDt = dateTime1.ToString("yyyy-MM-dd");
                string retReason = txtReturnReason.Text.Trim();

                bool stat = mg.UpdateEFTReturnStatus(txtBoxPinTxnCheck.Text.Trim(), returnDt, retReason, "INCENTIVE");
                if (stat)
                {
                    lblMarkReturnStatusMsg.Text = "Incentive Return Status Update Successfully...";
                    lblMarkReturnStatusMsg.ForeColor = Color.Green;
                }
            }
        }

        protected void dGridViewTxnCheckOutput_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.Cells[3].Text.ToUpper().Contains("SUCCESS"))
                {
                    e.Row.Cells[3].BackColor = Color.GreenYellow;
                }
                if (e.Row.Cells[3].Text.ToUpper().Contains("RETURN"))
                {
                    e.Row.Cells[3].BackColor = Color.LightPink;
                }
            }
        }
    }
}