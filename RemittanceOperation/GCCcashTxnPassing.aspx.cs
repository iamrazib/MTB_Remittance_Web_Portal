using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using RemittanceOperation.GCCServiceClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class GCCcashTxnPassing : System.Web.UI.Page
    {
        static IGCCService gcclient = new GCCServiceClient.GCCServiceClient();

        string loggedUser = "";
        static Manager mg = new Manager();
        static bool IS_INSERT_TO_LOG_TABLE = true;
        static string userId = "GCCRemit";
        public const string GCCSecurityCode = "GCC@@#@#10082021";
        static string downloadBranch = "0100";
        static string downloadUser = "";

        string[,] userArray = new string[,] { 
            { "31135", "166" }, { "32067", "993" }, { "32714", "1206" }, { "32731", "1208" }, { "32287", "1215" }, { "32740", "1226" }, { "32781", "1271" } 
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
                loggedUser = Session[CSessionName.S_CURRENT_USER_RM].ToString();
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

            if(!IsPostBack)
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

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string txnNum = textBoxRefNo.Text.Trim();
            if (!txnNum.Equals(""))
            {
                try
                {                    
                    CashTxnResponse cashDetail = gcclient.DisplayTransaction(GCCSecurityCode, txnNum);
                    if (cashDetail.ResponseCode.Equals("001"))
                    {
                        textBoxBenfName.Text = cashDetail.ReceiverFirstName.Trim() + " " + cashDetail.ReceiverMiddleName.Trim() + " " + cashDetail.ReceiverLastName.Trim() + " " + cashDetail.ReceiverFourthName.Trim();
                        textBoxBenfName.Text = Regex.Replace(textBoxBenfName.Text, @"\s+", " ");
                        textBoxBenfAddr.Text = cashDetail.ReceiverAddress;
                        textBoxBenfPhone.Text = cashDetail.ReceiverContactNo;
                        textBoxRemitStatus.Text = "RECEIVED";
                        textBoxAmount.Text = Convert.ToString(Math.Round(Convert.ToDouble(cashDetail.AmountToPay), 2));
                        textBoxSenderName.Text = cashDetail.SenderFirstName.Trim() + " " + cashDetail.SenderMiddleName.Trim() + " " + cashDetail.SenderLastName.Trim() + " " + cashDetail.SenderFourthName.Trim();
                        textBoxSenderName.Text = Regex.Replace(textBoxSenderName.Text, @"\s+", " ");
                        textBoxSenderAddr.Text = cashDetail.SenderAddress;
                        textBoxSenderContact.Text = cashDetail.SenderContactNo;
                        textBoxBranch.Text = "";

                        // save into db gcc table
                        bool isSaved = mg.InsertIntoGCCDataTable(userId, cashDetail, downloadBranch, downloadUser);

                        if (isSaved)
                        {
                            if (IS_INSERT_TO_LOG_TABLE)
                            { mg.InsertAutoFetchLog(userId, "Cash Txn", txnNum + ", Data Saved into GCC table OK."); }
                        }
                        else
                        {
                            if (IS_INSERT_TO_LOG_TABLE)
                            { mg.InsertAutoFetchLog(userId, "Cash Txn", "GCC_Number -> " + txnNum + ", Error in Saving Information into Database. "); }
                            
                            lblSearchStats.Text = "Error in Saving Information into Database";
                        }
                    }
                    else
                    {
                        string msg = "GCC_Number -> " + txnNum + ", ErrorCode: " + cashDetail.ResponseCode + ", Error_Message: " + cashDetail.ResponseMessage;
                        if (IS_INSERT_TO_LOG_TABLE)
                        { mg.InsertAutoFetchLog(userId, "Cash Txn", "GCC_Number -> " + txnNum + ", ERROR! Cash Txn Fetch >> " + msg); }

                        string msgScrn = "GCC_Number -> " + txnNum + "<br/>ErrorCode: " + cashDetail.ResponseCode + "<br/>Error_Message: " + cashDetail.ResponseMessage;
                        lblSearchStats.Text = msgScrn;
                    }
                }
                catch (Exception exc)
                {
                    if (IS_INSERT_TO_LOG_TABLE)
                    { mg.InsertAutoFetchLog(userId, "Cash Txn", "GCC_Number -> " + txnNum + ", ERROR! gcc client Download " + exc.ToString()); }

                    lblSearchStats.Text = "GCC_Number -> " + txnNum + "<br/>ERROR! gcc client Download ";
                }
            }
        }

        protected void btnPayment_Click(object sender, EventArgs e)
        {
            string txnNum = textBoxRefNo.Text.Trim();
            if (!txnNum.Equals(""))
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
                            lblSearchStats.Text = "";
                            PayTranResponses payTxnResp = gcclient.PayTransaction(GCCSecurityCode, txnNum, idType.ToString(), idNumber);

                            if (payTxnResp.ResponseCode.Equals("001"))
                            {
                                if (IS_INSERT_TO_LOG_TABLE)
                                { mg.InsertAutoFetchLog(userId, "Cash Txn", txnNum + ", PayTransaction to GCC OK."); }

                                mg.MoveGCCDataIntoRemitInfoTable(txnNum, idType, idNumber, mobileNum, kycAddrs, downloadUser);
                                mg.UpdateTxnStatusIntoGCCTable(txnNum, "C", "", downloadUser);

                                if (IS_INSERT_TO_LOG_TABLE)
                                { mg.InsertAutoFetchLog(userId, "Cash Txn", txnNum + ", Data Movement at RemitInfo Table for GCC OK."); }

                                lblPaymentMsg.Text = "GCC_Number -> " + txnNum + "  Confirm Success.";
                                lblSearchStats.Text = "";
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
    }
}