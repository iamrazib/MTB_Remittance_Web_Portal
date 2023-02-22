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
    public partial class BkashReversalFailed : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable dtBkashRevFailedListOutput = new DataTable();

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
                dtpickerFrom.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                dtpickerTo.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                lblReversalChangeStatus.Text = "";
            }
        }

        protected void btnSearchReversalFailTxn_Click(object sender, EventArgs e)
        {
            DateTime dateTime1 = DateTime.ParseExact(dtpickerFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dateTime2 = DateTime.ParseExact(dtpickerTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string dtValue1 = dateTime1.ToString("yyyy-MM-dd");
            string dtValue2 = dateTime2.ToString("yyyy-MM-dd");

            CreateOutputDataTable();

            DataTable dtExhList = mg.GetAPIActiveExchList();
            DataTable dtBkashRevFailedList = mg.GetBkashReversalFailedList(dtValue1, dtValue2);
            DataRow drowTotal;

            for (int ii = 0; ii < dtBkashRevFailedList.Rows.Count; ii++)
            {
                drowTotal = dtBkashRevFailedListOutput.NewRow();

                //SELECT AutoID, ID, PartyId, PIN, WalletNo, Amount, bKashResp, bKashRespDesc, RemitStatus, RequestTime, ProcessTime, Journal, CBSValueDate, CBSUniqueNumber

                //drowTotal[0] = dtBkashRevFailedList.Rows[ii]["PartyId"].ToString();
                drowTotal[0] = dtBkashRevFailedList.Rows[ii]["AutoID"].ToString();
                drowTotal[1] = dtBkashRevFailedList.Rows[ii]["ID"].ToString();
                drowTotal[2] = GetPartyName(dtBkashRevFailedList.Rows[ii]["PartyId"].ToString(), dtExhList);
                drowTotal[3] = dtBkashRevFailedList.Rows[ii]["PIN"].ToString();
                drowTotal[4] = dtBkashRevFailedList.Rows[ii]["WalletNo"].ToString();
                drowTotal[5] = dtBkashRevFailedList.Rows[ii]["Amount"].ToString();
                drowTotal[6] = dtBkashRevFailedList.Rows[ii]["bKashResp"].ToString();
                drowTotal[7] = dtBkashRevFailedList.Rows[ii]["bKashRespDesc"].ToString();
                drowTotal[8] = dtBkashRevFailedList.Rows[ii]["RemitStatus"].ToString();
                drowTotal[9] = dtBkashRevFailedList.Rows[ii]["RequestTime"].ToString();
                drowTotal[10] = dtBkashRevFailedList.Rows[ii]["ProcessTime"].ToString();
                drowTotal[11] = dtBkashRevFailedList.Rows[ii]["Journal"].ToString();
                drowTotal[12] = dtBkashRevFailedList.Rows[ii]["CBSValueDate"].ToString();
                drowTotal[13] = dtBkashRevFailedList.Rows[ii]["CBSUniqueNumber"].ToString();
                drowTotal[14] = dtBkashRevFailedList.Rows[ii]["isReversed"].ToString();

                dtBkashRevFailedListOutput.Rows.Add(drowTotal);
            }

            dataGridViewReversalFailTxn.DataSource = null;
            dataGridViewReversalFailTxn.DataSource = dtBkashRevFailedListOutput;
            dataGridViewReversalFailTxn.DataBind();

            lblRecordCount.Text = "Total Record(s): " + dtBkashRevFailedListOutput.Rows.Count;
        }

        private object GetPartyName(string PartyId, DataTable dtExhList)
        {
            for (int jj = 0; jj < dtExhList.Rows.Count; jj++)
            {
                if(dtExhList.Rows[jj]["PartyId"].ToString().Equals(PartyId))
                {
                    return dtExhList.Rows[jj]["ExShortName"].ToString();
                }
            }
            return "";
        }

        private void CreateOutputDataTable()
        {
            dtBkashRevFailedListOutput = new DataTable();

            dtBkashRevFailedListOutput.Columns.Add("AutoID");//0
            dtBkashRevFailedListOutput.Columns.Add("TRID");//1
            //dtBkashRevFailedListOutput.Columns.Add("PartyId");//0
            dtBkashRevFailedListOutput.Columns.Add("PartyName");//2
            dtBkashRevFailedListOutput.Columns.Add("PIN");//3
            dtBkashRevFailedListOutput.Columns.Add("WalletNo");//4
            dtBkashRevFailedListOutput.Columns.Add("Amount");//5
            dtBkashRevFailedListOutput.Columns.Add("bKashResp");//6
            dtBkashRevFailedListOutput.Columns.Add("bKashRespDesc");//7
            dtBkashRevFailedListOutput.Columns.Add("RemitStatus");//8
            dtBkashRevFailedListOutput.Columns.Add("RequestTime");//9
            dtBkashRevFailedListOutput.Columns.Add("ProcessTime");//10
            dtBkashRevFailedListOutput.Columns.Add("Journal");//11
            dtBkashRevFailedListOutput.Columns.Add("CBSValueDate");//12
            dtBkashRevFailedListOutput.Columns.Add("CBSUniqueNumber");//13
            dtBkashRevFailedListOutput.Columns.Add("isReversed");//14
        }

        protected void btnDownloadReversalFailDataAsExcel_Click(object sender, EventArgs e)
        {
            string fileName = "BkashRevFailedList__" + dtpickerFrom.Text + "_to_" + dtpickerTo.Text + ".xls";

            if (dtBkashRevFailedListOutput.Rows.Count > 0)
            {
                StringWriter tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = dtBkashRevFailedListOutput;
                dgGrid.DataBind();

                foreach (DataGridItem item in dgGrid.Items)
                {
                    for (int j = 0; j < item.Cells.Count; j++)
                    {
                        if (j == 4)
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

        protected void btnChangeStatusForReversal_Click(object sender, EventArgs e)
        {
            string pin = txtPinNoForReversal.Text.Trim();
            if (!String.IsNullOrEmpty(pin))
            {
                try
                {
                    bool status = mg.ChangeBkashIsReversedStatusForInitiateReversal(pin);
                    if (status)
                    {
                        lblReversalChangeStatus.Text = "Reversal Initiate Success";
                        lblReversalChangeStatus.ForeColor = Color.Green;
                        txtPinNoForReversal.Text = "";
                    }
                    else
                    {
                        lblReversalChangeStatus.Text = "Reversal Initiate FAILED, Try Again !!!";
                        lblReversalChangeStatus.ForeColor = Color.Red;
                    }
                }
                catch(Exception exc)
                {
                    lblReversalChangeStatus.Text = exc.Message;
                    lblReversalChangeStatus.ForeColor = Color.Red;
                }
            }
        }
    }
}