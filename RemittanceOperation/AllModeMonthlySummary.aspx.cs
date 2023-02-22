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
    public partial class AllModeMonthlySummary : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable dtAllPayModeAllExhouseData = new DataTable();
        static DataTable dtAllPayModeAllExhouseCommissionData = new DataTable();



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
                DateTime thisMonthFirstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                dtpickerMonthlyFrom.Text = thisMonthFirstDay.ToString("yyyy-MM-dd");
                dtpickerMonthlyTo.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

                //dtpickerMonthlyCommFrom.Text = thisMonthFirstDay.ToString("yyyy-MM-dd");
                //dtpickerMonthlyCommTo.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

            }
        }

        protected void btnSearchMonthlyTxn_Click(object sender, EventArgs e)
        {
            DateTime dateTime1 = DateTime.ParseExact(dtpickerMonthlyFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dateTime2 = DateTime.ParseExact(dtpickerMonthlyTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string dtValue1 = dateTime1.ToString("yyyy-MM-dd");
            string dtValue2 = dateTime2.ToString("yyyy-MM-dd");
            

            dtAllPayModeAllExhouseData = new DataTable();

            //DataTable dtAllPayModeAllExhouseData = new DataTable();
            dtAllPayModeAllExhouseData.Columns.Add("PartyId");
            dtAllPayModeAllExhouseData.Columns.Add("ExHouseName");
            dtAllPayModeAllExhouseData.Columns.Add("BEFTNCount");
            dtAllPayModeAllExhouseData.Columns.Add("BEFTNAmount");
            dtAllPayModeAllExhouseData.Columns.Add("EFTReturnCount");
            dtAllPayModeAllExhouseData.Columns.Add("EFTReturnAmount");
            dtAllPayModeAllExhouseData.Columns.Add("MTBCount");
            dtAllPayModeAllExhouseData.Columns.Add("MTBAmount");
            dtAllPayModeAllExhouseData.Columns.Add("CashCount");
            dtAllPayModeAllExhouseData.Columns.Add("CashAmount");
            dtAllPayModeAllExhouseData.Columns.Add("bKashCount");
            dtAllPayModeAllExhouseData.Columns.Add("bKashAmount");

            DataRow drowTotal;
            int exhId;
            string exhName;

            DataTable dtExhList = mg.GetAPIActiveExchList();

            for (int ii = 0; ii < dtExhList.Rows.Count; ii++)
            {
                exhId = Convert.ToInt32(dtExhList.Rows[ii][0]);
                exhName = dtExhList.Rows[ii][1].ToString();

                DataTable dtBEFTN = mg.GetBEFTNTxnSummaryByExchId(dtValue1, dtValue2, exhId);
                DataTable dtBEFTNReturn = mg.GetBEFTNReturnTxnSummaryByExchId(dtValue1, dtValue2, exhId);
                DataTable dtOwnAcCredit = mg.GetMTBTxnSummaryByExchId(dtValue1, dtValue2, exhId);
                DataTable dtCashCredit = mg.GetCashTxnSummaryByExchId(dtValue1, dtValue2, exhId);
                DataTable dtbKashReg = mg.GetbKashRegTxnSummaryByExchId(dtValue1, dtValue2, exhId);


                if (exhName.Contains("Direct"))
                {
                    DataTable dtbKashDir = mg.GetbKashDirectTxnSummaryByExchId(dtValue1, dtValue2, exhId);
                    if (dtbKashDir.Rows.Count > 0)
                    {
                        dtbKashReg.Rows[0][1] = Convert.ToInt32(dtbKashReg.Rows[0][1]) + Convert.ToInt32(dtbKashDir.Rows[0][1]);
                        dtbKashReg.Rows[0][2] = Convert.ToDecimal(dtbKashReg.Rows[0][2]) + Convert.ToDecimal(dtbKashDir.Rows[0][2]);
                    }
                }

                //------------------ripple ----------
                if (exhName.ToUpper().Contains("RIPPLE"))
                {
                    string validator = mg.GetRippleValidatorByPartyId(exhId);
                    DataTable dtbKashRipple = mg.GetRippleBkashTxnSummaryByExchId(dtValue1, dtValue2, validator);
                    if (dtbKashRipple.Rows.Count > 0)
                    {
                        dtbKashReg.Rows[0][1] = Convert.ToInt32(dtbKashReg.Rows[0][1]) + Convert.ToInt32(dtbKashRipple.Rows[0][0]);
                        dtbKashReg.Rows[0][2] = Convert.ToDecimal(dtbKashReg.Rows[0][2]) + Convert.ToDecimal(dtbKashRipple.Rows[0][1]);
                    }
                }
                //-----------------------------------


                drowTotal = dtAllPayModeAllExhouseData.NewRow();
                drowTotal[0] = dtExhList.Rows[ii][0].ToString();
                drowTotal[1] = dtExhList.Rows[ii][1].ToString();
                drowTotal[2] = dtBEFTN.Rows[0][1].ToString();
                drowTotal[3] = dtBEFTN.Rows[0][2].ToString();
                drowTotal[4] = dtBEFTNReturn.Rows[0][1].ToString();
                drowTotal[5] = dtBEFTNReturn.Rows[0][2].ToString();

                drowTotal[6] = dtOwnAcCredit.Rows[0][1].ToString();
                drowTotal[7] = dtOwnAcCredit.Rows[0][2].ToString();
                drowTotal[8] = dtCashCredit.Rows[0][1].ToString();
                drowTotal[9] = dtCashCredit.Rows[0][2].ToString();
                drowTotal[10] = dtbKashReg.Rows[0][1].ToString();
                drowTotal[11] = dtbKashReg.Rows[0][2].ToString();

                dtAllPayModeAllExhouseData.Rows.Add(drowTotal);

            }//for end

            //calculate ripple txn
            //calc ripple wallet

            /*
            DataTable dtbKashRipple = mg.GetRippleWalletTxnSummaryByDate(dtValue1, dtValue2);
            DataTable dtbKashRippleReturn = mg.GetRippleWalletReturnTxnSummaryByDate(dtValue1, dtValue2);
            dtbKashRippleReturn = FormatTableEntries(dtbKashRippleReturn);

            for (int ii = 0; ii < dtbKashRipple.Rows.Count; ii++)
            {
                drowTotal = dtAllPayModeAllExhouseData.NewRow();
                drowTotal[0] = FetchExhNameFromRippleValidator(dtbKashRipple.Rows[ii][0].ToString());

                drowTotal[1] = "0";
                drowTotal[2] = "0";
                drowTotal[3] = "0";
                drowTotal[4] = "0";
                drowTotal[5] = "0";
                drowTotal[6] = "0";
                drowTotal[7] = "0";
                drowTotal[8] = "0";
                if (dtbKashRippleReturn.Rows.Count > 0)
                {
                    drowTotal[9] = Convert.ToInt32(dtbKashRipple.Rows[ii][1].ToString()) - Convert.ToInt32(dtbKashRippleReturn.Rows[ii][1]);
                    drowTotal[10] = Convert.ToDecimal(dtbKashRipple.Rows[ii][2].ToString()) - Convert.ToDecimal(dtbKashRippleReturn.Rows[ii][2]);
                }
                else
                {
                    drowTotal[9] = Convert.ToInt32(dtbKashRipple.Rows[ii][1].ToString());
                    drowTotal[10] = Convert.ToDecimal(dtbKashRipple.Rows[ii][2].ToString());
                }
                dtAllPayModeAllExhouseData.Rows.Add(drowTotal);
            }
            */

            dataGridViewMonthlyTxn.DataSource = null;
            dataGridViewMonthlyTxn.DataSource = dtAllPayModeAllExhouseData;
            dataGridViewMonthlyTxn.DataBind();

            //if (dtAllPayModeAllExhouseData.Rows.Count > 0)
            //{
            //    btnDownloadMonthlyAPIDataAsExcel.Visible = true;
            //}
            //else
            //{
            //    btnDownloadMonthlyAPIDataAsExcel.Visible = false;
            //}

        }

        private object FetchExhNameFromRippleValidator(string str)
        {
            int idx = GetNthIndex(str, '.', 2);
            return str.Substring(idx + 1);
        }

        private DataTable FormatTableEntries(DataTable dtbKashRippleReturn)
        {
            string str;
            int idx = 0;

            for (int bb = 0; bb < dtbKashRippleReturn.Rows.Count; bb++)
            {
                str = dtbKashRippleReturn.Rows[bb][0].ToString();
                idx = GetNthIndex(str, '@', 1);
                dtbKashRippleReturn.Rows[bb][0] = str.Substring(idx + 1);
            }
            return dtbKashRippleReturn;
        }

        private int GetNthIndex(string str, char p1, int ntimes)
        {
            int count = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == p1)
                {
                    count++;
                    if (count == ntimes)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        protected void btnDownloadMonthlyAPIDataAsExcel_Click(object sender, EventArgs e)
        {
            string fileName = "MonthlySummaryAPIData__" + dtpickerMonthlyFrom.Text + "_to_" + dtpickerMonthlyTo.Text + ".xls"; 

            if (dtAllPayModeAllExhouseData.Rows.Count > 0)
            {
                StringWriter tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = dtAllPayModeAllExhouseData;
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