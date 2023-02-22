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
    public partial class DuplicateMTBTxnCheck : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable dtMtbDuplicateTxn = new DataTable();
        static DataTable dtMtbSameBeneMultiTxn = new DataTable();

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


                dtPickerFromDtSameBene.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                dtPickerToDtSameBene.Text = DateTime.Now.ToString("yyyy-MM-dd");

                lblMsgSameBeneMultiTxnDl.Text = "";
            }
        }

        protected void btnMTBDuplicateTxnSearch_Click(object sender, EventArgs e)
        {
            DateTime dateTime1, dateTime2;
            lblMsg.Text = "";

            dateTime1 = DateTime.ParseExact(dtPickerFromDt.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            dateTime2 = DateTime.ParseExact(dtPickerToDt.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string fromdt = dateTime1.ToString("yyyy-MM-dd");
            string todt = dateTime2.ToString("yyyy-MM-dd");

            dtMtbDuplicateTxn = new DataTable();
            dtMtbDuplicateTxn = mg.GetMTBAcDuplicateTxn(fromdt, todt, "SEARCH");
            dataGridViewMtbDuplicateTxn.DataSource = null;
            dataGridViewMtbDuplicateTxn.DataSource = dtMtbDuplicateTxn;
            dataGridViewMtbDuplicateTxn.DataBind();

            lblTotalRows.Text = " Total Rows: " + dtMtbDuplicateTxn.Rows.Count;
        }

        protected void btnDownloadDuplicateTxn_Click(object sender, EventArgs e)
        {
            if (dtMtbDuplicateTxn.Rows.Count < 1)
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

                dtMtbDuplicateTxn = new DataTable();
                dtMtbDuplicateTxn = mg.GetMTBAcDuplicateTxn(fromdt, todt, "DOWNLOAD");

                string fileName = "MTBAcDuplicateTxn_" + fromdt + "_to_" + todt + ".xls";

                StringWriter tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = dtMtbDuplicateTxn;
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




        protected void btnMTBMultiBeneTxnSearch_Click(object sender, EventArgs e)
        {
            DateTime dateTime1, dateTime2;
            lblMsg.Text = "";

            dateTime1 = DateTime.ParseExact(dtPickerFromDtSameBene.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            dateTime2 = DateTime.ParseExact(dtPickerToDtSameBene.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string fromdt = dateTime1.ToString("yyyy-MM-dd");
            string todt = dateTime2.ToString("yyyy-MM-dd");

            dtMtbSameBeneMultiTxn = new DataTable();
            dtMtbSameBeneMultiTxn = mg.GetMTBAcSameBeneficiaryMultiTxn(fromdt, todt, "SEARCH");
            dataGridViewMtbMultiBeneTxn.DataSource = null;
            dataGridViewMtbMultiBeneTxn.DataSource = dtMtbSameBeneMultiTxn;
            dataGridViewMtbMultiBeneTxn.DataBind();

            lblTotalRowsSameBeneMultiTxn.Text = " Total Rows: " + dtMtbSameBeneMultiTxn.Rows.Count;
        }


        protected void btnDownloadSameBeneMultiTxn_Click(object sender, EventArgs e)
        {
            if (dtMtbSameBeneMultiTxn.Rows.Count < 1)
            {
                lblMsgSameBeneMultiTxnDl.Text = "Nothing to Download !!!";
                lblMsgSameBeneMultiTxnDl.ForeColor = Color.Red;
            }
            else
            {
                lblMsgSameBeneMultiTxnDl.Text = "";

                DateTime dateTime1 = DateTime.ParseExact(dtPickerFromDtSameBene.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime dateTime2 = DateTime.ParseExact(dtPickerToDtSameBene.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                string fromdt = dateTime1.ToString("yyyy-MM-dd");
                string todt = dateTime2.ToString("yyyy-MM-dd");

                dtMtbSameBeneMultiTxn = new DataTable();
                dtMtbSameBeneMultiTxn = mg.GetMTBAcSameBeneficiaryMultiTxn(fromdt, todt, "DOWNLOAD");

                string fileName = "MTBAcSameBeneMultiTxn_" + fromdt + "_to_" + todt + ".xls";

                StringWriter tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = dtMtbSameBeneMultiTxn;
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

        protected void dataGridViewMtbMultiBeneTxn_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.RowIndex > 0)
                {
                    GridViewRow prevrow = dataGridViewMtbMultiBeneTxn.Rows[e.Row.RowIndex - 1];

                    if (prevrow.RowType == DataControlRowType.DataRow)
                    {
                        string currRowPIN = e.Row.Cells[1].Text.Trim();
                        string prevRowPIN = prevrow.Cells[1].Text.Trim();

                        if (currRowPIN.Equals(prevRowPIN))
                        {
                            e.Row.BackColor = Color.FromName("yellow");
                            prevrow.BackColor = Color.FromName("yellow");
                        }
                    }
                }
            }
        }

    }
}