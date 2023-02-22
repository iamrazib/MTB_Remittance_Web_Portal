using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class MTBAcTxn : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable dtAllMtbData = new DataTable();


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
                dtpickerFrom.Text = DateTime.Now.ToString("yyyy-MM-dd");
                dtpickerTo.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        protected void btnSearchMTBAcTxn_Click(object sender, EventArgs e)
        {
            DateTime dateTime1 = DateTime.ParseExact(dtpickerFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dateTime2 = DateTime.ParseExact(dtpickerTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string dtValue1 = dateTime1.ToString("yyyy-MM-dd");
            string dtValue2 = dateTime2.ToString("yyyy-MM-dd");

            dtAllMtbData = new DataTable();
            dtAllMtbData = mg.GetOwnBankData(dtValue1, dtValue2);

            dataGridViewMTBAcTxn.DataSource = null;
            dataGridViewMTBAcTxn.DataSource = dtAllMtbData;
            dataGridViewMTBAcTxn.DataBind();

            lblTotalRec.Text = "Total: " + dtAllMtbData.Rows.Count;
        }

        protected void btnDownloadMTBAcTxnAsExcel_Click(object sender, EventArgs e)
        {
            DateTime dateTime1 = DateTime.ParseExact(dtpickerFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dateTime2 = DateTime.ParseExact(dtpickerTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string dtValue1 = dateTime1.ToString("yyyy-MM-dd");
            string dtValue2 = dateTime2.ToString("yyyy-MM-dd");

            string fileName = "OWN_Bank_Report_AsOn_" + dtValue1 + "_to_" + dtValue2 + ".xls";

            if (dtAllMtbData.Rows.Count > 0)
            {
                StringWriter tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = dtAllMtbData;
                dgGrid.DataBind();

                foreach (DataGridItem item in dgGrid.Items)
                {
                    for (int j = 0; j < item.Cells.Count; j++)
                    {
                        if (j == 5)
                        {
                            item.Cells[j].Attributes.Add("style", "mso-number-format:0\\.00");
                        }
                        else
                        {
                            item.Cells[j].Attributes.Add("style", "mso-number-format:\\@");
                        }
                    }
                }

                dgGrid.RenderControl(hw);
                Response.ContentType = "application/vnd.ms-excel";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName + "");
                this.EnableViewState = false;
                Response.Write(tw.ToString());
                Response.End();

            }
        }
    }
}