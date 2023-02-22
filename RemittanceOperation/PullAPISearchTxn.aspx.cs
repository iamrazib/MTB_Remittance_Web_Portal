using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class PullAPISearchTxn : System.Web.UI.Page
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
                LoadExchList();
            }
        }

        private void LoadExchList()
        {
            comboBoxAPIExh.Items.Clear();
            comboBoxAPIExh.Items.Add("--Select--");

            comboBoxAPIExh.Items.Add("NBL");
            comboBoxAPIExh.Items.Add("ICTC");
            comboBoxAPIExh.Items.Add("GCC");

            comboBoxAPIExh.SelectedIndex = 0;
        }

        protected void btnAPIDataSearch_Click(object sender, EventArgs e)
        {
            string refNo = textBoxRefNo.Text.Trim();
            string accNo = textBoxAccountNo.Text.Trim();
            string bdtAmt = textBoxBdtAmt.Text.Trim();
            string exh = comboBoxAPIExh.Text;

            DataTable aDTable = new DataTable();

            string whereClause = "";
            lblErrorMsg.Text = "";

            if (comboBoxAPIExh.SelectedIndex != 0)
            {
                if (exh.Equals("NBL"))
                {
                    whereClause = "";
                    if (whereClause.Equals("") && !refNo.Equals(""))
                    {
                        whereClause = " WHERE [REFERENCE]='" + refNo + "'";
                    }
                    if (!accNo.Equals(""))
                    {
                        if (!whereClause.Equals(""))
                        {
                            whereClause += " AND [ACCOUNT_NO] like '%" + accNo + "%'";
                        }
                        else
                        {
                            whereClause += " WHERE [ACCOUNT_NO] like '%" + accNo + "%'";
                        }
                    }
                    if (!bdtAmt.Equals(""))
                    {
                        if (!whereClause.Equals(""))
                        {
                            whereClause += " AND [AMOUNT]=" + bdtAmt + "'";
                        }
                        else
                        {
                            whereClause += " WHERE [AMOUNT]='" + bdtAmt + "'";
                        }
                    }

                    aDTable = mg.GetIndividualTxnByWhereClause(exh, whereClause);

                    if (aDTable.Rows.Count < 1)
                    {
                        lblErrorMsg.Text = "No Data Found";
                    }
                }
                else if (exh.Equals("ICTC"))
                {
                    whereClause = "";
                    if (whereClause.Equals("") && !refNo.Equals(""))
                    {
                        whereClause = " WHERE ICTC_NUMBER='" + refNo + "'";
                    }

                    if (!accNo.Equals(""))
                    {
                        if (!whereClause.Equals(""))
                        {
                            whereClause += " AND BANK_ACCOUNT_NUMBER like '%" + accNo + "%'";
                        }
                        else
                        {
                            whereClause += " WHERE BANK_ACCOUNT_NUMBER like '%" + accNo + "%'";
                        }
                    }

                    if (!bdtAmt.Equals(""))
                    {
                        if (!whereClause.Equals(""))
                        {
                            whereClause += " AND PAYING_AMOUNT=" + bdtAmt + "";
                        }
                        else
                        {
                            whereClause += " WHERE PAYING_AMOUNT=" + bdtAmt + "";
                        }
                    }

                    aDTable = mg.GetIndividualTxnByWhereClause(exh, whereClause);
                    if (aDTable.Rows.Count < 1)
                    {
                        lblErrorMsg.Text = "No Data Found";
                    }
                }
                else if (exh.Equals("GCC"))
                {
                    whereClause = "";
                    if (whereClause.Equals("") && !refNo.Equals(""))
                    {
                        whereClause = " WHERE [TransactionNo]='" + refNo + "'";
                    }

                    if (!accNo.Equals(""))
                    {
                        if (!whereClause.Equals(""))
                        {
                            whereClause += " AND [BankAccountNo] like '%" + accNo + "%'";
                        }
                        else
                        {
                            whereClause += " WHERE [BankAccountNo] like '%" + accNo + "%'";
                        }
                    }

                    if (!bdtAmt.Equals(""))
                    {
                        if (!whereClause.Equals(""))
                        {
                            whereClause += " AND [AmountToPay]=" + bdtAmt + "";
                        }
                        else
                        {
                            whereClause += " WHERE [AmountToPay]=" + bdtAmt + "";
                        }
                    }

                    aDTable = mg.GetIndividualTxnByWhereClause(exh, whereClause);
                    if (aDTable.Rows.Count < 1)
                    {
                        lblErrorMsg.Text = "No Data Found";
                    }
                }

                dataGridViewTxnSearch.DataSource = null;
                dataGridViewTxnSearch.DataSource = aDTable;
                dataGridViewTxnSearch.DataBind();

                lblRecordCount.Text = "Total Rows=" + aDTable.Rows.Count;
            }
            else
            {
                lblErrorMsg.Text = "Please Select Exchange House !!!";
            }
        }
    }
}