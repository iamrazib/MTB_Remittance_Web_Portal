using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using RemittanceOperation.ICTCServiceClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class ICTCcashTxnPassing : System.Web.UI.Page
    {
        static CTCServiceClient ictcclient = new ICTCServiceClient.CTCServiceClient();
        string loggedUser = "";
        static Manager mg = new Manager();
        static bool IS_INSERT_TO_LOG_TABLE = true;
        static string userId = "InstantCash";
        public const string ICTCSecurityCode = "ICTC@@#@#16082021";
        static string downloadBranch = "0100";
        static string downloadUser = "";

        string[,] userArray = new string[,] { 
            { "31135", "166" }, { "32067", "993" }, { "32714", "1206" }, { "32731", "1208" }, { "32287", "1215" }, { "32740", "1226" }, { "32781", "1271" } 
        };


        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session[CSessionName.S_CURRENT_USER_RM] != null)
                {
                    loggedUser = Session[CSessionName.S_CURRENT_USER_RM].ToString();
                }
                else
                {
                    Response.Redirect("Login.aspx");
                }

                if (!IsPostBack)
                {
                    LoadIDType();
                    ClearAllFields();

                    lblSearchStats.Text = "";
                    lblPaymentMsg.Text = "";
                }

                for (int ii = 0; ii < userArray.GetLength(0); ii++)
                {
                    if (loggedUser.Equals(userArray[ii, 0]))
                    {
                        downloadUser = userArray[ii, 1];
                        break;
                    }
                }

                if (downloadUser.Equals(""))
                {
                    downloadUser = "1215";// default set
                }
            }
            catch(Exception ex)
            { }
        }

        private void ClearAllFields()
        {
            textBoxRefNo.Text = "";
            textBoxBenfName.Text = "";
            textBoxBenfAddr.Text = "";
            textBoxBenfPhone.Text = "";
            textBoxRemitStatus.Text = "";
            textBoxAmount.Text = "";
            textBoxSenderName.Text = "";
            textBoxSenderAddr.Text = "";
            textBoxSenderContact.Text = "";
            textBoxBranch.Text = "";
            textBoxIDNum.Text = "";
            textBoxMobile.Text = "";
            textBoxAddress.Text = "";

            cbIDType.SelectedIndex = 0;
        }

        private void LoadIDType()
        {
            cbIDType.Items.Clear();
            cbIDType.Items.Add("0-Select KYC Type");
            cbIDType.Items.Add("1-Passport");
            cbIDType.Items.Add("2-National ID");
            cbIDType.Items.Add("3-Driving Licence");
            cbIDType.Items.Add("4-Telephone Bill");
            cbIDType.SelectedIndex = 0;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string icNum = textBoxRefNo.Text.Trim();
            if (!icNum.Equals(""))
            {
                try
                {
                    ConfirmTranResponse confirmTranResp = new ConfirmTranResponse();
                    CashTxnDetails cashDetail = ictcclient.ReceivePayment(ICTCSecurityCode, icNum);

                    if (cashDetail.Result_Flag.Equals("1"))
                    {
                        textBoxBenfName.Text = cashDetail.Beneficiary_Name;
                        textBoxBenfAddr.Text = cashDetail.Beneficiary_Address;
                        textBoxBenfPhone.Text = cashDetail.Beneficiary_MobileNo;
                        textBoxRemitStatus.Text = "RECEIVED";
                        textBoxAmount.Text = cashDetail.Paying_Amount;
                        textBoxSenderName.Text = cashDetail.Remitter_Name;
                        textBoxSenderAddr.Text = cashDetail.Remitter_Address;
                        textBoxSenderContact.Text = "";
                        textBoxBranch.Text = "";

                        bool isSaved = mg.InsertIntoICDataTable(userId, cashDetail, downloadBranch, downloadUser);
                        if (isSaved)
                        {
                            if (IS_INSERT_TO_LOG_TABLE)
                            { mg.InsertAutoFetchLog(userId, "Cash Txn", icNum + ", Data Saved into ICTC table OK."); }

                            mg.UpdateConfirmDownloadAccountCreditTxnICTCTable(icNum, "D", "");

                            if (IS_INSERT_TO_LOG_TABLE)
                            { mg.InsertAutoFetchLog(userId, "Cash Txn", icNum + ", Confirm marked into ICTC table OK."); }
                        }
                        else
                        {
                            if (IS_INSERT_TO_LOG_TABLE)
                            { mg.InsertAutoFetchLog(userId, "Cash Txn", "ICTC_Number -> " + icNum + ", Error in Saving Information into Database. "); }
                            
                            lblSearchStats.Text = "Error in Saving Information into Database";

                            UnlockReceiveTxnDueToAnyIssue(icNum, "SavedDownloadError");
                        }
                    }
                    else
                    {
                        string msg = "ICTC_Number -> " + icNum + ", Error_Code: " + cashDetail.Error_Code + ", Error_Message: " + cashDetail.Error_Message + ", Error_Description: " + cashDetail.Error_Description;
                        
                        if (IS_INSERT_TO_LOG_TABLE)
                        { mg.InsertAutoFetchLog(userId, "Cash Txn", "ERROR! Cash Txn Fetch >> " + msg); }
                        
                        string msgScrn = "ICTC_Number -> " + icNum + "<br/>Error_Code: " + cashDetail.Error_Code + "<br/>ErrMessage: " + cashDetail.Error_Message + "<br/>ErrDesc: " + cashDetail.Error_Description;
                        lblSearchStats.Text = msgScrn;
                    }
                }
                catch (Exception exc)
                {
                    if (IS_INSERT_TO_LOG_TABLE)
                    { mg.InsertAutoFetchLog(userId, "Cash Txn", "ERROR! ictcclient Download " + exc.ToString()); }

                    lblSearchStats.Text = "ERROR! ictc client Download ";
                }
            }
        }

        private void UnlockReceiveTxnDueToAnyIssue(string icNum, string issue)
        {
            // if any issue happens during saving, unlock txn
            try
            {
                UnlockTranResponse unlockTxnResp = new UnlockTranResponse();
                unlockTxnResp = ictcclient.UnlockTransaction(ICTCSecurityCode, icNum);

                if (unlockTxnResp.Result_Flag.Equals("1"))
                {
                    if (IS_INSERT_TO_LOG_TABLE)
                    { mg.InsertAutoFetchLog(userId, "Unlock Txn", "ICTC_Number -> " + icNum + "  Unlock Success. Reason=" + issue); }
                }
            }
            catch (Exception exc)
            {
                if (IS_INSERT_TO_LOG_TABLE)
                { mg.InsertAutoFetchLog(userId, "Unlock Txn", "ICTC_Number -> " + icNum + "  ERROR ! Unlock Txn, Reason=" + exc.ToString()); }
            }
        }

        protected void btnPayment_Click(object sender, EventArgs e)
        {
            string icNum = textBoxRefNo.Text.Trim();
            lblSearchStats.Text = "";
            lblPaymentMsg.Text = "";
            lblUnlockMsg.Text = "";

            if (!icNum.Equals(""))
            {
                try
                {
                    if (cbIDType.SelectedIndex != 0)
                    {
                        int idType = Convert.ToInt32(cbIDType.SelectedItem.ToString().Split('-')[0]);
                        string idNumber = textBoxIDNum.Text.Trim();
                        string mobileNum = textBoxMobile.Text.Trim();
                        string kycAddrs = textBoxAddress.Text.Trim();

                        if (!idNumber.Equals("") && !mobileNum.Equals("") && !kycAddrs.Equals(""))
                        {
                            ConfirmTranResponse confirmTranResp = new ConfirmTranResponse();
                            confirmTranResp = ictcclient.ConfirmTransaction(ICTCSecurityCode, icNum, "Y", "");

                            if (confirmTranResp.Result_Flag.Equals("1") && confirmTranResp.Confirmed.Equals("true"))
                            {
                                if (confirmTranResp.Description.Equals(""))
                                {
                                    if (IS_INSERT_TO_LOG_TABLE)
                                    { mg.InsertAutoFetchLog(userId, "Cash Txn", icNum + ", Confirm Transaction to ICTC OK."); }

                                    mg.MoveICTCDataIntoRemitInfoTable(icNum, idType, idNumber, mobileNum, kycAddrs, downloadUser);
                                    mg.UpdateConfirmDownloadAccountCreditTxnICTCTable(icNum, "Y", "");

                                    if (IS_INSERT_TO_LOG_TABLE)
                                    { mg.InsertAutoFetchLog(userId, "Cash Txn", icNum + ", Data Movement at RemitInfo Table OK."); }

                                    lblPaymentMsg.Text = "ICTC_Number -> " + icNum + "  Confirm Success.";
                                    lblSearchStats.Text = "";
                                }

                                if (!confirmTranResp.Description.Equals("") && confirmTranResp.Description.ToLower().Contains("already"))
                                {
                                    if (IS_INSERT_TO_LOG_TABLE)
                                    { mg.InsertAutoFetchLog(userId, "Cash Txn", icNum + ", " + confirmTranResp.Description); }

                                    lblSearchStats.Text = "ICTC_Number -> " + icNum + "<br/>" + confirmTranResp.Description;
                                }
                            }
                            else
                            {
                                UnlockReceiveTxnDueToAnyIssue(icNum, "ConfirmError");
                            }
                        }
                        else
                        {
                            lblSearchStats.Text = "Please Provide Necessary Information and proceed !!!";
                        }
                    }
                    else
                    {
                        lblSearchStats.Text = "Please Select ID Type !!!";
                    }
                }
                catch (Exception ex) { }
            }
        }

        protected void btnUnlock_Click(object sender, EventArgs e)
        {
            string icNum = textBoxRefNo.Text.Trim();
            lblSearchStats.Text = "";
            lblPaymentMsg.Text = "";
            lblUnlockMsg.Text = "";

            if (!icNum.Equals(""))
            {
                try
                {                    
                    UnlockTranResponse unlockTxnResp = new UnlockTranResponse();
                    unlockTxnResp = ictcclient.UnlockTransaction(ICTCSecurityCode, icNum);
                    
                    if (unlockTxnResp.Result_Flag.Equals("1"))
                    {
                        if (IS_INSERT_TO_LOG_TABLE)
                        { mg.InsertAutoFetchLog(userId, "Cash Txn", icNum + ", Unlock Success "); }

                        mg.UpdateConfirmDownloadAccountCreditTxnICTCTable(icNum, "U", "Unlock Txn");

                        lblUnlockMsg.Text = "ICTC_Number -> " + icNum + "  Unlock Success";
                        ClearAllFields();
                    }
                    else
                    {
                        string msg = "ICTC_Number -> " + icNum + ", ErrCode: " + unlockTxnResp.Error_Code + ", ErrMsg: " + unlockTxnResp.Error_Message + ", Err_Desc: " + unlockTxnResp.Error_Description;
                        if (IS_INSERT_TO_LOG_TABLE)
                        { mg.InsertAutoFetchLog(userId, "Cash Txn", "ICTC_Number -> " + icNum + ", ERROR! Unlock >> " + msg); }

                        string msgScrn = "ICTC_Number -> " + icNum + "<br/>ErrCode: " + unlockTxnResp.Error_Code + "<br/>ErrMsg: " + unlockTxnResp.Error_Message + "<br/>Err_Desc: " + unlockTxnResp.Error_Description;

                        lblSearchStats.Text = msgScrn;
                        ClearAllFields();
                    }
                }
                catch (Exception exc)
                {
                    if (IS_INSERT_TO_LOG_TABLE)
                    { mg.InsertAutoFetchLog(userId, "Unlock Txn", "ICTC_Number -> " + icNum + ", ERROR! Unlock Txn " + exc.ToString()); }

                    lblUnlockMsg.Text = "ERROR! Unlock Txn ";
                    ClearAllFields();
                }
            }
        }


    }
}