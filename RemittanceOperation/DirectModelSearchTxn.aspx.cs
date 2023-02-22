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
    public partial class DirectModelSearchTxn : System.Web.UI.Page
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
                dateTimePickerDirect.Text = DateTime.Now.ToString("yyyy-MM-dd");
                LoadDirectMTOName();
                lblConvertPendingStatToRecvdStatMsg.Text = "";
            }
        }

        private void LoadDirectMTOName()
        {
            DataTable dtExchs = mg.GetDirectMTOName();
            comboBoxDirectMTO.Items.Clear();
            comboBoxDirectMTO.Items.Add("--ALL--");
            for (int rows = 0; rows < dtExchs.Rows.Count; rows++)
            {
                comboBoxDirectMTO.Items.Add(dtExchs.Rows[rows][0].ToString());
            }
            comboBoxDirectMTO.SelectedIndex = 0;
        }

        protected void btnSearchTodayDirectTxn_Click(object sender, EventArgs e)
        {
            DateTime dateTime1 = DateTime.ParseExact(dateTimePickerDirect.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            string dtDirect = dateTime1.ToString("yyyy-MM-dd");
            string dtDirectProcsTime = dateTime1.ToString("yyyyMMdd");

            int prtyId = 0;
            string pendingSumOfAmount = "";

            if (comboBoxDirectMTO.SelectedIndex != 0)
            {
                prtyId = Convert.ToInt32(comboBoxDirectMTO.Text.Split('-')[0]);
            }

            DataTable dtDirectTodayAll = mg.GetDirectModeTxnDetailByPartyIdAndDate(dtDirectProcsTime, prtyId);
            dataGridViewDirectAll.DataSource = null;
            dataGridViewDirectAll.DataSource = dtDirectTodayAll;
            dataGridViewDirectAll.DataBind();

            lblDirectTodayAllCount.Text = "Count: " + dtDirectTodayAll.Rows.Count.ToString();

            DataTable dtPendingDirectTodayAML = mg.GetDirectModeTxnPendingByPartyIdAndDate(dtDirectProcsTime, prtyId, ref pendingSumOfAmount);
            dataGridViewDirectAMLPending.DataSource = null;
            dataGridViewDirectAMLPending.DataSource = dtPendingDirectTodayAML;
            dataGridViewDirectAMLPending.DataBind();

            lblDirectAMLCount.Text = "Count: " + dtPendingDirectTodayAML.Rows.Count.ToString();
            lblDirectAMLAmount.Text = "Amount = " + pendingSumOfAmount;

            DataTable dtDirectSummary = mg.GetDirectModeSummaryTxn(dtDirectProcsTime);
            dataGridViewDirectSummary.DataSource = null;
            dataGridViewDirectSummary.DataSource = dtDirectSummary;
            dataGridViewDirectSummary.DataBind();

        }

        protected void btnPendingAML_Click(object sender, EventArgs e)
        {
            string sumOfPendingTxn = "";
            DataTable dtDirectPendingAll = mg.GetDirectModeAllPendingTxn(ref sumOfPendingTxn);
            dataGridViewDirectAMLPending.DataSource = null;
            dataGridViewDirectAMLPending.DataSource = dtDirectPendingAll;
            dataGridViewDirectAMLPending.DataBind();
                        
            lblDirectAMLCount.Text = "Count: " + dtDirectPendingAll.Rows.Count.ToString();
            lblDirectAMLAmount.Text = "Amount = " + sumOfPendingTxn;

            lblConvertPendingStatToRecvdStatMsg.Text = "";
        }

        protected void btnConvertPendingStatToRecvdStat_Click(object sender, EventArgs e)
        {
            bool stats = mg.ConvertDirectModePendingStatusToRecvdStatus();
            lblConvertPendingStatToRecvdStatMsg.Text = "Status Update Successfully ...";
            btnPendingAML_Click(sender, e);
        }

    }
}