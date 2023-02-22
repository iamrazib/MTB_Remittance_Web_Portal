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
    public partial class RemittanceIncentiveReport : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable IncentiveReportTxnData = new DataTable();

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

                LoadParty();
                LoadStatus();
                LoadTranType();
                lblTotalRecords.Text = "";
            }
        }

        private void LoadTranType()
        {
            ddlTrnType.Items.Clear();
            ddlTrnType.Items.Add("103-BEFTN");
            ddlTrnType.Items.Add("101-MTB A/C");
            ddlTrnType.SelectedIndex = 0;
        }

        private void LoadStatus()
        {
            ddlStatus.Items.Clear();
            ddlStatus.Items.Add("1-Success");
            ddlStatus.Items.Add("0-Failed");
            ddlStatus.SelectedIndex = 0;
        }

        private void LoadParty()
        {
            DataTable dtExchs = mg.GetAPIActiveExchList();
            ddlParty.Items.Clear();
            ddlParty.Items.Add("ALL");
            //ddlParty.Items.Add("0-ALL");

            ddlPartyId.Items.Clear();
            ddlPartyId.Items.Add("0");

            for (int rows = 0; rows < dtExchs.Rows.Count; rows++)
            {
                if (!dtExchs.Rows[rows]["PartyId"].ToString().Equals("0"))
                {
                    //ddlParty.Items.Add(dtExchs.Rows[rows]["PartyId"] + "-" + dtExchs.Rows[rows]["ExShortName"]);
                    ddlParty.Items.Add(dtExchs.Rows[rows]["ExShortName"]+"");
                    ddlPartyId.Items.Add(dtExchs.Rows[rows]["PartyId"] + "");
                }
            }
            ddlParty.SelectedIndex = 0;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {

            DateTime dateTime1 = DateTime.ParseExact(dtpickerFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dateTime2 = DateTime.ParseExact(dtpickerTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string dtValue1 = dateTime1.ToString("yyyy-MM-dd");
            string dtValue2 = dateTime2.ToString("yyyy-MM-dd");

            int partyId = Convert.ToInt32(ddlPartyId.Text);
            int tranStatus = Convert.ToInt32(ddlStatus.Text.Split('-')[0]);
            int tranType = Convert.ToInt32(ddlTrnType.Text.Split('-')[0]);

            DataTable dtInctRptData = new DataTable();
            //DataTable dtInctBEFTRpt = new DataTable();

            if (tranType == 101)
            {
                dtInctRptData = mg.GetIncentiveFTReportData(dtValue1, dtValue2, partyId, tranStatus, tranType);
            }
            else
            {
                dtInctRptData = mg.GetIncentiveEFTReportData(dtValue1, dtValue2, partyId, tranStatus, tranType);
            }

            IncentiveReportTxnData = dtInctRptData;
            dataGridViewIncentiveRptData.DataSource = null;
            dataGridViewIncentiveRptData.DataSource = dtInctRptData;
            dataGridViewIncentiveRptData.DataBind();

            lblTotalRecords.Text = "Total Records: " + dtInctRptData.Rows.Count;
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {

            DateTime dateTime1 = DateTime.ParseExact(dtpickerFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dateTime2 = DateTime.ParseExact(dtpickerTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            string dtValue1 = dateTime1.ToString("ddMMyyyy");
            string dtValue2 = dateTime2.ToString("ddMMyyyy");

            string tranStatus = ddlStatus.Text.Split('-')[1];
            int tranType = Convert.ToInt32(ddlTrnType.Text.Split('-')[0]);
            string trnType = tranType == 103 ? "BEFTN" : "MTB_Account";
            
            string fileName = "Remittance_Incentive_" + trnType + "_" + tranStatus + "_Report_" + dtValue1 + "_to_" + dtValue2 + ".xls";

            if (IncentiveReportTxnData.Rows.Count > 0)
            {
                StringWriter tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = IncentiveReportTxnData;
                dgGrid.DataBind();

                foreach (DataGridItem item in dgGrid.Items)
                {
                    for (int j = 0; j < item.Cells.Count; j++)
                    {
                        if (j == 11 || j == 12)
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

        protected void ddlParty_SelectedIndexChanged(object sender, EventArgs e)
        {
            int prtyIndx = ddlParty.SelectedIndex;
            ddlPartyId.SelectedIndex = prtyIndx;
        }

        protected void dataGridViewIncentiveRptData_PageIndexChanged(object sender, EventArgs e)
        {
            dataGridViewIncentiveRptData.DataSource = IncentiveReportTxnData;
            dataGridViewIncentiveRptData.DataBind();
        }

        protected void dataGridViewIncentiveRptData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex == -1)
                dataGridViewIncentiveRptData.PageIndex = Int32.MaxValue;
            else
                dataGridViewIncentiveRptData.PageIndex = e.NewPageIndex;
        }
    }
}