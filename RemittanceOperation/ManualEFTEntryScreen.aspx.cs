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
    public partial class ManualEFTEntryScreen : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable tellerScreenExhFileData = new DataTable();
        string fileProcessUserType = "", IsMailReceive = "", userName = "", userEmail = "";
        string loggedUserId = "", loggedUserRM = "";

        static int EFT_ACCOUNT_NUMBER_MAX_LENGTH = 17;
        static int EFT_ROUTING_NUMBER_MAX_LENGTH = 9;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
                fileProcessUserType = Session[CSessionName.S_FILE_PROCESS_USER_TYPE].ToString();
                IsMailReceive = Session[CSessionName.S_IS_MAIL_RECEIVE].ToString();

                loggedUserId = Session[CSessionName.S_CURRENT_USERID].ToString();
                loggedUserRM = Session[CSessionName.S_CURRENT_USER_RM].ToString();
                userName = Session[CSessionName.S_CURRENT_USER_FULL_NAME].ToString();
                userEmail = Session[CSessionName.S_CURRENT_USER_EMAIL].ToString();
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                LoadExhouseList();
            }
        }

        private void LoadExhouseList()
        {
            DataTable dtExchs = mg.LoadManualEFTExhouseList();
            cbExh.Items.Clear();
            cbExh.Items.Add("--Select--");

            for (int rows = 0; rows < dtExchs.Rows.Count; rows++)
            {
                cbExh.Items.Add(dtExchs.Rows[rows][0] + "");
            }
            cbExh.SelectedIndex = 0;
        }

        protected void buttonUploadDataFromTeller_Click(object sender, EventArgs e)
        {
            if (this.fileProcessUserType.ToLower().Equals("teller") || this.fileProcessUserType.ToLower().Equals("admin") || this.fileProcessUserType.ToLower().Equals("superadmin"))
            {
                //this will validate EFT 9 digit routing number
                bool isSuccess = ValidateTxn(tellerScreenExhFileData);

                if (isSuccess)
                {
                    bool isExistsRecord = false;
                    bool isSaved = false;
                    lblTellerFileUploadMsg.Text = "";

                    if (tellerScreenExhFileData.Rows.Count > 0)
                    {
                        DataTable dtAuthAndSAEmailList = mg.GetAuthorizerAndSuperAdminEmailList();

                        int exhId = Convert.ToInt32(cbExh.Text.Split('-')[0]);
                        string exhName = Convert.ToString(cbExh.Text.Split('-')[1]).Trim();
                        int recordCount = tellerScreenExhFileData.Rows.Count;
                        string ticks = Convert.ToString(DateTime.Now.Ticks);

                        for (int ii = 0; ii < tellerScreenExhFileData.Rows.Count; ii++)
                        {
                            isExistsRecord = mg.IsThisTransactionExistBeforeManualEFTProcess(exhId, Convert.ToString(tellerScreenExhFileData.Rows[ii][0]));
                            if (!isExistsRecord)
                            {
                                isSaved = mg.SaveExhDataManualEFTProcess(exhId, tellerScreenExhFileData.Rows[ii], loggedUserId, ticks);
                            }
                            else
                            {
                                listBoxError.Text += "PIN -> " + tellerScreenExhFileData.Rows[ii][0].ToString().Trim() + " - Already Exist." + Environment.NewLine;
                            }

                        } //for end

                        if (isSaved)
                        {
                            mg.ChangeStatusFromUploadedToReceivedManualEFTProcess(ticks);
                            lblTellerFileUploadMsg.Text = "Upload Successfully for Authorization !!!";
                            lblTellerFileUploadMsg.ForeColor = Color.Green;

                            tellerScreenExhFileData = new DataTable();
                            dataGridViewExhDataTellerScreen.DataSource = null;

                            DataTable dtSummaryUploadedRecord = mg.GetSummaryTellerUploadedRecordManualEFTProcess(ticks);
                            SendMailToAuthorizer(exhName, recordCount, dtAuthAndSAEmailList, userName, userEmail, dtSummaryUploadedRecord);
                        }
                        else
                        {
                            lblTellerFileUploadMsg.Text = "Error Occured when uploading data !!!";
                            lblTellerFileUploadMsg.ForeColor = Color.Red;
                        }

                    }//if end
                }
                else
                {
                    lblTellerFileUploadMsg.Text = "One or more transaction has Invalid Routing number/Data. Please Fix it !!!";
                    lblTellerFileUploadMsg.ForeColor = Color.Red;
                }
            }
            else
            {
                lblTellerFileUploadMsg.Text = "Only Teller can upload Transactions !!!";
                lblTellerFileUploadMsg.ForeColor = Color.Red;
            }
        }

        private DataTable GetExchangeDataFromExcel(ExcelWorksheet workSheet, string exhName)
        {
            DataTable dtFile = CreateDataTableCommon();

            try
            {
                if (exhName.Contains("GHURAIR"))
                {
                    dtFile = ALGHURAIRData(dtFile, workSheet);
                }
                else if (exhName.Contains("GULF"))
                {
                    dtFile = GULFData(dtFile, workSheet);
                }
                else if (exhName.Contains("ISLAMIC"))
                {
                    dtFile = ISLAMICData(dtFile, workSheet);
                }
                else if (exhName.Contains("UAE EXCHANGE"))
                {
                    dtFile = UAEEXCHANGEData(dtFile, workSheet);
                }
                else if (exhName.Contains("DOHA"))
                {
                    dtFile = DOHAData(dtFile, workSheet);
                }
                else if (exhName.Contains("ZAMAN"))
                {
                    dtFile = ALZAMANData(dtFile, workSheet);
                }
                else if (exhName.Contains("UNIVERSAL"))
                {
                    dtFile = UNIVERSALData(dtFile, workSheet);
                }
                else if (exhName.Contains("INSTANT CASH"))
                {
                    dtFile = INSTANTCASHData(dtFile, workSheet);
                }
            }
            catch (Exception ec)
            {
            }

            return dtFile;
        }

        private DataTable INSTANTCASHData(DataTable dtFile, ExcelWorksheet workSheet)
        {
            DataRow drow;

            int endrow = workSheet.Dimension.End.Row;
            string pinno, bankNm = "", accountNo = "";

            for (var rowNumber = 6; rowNumber <= endrow; rowNumber++)
            {
                pinno = Convert.ToString(workSheet.Cells[rowNumber, 1].Text);

                int pinLength = pinno != null ? pinno.Length : 0;

                if (pinLength > 0) //(pinno != null || pinno.Length>0)
                {
                    drow = dtFile.NewRow();

                    drow["RefNo"] = Convert.ToString(workSheet.Cells[rowNumber, 1].Text);
                    drow["BeneficiaryName"] = Convert.ToString(workSheet.Cells[rowNumber, 4].Text);
                    accountNo = Convert.ToString(workSheet.Cells[rowNumber, 6].Text);
                    accountNo = (accountNo == null || accountNo.Equals("")) ? "" : accountNo;
                    drow["AccountNo"] = accountNo;
                    drow["BankName"] = Convert.ToString(workSheet.Cells[rowNumber, 8].Text);
                    drow["BranchName"] = Convert.ToString(workSheet.Cells[rowNumber, 9].Text);
                    drow["RoutingNo"] = Convert.ToString(workSheet.Cells[rowNumber, 10].Text);
                    drow["Amount"] = Math.Round(Convert.ToDouble(workSheet.Cells[rowNumber, 11].Text), 2);
                    drow["BeneficiaryAddress"] = Convert.ToString(workSheet.Cells[rowNumber, 5].Text);
                    drow["RemitterName"] = Convert.ToString(workSheet.Cells[rowNumber, 2].Text);
                    drow["RemitterAddress"] = Convert.ToString(workSheet.Cells[rowNumber, 3].Text);
                    drow["Purpose"] = "Family Maintenance";
                    drow["BeneficiaryContactNo"] = Convert.ToString(workSheet.Cells[rowNumber, 14].Text);

                    bankNm = Convert.ToString(workSheet.Cells[rowNumber, 8].Text);
                    if (accountNo.Trim().Equals("") || accountNo.Trim().ToLower().Contains("coc") || accountNo.Trim().ToLower().Contains("cash"))
                    {
                        drow["PayMode"] = "CASH";
                    }
                    else if (!accountNo.Trim().Equals("") && (bankNm.ToUpper().Contains("MUTUAL") || bankNm.ToUpper().Contains("MTB")))
                    {
                        drow["PayMode"] = "MTB";
                    }
                    else
                    {
                        drow["PayMode"] = "EFT";
                    }
                    dtFile.Rows.Add(drow);
                }
            }
            return dtFile;
        }

        private DataTable UNIVERSALData(DataTable dtFile, ExcelWorksheet workSheet)
        {
            DataRow drow;

            int endrow = workSheet.Dimension.End.Row;
            string pinno, bankNm = "", accountNo = "";

            for (var rowNumber = 2; rowNumber <= endrow; rowNumber++)
            {
                pinno = Convert.ToString(workSheet.Cells[rowNumber, 1].Text);

                if (pinno != null)
                {
                    drow = dtFile.NewRow();

                    drow["RefNo"] = Convert.ToString(workSheet.Cells[rowNumber, 1].Text);
                    drow["BeneficiaryName"] = Convert.ToString(workSheet.Cells[rowNumber, 4].Text);
                    accountNo = Convert.ToString(workSheet.Cells[rowNumber, 6].Text);
                    accountNo = (accountNo == null || accountNo.Equals("")) ? "" : accountNo;
                    drow["AccountNo"] = accountNo;
                    drow["BankName"] = Convert.ToString(workSheet.Cells[rowNumber, 8].Text);
                    drow["BranchName"] = Convert.ToString(workSheet.Cells[rowNumber, 9].Text);
                    drow["RoutingNo"] = Convert.ToString(workSheet.Cells[rowNumber, 10].Text);
                    drow["Amount"] = Math.Round(Convert.ToDouble(workSheet.Cells[rowNumber, 11].Text), 2);
                    drow["BeneficiaryAddress"] = Convert.ToString(workSheet.Cells[rowNumber, 5].Text);
                    drow["RemitterName"] = Convert.ToString(workSheet.Cells[rowNumber, 2].Text);
                    drow["RemitterAddress"] = Convert.ToString(workSheet.Cells[rowNumber, 3].Text);
                    drow["Purpose"] = "Family Maintenance";
                    drow["BeneficiaryContactNo"] = "";

                    bankNm = Convert.ToString(workSheet.Cells[rowNumber, 8].Text);
                    if (accountNo.Trim().Equals("") || accountNo.Trim().ToLower().Contains("coc") || accountNo.Trim().ToLower().Contains("cash"))
                    {
                        drow["PayMode"] = "CASH";
                    }
                    else if (!accountNo.Trim().Equals("") && (bankNm.ToUpper().Contains("MUTUAL") || bankNm.ToUpper().Contains("MTB")))
                    {
                        drow["PayMode"] = "MTB";
                    }
                    else
                    {
                        drow["PayMode"] = "EFT";
                    }
                    dtFile.Rows.Add(drow);
                }
            }

            return dtFile;
        }

        private DataTable ALZAMANData(DataTable dtFile, ExcelWorksheet workSheet)
        {
            DataRow drow;

            int endrow = workSheet.Dimension.End.Row;
            string pinno, bankNm = "", accountNo = "";

            for (var rowNumber = 2; rowNumber <= endrow; rowNumber++)
            {
                pinno = Convert.ToString(workSheet.Cells[rowNumber, 2].Text);

                if (pinno != null)
                {
                    drow = dtFile.NewRow();

                    drow["RefNo"] = Convert.ToString(workSheet.Cells[rowNumber, 2].Text);
                    drow["BeneficiaryName"] = Convert.ToString(workSheet.Cells[rowNumber, 3].Text);
                    accountNo = Convert.ToString(workSheet.Cells[rowNumber, 4].Text);
                    accountNo = (accountNo == null || accountNo.Equals("")) ? "" : accountNo;
                    drow["AccountNo"] = accountNo;
                    drow["BankName"] = Convert.ToString(workSheet.Cells[rowNumber, 5].Text);
                    drow["BranchName"] = Convert.ToString(workSheet.Cells[rowNumber, 6].Text);
                    drow["RoutingNo"] = Convert.ToString(workSheet.Cells[rowNumber, 11].Text);
                    drow["Amount"] = Math.Round(Convert.ToDouble(workSheet.Cells[rowNumber, 7].Text), 2);
                    drow["BeneficiaryAddress"] = Convert.ToString(workSheet.Cells[rowNumber, 9].Text);
                    drow["RemitterName"] = Convert.ToString(workSheet.Cells[rowNumber, 8].Text);
                    drow["RemitterAddress"] = Convert.ToString(workSheet.Cells[rowNumber, 9].Text);
                    drow["Purpose"] = "Family Maintenance";
                    drow["BeneficiaryContactNo"] = Convert.ToString(workSheet.Cells[rowNumber, 10].Text);

                    bankNm = Convert.ToString(workSheet.Cells[rowNumber, 5].Text);
                    if (accountNo.Trim().Equals("") || accountNo.Trim().ToLower().Contains("coc") || accountNo.Trim().ToLower().Contains("cash"))
                    {
                        drow["PayMode"] = "CASH";
                    }
                    else if (!accountNo.Trim().Equals("") && (bankNm.ToUpper().Contains("MUTUAL") || bankNm.ToUpper().Contains("MTB")))
                    {
                        drow["PayMode"] = "MTB";
                    }
                    else
                    {
                        drow["PayMode"] = "EFT";
                    }
                    dtFile.Rows.Add(drow);
                }
            }

            return dtFile;
        }

        private DataTable DOHAData(DataTable dtFile, ExcelWorksheet workSheet)
        {
            DataRow drow;

            int endrow = workSheet.Dimension.End.Row;
            string pinno, bankNm = "", accountNo = "";

            for (var rowNumber = 6; rowNumber <= endrow; rowNumber++)
            {
                pinno = Convert.ToString(workSheet.Cells[rowNumber, 1].Text);

                if (pinno != null)
                {
                    drow = dtFile.NewRow();

                    drow["RefNo"] = Convert.ToString(workSheet.Cells[rowNumber, 1].Text);
                    drow["BeneficiaryName"] = Convert.ToString(workSheet.Cells[rowNumber, 6].Text);
                    accountNo = Convert.ToString(workSheet.Cells[rowNumber, 7].Text);
                    accountNo = (accountNo == null || accountNo.Equals("")) ? "" : accountNo;
                    drow["AccountNo"] = accountNo;
                    drow["BankName"] = Convert.ToString(workSheet.Cells[rowNumber, 8].Text);
                    drow["BranchName"] = Convert.ToString(workSheet.Cells[rowNumber, 9].Text);
                    drow["RoutingNo"] = Convert.ToString(workSheet.Cells[rowNumber, 10].Text);
                    drow["Amount"] = Math.Round(Convert.ToDouble(workSheet.Cells[rowNumber, 12].Text), 2);
                    drow["BeneficiaryAddress"] = Convert.ToString(workSheet.Cells[rowNumber, 3].Text);
                    drow["RemitterName"] = Convert.ToString(workSheet.Cells[rowNumber, 2].Text);
                    drow["RemitterAddress"] = Convert.ToString(workSheet.Cells[rowNumber, 4].Text);
                    drow["Purpose"] = "Family Maintenance";
                    drow["BeneficiaryContactNo"] = "";

                    bankNm = Convert.ToString(workSheet.Cells[rowNumber, 8].Text);
                    if (accountNo.Trim().Equals("") || accountNo.Trim().ToLower().Contains("coc") || accountNo.Trim().ToLower().Contains("cash"))
                    {
                        drow["PayMode"] = "CASH";
                    }
                    else if (!accountNo.Trim().Equals("") && (bankNm.ToUpper().Contains("MUTUAL") || bankNm.ToUpper().Contains("MTB")))
                    {
                        drow["PayMode"] = "MTB";
                    }
                    else
                    {
                        drow["PayMode"] = "EFT";
                    }
                    dtFile.Rows.Add(drow);
                }
            }
            return dtFile;
        }

        private DataTable UAEEXCHANGEData(DataTable dtFile, ExcelWorksheet workSheet)
        {
            DataRow drow;

            int endrow = workSheet.Dimension.End.Row;
            string pinno, bankNm = "", accountNo = "";

            for (var rowNumber = 2; rowNumber <= endrow; rowNumber++)
            {
                pinno = Convert.ToString(workSheet.Cells[rowNumber, 1].Text);

                if (pinno != null)
                {
                    drow = dtFile.NewRow();

                    drow["RefNo"] = Convert.ToString(workSheet.Cells[rowNumber, 1].Text);
                    drow["BeneficiaryName"] = Convert.ToString(workSheet.Cells[rowNumber, 5].Text);
                    accountNo = Convert.ToString(workSheet.Cells[rowNumber, 6].Text);
                    accountNo = (accountNo == null || accountNo.Equals("")) ? "" : accountNo;
                    drow["AccountNo"] = accountNo;
                    drow["BankName"] = Convert.ToString(workSheet.Cells[rowNumber, 7].Text);
                    drow["BranchName"] = Convert.ToString(workSheet.Cells[rowNumber, 8].Text);
                    drow["RoutingNo"] = Convert.ToString(workSheet.Cells[rowNumber, 9].Text);
                    drow["Amount"] = Math.Round(Convert.ToDouble(workSheet.Cells[rowNumber, 11].Text), 2);
                    drow["BeneficiaryAddress"] = Convert.ToString(workSheet.Cells[rowNumber, 3].Text);
                    drow["RemitterName"] = Convert.ToString(workSheet.Cells[rowNumber, 2].Text);
                    drow["RemitterAddress"] = Convert.ToString(workSheet.Cells[rowNumber, 4].Text);
                    drow["Purpose"] = "Family Maintenance";
                    drow["BeneficiaryContactNo"] = "";

                    bankNm = Convert.ToString(workSheet.Cells[rowNumber, 7].Text);
                    if (accountNo.Trim().Equals("") || accountNo.Trim().ToLower().Contains("coc") || accountNo.Trim().ToLower().Contains("cash"))
                    {
                        drow["PayMode"] = "CASH";
                    }
                    else if (!accountNo.Trim().Equals("") && (bankNm.ToUpper().Contains("MUTUAL") || bankNm.ToUpper().Contains("MTB")))
                    {
                        drow["PayMode"] = "MTB";
                    }
                    else
                    {
                        drow["PayMode"] = "EFT";
                    }
                    dtFile.Rows.Add(drow);
                }
            }

            return dtFile;
        }

        private DataTable ISLAMICData(DataTable dtFile, ExcelWorksheet workSheet)
        {
            DataRow drow;

            int endrow = workSheet.Dimension.End.Row;
            string pinno, bankNm = "", accountNo = "";

            for (var rowNumber = 2; rowNumber <= endrow; rowNumber++)
            {
                pinno = Convert.ToString(workSheet.Cells[rowNumber, 1].Text);

                if (pinno != null)
                {
                    drow = dtFile.NewRow();

                    drow["RefNo"] = Convert.ToString(workSheet.Cells[rowNumber, 1].Text);
                    drow["BeneficiaryName"] = Convert.ToString(workSheet.Cells[rowNumber, 4].Text);
                    accountNo = Convert.ToString(workSheet.Cells[rowNumber, 6].Text);
                    accountNo = (accountNo == null || accountNo.Equals("")) ? "" : accountNo;
                    drow["AccountNo"] = accountNo;
                    drow["BankName"] = Convert.ToString(workSheet.Cells[rowNumber, 8].Text);
                    drow["BranchName"] = Convert.ToString(workSheet.Cells[rowNumber, 9].Text);
                    drow["RoutingNo"] = Convert.ToString(workSheet.Cells[rowNumber, 10].Text);
                    drow["Amount"] = Math.Round(Convert.ToDouble(workSheet.Cells[rowNumber, 11].Text), 2);
                    drow["BeneficiaryAddress"] = Convert.ToString(workSheet.Cells[rowNumber, 5].Text);
                    drow["RemitterName"] = Convert.ToString(workSheet.Cells[rowNumber, 2].Text);
                    drow["RemitterAddress"] = Convert.ToString(workSheet.Cells[rowNumber, 3].Text);
                    drow["Purpose"] = Convert.ToString(workSheet.Cells[rowNumber, 12].Text);
                    drow["BeneficiaryContactNo"] = "";

                    bankNm = Convert.ToString(workSheet.Cells[rowNumber, 8].Text);
                    if (accountNo.Trim().Equals("") || accountNo.Trim().ToLower().Contains("coc") || accountNo.Trim().ToLower().Contains("cash"))
                    {
                        drow["PayMode"] = "CASH";
                    }
                    else if (!accountNo.Trim().Equals("") && (bankNm.ToUpper().Contains("MUTUAL") || bankNm.ToUpper().Contains("MTB")))
                    {
                        drow["PayMode"] = "MTB";
                    }
                    else
                    {
                        drow["PayMode"] = "EFT";
                    }

                    dtFile.Rows.Add(drow);
                }
            }

            return dtFile;
        }

        private DataTable GULFData(DataTable dtFile, ExcelWorksheet workSheet)
        {
            DataRow drow;

            int endrow = workSheet.Dimension.End.Row;
            string pinno, bankNm = "", accountNo = "";

            for (var rowNumber = 2; rowNumber <= endrow; rowNumber++)
            {
                pinno = Convert.ToString(workSheet.Cells[rowNumber, 1].Text);

                if (pinno != null)
                {
                    drow = dtFile.NewRow();

                    drow["RefNo"] = Convert.ToString(workSheet.Cells[rowNumber, 1].Text);
                    drow["BeneficiaryName"] = Convert.ToString(workSheet.Cells[rowNumber, 4].Text);
                    accountNo = Convert.ToString(workSheet.Cells[rowNumber, 6].Text);
                    accountNo = (accountNo == null || accountNo.Equals("")) ? "" : accountNo;
                    drow["AccountNo"] = accountNo;
                    drow["BankName"] = Convert.ToString(workSheet.Cells[rowNumber, 8].Text);
                    drow["BranchName"] = Convert.ToString(workSheet.Cells[rowNumber, 9].Text);
                    drow["RoutingNo"] = Convert.ToString(workSheet.Cells[rowNumber, 11].Text);
                    drow["Amount"] = Math.Round(Convert.ToDouble(workSheet.Cells[rowNumber, 10].Text), 2);
                    drow["BeneficiaryAddress"] = Convert.ToString(workSheet.Cells[rowNumber, 5].Text);
                    drow["RemitterName"] = Convert.ToString(workSheet.Cells[rowNumber, 2].Text);
                    drow["RemitterAddress"] = Convert.ToString(workSheet.Cells[rowNumber, 3].Text);
                    drow["Purpose"] = Convert.ToString(workSheet.Cells[rowNumber, 13].Text);
                    drow["BeneficiaryContactNo"] = "";

                    bankNm = Convert.ToString(workSheet.Cells[rowNumber, 8].Text);
                    if (accountNo.Trim().Equals("") || accountNo.Trim().ToLower().Contains("coc") || accountNo.Trim().ToLower().Contains("cash"))
                    {
                        drow["PayMode"] = "CASH";
                    }
                    else if (!accountNo.Trim().Equals("") && (bankNm.ToUpper().Contains("MUTUAL") || bankNm.ToUpper().Contains("MTB")))
                    {
                        drow["PayMode"] = "MTB";
                    }
                    else
                    {
                        drow["PayMode"] = "EFT";
                    }

                    dtFile.Rows.Add(drow);
                }
            }

            return dtFile;
        }

        private DataTable ALGHURAIRData(DataTable dtFile, ExcelWorksheet workSheet)
        {
            DataRow drow;

            int endrow = workSheet.Dimension.End.Row;
            string pinno, bankNm = "", accountNo = "";

            for (var rowNumber = 2; rowNumber <= endrow; rowNumber++)
            {
                pinno = Convert.ToString(workSheet.Cells[rowNumber, 2].Text);

                if (pinno != null)
                {
                    drow = dtFile.NewRow();

                    drow["RefNo"] = Convert.ToString(workSheet.Cells[rowNumber, 2].Text);
                    drow["BeneficiaryName"] = Convert.ToString(workSheet.Cells[rowNumber, 3].Text);
                    accountNo = Convert.ToString(workSheet.Cells[rowNumber, 4].Text);
                    accountNo = (accountNo == null || accountNo.Equals("")) ? "" : accountNo;
                    drow["AccountNo"] = accountNo;
                    drow["BankName"] = Convert.ToString(workSheet.Cells[rowNumber, 5].Text);
                    drow["BranchName"] = Convert.ToString(workSheet.Cells[rowNumber, 6].Text);
                    drow["RoutingNo"] = Convert.ToString(workSheet.Cells[rowNumber, 11].Text);
                    drow["Amount"] = Math.Round(Convert.ToDouble(workSheet.Cells[rowNumber, 7].Text), 2);
                    drow["BeneficiaryAddress"] = "";
                    drow["RemitterName"] = Convert.ToString(workSheet.Cells[rowNumber, 8].Text);
                    drow["RemitterAddress"] = Convert.ToString(workSheet.Cells[rowNumber, 9].Text);
                    drow["BeneficiaryContactNo"] = Convert.ToString(workSheet.Cells[rowNumber, 10].Text);
                    drow["Purpose"] = "";

                    bankNm = Convert.ToString(workSheet.Cells[rowNumber, 5].Text);

                    if (accountNo.Trim().Equals("") || accountNo.Trim().ToLower().Contains("coc") || accountNo.Trim().ToLower().Contains("cash"))
                    {
                        drow["PayMode"] = "CASH";
                    }
                    else if (!accountNo.Trim().Equals("") && (bankNm.ToUpper().Contains("MUTUAL") || bankNm.ToUpper().Contains("MTB")))
                    {
                        drow["PayMode"] = "MTB";
                    }
                    else
                    {
                        drow["PayMode"] = "EFT";
                    }

                    dtFile.Rows.Add(drow);

                }//if

            }//for

            return dtFile;
        }

        private DataTable CreateDataTableCommon()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("RefNo");//0
            dt.Columns.Add("PayMode");//1
            dt.Columns.Add("BeneficiaryName");//2
            dt.Columns.Add("AccountNo");//3
            dt.Columns.Add("BankName");//4
            dt.Columns.Add("BranchName");//5
            dt.Columns.Add("RoutingNo");//6
            dt.Columns.Add("Amount");//7
            dt.Columns.Add("BeneficiaryAddress");//8
            dt.Columns.Add("RemitterName");//9
            dt.Columns.Add("RemitterAddress");//10
            dt.Columns.Add("Purpose");//11            
            dt.Columns.Add("BeneficiaryContactNo");//12
            return dt;
        }

        protected void btnLoadExcelFile_Click(object sender, EventArgs e)
        {
            if (FileUploadTextBoxExhFile.HasFile)
            {
                if (Path.GetExtension(FileUploadTextBoxExhFile.FileName) == ".xls")
                {
                    lblUserMsg.Text = "Please Select .xlsx  File ";
                    lblUserMsg.ForeColor = Color.Red;
                }
                else
                {
                    decimal totalAmount = 0;
                    lblUserMsg.Text = "";
                    lblTellerFileUploadMsg.Text = "";
                    tellerScreenExhFileData = new DataTable();                    

                    if (cbExh.SelectedIndex != 0)
                    {
                        string exhName = cbExh.Text.Split('-')[1].Trim();

                        ExcelPackage package = new ExcelPackage(FileUploadTextBoxExhFile.FileContent);
                        ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

                        tellerScreenExhFileData = GetExchangeDataFromExcel(workSheet, exhName);

                        dataGridViewExhDataTellerScreen.DataSource = null;
                        dataGridViewExhDataTellerScreen.DataSource = tellerScreenExhFileData;
                        dataGridViewExhDataTellerScreen.DataBind();


                        // calculate total amount
                        for (int rows = 0; rows < tellerScreenExhFileData.Rows.Count; rows++)
                        {
                            totalAmount += Convert.ToDecimal(tellerScreenExhFileData.Rows[rows]["Amount"]);
                        }
                        lblUserMsg.Text = "Total Records: " + tellerScreenExhFileData.Rows.Count + " , Amount: " + String.Format("{0:0.00}", totalAmount);
                    }
                    else
                    {
                        lblUserMsg.Text = "Please Select Exchange House...";
                        lblUserMsg.ForeColor = Color.Red;
                    }
                }
            }
        }

        private void SendMailToAuthorizer(string exhName, int recordCount, DataTable dtAuthAndSAEmailList, string userName, string userEmail, DataTable dtSummaryUploadedRecord)
        {
            MailManager mailManager = new MailManager();
            string tomail = "", ccmail = "", bccmail = "", frommail = "";

            frommail = userEmail;
            string tellerUserName = userName;

            for (int jj = 0; jj < dtAuthAndSAEmailList.Rows.Count; jj++)
            {
                if (dtAuthAndSAEmailList.Rows[jj]["FileProcessUserType"].ToString().Trim().Equals("Authorizer"))
                {
                    if (tomail.Equals(""))
                    {
                        tomail += dtAuthAndSAEmailList.Rows[jj]["UserEmail"].ToString().Trim();
                    }
                    else
                    {
                        tomail += "; " + dtAuthAndSAEmailList.Rows[jj]["UserEmail"].ToString().Trim();
                    }
                }
            }

            for (int jj = 0; jj < dtAuthAndSAEmailList.Rows.Count; jj++)
            {
                if (dtAuthAndSAEmailList.Rows[jj]["FileProcessUserType"].ToString().Trim().Equals("SuperAdmin")
                    || dtAuthAndSAEmailList.Rows[jj]["FileProcessUserType"].ToString().Trim().Equals("Admin"))
                {
                    if (ccmail.Equals(""))
                    {
                        ccmail += dtAuthAndSAEmailList.Rows[jj]["UserEmail"].ToString().Trim();
                    }
                    else
                    {
                        ccmail += "; " + dtAuthAndSAEmailList.Rows[jj]["UserEmail"].ToString().Trim();
                    }
                }
            }

            //ccmail += "; mtbremittance@mutualtrustbank.com";

            if (!ccmail.Equals(""))
            {
                ccmail += "; " + frommail;
            }
            else
            {
                ccmail += frommail;
            }

            bccmail = "";

            string subject = "Manual EFT Transaction To Authorize: Exh- " + exhName + " , Record:" + recordCount;
            string emailbody = "Dear Sir/Madam,";
            emailbody += "<br><br>Please authorize File based transactions.";
            emailbody += "<br><br>Exchange House: " + exhName + " , Total Record:" + recordCount;

            emailbody += "<br><br>";

            emailbody += "<style type=\"text/css\"> "
                + " .tg  {border-collapse:collapse;border-spacing:0;margin:0px auto;} "
                + " .tg td{border-color:black;border-style:solid;border-width:1px;font-family:Arial, sans-serif;font-size:14px; overflow:hidden;padding:10px 5px;word-break:normal;} "
                + " .tg th{border-color:black;border-style:solid;border-width:1px;font-family:Arial, sans-serif;font-size:14px; "
                + " font-weight:normal;overflow:hidden;padding:10px 5px;word-break:normal;} "
                + " .tg .tg-c3ow{border-color:inherit;text-align:center;vertical-align:top} "
                + " .tg .tg-fymr{border-color:inherit;font-weight:bold;text-align:left;vertical-align:top} "
                + " .tg .tg-7btt{border-color:inherit;font-weight:bold;text-align:center;vertical-align:top} "
                + " .tg .tg-0pky{border-color:inherit;text-align:left;vertical-align:top} "
                + " </style> "
                + " <table class=\"tg\"> "
                + " <thead> "
                + " <tr> "
                + " <th class=\"tg-fymr\"><span style=\"color:#002060\">Payment Mode</span></th> "
                + " <th class=\"tg-7btt\"><span style=\"color:#002060\">No of Txn</span></th> "
                + " <th class=\"tg-7btt\"><span style=\"color:#002060\">Total Amount</span></th> "
                + " </tr> "
                + " </thead> "
                + " <tbody> ";

            for (int rw = 0; rw < dtSummaryUploadedRecord.Rows.Count; rw++)
            {
                emailbody += " <tr> "
                    + " <td class=\"tg-0pky\">" + dtSummaryUploadedRecord.Rows[rw][0].ToString() + "</td>"
                    + " <td class=\"tg-c3ow\">" + dtSummaryUploadedRecord.Rows[rw][1].ToString() + "</td>"
                    + " <td class=\"tg-c3ow\">" + dtSummaryUploadedRecord.Rows[rw][2].ToString() + "</td>"
                    + "</tr>";
            }

            emailbody += "</tbody></table>";


            emailbody += "<br><br><br>Thanks & Regards";
            emailbody += "<br>" + tellerUserName;

            bool mailstatus = mailManager.SendMail(frommail, tomail, ccmail, bccmail, subject, emailbody);
        }

        private bool ValidateTxn(DataTable tellerScreenExhFileData)
        {
            string paymode = "", routingNo = "", accountNumber = "";
            bool isValid = true;
            listBoxError.Text = "";

            for (int ii = 0; ii < tellerScreenExhFileData.Rows.Count; ii++)
            {
                paymode = tellerScreenExhFileData.Rows[ii]["PayMode"].ToString().Trim();
                routingNo = tellerScreenExhFileData.Rows[ii]["RoutingNo"].ToString().Trim();
                accountNumber = tellerScreenExhFileData.Rows[ii]["AccountNo"].ToString().Trim();

                if (paymode.Equals("EFT"))
                {
                    if (routingNo.Length != EFT_ROUTING_NUMBER_MAX_LENGTH)
                    {
                        isValid = false; //break;
                        listBoxError.Text += "PIN -> " + tellerScreenExhFileData.Rows[ii]["RefNo"].ToString().Trim() + " - " + routingNo+" : INVALID Routing Length" + Environment.NewLine;
                    }
                    else if (mg.GetBranchNameByRoutingCode(routingNo).Rows.Count < 1)
                    {
                        isValid = false; //break;
                        listBoxError.Text += "PIN -> " + tellerScreenExhFileData.Rows[ii]["RefNo"].ToString().Trim() + " - " + routingNo + " : Routing NOT Found In Our System" + Environment.NewLine;
                    }

                    if (accountNumber.Length > EFT_ACCOUNT_NUMBER_MAX_LENGTH)
                    {
                        isValid = false; //break;
                        listBoxError.Text += "PIN -> " + tellerScreenExhFileData.Rows[ii]["RefNo"].ToString().Trim() + " - " + accountNumber + " : INVALID Account Length" + Environment.NewLine;
                    }
                }
            }
            return isValid;
        }


    }
}