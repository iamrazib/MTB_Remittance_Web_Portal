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
    public partial class ManualEFTAuthScreen : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        string fileProcessUserType = "", IsMailReceive = "", userName = "", userEmail = "";
        string loggedUserId = "", loggedUserRM = "";


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
                dtpickerFrom.Text = DateTime.Now.ToString("yyyy-MM-dd");
                dtpickerTo.Text = DateTime.Now.ToString("yyyy-MM-dd");
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

        protected void btnSearchUnAuthorizedTxn_Click(object sender, EventArgs e)
        {
            if (this.fileProcessUserType.ToLower().Equals("authorizer") || this.fileProcessUserType.ToLower().Equals("admin") || this.fileProcessUserType.ToLower().Equals("superadmin"))
            {
                listBoxAuthOutput.Text = "";


            }
        }

        protected void btnAuthFileTxn_Click(object sender, EventArgs e)
        {

        }
    }
}