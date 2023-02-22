using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using RemittanceOperation.EzRemitServiceClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class EzRemitCashTxnPassing : System.Web.UI.Page
    {
        static IEzRemitService ezclient = new EzRemitServiceClient.EzRemitServiceClient();

        string loggedUser = "";
        static Manager mg = new Manager();
        static bool IS_INSERT_TO_LOG_TABLE = true;
        static string userId = "EzRemit";
        public const string EZRSecurityCode = "EZR@@#@#10042022";
        static string downloadBranch = "0100";
        static string downloadUser = "";
        string roleNm = "";

        string[,] userArray = new string[,] { 
            { "31135", "166" }, { "32067", "993" }, { "32714", "1206" }, { "32731", "1208" }, { "32287", "1215" }, { "32740", "1226" }, { "32781", "1271" } 
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
                loggedUser = Session[CSessionName.S_CURRENT_USER_RM].ToString();
                roleNm = Session[CSessionName.S_ROLE_NAME].ToString();
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

        private void LoadIDType()
        {
            cbIDType.Items.Clear();
            cbIDType.Items.Add("0-Select KYC Type");
            cbIDType.Items.Add("1-Passport");
            cbIDType.Items.Add("2-NationalID");
            cbIDType.Items.Add("3-DrivingLicence");
            cbIDType.Items.Add("4-TelephoneBill");

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
                    LoadIDType();
                    textBoxIDNum.Text = "";
                    textBoxMobile.Text = "";
                    textBoxAddress.Text = "";
                    lblPaymentMsg.Text = "";

                    CashTxnDetails cashDetail = ezclient.CashReceivePayment(EZRSecurityCode, txnNum);

                    if (cashDetail.ResponseCode.Equals("000"))
                    {
                        textBoxBenfName.Text = cashDetail.TransactionBeneficiary.Name == null ? "" : cashDetail.TransactionBeneficiary.Name.Trim();
                        textBoxBenfAddr.Text = cashDetail.TransactionBeneficiary.Address;
                        textBoxBenfPhone.Text = cashDetail.TransactionBeneficiary.TelephoneNumber;

                        if (cashDetail.TransactionStatus.Equals("SAVED"))
                        {
                            textBoxRemitStatus.Text = "RECEIVED";
                            textBoxRemitStatus.BackColor = Color.LightGreen;
                        }
                        else
                        {
                            textBoxRemitStatus.Text = cashDetail.TransactionStatus;
                            textBoxRemitStatus.BackColor = Color.Yellow;
                        }
                        
                        textBoxAmount.Text = Convert.ToString(Math.Round(Convert.ToDouble(cashDetail.TransactionPaymentDetails.FxAmount), 2));
                        textBoxSenderName.Text = cashDetail.SendingCustomer.CustomerName == null ? "" : cashDetail.SendingCustomer.CustomerName.Trim();
                        textBoxSenderAddr.Text = cashDetail.SendingCustomer.CustomerAddress;
                        textBoxSenderContact.Text = cashDetail.SendingCustomer.CustMobileNumber;
                        textBoxBranch.Text = "";

                        if (!cashDetail.TransactionStatus.Equals("PAID"))
                        {
                            // save into db EzRemit table
                            bool isSaved = mg.InsertCashDataIntoEzRemitDataTable(userId, cashDetail, downloadBranch, downloadUser);

                            if (isSaved)
                            {
                                if (IS_INSERT_TO_LOG_TABLE)
                                { mg.InsertAutoFetchLog(userId, "Cash Txn", txnNum + ", Data Saved into EzRemit table OK."); }
                            }
                            else
                            {
                                if (IS_INSERT_TO_LOG_TABLE)
                                { mg.InsertAutoFetchLog(userId, "Cash Txn", "EzRemit_Number -> " + txnNum + ", Error in Saving Information into Database. "); }

                                lblSearchStats.Text = "Error in Saving Information into Database";
                            }
                        }
                    }
                    else
                    {
                        string msg = "EzRemit_Number -> " + txnNum + ", ErrorCode: " + cashDetail.ResponseCode + ", Error_Message: " + cashDetail.ResponseMessage;
                        if (IS_INSERT_TO_LOG_TABLE)
                        { mg.InsertAutoFetchLog(userId, "Cash Txn", "EzRemit_Number -> " + txnNum + ", ERROR! Cash Txn Fetch >> " + msg); }

                        string msgScrn = "EzRemit_Number -> " + txnNum + "<br/>ErrorCode: " + cashDetail.ResponseCode + "<br/>Error_Message: " + cashDetail.ResponseMessage;
                        lblSearchStats.Text = msgScrn;
                    }
                }
                catch (Exception exc)
                {
                    if (IS_INSERT_TO_LOG_TABLE)
                    { mg.InsertAutoFetchLog(userId, "Cash Txn", "EzRemit_Number -> " + txnNum + ", ERROR! Ezremit client Download " + exc.ToString()); }

                    lblSearchStats.Text = "EzRemit_Number -> " + txnNum + "<br/>ERROR! Ezremit client Download ";
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
                        string beneIdType = cbIDType.SelectedItem.ToString().Split('-')[1];
                        string idNumber = textBoxIDNum.Text.Trim();
                        string mobileNum = textBoxMobile.Text.Trim();
                        string kycAddrs = textBoxAddress.Text.Trim();

                        if (!idNumber.Equals("") && !mobileNum.Equals("") && !kycAddrs.Equals(""))
                        {
                            lblSearchStats.Text = "";

                            PayoutResponse payTxnResp = ezclient.CashPayout(EZRSecurityCode, txnNum, idNumber, mobileNum, beneIdType);
                            
                            if (payTxnResp.ResponseCode.Equals("000"))
                            {
                                if (IS_INSERT_TO_LOG_TABLE)
                                { mg.InsertAutoFetchLog(userId, "Cash Txn", txnNum + ", PayTransaction to EzRemit OK."); }

                                mg.MoveEzRemitDataIntoRemitInfoTable(txnNum, idType, idNumber, mobileNum, kycAddrs, downloadUser);
                                mg.UpdateTxnStatusIntoEzRemitTable(txnNum, "C", "", downloadUser);

                                if (IS_INSERT_TO_LOG_TABLE)
                                { mg.InsertAutoFetchLog(userId, "Cash Txn", txnNum + ", Data Movement at RemitInfo Table for EzRemit OK."); }

                                lblPaymentMsg.Text = "EzRemit_Number -> " + txnNum + "  Confirm Success.";
                                lblPaymentMsg.ForeColor = Color.Green;
                                lblSearchStats.Text = "";
                            }
                            else
                            {
                                if (IS_INSERT_TO_LOG_TABLE)
                                { mg.InsertAutoFetchLog(userId, "Cash Txn", "EzRemit_Number -> " + txnNum + ", ERROR! payTxnResp = " + payTxnResp.Message); }

                                mg.UpdateTxnStatusIntoEzRemitTable(txnNum, "E", payTxnResp.Message, downloadUser);
                                lblPaymentMsg.Text = "EzRemit_Number -> " + txnNum + "<br/>" + payTxnResp.Message;
                                lblPaymentMsg.ForeColor = Color.Red;
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