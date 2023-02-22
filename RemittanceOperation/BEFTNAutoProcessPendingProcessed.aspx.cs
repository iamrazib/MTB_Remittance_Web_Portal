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
    public partial class BEFTNAutoProcessPendingProcessed : System.Web.UI.Page
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
                //dtPickerAutoProcessFromDt.Text = DateTime.Now.ToString("dd-MMM-yyyy");
                //dtPickerAutoProcessToDt.Text = DateTime.Now.ToString("dd-MMM-yyyy");

                dtPickerAutoProcessFromDt.Text = DateTime.Now.ToString("yyyy-MM-dd");
                dtPickerAutoProcessToDt.Text = DateTime.Now.ToString("yyyy-MM-dd");

                btnSearchPendingBeftn_Click(sender, e);
            }
        }

        protected void btnSearchPendingBeftn_Click(object sender, EventArgs e)
        {
            DataTable dtBeftnPendingToProcess = mg.GetBEFTNTxnPendingToProcess();
            //DataTable dtBeftnPendingCount = mg.GetBEFTNTxnPendingToProcessCount();
            string BeftnPendingCount = mg.GetBEFTNTxnPendingToProcessCount();

            string onProcessingCount = "";
            DataTable dtOnProcessing = mg.GetBEFTNOnProcessing(ref onProcessingCount);

            dataGridViewPendingBEFTNTxn.DataSource = null;
            dataGridViewPendingBEFTNTxn.DataSource = dtBeftnPendingToProcess;
            dataGridViewPendingBEFTNTxn.DataBind();
            lblPendingBeftnCount.Text = BeftnPendingCount; //"Count = " + Convert.ToInt32(dtBeftnPendingCount.Rows[0][0]).ToString();
                        
            dataGridViewOnProcess.DataSource = null;
            dataGridViewOnProcess.DataSource = dtOnProcessing;
            dataGridViewOnProcess.DataBind();
            lblAutoProcessRunning.Text = "Processing >> " + onProcessingCount; // dtOnProcessing.Rows.Count;
        }

        protected void btnBEFTNAutoProcessTxnSearch_Click(object sender, EventArgs e)
        {
            DateTime dateTime1, dateTime2;
            //string lblMsg = "";

            //dateTime1 = DateTime.ParseExact(dtPickerAutoProcessFromDt.Text, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
            //dateTime2 = DateTime.ParseExact(dtPickerAutoProcessToDt.Text, "dd-MMM-yyyy", CultureInfo.InvariantCulture);

            dateTime1 = DateTime.ParseExact(dtPickerAutoProcessFromDt.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            dateTime2 = DateTime.ParseExact(dtPickerAutoProcessToDt.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string fromdt = dateTime1.ToString("yyyy-MM-dd");
            string todt = dateTime2.ToString("yyyy-MM-dd");

            DataTable dtBeftnAutoProcessedTxn = mg.GetBEFTNTxnAutoProcessed(fromdt, todt);

            string successCount = "", failCount = "";
            DataTable dtSuccessFailCount = mg.GetAutoProcessSuccessFailCount(fromdt, todt, ref successCount, ref failCount);
                        
            dataGridViewBeftnAutoProcessedTxn.DataSource = null;
            dataGridViewBeftnAutoProcessedTxn.DataSource = dtBeftnAutoProcessedTxn;
            dataGridViewBeftnAutoProcessedTxn.DataBind();

            lblBEFTNAutoSuccess.Text = successCount;
            lblBEFTNAutoFail.Text = failCount;
            lblAutoProcessDataFetchTime.Text = "Time: " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss tt");
        }

        
        protected void btnDownloadAutoProcessTxn_Click(object sender, EventArgs e)
        {
            DateTime dateTime1 = DateTime.ParseExact(dtPickerAutoProcessFromDt.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dateTime2 = DateTime.ParseExact(dtPickerAutoProcessToDt.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string fromdt = dateTime1.ToString("yyyy-MM-dd");
            string todt = dateTime2.ToString("yyyy-MM-dd");

            DataTable dtBeftnAutoProcessedTxn = mg.GetBEFTNTxnAutoProcessedForDownload(fromdt, todt);

            string fileName = "BeftnAutoProcessedTxn_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

            if (dtBeftnAutoProcessedTxn.Rows.Count > 0)
            {
                StringWriter tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = dtBeftnAutoProcessedTxn;
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

        protected void btnDownloadOnProcessRecords_Click(object sender, EventArgs e)
        {
            DataTable dtOnProcessingDL = mg.GetBEFTNOnProcessingForDownload();
            string fileName = "BeftnPendingTxnToProcess_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

            if (dtOnProcessingDL.Rows.Count > 0)
            {
                StringWriter tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = dtOnProcessingDL;
                dgGrid.DataBind();

                foreach (DataGridItem item in dgGrid.Items)
                {
                    for (int j = 0; j < item.Cells.Count; j++)
                    {
                        if (j == 2)
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