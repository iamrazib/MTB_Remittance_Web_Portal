using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class DuplicateCashTxnCheck : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        string userRmCode = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
                userRmCode = Session[CSessionName.S_CURRENT_USER_RM].ToString();
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                LoadExhouseDropDownList();

                dtPickerCashCheckFrom.Text = DateTime.Now.ToString("yyyy-MM-dd");
                dtPickerCashCheckTo.Text = DateTime.Now.ToString("yyyy-MM-dd");

                LoadTodaysSavedTransaction(dtPickerCashCheckFrom.Text, dtPickerCashCheckTo.Text);
                LoadSummaryList(dtPickerCashCheckFrom.Text, dtPickerCashCheckTo.Text);
            }
        }

        private void LoadTodaysSavedTransaction(string fromDate, string toDate)
        {
            DataTable dtSavedData = mg.GetSavedCashPassingDataByDates(fromDate, toDate);

            dataGridViewCashSavedTxn.DataSource = null;
            dataGridViewCashSavedTxn.DataSource = dtSavedData;
            dataGridViewCashSavedTxn.DataBind();

            lblRowCount.Text = "Total Rows = " + dtSavedData.Rows.Count;
        }

        private void LoadExhouseDropDownList()
        {
            DataTable Exchlist = mg.GetExchList();
            cbExchDupChk.Items.Clear();
            cbExchDupChk.Items.Add("----- Select Exchange House -----");

            for (int rw = 0; rw < Exchlist.Rows.Count; rw++)
            {
                cbExchDupChk.Items.Add(Exchlist.Rows[rw][1].ToString() + " - " + Exchlist.Rows[rw][0].ToString());
            }
            cbExchDupChk.SelectedIndex = 0;
        }

        protected void btnCheckAndSave_Click(object sender, EventArgs e)
        {
            string pinnumber = txtCashCheckPinNumber.Text.Trim();
            string journal = txtCashCheckJournal.Text.Trim();
            string amount = txtCashCheckAmount.Text.Trim();
            amount = amount.Replace(",", "");

            //bool journalEmptyIsOk = true;

            if(pinnumber.Equals(""))
            {
                lblPINStatus.Text = "PIN Number Cannot Empty !!!";
                lblPINStatus.ForeColor = Color.Red;
            }
            else if (journal.Equals(""))
            {
                lblPINStatus.Text = "Journal Number Cannot Empty !!!";
                lblPINStatus.ForeColor = Color.Red;
            }
            else
            {
                lblPINStatus.Text = "";
                pinnumber = RemoveNonAlphaCharacter(pinnumber);
                DataTable dtThisPinInfo = mg.CashTxnThisPinInfoInDatabase(pinnumber);

                if (dtThisPinInfo.Rows.Count > 0)
                {
                    lblPINStatus.Text = "Duplicate PIN Found !!!";
                    lblPINStatus.ForeColor = Color.Red;

                    string inpdt = Convert.ToString(dtThisPinInfo.Rows[0]["InputDate"]);
                    string jrnl = Convert.ToString(dtThisPinInfo.Rows[0]["JournalNo"]);
                    string pin = Convert.ToString(dtThisPinInfo.Rows[0]["PINNumber"]).Trim();
                    string amt = Convert.ToString(dtThisPinInfo.Rows[0]["Amount"]);
                    string exh = Convert.ToString(dtThisPinInfo.Rows[0]["exhName"]);
                    string usr = Convert.ToString(dtThisPinInfo.Rows[0]["UserName"]);

                    lblDuplicatePinInfo.Text = "Date: " + inpdt + ", Journal:" + jrnl + ", PIN: " + pin + ", Amount:" + amt + ", ExhName: " + exh + ", User: " + usr;
                }
                else
                {
                    lblDuplicatePinInfo.Text = "";
                    lblPINStatus.Text = "";

                    string exch = cbExchDupChk.SelectedItem.ToString();
                    string exchId = exch.Split('-')[1].Trim();
                    string benfName = txtCashCheckBeneficiary.Text.Trim();

                    bool status = mg.SaveCashTxnData(pinnumber, journal, amount, exchId, benfName, userRmCode);
                    if (status)
                    {
                        lblPINStatus.Text = "Unique PIN Saved Successfully ...";
                        lblPINStatus.ForeColor = Color.Green;

                        ClearFields();

                        string fromDate = dtPickerCashCheckFrom.Text;
                        string toDate = dtPickerCashCheckTo.Text;
                        LoadTodaysSavedTransaction(fromDate, toDate);
                        LoadSummaryList(fromDate, toDate);
                    }
                    else
                    {
                        lblPINStatus.Text = "Saving ERROR !!!";
                        lblPINStatus.ForeColor = Color.Red;
                    }
                }
            }
        }

        private void ClearFields()
        {
            txtCashCheckPinNumber.Text = "";
            txtCashCheckJournal.Text = "";
            txtCashCheckBeneficiary.Text = "";
            txtCashCheckAmount.Text = "";
            LoadExhouseDropDownList();

            txtCashCheckPinNumber.Focus();
        }

        private string RemoveNonAlphaCharacter(string pinnumber)
        {
            return Regex.Replace(pinnumber, "[^a-zA-Z0-9]", String.Empty).ToUpper();
        }

        protected void btnSearchSavedCash_Click(object sender, EventArgs e)
        {
            string fromDate = dtPickerCashCheckFrom.Text;
            string toDate = dtPickerCashCheckTo.Text;
            LoadTodaysSavedTransaction(fromDate, toDate);
            LoadSummaryList(fromDate, toDate);
        }

        private void LoadSummaryList(string fromDate, string toDate)
        {
            DataTable dtSummaryList = mg.GetUserWisePerformedTxnSummaryByDates(fromDate, toDate);
            GridViewCashPassingSummary.DataSource = null;
            GridViewCashPassingSummary.DataSource = dtSummaryList;
            GridViewCashPassingSummary.DataBind();
        }
    }
}