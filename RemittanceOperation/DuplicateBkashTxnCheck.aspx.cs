using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class DuplicateBkashTxnCheck : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable dtBkashDuplicateTxn = new DataTable();

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
                dtPickerFromDt.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                dtPickerToDt.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        protected void btnBkashDuplicateTxnSearch_Click(object sender, EventArgs e)
        {
            DateTime dateTime1, dateTime2;
            lblMsg.Text = "";

            dateTime1 = DateTime.ParseExact(dtPickerFromDt.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            dateTime2 = DateTime.ParseExact(dtPickerToDt.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string fromdt = dateTime1.ToString("yyyy-MM-dd");
            string todt = dateTime2.ToString("yyyy-MM-dd");

            dtBkashDuplicateTxn = new DataTable();
            dtBkashDuplicateTxn = mg.GetBkashDuplicateTxn(fromdt, todt, "SEARCH");
            dataGridViewBkashDuplicateTxn.DataSource = null;
            dataGridViewBkashDuplicateTxn.DataSource = dtBkashDuplicateTxn;
            dataGridViewBkashDuplicateTxn.DataBind();

            lblTotalRows.Text = " Total Rows: " + dtBkashDuplicateTxn.Rows.Count;
        }

        protected void btnDownloadDuplicateTxn_Click(object sender, EventArgs e)
        {
            if (dtBkashDuplicateTxn.Rows.Count < 1)
            {
                lblMsg.Text = "Nothing to Download !!!";
                lblMsg.ForeColor = Color.Red;
            }
            else
            {
                lblMsg.Text = "";

                DateTime dateTime1 = DateTime.ParseExact(dtPickerFromDt.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime dateTime2 = DateTime.ParseExact(dtPickerToDt.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                string fromdt = dateTime1.ToString("yyyy-MM-dd");
                string todt = dateTime2.ToString("yyyy-MM-dd");

                dtBkashDuplicateTxn = new DataTable();
                dtBkashDuplicateTxn = mg.GetBkashDuplicateTxn(fromdt, todt, "DOWNLOAD");

                string fileName = "BkashDuplicateTxn_" + fromdt + "_to_" + todt + ".xls";

                StringWriter tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = dtBkashDuplicateTxn;
                dgGrid.DataBind();

                foreach (DataGridItem item in dgGrid.Items)
                {
                    for (int j = 0; j < item.Cells.Count; j++)
                    {
                        item.Cells[j].Attributes.Add("style", "mso-number-format:\\@");
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