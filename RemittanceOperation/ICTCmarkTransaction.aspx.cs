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
    public partial class ICTCmarkTransaction : System.Web.UI.Page
    {
        static CTCServiceClient ictcclient = new ICTCServiceClient.CTCServiceClient();
        public const string ICTCSecurityCode = "ICTC@@#@#16082021";
        static Manager mg = new Manager();
        static string userId = "InstantCash";
        static bool IS_INSERT_TO_LOG_TABLE = true;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
                //loggedUser = Session[CSessionName.S_CURRENT_USER_RM].ToString();
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                lblRejectStatus.Text = "";
                lblManualConfirmStatus.Text = "";
            }
        }

        protected void btnMarkReject_Click(object sender, EventArgs e)
        {
            string icNum = textBoxRefNo.Text.Trim();
            string rejRemarks = txtRejectRemark.Text.Trim();

            if (!icNum.Equals("") && !rejRemarks.Equals(""))
            {
                try
                {
                    ConfirmTranResponse confirmTranResp = ictcclient.ConfirmTransaction(ICTCSecurityCode, icNum, "X", rejRemarks);

                    mg.UpdateICTCConfirmDownloadAccountCreditTxnTable(icNum, "X", rejRemarks);
                    
                    if (IS_INSERT_TO_LOG_TABLE)
                    { mg.InsertAutoFetchLog(userId, "RejectTxnAtICTCEndDueToPaymentFail", "RefNo=" + icNum + ", Txn Update at DB Complete.."); }

                    lblRejectStatus.Text = "ICTC_Number -> " + icNum + "  Reject Success";
                }
                catch (Exception ex)
                {
                    if (IS_INSERT_TO_LOG_TABLE)
                    { mg.InsertAutoFetchLog(userId, "RejectTxnAtICTCEndDueToPaymentFail", "RefNo=" + icNum + ", RejectTxnAtICTCEndDueToPaymentFail Error: " + ex); }
                }
            }
        }

        protected void btnMarkConfirm_Click(object sender, EventArgs e)
        {
            string icNum = textBoxRefNo.Text.Trim();
            
            if (!icNum.Equals(""))
            {
                try
                {
                    ConfirmTranResponse confirmTranResp = new ConfirmTranResponse();

                    confirmTranResp = ictcclient.ConfirmTransaction(ICTCSecurityCode, icNum, "D", "");
                    if (confirmTranResp.Result_Flag.Equals("1") && confirmTranResp.Confirmed.Equals("true"))
                    {
                        mg.UpdateICTCConfirmDownloadAccountCreditTxnTable(icNum, "D", "");
                        lblManualConfirmStatus.Text = "ICTC_Number -> " + icNum + "  Confirm Success !!!";
                    }
                }
                catch (Exception ex)
                {
                    if (IS_INSERT_TO_LOG_TABLE)
                    { mg.InsertAutoFetchLog(userId, "ManuallyConfirmICTCtransaction", "RefNo=" + icNum + ", ManuallyConfirmICTCtransaction Error: " + ex); }
                }
            }
        }
    }
}