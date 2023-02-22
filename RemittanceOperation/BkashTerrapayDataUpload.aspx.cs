using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using RemittanceOperation.ModelClass;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class BkashTerrapayDataUpload : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable bkashTxnData = new DataTable();
        string FOLDER_PATH = ConfigurationManager.AppSettings["FolderPathTerrapayFiles"]; 

        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            //{
            //}
            //else
            //{
            //    Response.Redirect("Login.aspx");
            //}

            if (!IsPostBack)
            {
                DateTime thisMonthFirstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                dtpickerFrom.Text = thisMonthFirstDay.ToString("yyyy-MM-dd");
                dtpickerTo.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                lblFileUploadMsg.Text = "";

                LoadBkashParty();
            }
        }

        private void LoadBkashParty()
        {
            ddlBkashParty.Items.Clear();
            ddlBkashParty.Items.Add("-- Select --");
            ddlBkashParty.Items.Add("1-BKASH REGULAR");

            DataTable dtDirectMTO = mg.GetDirectMTOName();

            for (int rows = 0; rows < dtDirectMTO.Rows.Count; rows++)
            {
                ddlBkashParty.Items.Add(dtDirectMTO.Rows[rows][0].ToString());
            }
            ddlBkashParty.Items.Add("=====================");

            DataTable dtRippleMTO = mg.GetRippleMTOName();
            for (int rows = 0; rows < dtRippleMTO.Rows.Count; rows++)
            {
                ddlBkashParty.Items.Add(dtRippleMTO.Rows[rows][0].ToString());
            }
            
            ddlBkashParty.Items.Add("=====================");
            ddlBkashParty.Items.Add("2-BKASH B2C");

            //DataTable dtServiceRemMTO = mg.GetServiceRemMTOName();
            //for (int rows = 0; rows < dtServiceRemMTO.Rows.Count; rows++)
            //{
            //    ddlBkashParty.Items.Add(dtServiceRemMTO.Rows[rows][0].ToString());
            //}

            ddlBkashParty.SelectedIndex = 0;
        }

        protected void btnUploadFile_Click(object sender, EventArgs e)
        {

            DateTime dateTime1 = DateTime.ParseExact(dtpickerFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dateTime2 = DateTime.ParseExact(dtpickerTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string fromdt = dateTime1.ToString("yyyy-MM-dd");
            string todt = dateTime2.ToString("yyyy-MM-dd");


            if (ddlBkashParty.SelectedIndex == 0)
            {

            }
            else if(ddlBkashParty.Text.Contains("===="))
            {

            }
            else
            {
                int id = Convert.ToInt32(ddlBkashParty.Text.Split('-')[0]);
                string partyName = Convert.ToString(ddlBkashParty.Text.Split('-')[1]);


                string fileFolderPath = Server.MapPath(FOLDER_PATH);
                Utility.DeleteOldDaysFiles(fileFolderPath);
                string csvPath = fileFolderPath + "\\" + FileUploadBkashFile.FileName.Split('\\')[FileUploadBkashFile.FileName.Split('\\').Length - 1];
                FileUploadBkashFile.SaveAs(csvPath);


                string whole_file = File.ReadAllText(csvPath);
                whole_file = whole_file.Replace('\n', '\r');
                string[] lines = whole_file.Split(new char[] { '\r' }, StringSplitOptions.RemoveEmptyEntries);

                int num_rows = lines.Length;
                int num_cols = 19;
                string[,] values = new string[num_rows, num_cols];

                for (int r = 0; r < num_rows; r++)
                {
                    string[] line_r = lines[r].Split(',');
                    for (int c = 0; c < num_cols; c++)
                    {
                        if (c < line_r.Length)
                        {
                            values[r, c] = line_r[c];
                        }
                    }
                }

                DataTable dtTPFile = CreateDataTable();
                DataRow drow;

                string remitStatus = "", convid;
                string txnmode = GetModeName(id, partyName);

                for (int row = 9; row < num_rows; row++)
                {
                    remitStatus = values[row, 15];

                    if (remitStatus != null && remitStatus.ToLower().Contains("success"))
                    {
                        convid = values[row, 3];
                        if (!String.IsNullOrEmpty(convid))
                        {
                            drow = dtTPFile.NewRow();
                            drow["TxnMode"] = txnmode;
                            drow["TxnNo"] = Convert.ToString(values[row, 0]);
                            drow["ReceivedTime"] = Convert.ToString(values[row, 1]);
                            drow["LastUpdateTime"] = Convert.ToString(values[row, 2]);
                            drow["ConversationId"] = Convert.ToString(values[row, 3]);
                            drow["PartnerTxnId"] = Convert.ToString(values[row, 4]);
                            drow["PartnerName"] = Convert.ToString(values[row, 5]);
                            drow["MTOName"] = Convert.ToString(values[row, 6]);
                            drow["SenderMSISDN"] = Convert.ToString(values[row, 7]);
                            drow["SourceCountry"] = Convert.ToString(values[row, 8]);
                            drow["SourceCurrency"] = Convert.ToString(values[row, 9]);
                            drow["SourceAmount"] = Convert.ToString(values[row, 10]);
                            drow["DestAmount"] = Convert.ToString(values[row, 11]);
                            drow["ReceiverWallet"] = Convert.ToString(values[row, 13]);
                            drow["bKashTxnId"] = Convert.ToString(values[row, 14]);
                            drow["SenderName"] = Convert.ToString(values[row, 16]);
                            drow["ReceiverName"] = Convert.ToString(values[row, 17]);
                            drow["ReceiverAddress"] = Convert.ToString(values[row, 18]);
                            drow["ReportFromDate"] = dtpickerFrom.Text;
                            drow["ReportToDate"] = dtpickerTo.Text;

                            if (partyName.ToUpper().Contains("DIRECT"))
                            {
                                drow["RefNo"] = Convert.ToString(values[row, 4]);
                                drow["PartyId"] = id;
                            }
                            else if (partyName.ToUpper().Contains("RIPPLE"))
                            {
                                drow["RefNo"] = Convert.ToString(values[row, 4]);
                                drow["PartyId"] = id;
                            }
                            else
                            {
                                drow["RefNo"] = Convert.ToString(values[row, 4].Split('-')[0]);
                                drow["PartyId"] = Convert.ToString(values[row, 4].Split('-')[1]);
                            }

                            dtTPFile.Rows.Add(drow);
                        }
                    }
                }

                /*dtTemp.Columns.Add("TxnMode");  dtTemp.Columns.Add("TxnNo"); dtTemp.Columns.Add("ReceivedTime"); dtTemp.Columns.Add("LastUpdateTime");
                dtTemp.Columns.Add("ConversationId");  dtTemp.Columns.Add("PartnerTxnId");  dtTemp.Columns.Add("PartnerName");  dtTemp.Columns.Add("MTOName");
                dtTemp.Columns.Add("SenderMSISDN");  dtTemp.Columns.Add("SourceCountry"); dtTemp.Columns.Add("SourceCurrency"); dtTemp.Columns.Add("SourceAmount");
                dtTemp.Columns.Add("DestAmount");  dtTemp.Columns.Add("ReceiverWallet"); dtTemp.Columns.Add("bKashTxnId");  dtTemp.Columns.Add("SenderName");
                dtTemp.Columns.Add("ReceiverName");  dtTemp.Columns.Add("ReceiverAddress");  dtTemp.Columns.Add("ReportFromDate"); dtTemp.Columns.Add("ReportToDate");
                dtTemp.Columns.Add("TxnId");  dtTemp.Columns.Add("PartyId");*/


                bkashTxnData = dtTPFile;
                dataGridViewBkashData.DataSource = null;
                dataGridViewBkashData.DataSource = bkashTxnData;
                dataGridViewBkashData.DataBind();
                lblTotalRecords.Text = bkashTxnData.Rows.Count + "";

                if (bkashTxnData.Rows.Count > 0)
                {
                    lblFileUploadMsg.Text = "File Uploaded Successfully...";
                }
            }
        }

        private string GetModeName(int id, string partyName)
        {
            string mode = "";

            if (partyName.ToUpper().Contains("REGULAR"))
                mode = "REGULAR";
            else if (partyName.ToUpper().Contains("DIRECT"))
                mode = "DIRECT";
            else if (partyName.ToUpper().Contains("RIPPLE"))
                mode = "RIPPLE";
            else if (partyName.ToUpper().Contains("SERVICE"))
                mode = "B2C";
            else if (partyName.ToUpper().Contains("DLOCAL"))
                mode = "B2C";
            else if (partyName.ToUpper().Contains("B2C"))
                mode = "B2C";
            else
                mode = "REGULAR";
            return mode;
        }

        private DataTable CreateDataTable()
        {
            DataTable dtTemp = new DataTable();
            dtTemp.Columns.Add("TxnMode");
            dtTemp.Columns.Add("TxnNo");
            dtTemp.Columns.Add("ReceivedTime");
            dtTemp.Columns.Add("LastUpdateTime");
            dtTemp.Columns.Add("ConversationId");
            dtTemp.Columns.Add("PartnerTxnId");
            dtTemp.Columns.Add("PartnerName");
            dtTemp.Columns.Add("MTOName");
            dtTemp.Columns.Add("SenderMSISDN");
            dtTemp.Columns.Add("SourceCountry");
            dtTemp.Columns.Add("SourceCurrency");
            dtTemp.Columns.Add("SourceAmount");
            dtTemp.Columns.Add("DestAmount");
            dtTemp.Columns.Add("ReceiverWallet");
            dtTemp.Columns.Add("bKashTxnId");
            dtTemp.Columns.Add("SenderName");
            dtTemp.Columns.Add("ReceiverName");
            dtTemp.Columns.Add("ReceiverAddress");
            dtTemp.Columns.Add("ReportFromDate");
            dtTemp.Columns.Add("ReportToDate");
            dtTemp.Columns.Add("RefNo");
            dtTemp.Columns.Add("PartyId");
            return dtTemp;
        }

        protected void ddlBkashParty_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblFileUploadMsg.Text = "";
        }

        protected void btnDataUploadSystem_Click(object sender, EventArgs e)
        {
            if (bkashTxnData.Rows.Count > 0)
            {
                lblFileUploadMsg.Text = "";

                DateTime dateTime1 = DateTime.ParseExact(dtpickerFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime dateTime2 = DateTime.ParseExact(dtpickerTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                string fromdt = dateTime1.ToString("yyyy-MM-dd");
                string todt = dateTime2.ToString("yyyy-MM-dd");

                int id = Convert.ToInt32(ddlBkashParty.Text.Split('-')[0]);
                string txnMode = bkashTxnData.Rows[0]["TxnMode"].ToString().Trim();

                mg.Delete_BkashTerrapayCommission_TableDataByModeDate(fromdt, todt, txnMode, id);

                //int TxnSaveCount = 0;
                //bool saveStatus = mg.SaveBkashTerrapayCommissionDataIntoDB(bkashTxnData, ref TxnSaveCount);

                //-------------------------------

                bool indivRowSaveStat = false;
                int rowCount = 0;

                for (int rows = 0; rows < bkashTxnData.Rows.Count; rows++)
                {
                    indivRowSaveStat = mg.SaveBkashTerrapayCommissionDataRowWiseIntoDB(bkashTxnData.Rows[rows]);
                    if(indivRowSaveStat)
                    {
                        rowCount++;
                        lblUploadStatus.Text = "Save Success -> " + rowCount + " of " + bkashTxnData.Rows.Count;
                    }
                }

                //-------------------------------

                //lblDataCountSaveIntoDB.Text = "Input File Count: " + bkashTxnData.Rows.Count + " , Save Count Into DB: " + TxnSaveCount;

                lblDataCountSaveIntoDB.Text = "Input File Count: " + bkashTxnData.Rows.Count + " , Save Count Into DB: " + rowCount;
            }
            else
            {
                lblFileUploadMsg.Text = "NO DATA TO UPLOAD INTO SYSTEM !!!!";
            }
        }

        protected void dataGridViewBkashData_PageIndexChanged(object sender, EventArgs e)
        {
            dataGridViewBkashData.DataSource = bkashTxnData;
            dataGridViewBkashData.DataBind();
        }

        protected void dataGridViewBkashData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex == -1)
                dataGridViewBkashData.PageIndex = Int32.MaxValue;
            else
                dataGridViewBkashData.PageIndex = e.NewPageIndex;
        }
    }
}