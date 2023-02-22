using OfficeOpenXml;
using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class DirectModelManuallyProcess : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable dtFile = new DataTable();

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
                LoadDirectMTOName();
                LoadDirectMTONameForUpdate();
            }
        }

        private void LoadDirectMTONameForUpdate()
        {
            DataTable dtExchs = mg.GetDirectMTOName();
            ddlDirectMTO.Items.Clear();
            ddlDirectMTO.Items.Add("-- SELECT --");
            for (int rows = 0; rows < dtExchs.Rows.Count; rows++)
            {
                ddlDirectMTO.Items.Add(dtExchs.Rows[rows][0].ToString());
            }
            ddlDirectMTO.SelectedIndex = 0;
        }

        private void LoadDirectMTOName()
        {
            DataTable dtExchs = mg.GetDirectMTOName();
            comboBoxDirectMTO.Items.Clear();
            comboBoxDirectMTO.Items.Add("-- SELECT --");
            for (int rows = 0; rows < dtExchs.Rows.Count; rows++)
            {
                comboBoxDirectMTO.Items.Add(dtExchs.Rows[rows][0].ToString());
            }
            comboBoxDirectMTO.SelectedIndex = 0;
        }

        protected void btnSearchUnprocsTxn_Click(object sender, EventArgs e)
        {
            int prtyId = 0;

            if (comboBoxDirectMTO.SelectedIndex != 0)
            {
                prtyId = Convert.ToInt32(comboBoxDirectMTO.Text.Split('-')[0]);

                DataTable dtDirectPending = mg.GetBkashDirectPendingByPartyId(prtyId);
                dataGridViewDirectUnprocessedTxn.DataSource = null;
                dataGridViewDirectUnprocessedTxn.DataSource = dtDirectPending;
                dataGridViewDirectUnprocessedTxn.DataBind();

                lblPendingTxnCount.Text = "Total Txn: " + dtDirectPending.Rows.Count.ToString();

                lblFileSaveProgress.Text = "";
            }
        }

        protected void btnDownloadExcelUnprocs_Click(object sender, EventArgs e)
        {
            int prtyId = 0;
            string drRemks = "", crRemks = "";
            DataRow drow;

            if (comboBoxDirectMTO.SelectedIndex != 0)
            {
                prtyId = Convert.ToInt32(comboBoxDirectMTO.Text.Split('-')[0]);
                DataTable dtDirectPending = mg.GetBkashDirectPendingByPartyId(prtyId);

                DataTable dtDirectPendingWithRemarks = CreateDataTableBkashDirectWithRemarks();

                for (int rw = 0; rw < dtDirectPending.Rows.Count; rw++)
                {
                    drow = dtDirectPendingWithRemarks.NewRow();

                    drow["ID"] = dtDirectPending.Rows[rw]["ID"];
                    drow["PartyID"] = dtDirectPending.Rows[rw]["PartyID"];
                    drow["TxnId"] = dtDirectPending.Rows[rw]["TxnId"].ToString().Trim();
                    drow["MtoName"] = dtDirectPending.Rows[rw]["MtoName"];
                    drow["Amount"] = Convert.ToDecimal(dtDirectPending.Rows[rw]["Amount"]);
                    drow["BeneficiaryName"] = dtDirectPending.Rows[rw]["BeneficiaryName"];
                    drow["BeneWallet"] = dtDirectPending.Rows[rw]["BeneWallet"];

                    drRemks = dtDirectPending.Rows[rw]["TxnId"] + "|bkashDirect|" + dtDirectPending.Rows[rw]["BeneWallet"] + "|" + dtDirectPending.Rows[rw]["BeneficiaryName"];
                    if (drRemks.Length > 47)
                    {
                        drRemks = drRemks.Substring(0, 47);
                    }

                    crRemks = drRemks + ".";

                    drow["DebitRemarks"] = drRemks;
                    drow["CreditRemarks"] = crRemks;
                    
                    dtDirectPendingWithRemarks.Rows.Add(drow);
                    lblFileSaveProgress.Text = "";
                }

                string mtoName = comboBoxDirectMTO.Text.Split('-')[1].ToString();
                string fileName = "bKashDirect_PendingTxnList_" + mtoName + "_" + DateTime.Now.ToString("yyyyMMdd'T'HHmmss") + ".xls";

                if (dtDirectPendingWithRemarks.Rows.Count > 0)
                {
                    StringWriter tw = new StringWriter();
                    HtmlTextWriter hw = new HtmlTextWriter(tw);
                    DataGrid dgGrid = new DataGrid();
                    dgGrid.DataSource = dtDirectPendingWithRemarks;
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
                else
                {
                    lblFileSaveProgress.Text = "Nothing to Download !!!";
                }
            }

        }

        private DataTable CreateDataTableBkashDirectWithRemarks()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("PartyID");
            dt.Columns.Add("TxnId");
            dt.Columns.Add("MtoName");
            dt.Columns.Add("Amount");
            dt.Columns.Add("BeneficiaryName");
            dt.Columns.Add("BeneWallet");
            dt.Columns.Add("DebitRemarks");
            dt.Columns.Add("CreditRemarks");
            return dt;
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Verifies that the control is rendered */
        }

        protected void btnLoadDirectFile_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                if (Path.GetExtension(FileUpload1.FileName) == ".xlsx")  // || Path.GetExtension(FileUpload1.FileName) == ".xls"
                {
                    ExcelPackage package = new ExcelPackage(FileUpload1.FileContent);
                    ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
                    DataTable table = new DataTable();

                    foreach (var firstRowCell in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
                    {
                        table.Columns.Add(firstRowCell.Text);
                    }

                    for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
                    {
                        var row = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];
                        var newRow = table.NewRow();
                        foreach (var cell in row)
                        {
                            newRow[cell.Start.Column - 1] = cell.Text;
                        }
                        table.Rows.Add(newRow);
                    }

                    if (table.Rows.Count > 0)
                    {
                        string autoid = "", partyId = "", txnId = "";
                        DataRow drow;
                        dtFile = new DataTable();
                        dtFile = CreateDataTableCommon();

                        for (int rowCount = 0; rowCount < table.Rows.Count; rowCount++)
                        {
                            autoid = Convert.ToString(table.Rows[rowCount][0]);
                            
                            if (autoid != null)
                            {
                                drow = dtFile.NewRow();
                                partyId = Convert.ToString(table.Rows[rowCount][1]);
                                txnId = Convert.ToString(table.Rows[rowCount][2]);

                                drow["ID"] = autoid;
                                drow["PartyID"] = partyId;
                                drow["TxnId"] = txnId;

                                dtFile.Rows.Add(drow);
                                lblFileLoadMsg.Text = "Loading " + txnId + " ( Remain: " + (table.Rows.Count - rowCount + 1) + ")";
                                lblFileLoadMsg.ForeColor = Color.Green;
                            }
                        }

                        lblFileLoadMsg.Text = "Load Complete";
                        lblFileLoadMsg.ForeColor = Color.Green;
                    }
                }
                else
                {
                    lblFileLoadMsg.Text = "Please Select .xlsx File to proceed !!!";
                    lblFileLoadMsg.ForeColor = Color.Red;
                }
            }
        }

        private DataTable CreateDataTableCommon()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");//0
            dt.Columns.Add("PartyID");//1
            dt.Columns.Add("TxnId");//2
            return dt;
        }

        protected void ddlDirectMTO_SelectedIndexChanged(object sender, EventArgs e)
        {
            int prtyId = 0;
            if (ddlDirectMTO.SelectedIndex != 0)
            {
                prtyId = Convert.ToInt32(ddlDirectMTO.Text.Split('-')[0]);
                DataTable dtDirectMTOAccNum = mg.GetBkashDirectMTOAccountNumberByPartyId(prtyId);
                
                if (dtDirectMTOAccNum.Rows.Count > 0)
                {
                    txtFromAcc.Text = dtDirectMTOAccNum.Rows[0]["AccountNo"].ToString();
                    txtToAcc.Text = dtDirectMTOAccNum.Rows[0]["MobileWalletPaymentAccount"].ToString();
                }
            }
        }

        protected void btnProcess_Click(object sender, EventArgs e)
        {
            string fromAcc = txtFromAcc.Text.Trim();
            string toAcc = txtToAcc.Text.Trim();
            string journal = txtJournalNumber.Text.Trim();

            string autoid = "", partyId = "", txnId = "";
            bool updDirectSettlement, isExists;

            if (!fromAcc.Equals("") && !toAcc.Equals("") && !journal.Equals(""))
            {
                for (int ii = 0; ii < dtFile.Rows.Count; ii++)
                {
                    autoid = dtFile.Rows[ii][0].ToString().Trim();
                    partyId = dtFile.Rows[ii][1].ToString().Trim();
                    txnId = dtFile.Rows[ii][2].ToString().Trim();

                    updDirectSettlement = mg.UpdateRemitStatusDirectSettlementTable(autoid, txnId, fromAcc, toAcc, journal);

                    isExists = mg.IsTrxnExistInDirectRemitProcessTable(partyId, autoid);
                    if (!isExists)
                    {
                        mg.SaveTrxnIntoDirectRemitProcessTable(partyId, autoid);
                    }

                    lblFileProcessMsg.Text = "Processing " + txnId;
                }

                lblFileProcessMsg.Text = "DONE";
            }
            else
            {
                lblFileProcessMsg.Text = "Please Fill up Necessary Fields and Try";
            }
        }


    }
}