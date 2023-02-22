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
    public partial class ExBalance_pre : System.Web.UI.Page
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

        protected void btnRefreshAPIExchBalance_Click(object sender, EventArgs e)
        {
            //dataGridViewAPIExchangeHouse

            //DataTable dtApiBalance = mg.GetAPIBasedExchBalance();

            //dataGridViewAPIExchangeHouse.DataSource = null;
            //dataGridViewAPIExchangeHouse.DataSource = dtApiBalance;
            //dataGridViewAPIExchangeHouse.DataBind();
        }

        protected void dataGridViewAPIExchangeHouse_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    //dataGridViewAPIExchangeHouse.Columns["NRTAccount"].ItemStyle.Width = 80;
            //    //dataGridViewAPIExchangeHouse.Columns[2].ItemStyle.Width = 80;
            //    //dataGridViewAPIExchangeHouse.Columns[3].ItemStyle.Width = 80;
            //    //dataGridViewAPIExchangeHouse.Columns[4].ItemStyle.Width = 100;
            //}

            //  0             1                  2              3             4          5           6
            //[PartyId],[ExchangeHouseName],[NRTAccount],[WalletAccount],[NRTBalance],LastUpdate, ExhType 

            double nrtBalance;

            e.Row.Cells[2].Attributes["width"] = "130px"; //NRTAccount
            e.Row.Cells[3].Attributes["width"] = "130px"; //WalletAccount
            e.Row.Cells[4].Attributes["width"] = "100px"; // NRTBalance
            e.Row.Cells[5].Attributes["width"] = "150px"; // Date

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[5].HorizontalAlign = HorizontalAlign.Center;

                nrtBalance = Convert.ToDouble(e.Row.Cells[4].Text);
                if (nrtBalance < 5000)
                {
                    e.Row.BackColor = Color.FromName("yellow");
                }
            }
        }

        protected void btnRefreshFileExchBalance_Click(object sender, EventArgs e)
        {
            //DataTable dtFileBasedExch = mg.GetFileBasedExchBalance();
            //dGridViewFileBasedExch.DataSource = null;
            //dGridViewFileBasedExch.DataSource = dtFileBasedExch;
            //dGridViewFileBasedExch.DataBind();
        }

        protected void dGridViewFileBasedExch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            double nrtBalance;
            e.Row.Cells[2].Attributes["width"] = "130px"; //Account
            e.Row.Cells[3].Attributes["width"] = "100px"; // NRTBalance
            e.Row.Cells[4].Attributes["width"] = "150px"; // Date

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Right;
                e.Row.Cells[4].HorizontalAlign = HorizontalAlign.Center;

                nrtBalance = Convert.ToDouble(e.Row.Cells[3].Text);
                if (nrtBalance < 5000)
                {
                    e.Row.BackColor = Color.FromName("yellow");
                }
            }
        }


    }
}