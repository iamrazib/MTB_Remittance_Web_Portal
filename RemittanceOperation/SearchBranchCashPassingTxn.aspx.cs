using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class SearchBranchCashPassingTxn : System.Web.UI.Page
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

            if(!IsPostBack)
            {
                txtJournalOrPin.Text = "";
                ClearLableValue();
            }
        }

        private void ClearLableValue()
        {
            lblSlNo.Text = "";
            lblProcessDate.Text = "";
            lblJournalNo.Text = "";
            lblPinNumber.Text = "";
            lblAmount.Text = "";
            lblExchName.Text = "";
            lblBenfName.Text = "";
            lblAuthBy.Text = "";
        }

        protected void btnSearchCashTxn_Click(object sender, EventArgs e)
        {
            string journalOrPin = txtJournalOrPin.Text.Trim();

            if (!journalOrPin.Equals(""))
            {
                DataTable dtSearch = mg.SearchTransactionByJournalOrPin(journalOrPin);
                if (dtSearch.Rows.Count > 0)
                {
                    lblSlNo.Text = Convert.ToString(dtSearch.Rows[0]["AutoId"]);
                    lblProcessDate.Text = Convert.ToString(dtSearch.Rows[0]["TxnProcessDate"]);
                    lblJournalNo.Text = Convert.ToString(dtSearch.Rows[0]["JournalNo"]);
                    lblPinNumber.Text = Convert.ToString(dtSearch.Rows[0]["PINNumber"]);
                    lblAmount.Text = Convert.ToString(dtSearch.Rows[0]["Amount"]);
                    lblExchName.Text = Convert.ToString(dtSearch.Rows[0]["ExHouse"]);
                    lblBenfName.Text = Convert.ToString(dtSearch.Rows[0]["Beneficiary"]);
                    lblAuthBy.Text = Convert.ToString(dtSearch.Rows[0]["PaymentUser"]);
                }
                else
                {
                    ClearLableValue();
                    lblMsg.Text = "No Data Found for this Journal/Pin Number";
                    lblMsg.ForeColor = Color.Red;
                }
            }
        }
    }
}