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
    public partial class TxnSearch : System.Web.UI.Page
    {
        static Manager mg = new Manager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {                
                // float height = SystemInformation.VirtualScreen.Height;
                //  float width = SystemInformation.VirtualScreen.Width;
                
                //string height = HttpContext.Current.Request.Params["clientScreenHeight"];
                //string width = HttpContext.Current.Request.Params["clientScreenWidth"];                
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

            if(!IsPostBack)
            {
                lblPaymentModeNameTxnCheckPage.Text = "";
            }
        }

        protected void btnSearchTxnCheck_Click(object sender, EventArgs e)
        {
            if(!txtBoxPinTxnCheck.Text.Trim().Equals(""))
            {
                //Cursor.Current = Cursors.WaitCursor;

                bool foundInNewDb = false;

                DataTable aDataTable = mg.GetBEFTNDataFromNewSystem(txtBoxPinTxnCheck.Text.Trim());                               
                if (aDataTable.Rows.Count > 0)
                {
                    lblPaymentModeNameTxnCheckPage.Text = "BEFTN";
                    foundInNewDb = true;
                }
                else
                {
                    lblPaymentModeNameTxnCheckPage.Text = "";
                    foundInNewDb = false;
                }

                dGridViewTxnCheckOutputNewSys.DataSource = null;
                dGridViewTxnCheckOutputNewSys.DataSource = aDataTable;
                dGridViewTxnCheckOutputNewSys.DataBind();

                //// checking MTB txn
                if (!foundInNewDb)
                {
                    DataTable aDataTableMtb = mg.GetMTBDataFromNewSystem(txtBoxPinTxnCheck.Text.Trim());
                    
                    if (aDataTableMtb.Rows.Count > 0)
                    {
                        lblPaymentModeNameTxnCheckPage.Text = "MTB A/C :";
                        foundInNewDb = true;
                    }
                    else
                    {
                        lblPaymentModeNameTxnCheckPage.Text = "";
                        foundInNewDb = false;
                    }

                    dGridViewTxnCheckOutputNewSys.DataSource = null;
                    dGridViewTxnCheckOutputNewSys.DataSource = aDataTableMtb;
                    dGridViewTxnCheckOutputNewSys.DataBind();
                }

                // checking bKash Regular
                if (!foundInNewDb)
                {
                    DataTable aDTBkashReg = mg.GetBkashRegularDataFromNewSystem(txtBoxPinTxnCheck.Text.Trim());
                    
                    if (aDTBkashReg.Rows.Count > 0)
                    {
                        lblPaymentModeNameTxnCheckPage.Text = "bKash Regular :";
                        foundInNewDb = true;
                    }
                    else
                    {
                        lblPaymentModeNameTxnCheckPage.Text = "";
                        foundInNewDb = false;
                    }

                    dGridViewTxnCheckOutputNewSys.DataSource = null;
                    dGridViewTxnCheckOutputNewSys.DataSource = aDTBkashReg;
                    dGridViewTxnCheckOutputNewSys.DataBind();
                }

                // checking bKash Direct
                if (!foundInNewDb)
                {
                    DataTable aDTBkashDir = mg.GetBkashDirectDataFromNewSystem(txtBoxPinTxnCheck.Text.Trim());
                    
                    if (aDTBkashDir.Rows.Count > 0)
                    {
                        lblPaymentModeNameTxnCheckPage.Text = "bKash Direct :";
                        foundInNewDb = true;
                    }
                    else
                    {
                        lblPaymentModeNameTxnCheckPage.Text = "";
                        foundInNewDb = false;
                    }

                    dGridViewTxnCheckOutputNewSys.DataSource = null;
                    dGridViewTxnCheckOutputNewSys.DataSource = aDTBkashDir;
                    dGridViewTxnCheckOutputNewSys.DataBind();
                }


                // checking Cash Txn
                if (!foundInNewDb)
                {
                    DataTable aDataTableCash = mg.GetCashTxnDataFromNewSystem(txtBoxPinTxnCheck.Text.Trim());
                    
                    if (aDataTableCash.Rows.Count > 0)
                    {
                        lblPaymentModeNameTxnCheckPage.Text = "Cash Txn :";
                        foundInNewDb = true;
                    }
                    else
                    {
                        lblPaymentModeNameTxnCheckPage.Text = "";
                        foundInNewDb = false;
                    }

                    dGridViewTxnCheckOutputNewSys.DataSource = null;
                    dGridViewTxnCheckOutputNewSys.DataSource = aDataTableCash;
                    dGridViewTxnCheckOutputNewSys.DataBind();
                }

                Label1.Text = "";

                // checking Ripple Txn
                DataTable aDataTableRipple = mg.GetRippleTxnDataFromNewSystem(txtBoxPinTxnCheck.Text.Trim());
                dGridViewTxnCheckRippleTable.DataSource = null;
                dGridViewTxnCheckRippleTable.DataSource = aDataTableRipple;
                dGridViewTxnCheckRippleTable.DataBind();

                if (aDataTableRipple.Rows.Count > 0)
                {
                    Label1.Text = "Ripple Data In Table :";
                }

                //========================================== PULL API TXN CHECK ===================================================
                string whereClause = " WHERE [REFERENCE]='" + txtBoxPinTxnCheck.Text.Trim() + "'";
                DataTable aDataTableNBL = mg.GetIndividualTxnByWhereClause("NBL", whereClause);
                if (aDataTableNBL.Rows.Count > 0)
                {
                    Label1.Text = "NBL Data: ";
                    dGridViewTxnCheckRippleTable.DataSource = null;
                    dGridViewTxnCheckRippleTable.DataSource = aDataTableNBL;
                    dGridViewTxnCheckRippleTable.DataBind();
                }

                whereClause = " WHERE [ICTC_NUMBER]='" + txtBoxPinTxnCheck.Text.Trim() + "'";
                DataTable aDataTableICTC = mg.GetIndividualTxnByWhereClause("ICTC", whereClause);
                if (aDataTableICTC.Rows.Count > 0)
                {
                    Label1.Text = "ICTC Data: ";
                    dGridViewTxnCheckRippleTable.DataSource = null;
                    dGridViewTxnCheckRippleTable.DataSource = aDataTableICTC;
                    dGridViewTxnCheckRippleTable.DataBind();
                }

                whereClause = " WHERE [TransactionNo]='" + txtBoxPinTxnCheck.Text.Trim() + "'";
                DataTable aDataTableGCC = mg.GetIndividualTxnByWhereClause("GCC", whereClause);
                if (aDataTableGCC.Rows.Count > 0)
                {
                    Label1.Text = "GCC Data: ";
                    dGridViewTxnCheckRippleTable.DataSource = null;
                    dGridViewTxnCheckRippleTable.DataSource = aDataTableGCC;
                    dGridViewTxnCheckRippleTable.DataBind();
                }

                //===============================================================================================

                //===  OLD system  ===========================================================================

                bool foundInOldDb = false;                

                //----- not found at all
                if (!foundInNewDb && !foundInOldDb)
                {
                    lblTxnCheckNoDataFound.Text = "NO DATA FOUND !!!";
                }
                else
                {
                    lblTxnCheckNoDataFound.Text = "";
                }
            }
            else
            {
                txtBoxPinTxnCheck.Focus();
            }
        }

        protected void btnSearchTxnCheckByMobile_Click(object sender, EventArgs e)
        {
            if (!txtMobileNoTxnCheck.Text.Trim().Equals(""))
            {
                //Cursor.Current = Cursors.WaitCursor;
                bool foundInNewDb = false;

                DataTable aDTBkashReg = mg.GetDataUsingMobileNoFromNewSystem(txtMobileNoTxnCheck.Text.Trim(), "BkashRegular");
                dGridViewTxnCheckOutputNewSys.DataSource = null;
                dGridViewTxnCheckOutputNewSys.DataSource = aDTBkashReg;
                dGridViewTxnCheckOutputNewSys.DataBind();

                if (aDTBkashReg.Rows.Count > 0)
                {
                    lblPaymentModeNameTxnCheckPage.Text = "bKash Regular :" + " Total Rows - " + aDTBkashReg.Rows.Count;
                    foundInNewDb = true;
                }
                else
                {
                    lblPaymentModeNameTxnCheckPage.Text = "";
                    foundInNewDb = false;
                }


                // checking bKash Direct
                if (!foundInNewDb)
                {
                    DataTable aDTBkashDir = mg.GetDataUsingMobileNoFromNewSystem(txtMobileNoTxnCheck.Text.Trim(), "BkashDirect");
                    dGridViewTxnCheckOutputNewSys.DataSource = null;
                    dGridViewTxnCheckOutputNewSys.DataSource = aDTBkashDir;
                    dGridViewTxnCheckOutputNewSys.DataBind();

                    if (aDTBkashDir.Rows.Count > 0)
                    {
                        lblPaymentModeNameTxnCheckPage.Text = "bKash Direct : " + " Total Rows - " + aDTBkashDir.Rows.Count;
                        foundInNewDb = true;

                        //dGridViewTxnCheckOutputNewSys.Columns["ID"].Width = 70;
                        //dGridViewTxnCheckOutputNewSys.Columns["SrcCountry"].Width = 70;
                        //dGridViewTxnCheckOutputNewSys.Columns["Amount"].Width = 70;
                        //dGridViewTxnCheckOutputNewSys.Columns["ProcStats"].Width = 80;
                    }
                    else
                    {
                        lblPaymentModeNameTxnCheckPage.Text = "";
                        foundInNewDb = false;
                    }
                }


                //===  OLD system  ===========================================================================

                bool foundInOldDb = false;
                DataTable aDataTableOld = new DataTable();
                /*
                if (!foundInOldDb)
                {
                    aDataTableOld = mg.GetDataUsingMobileNoFromOldSystem(txtMobileNoTxnCheck.Text.Trim(), "BkashRegular");
                    dGridViewTxnCheckOutputOldSys.DataSource = null;
                    dGridViewTxnCheckOutputOldSys.DataSource = aDataTableOld;
                    dGridViewTxnCheckOutputOldSys.DataBind();

                    if (aDataTableOld.Rows.Count > 0)
                    {
                        lblDataInOldSys.Text = "Data In Old System : BkashRegular";
                        foundInOldDb = true;
                    }
                    else
                    {
                        lblDataInOldSys.Text = "Data In Old System :";
                        foundInOldDb = false;
                    }
                }

                if (!foundInOldDb)
                {
                    aDataTableOld = mg.GetDataUsingMobileNoFromOldSystem(txtMobileNoTxnCheck.Text.Trim(), "BkashDirect");
                    dGridViewTxnCheckOutputOldSys.DataSource = null;
                    dGridViewTxnCheckOutputOldSys.DataSource = aDataTableOld;
                    dGridViewTxnCheckOutputOldSys.DataBind();

                    if (aDataTableOld.Rows.Count > 0)
                    {
                        lblDataInOldSys.Text = "Data In Old System : BkashDirect";
                        foundInOldDb = true;

                        //dGridViewTxnCheckOutputOldSys.Columns["ID"].Width = 70;
                        //dGridViewTxnCheckOutputOldSys.Columns["SrcCountry"].Width = 70;
                        //dGridViewTxnCheckOutputOldSys.Columns["Amount"].Width = 70;
                        //dGridViewTxnCheckOutputOldSys.Columns["ProcStats"].Width = 80;
                    }
                    else
                    {
                        lblDataInOldSys.Text = "Data In Old System :";
                        foundInOldDb = false;
                    }
                }
                */

                //----- not found at all -----------
                if (!foundInNewDb && !foundInOldDb)
                {
                    lblTxnCheckNoDataFound.Text = "NO DATA FOUND !!!";
                }
                else
                {
                    lblTxnCheckNoDataFound.Text = "";
                }
                //------------------------------------

                //Cursor.Current = Cursors.Default;
            }
            else
            {
                //MessageBox.Show("Please enter 'Mobile No' for Search", "Empty Search Value", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtMobileNoTxnCheck.Focus();
            }
        }

        protected void dGridViewTxnCheckOutputNewSys_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            string statusVal = "";

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (lblPaymentModeNameTxnCheckPage.Text.Contains("BEFTN"))
                {
                    statusVal = e.Row.Cells[3].Text;
                    if (statusVal.Contains("Success"))
                    {
                        e.Row.Cells[3].BackColor = Color.FromName("#99ff99");
                    }
                    else
                    {
                        e.Row.Cells[3].BackColor = Color.FromName("yellow");
                    }
                }
                else if (lblPaymentModeNameTxnCheckPage.Text.Contains("MTB A/C"))
                {
                    statusVal = e.Row.Cells[2].Text;
                    if (statusVal.Contains("Success"))
                    {
                        e.Row.Cells[2].BackColor = Color.FromName("#99ff99");
                    }
                    else
                    {
                        e.Row.Cells[2].BackColor = Color.FromName("yellow");
                    }
                }
                else if (lblPaymentModeNameTxnCheckPage.Text.Contains("bKash Regular"))
                {
                    statusVal = e.Row.Cells[2].Text;
                    if (statusVal.Contains("Success"))
                    {
                        e.Row.Cells[2].BackColor = Color.FromName("#99ff99");
                    }
                    else
                    {
                        e.Row.Cells[2].BackColor = Color.FromName("yellow");
                    }
                }
                else if (lblPaymentModeNameTxnCheckPage.Text.Contains("bKash Direct"))
                {
                    statusVal = e.Row.Cells[2].Text;
                    if (statusVal.Contains("Success"))
                    {
                        e.Row.Cells[2].BackColor = Color.FromName("#99ff99");
                    }
                    else
                    {
                        e.Row.Cells[2].BackColor = Color.FromName("yellow");
                    }
                }
                else if (lblPaymentModeNameTxnCheckPage.Text.Contains("Cash Txn"))
                {
                    statusVal = e.Row.Cells[2].Text;
                    if (statusVal.Contains("Disbursed"))
                    {
                        e.Row.Cells[2].BackColor = Color.FromName("#99ff99");
                    }
                    else
                    {
                        e.Row.Cells[2].BackColor = Color.FromName("yellow");
                    }
                }
                else
                {

                }
            }
        }


    }
}