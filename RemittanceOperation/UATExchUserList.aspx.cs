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
    public partial class UATExchUserList : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static ConnectionInfo connInfo = new ConnectionInfo();
        static DataTable dtExhouseUser = new DataTable();

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
                DisplayExhUserList();
            }
        }

        private void DisplayExhUserList()
        {
            DataTable dtUsers = mg.GetUATExchangeHouseUserList();

            dtExhouseUser = new DataTable();
            dtExhouseUser.Columns.Add("ID");//0
            dtExhouseUser.Columns.Add("PartyId");//1
            dtExhouseUser.Columns.Add("UserId");//2
            dtExhouseUser.Columns.Add("PlainPassword");//3
            dtExhouseUser.Columns.Add("IsActive");//4
            dtExhouseUser.Columns.Add("NRTAccount");//5
            dtExhouseUser.Columns.Add("WalletAc");//6
            dtExhouseUser.Columns.Add("PartyType");//7

            DataRow drowExhUsr;
            string encpass;

            foreach (DataRow dr in dtUsers.AsEnumerable())
            {
                drowExhUsr = dtExhouseUser.NewRow();

                drowExhUsr[0] = dr["Id"].ToString().Trim();
                drowExhUsr[1] = dr["PartyId"].ToString().Trim();
                drowExhUsr[2] = dr["UserId"].ToString().Trim();
                encpass = dr["Password"].ToString().Trim();
                drowExhUsr[3] = connInfo.getDecrypt(encpass);
                drowExhUsr[4] = dr["isActive"].ToString().Trim();
                drowExhUsr[5] = dr["AccountNo"].ToString().Trim();
                drowExhUsr[6] = dr["MobileWalletPaymentAccount"].ToString().Trim();
                drowExhUsr[7] = dr["PartyTypeId"].ToString().Trim();

                dtExhouseUser.Rows.Add(drowExhUsr);
            }

            dgridViewUserInfos.DataSource = null;
            dgridViewUserInfos.DataSource = dtExhouseUser;
            dgridViewUserInfos.DataBind();
        }

        protected void dgridViewUserInfos_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = dgridViewUserInfos.SelectedRow;

            txtIdUpd.Text = row.Cells[1].Text;
            txtPartyIdUpd.Text = row.Cells[2].Text;
            txtUserIdUpd.Text = row.Cells[3].Text;
            txtPassUpd.Text = row.Cells[4].Text;

            ddlUserActiveUpd.SelectedIndex = ddlUserActiveUpd.Items.IndexOf(ddlUserActiveUpd.Items.FindByText(row.Cells[5].Text.ToString().Trim()));
            txtNRTUpd.Text = row.Cells[6].Text;
            txtWalletUpd.Text = row.Cells[7].Text;
            ddlPartyTypeUpd.SelectedIndex = ddlPartyTypeUpd.Items.IndexOf(ddlPartyTypeUpd.Items.FindByText(row.Cells[8].Text.ToString().Trim()));

            lblUpdateSuccMsg.Text = "";
            lblSaveSuccMsg.Text = "";
        }

        protected void btnUpdateExchUserInfo_Click(object sender, EventArgs e)
        {
            int sl = Convert.ToInt32(txtIdUpd.Text);
            string pId = txtPartyIdUpd.Text;
            string uId = txtUserIdUpd.Text.Trim();
            string pass = connInfo.getEncrypt(txtPassUpd.Text.Trim());
            int uActv = Convert.ToInt32(ddlUserActiveUpd.Text);
            string nrt = txtNRTUpd.Text.Trim().Replace("-", "");
            string wallet = txtWalletUpd.Text.Trim().Replace("-", "");
            int partyType = Convert.ToInt32(ddlPartyTypeUpd.SelectedValue);
           
            try
            {
                bool stats = mg.UpdateUATExchUserInfo(sl, pId, uId, pass, uActv, nrt, wallet, partyType);
                if (stats)
                {
                    lblUpdateSuccMsg.Text = "Exchange House Updated Successfully...";
                    ClearUpdateUserControl();
                    DisplayExhUserList();
                }
                else
                {
                    lblUpdateErrorMsg.Text = "ERROR !! Exchange House Update Failed";
                    DisplayExhUserList();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void ClearUpdateUserControl()
        {
            txtIdUpd.Text = "";
            txtPartyIdUpd.Text = "";
            txtUserIdUpd.Text = "";
            txtPassUpd.Text = "";
            ddlUserActiveUpd.SelectedIndex = 0;
            txtNRTUpd.Text = "";
            txtWalletUpd.Text = "";
            ddlPartyTypeUpd.SelectedIndex = 0;
        }

        protected void btnSaveExchUserInfo_Click(object sender, EventArgs e)
        {
            string pId = txtNewPartyId.Text;
            string uId = txtNewUserId.Text.Trim();
            string pass = connInfo.getEncrypt("123456"); //default pass set as 123456
            string nrt = txtNewNRT.Text.Trim().Replace("-", "");
            string wallet = txtNewWallet.Text.Trim().Replace("-", "");
            int partyType = Convert.ToInt32(ddlPartyTypeNew.SelectedValue);

            try
            {
                if (!mg.IsUATUserIdExistsAlready(uId) && !mg.IsUATPartyIdExistsAlready(pId))
                {
                    bool stats = mg.SaveUATExchUserInfo(pId, uId, pass, nrt, wallet, partyType);
                    if (stats)
                    {
                        lblSaveSuccMsg.Text = "Exchange House  Saved Successfully...";
                        ClearNewUserControl();
                        DisplayExhUserList();
                    }
                    else
                    {
                        lblSaveErrorMsg.Text = "ERROR !! Exchange House Save Failed";
                        DisplayExhUserList();
                    }
                }
                else
                {
                    lblSaveErrorMsg.Text = "UserId/PartyId ALREADY Exists into UAT System !!!";
                }
            }
            catch (Exception ex)
            {
                lblSaveErrorMsg.Text = ex.Message;
            }
        }

        private void ClearNewUserControl()
        {
            txtNewPartyId.Text = "";
            txtNewUserId.Text = "";
            txtNewNRT.Text = "";
            txtNewWallet.Text = "";
            ddlPartyTypeNew.SelectedIndex = 0;
        }
    }
}