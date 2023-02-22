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
    public partial class AllExhouseSummary : System.Web.UI.Page
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
                dtpickerWeeklyFrom.Text = DateTime.Now.ToString("yyyy-MM-dd");
                dtpickerWeeklyTo.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        protected void btnSearchWeeklyTxn_Click(object sender, EventArgs e)
        {            
            DateTime dateTime1, dateTime2;
            string dt1, dt2;

            try
            {
                dateTime1 = DateTime.ParseExact(dtpickerWeeklyFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                dateTime2 = DateTime.ParseExact(dtpickerWeeklyTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                
                dt1 = dateTime1.ToString("yyyy-MM-dd");
                dt2 = dateTime2.ToString("yyyy-MM-dd");

                //dt1 += " 00:00:01";
                //dt2 += " 23:59:59";

                DataTable dtBEFTN = mg.GetBEFTNTxnAllExchSummary(dt1, dt2);
                DataTable dtOwnAcCredit = mg.GetMTBTxnAllExchSummary(dt1, dt2);
                DataTable dtCashCredit = mg.GetCashTxnAllExchSummary(dt1, dt2);
                DataTable dtbKashReg = mg.GetbKashRegTxnAllExchSummary(dt1, dt2);
                DataTable dtbKashServiceRem = mg.GetbKashServiceRemTxnAllExchSummary(dt1, dt2);
                DataTable dtbKashDirect = mg.GetbKashDirectTxnAllExchSummary(dt1, dt2);
                DataTable dtRippleTxn = mg.GetRippleTxnAllExchSummary(dt1, dt2);
                DataTable dtRippleReturnTxn = mg.GetRippleReturnTxnAllExchSummary(dt1, dt2);

                

                DataTable dtBkashRegular = new DataTable();
                dtBkashRegular.Columns.Add("PayMode");
                dtBkashRegular.Columns.Add("Count");
                dtBkashRegular.Columns.Add("Amount");

                DataTable dtBkashServiceRem = new DataTable();
                dtBkashServiceRem.Columns.Add("PayMode");
                dtBkashServiceRem.Columns.Add("Count");
                dtBkashServiceRem.Columns.Add("Amount");

                DataTable dtBkashDirect = new DataTable();
                dtBkashDirect.Columns.Add("PayMode");
                dtBkashDirect.Columns.Add("Count");
                dtBkashDirect.Columns.Add("Amount");

                DataRow drowRg = dtBkashRegular.NewRow();
                drowRg[0] = "bKash (Regular)";
                drowRg[1] = Convert.ToInt32(dtbKashReg.Rows[0][1]);
                drowRg[2] = decimal.Round(decimal.Parse(dtbKashReg.Rows[0][2].ToString()), 2);
                dtBkashRegular.Rows.Add(drowRg);

                DataRow drowSrv = dtBkashServiceRem.NewRow();
                drowSrv[0] = "bKash (Service Rem)";
                drowSrv[1] = Convert.ToInt32(dtbKashServiceRem.Rows[0][1]);
                drowSrv[2] = decimal.Round(decimal.Parse(dtbKashServiceRem.Rows[0][2].ToString()), 2);
                dtBkashServiceRem.Rows.Add(drowSrv);

                DataRow drowDir = dtBkashDirect.NewRow();
                drowDir[0] = "bKash (Direct)";
                drowDir[1] = Convert.ToInt32(dtbKashDirect.Rows[0][1]);
                drowDir[2] = decimal.Round(decimal.Parse(dtbKashDirect.Rows[0][2].ToString()), 2);
                dtBkashDirect.Rows.Add(drowDir);

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
                drowTotal[0] = dtBkashRegular.Rows[0][0].ToString();
                drowTotal[1] = dtBkashRegular.Rows[0][1].ToString();
                drowTotal[2] = dtBkashRegular.Rows[0][2].ToString();
                dtAllPayModeData.Rows.Add(drowTotal);

                drowTotal = dtAllPayModeData.NewRow();
                drowTotal[0] = dtBkashServiceRem.Rows[0][0].ToString();
                drowTotal[1] = dtBkashServiceRem.Rows[0][1].ToString();
                drowTotal[2] = dtBkashServiceRem.Rows[0][2].ToString();
                dtAllPayModeData.Rows.Add(drowTotal);

                drowTotal = dtAllPayModeData.NewRow();
                drowTotal[0] = dtBkashDirect.Rows[0][0].ToString();
                drowTotal[1] = dtBkashDirect.Rows[0][1].ToString();
                drowTotal[2] = dtBkashDirect.Rows[0][2].ToString();
                dtAllPayModeData.Rows.Add(drowTotal);


                drowTotal = dtAllPayModeData.NewRow();
                drowTotal[0] = dtCashCredit.Rows[0][0].ToString();
                drowTotal[1] = dtCashCredit.Rows[0][1].ToString();
                drowTotal[2] = dtCashCredit.Rows[0][2].ToString();
                dtAllPayModeData.Rows.Add(drowTotal);


                drowTotal = dtAllPayModeData.NewRow();
                drowTotal[0] = dtRippleTxn.Rows[0][0].ToString();
                drowTotal[1] = Convert.ToInt32(dtRippleTxn.Rows[0][1].ToString()) - Convert.ToInt32(dtRippleReturnTxn.Rows[0][1].ToString());
                drowTotal[2] = Convert.ToSingle(dtRippleTxn.Rows[0][2].ToString()) - Convert.ToSingle(dtRippleReturnTxn.Rows[0][2].ToString());
                dtAllPayModeData.Rows.Add(drowTotal);


                dataGridViewWeeklyTxn.DataSource = null;
                dataGridViewWeeklyTxn.DataSource = dtAllPayModeData;
                dataGridViewWeeklyTxn.DataBind();

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
            }
        }
    }
}