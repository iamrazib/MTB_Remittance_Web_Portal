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
    public partial class ExhouseBalanceNew : System.Web.UI.Page
    {
        static Manager mg = new Manager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
                btnRefreshExchBalance_Click(sender, e);
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

        protected void btnRefreshExchBalance_Click(object sender, EventArgs e)
        {
            DataTable dtBalance = mg.GetExhouseBalanceNew();
            dataGridViewExchangeHouseBalance.DataSource = null;
            dataGridViewExchangeHouseBalance.DataSource = dtBalance;
            dataGridViewExchangeHouseBalance.DataBind();

            lblTotalBalance.Text = mg.GetTotalExchBalanceNew();
            lblTotalWageBalance.Text = mg.GetTotalWageEarnersExchBalanceNew();
            lblTotalServiceBalance.Text = mg.GetTotalServiceRemExchBalanceNew();
        }

        protected void dataGridViewExchangeHouseBalance_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //[PartyId],[UserId],ExHName,[NRTAccount],[WalletAccount],[USDAccount],[AEDAccount],[NRTBalance],USDBalance,AEDBalance,LastUpdate,'Api/File'
            double nrtBalance;
            int partyid;

            e.Row.Cells[3].Attributes["width"] = "130px"; //NRTAccount
            e.Row.Cells[4].Attributes["width"] = "130px"; //WalletAccount
            e.Row.Cells[5].Attributes["width"] = "130px"; //USDAccount
            e.Row.Cells[6].Attributes["width"] = "130px"; //AEDAccount
            e.Row.Cells[7].Attributes["width"] = "100px"; // NRTBalance
            e.Row.Cells[10].Attributes["width"] = "150px"; // Date

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[7].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[10].HorizontalAlign = HorizontalAlign.Center;

                nrtBalance = Convert.ToDouble(e.Row.Cells[7].Text);
                partyid = Convert.ToInt32(e.Row.Cells[0].Text);

                if (partyid != 0 && partyid > 10000)
                {
                    if (nrtBalance < 10000)
                    {
                        e.Row.BackColor = Color.FromName("yellow");
                    }
                }
            }
        }
    }
}