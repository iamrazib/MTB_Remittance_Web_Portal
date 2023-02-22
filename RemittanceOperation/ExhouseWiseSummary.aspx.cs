using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class ExhouseWiseSummary : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        
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
                dTPickerFromSumrExchWise.Text = DateTime.Now.ToString("yyyy-MM-dd");
                dTPickerToSumrExchWise.Text = DateTime.Now.ToString("yyyy-MM-dd");

                LoadAPIBasedExchListSummary();
            }

        }

        private void LoadAPIBasedExchListSummary()
        {
            DataTable dtExchs = mg.GetAPIBasedExchListSummary();
            cbExchWiseSumr.Items.Clear();
            cbExchWiseSumr.Items.Add("--Select--");

            for (int rows = 0; rows < dtExchs.Rows.Count; rows++)
            {
                cbExchWiseSumr.Items.Add(dtExchs.Rows[rows][0]+"");
            }
            cbExchWiseSumr.SelectedIndex = 0;
        }

        protected void btnExchWiseSumrSearch_Click(object sender, EventArgs e)
        {
            if (cbExchWiseSumr.SelectedIndex != 0)
            {
                DateTime dateTime1 = DateTime.ParseExact(dTPickerFromSumrExchWise.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime dateTime2 = DateTime.ParseExact(dTPickerToSumrExchWise.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                string dtValue1 = dateTime1.ToString("yyyy-MM-dd");
                string dtValue2 = dateTime2.ToString("yyyy-MM-dd");

                int exhId = Convert.ToInt32(cbExchWiseSumr.Text.Split('-')[0]);
                string exhName = cbExchWiseSumr.Text.Split('-')[1].Trim();

                DataTable dtBEFTN = mg.GetBEFTNTxnSummaryByExchId(dtValue1, dtValue2, exhId);
                DataTable dtOwnAcCredit = mg.GetMTBTxnSummaryByExchId(dtValue1, dtValue2, exhId);
                DataTable dtCashCredit = mg.GetCashTxnSummaryByExchId(dtValue1, dtValue2, exhId);
                DataTable dtbKashReg = mg.GetbKashRegTxnSummaryByExchId(dtValue1, dtValue2, exhId);


                DataTable dtBkashAll = new DataTable();
                dtBkashAll.Columns.Add("PayMode");
                dtBkashAll.Columns.Add("Count");
                dtBkashAll.Columns.Add("Amount");

                DataRow drow = dtBkashAll.NewRow();
                drow[0] = "bKash (Regular + Direct)";
                drow[1] = Convert.ToInt32(dtbKashReg.Rows[0][1]);
                drow[2] = decimal.Round(decimal.Parse(dtbKashReg.Rows[0][2].ToString()), 2);
                dtBkashAll.Rows.Add(drow);

                if (exhName.Contains("Direct"))
                {
                    DataTable dtbKashDir = mg.GetbKashDirectTxnSummaryByExchId(dtValue1, dtValue2, exhId);
                    if (dtbKashDir.Rows.Count > 0)
                    {
                        dtBkashAll.Rows[0][1] = Convert.ToInt32(dtBkashAll.Rows[0][1]) + Convert.ToInt32(dtbKashDir.Rows[0][1]);
                        dtBkashAll.Rows[0][2] = Convert.ToDecimal(dtBkashAll.Rows[0][2]) + Convert.ToDecimal(dtbKashDir.Rows[0][2]);
                    }
                }

                DataTable dtAllPayModeData = new DataTable();
                dtAllPayModeData.Columns.Add("PayMode");
                dtAllPayModeData.Columns.Add("Count");
                dtAllPayModeData.Columns.Add("Amount");

                DataRow drowTotal = dtAllPayModeData.NewRow();

                drowTotal[0] = dtBEFTN.Rows[0][0].ToString();
                drowTotal[1] = dtBEFTN.Rows[0][1].ToString();
                drowTotal[2] = dtBEFTN.Rows[0][2].ToString();
                dtAllPayModeData.Rows.Add(drowTotal);

                drowTotal = dtAllPayModeData.NewRow();
                drowTotal[0] = dtOwnAcCredit.Rows[0][0].ToString();
                drowTotal[1] = dtOwnAcCredit.Rows[0][1].ToString();
                drowTotal[2] = dtOwnAcCredit.Rows[0][2].ToString();
                dtAllPayModeData.Rows.Add(drowTotal);

                drowTotal = dtAllPayModeData.NewRow();
                drowTotal[0] = dtCashCredit.Rows[0][0].ToString();
                drowTotal[1] = dtCashCredit.Rows[0][1].ToString();
                drowTotal[2] = dtCashCredit.Rows[0][2].ToString();
                dtAllPayModeData.Rows.Add(drowTotal);

                drowTotal = dtAllPayModeData.NewRow();
                drowTotal[0] = dtBkashAll.Rows[0][0].ToString();
                drowTotal[1] = dtBkashAll.Rows[0][1].ToString();
                drowTotal[2] = dtBkashAll.Rows[0][2].ToString();
                dtAllPayModeData.Rows.Add(drowTotal);

                dataGridViewSumrExchWise.DataSource = null;
                dataGridViewSumrExchWise.DataSource = dtAllPayModeData;
                dataGridViewSumrExchWise.DataBind();

            }
        }
    }
}