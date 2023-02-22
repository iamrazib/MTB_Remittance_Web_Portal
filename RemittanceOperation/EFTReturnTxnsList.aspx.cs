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
    public partial class EFTReturnTxnsList : System.Web.UI.Page
    {
        static Manager mg = new Manager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
                //Panel1.Visible = false;
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                dTPickerFromReturnList.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                dTPickerToReturnList.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

                btnSearchReturnTxnList_Click(sender, e);
            }

        }

        protected void btnSearchReturnTxnList_Click(object sender, EventArgs e)
        {
            DateTime dateTime1, dateTime2;

            dateTime1 = DateTime.ParseExact(dTPickerFromReturnList.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            dateTime2 = DateTime.ParseExact(dTPickerToReturnList.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string dtValueFrom = dateTime1.ToString("yyyy-MM-dd");
            string dtValueTo = dateTime2.ToString("yyyy-MM-dd");

            DataTable dtBeftnReturnTxnList = mg.GetBEFTNReturnTxnList(dtValueFrom, dtValueTo);

            dataGridViewReturnTxnList.DataSource = null;
            dataGridViewReturnTxnList.DataSource = dtBeftnReturnTxnList;
            dataGridViewReturnTxnList.DataBind();

            lblTotalRows.Text = "Total Rows: " + dtBeftnReturnTxnList.Rows.Count;
        }

        protected void btnDownloadReturnTxnList_Click(object sender, EventArgs e)
        {
            DateTime dateTime1 = DateTime.ParseExact(dTPickerFromReturnList.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dateTime2 = DateTime.ParseExact(dTPickerToReturnList.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string dtValueFrom = dateTime1.ToString("yyyy-MM-dd");
            string dtValueTo = dateTime2.ToString("yyyy-MM-dd");

            DataTable dtBeftnReturnTxnList = mg.GetBEFTNReturnTxnList(dtValueFrom, dtValueTo);

            string fileName = "Returned_Txn_List_" + dtValueFrom + "_to_" + dtValueTo + ".xls";

            if (dtBeftnReturnTxnList.Rows.Count > 0)
            {
                StringWriter tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = dtBeftnReturnTxnList;
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