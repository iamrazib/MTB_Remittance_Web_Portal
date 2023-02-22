using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class GCCReportScreen : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        //static DataTable dtReconcileTxns = new DataTable();

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
                dtpickerFromRpt.Text = DateTime.Now.ToString("yyyy-MM-dd");
                dtpickerToRpt.Text = DateTime.Now.ToString("yyyy-MM-dd");
                btnViewReportSearch_Click(sender, e);
            }
        }

        protected void btnViewReportSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string dtValueFrm = dtpickerFromRpt.Text;
                string dtValueTo = dtpickerToRpt.Text;

                DataTable dtReconcileTxns = new DataTable();
                dtReconcileTxns = mg.GetGCCPaidTxnReportList(dtValueFrm, dtValueTo);
                dataGridViewReportData.DataSource = null;
                dataGridViewReportData.DataSource = dtReconcileTxns;
                dataGridViewReportData.DataBind();

                lblGCCRptTotalRows.Text = "Total Row(s) : " + dtReconcileTxns.Rows.Count;                       
            }
            catch (Exception exc)
            {
            }
        }

        protected void btnDownloadGCCRptAsExcel_Click(object sender, EventArgs e)
        {
            string dtValueFrm = dtpickerFromRpt.Text;
            string dtValueTo = dtpickerToRpt.Text;

            string fileName = "GCC_Report_" + dtValueFrm + "_to_" + dtValueTo;
            DataTable dtReconcileTxns = new DataTable();
            dtReconcileTxns = mg.GetGCCPaidTxnReportList(dtValueFrm, dtValueTo);

            if (dtReconcileTxns.Rows.Count > 0)
            {
                StringWriter tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = dtReconcileTxns;
                dgGrid.DataBind();

                foreach (DataGridItem item in dgGrid.Items)
                {
                    for (int j = 0; j < item.Cells.Count; j++)
                    {
                        if (j == 1)
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