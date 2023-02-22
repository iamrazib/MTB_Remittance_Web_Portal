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
    public partial class MonitorMTBAcTxn : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable dtMtbFailedTxn = new DataTable();
        static DataTable dtMtbSuccessTxn = new DataTable();

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
                dtPickerFromDt.Text = DateTime.Now.AddDays(0).ToString("yyyy-MM-dd");
                dtPickerToDt.Text = DateTime.Now.ToString("yyyy-MM-dd");

                dtPickerFromDtSuccs.Text = DateTime.Now.AddDays(0).ToString("yyyy-MM-dd");
                dtPickerToDtSuccs.Text = DateTime.Now.ToString("yyyy-MM-dd");

                lblTotalRows.Text = "";
                lblTotalRowsSuccs.Text = "";
            }
        }

        protected void btnMTBFailedTxnSearch_Click(object sender, EventArgs e)
        {
            DateTime dateTime1, dateTime2;
            lblMsg.Text = "";

            dateTime1 = DateTime.ParseExact(dtPickerFromDt.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            dateTime2 = DateTime.ParseExact(dtPickerToDt.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string fromdt = dateTime1.ToString("yyyy-MM-dd");
            string todt = dateTime2.ToString("yyyy-MM-dd");

            dtMtbFailedTxn = new DataTable();
            dtMtbFailedTxn = mg.GetMTBAcFailedOrSuccessTxn(fromdt, todt, "FAILED");
            dataGridViewMtbFailedTxn.DataSource = null;
            dataGridViewMtbFailedTxn.DataSource = dtMtbFailedTxn;
            dataGridViewMtbFailedTxn.DataBind();

            lblTotalRows.Text = " Total Rows: " + dtMtbFailedTxn.Rows.Count;
        }

        protected void btnDownloadFailedTxn_Click(object sender, EventArgs e)
        {
            if (dtMtbFailedTxn.Rows.Count < 1)
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

                dtMtbFailedTxn = new DataTable();
                dtMtbFailedTxn = mg.GetMTBAcFailedOrSuccessTxn(fromdt, todt, "FAILED");

                string fileName = "MTBAcFailedTxn_" + fromdt + "_to_" + todt + ".xls";

                StringWriter tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = dtMtbFailedTxn;
                dgGrid.DataBind();

                foreach (DataGridItem item in dgGrid.Items)
                {
                    for (int j = 0; j < item.Cells.Count; j++)
                    {
                        if (j == 8)
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

        protected void btnMTBSuccessTxnSearch_Click(object sender, EventArgs e)
        {
            DateTime dateTime1, dateTime2;
            lblMsg.Text = "";

            dateTime1 = DateTime.ParseExact(dtPickerFromDtSuccs.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            dateTime2 = DateTime.ParseExact(dtPickerToDtSuccs.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string fromdt = dateTime1.ToString("yyyy-MM-dd");
            string todt = dateTime2.ToString("yyyy-MM-dd");

            dtMtbSuccessTxn = new DataTable();
            dtMtbSuccessTxn = mg.GetMTBAcFailedOrSuccessTxn(fromdt, todt, "SUCCESS");
            dataGridViewMtbSuccessfulTxn.DataSource = null;
            dataGridViewMtbSuccessfulTxn.DataSource = dtMtbSuccessTxn;
            dataGridViewMtbSuccessfulTxn.DataBind();

            lblTotalRowsSuccs.Text = " Total Rows: " + dtMtbSuccessTxn.Rows.Count;
        }

        protected void btnDownloadSuccessfulTxn_Click(object sender, EventArgs e)
        {
            if (dtMtbSuccessTxn.Rows.Count < 1)
            {
                lblMsg2.Text = "Nothing to Download !!!";
                lblMsg2.ForeColor = Color.Red;
            }
            else
            {
                lblMsg2.Text = "";

                DateTime dateTime1 = DateTime.ParseExact(dtPickerFromDtSuccs.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime dateTime2 = DateTime.ParseExact(dtPickerToDtSuccs.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                string fromdt = dateTime1.ToString("yyyy-MM-dd");
                string todt = dateTime2.ToString("yyyy-MM-dd");

                dtMtbSuccessTxn = new DataTable();
                dtMtbSuccessTxn = mg.GetMTBAcFailedOrSuccessTxn(fromdt, todt, "SUCCESS");

                string fileName = "MTBAcSuccessTxn_" + fromdt + "_to_" + todt + ".xls";

                StringWriter tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = dtMtbSuccessTxn;
                dgGrid.DataBind();

                foreach (DataGridItem item in dgGrid.Items)
                {
                    for (int j = 0; j < item.Cells.Count; j++)
                    {
                        if (j == 8)
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