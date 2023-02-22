using Newtonsoft.Json;
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
    public partial class BEFTNAutoProcessFailedTxn : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable dtFailedTxn; 

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
                dTPickerBEFTNAutoFailedTxnFrom.Text = DateTime.Now.ToString("yyyy-MM-dd");
                dTPickerBEFTNAutoFailedTxnTo.Text = DateTime.Now.ToString("yyyy-MM-dd");
                lblDownloadMsg.Text = "";
                btnBEFTNAutoFailedTxnSearch_Click(sender, e);
            }
        }

        protected void btnBEFTNAutoFailedTxnSearch_Click(object sender, EventArgs e)
        {
            DateTime dateTime1, dateTime2;
            lblDownloadMsg.Text = "";

            dateTime1 = DateTime.ParseExact(dTPickerBEFTNAutoFailedTxnFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            dateTime2 = DateTime.ParseExact(dTPickerBEFTNAutoFailedTxnTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string fromdt = dateTime1.ToString("yyyy-MM-dd");
            string todt = dateTime2.ToString("yyyy-MM-dd");

            //DataTable dtBeftnAutoProcessedFailedTxn = mg.GetBEFTNAutoProcessedFailed(fromdt, todt);

            DataRow drow;
            dtFailedTxn = CreateDataTableFailedTxn();
            string resp = "", failCount = "";

            BEFTNFailedResponseData clsResponseData = new BEFTNFailedResponseData();

            DataTable dtBeftnAutoProcessedFailedTxn = mg.GetBEFTNAutoProcessedFailedWithErrorResp(fromdt, todt, ref failCount);

            for (int rw = 0; rw < dtBeftnAutoProcessedFailedTxn.Rows.Count; rw++)
            {
                drow = dtFailedTxn.NewRow();

                drow["Exh"] = dtBeftnAutoProcessedFailedTxn.Rows[rw]["Exh"];
                drow["ReferenceNo"] = dtBeftnAutoProcessedFailedTxn.Rows[rw]["ReferenceNo"];
                drow["Amount"] = dtBeftnAutoProcessedFailedTxn.Rows[rw]["Amount"];
                drow["BeneAcc"] = dtBeftnAutoProcessedFailedTxn.Rows[rw]["BeneAcc"];
                drow["TrxnDate"] = dtBeftnAutoProcessedFailedTxn.Rows[rw]["TransDate"];
                drow["Beneficiary"] = dtBeftnAutoProcessedFailedTxn.Rows[rw]["BeneficiaryName"];
                drow["Status"] = dtBeftnAutoProcessedFailedTxn.Rows[rw]["Status"];
                drow["Remarks"] = dtBeftnAutoProcessedFailedTxn.Rows[rw]["Remarks"];
                //drow["EFTReqId"] = dtBeftnAutoProcessedFailedTxn.Rows[rw]["EFTReqId"];
                drow["TxnType"] = dtBeftnAutoProcessedFailedTxn.Rows[rw]["TxnType"];
                resp = dtBeftnAutoProcessedFailedTxn.Rows[rw]["RespData"].ToString();

                if (!resp.Equals(""))
                {
                    try
                    {
                        clsResponseData = JsonConvert.DeserializeObject<BEFTNFailedResponseData>(resp);
                        drow["ErrorMsg"] = clsResponseData.apiMsg;
                        drow["RespMsg"] = clsResponseData.resMsg;
                    }
                    catch (Exception exc)
                    { }
                }
                else
                {
                    drow["ErrorMsg"] = "";
                    drow["RespMsg"] = "";
                }

                dtFailedTxn.Rows.Add(drow);
            }

            dataGridViewBEFTNAutoFailedTxn.DataSource = null;
            dataGridViewBEFTNAutoFailedTxn.DataSource = dtFailedTxn;
            dataGridViewBEFTNAutoFailedTxn.DataBind();

            lblBEFTNAutoFailedCnt.Text = failCount; //"Count=" + dtFailedTxn.Rows.Count;
        }

        private DataTable CreateDataTableFailedTxn()
        {
            /*
             Exh,[ReferenceNo],[Amount], ExhAcc, TransDate,[BeneficiaryName],[Status],[Remarks], EFTReqId , RespData
             */
            DataTable dt = new DataTable();
            dt.Columns.Add("Exh");
            dt.Columns.Add("TxnType");
            dt.Columns.Add("ReferenceNo");
            dt.Columns.Add("Amount");
            dt.Columns.Add("BeneAcc");
            dt.Columns.Add("TrxnDate");
            dt.Columns.Add("Beneficiary");
            dt.Columns.Add("Status");
            dt.Columns.Add("Remarks");            
            //dt.Columns.Add("EFTReqId");
            dt.Columns.Add("ErrorMsg");
            dt.Columns.Add("RespMsg");
            return dt;
        }

        protected void btnDownloadAutoProcessFailedTxn_Click(object sender, EventArgs e)
        {
            string fileName = "BeftnAutoProcessFailedTxn_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";

            if (dtFailedTxn.Rows.Count > 0)
            {
                StringWriter tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = dtFailedTxn;
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
            else
            {
                lblDownloadMsg.Text = "Nothing to download !!!";
                lblDownloadMsg.ForeColor = Color.Red;
            }
        }
    }

    public class BEFTNFailedResponseData
    {
        public string apiMsg { get; set; }
        public string resCode { get; set; }
        public string resMsg { get; set; }
    }

}