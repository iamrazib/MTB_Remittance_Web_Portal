using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using RemittanceOperation.ModelClass;
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
    public partial class ExHouseConfigure : System.Web.UI.Page
    {
        string roleName = "";
        string permittedRole = "ADMIN";
        static Manager mg = new Manager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
                roleName = Session[CSessionName.S_ROLE_NAME].ToString();

                if (!roleName.Equals(permittedRole))
                {
                    lblUserAuthorizationMsg.Text = "You are NOT Authorized to take any action in this screen !!!";
                    lblUserAuthorizationMsg.ForeColor = Color.Red;
                }
                else
                {
                    lblUserAuthorizationMsg.Text = "";
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                DisplayExchangeHouseList();
                DisplayCOCExchListCount();
            }
        }

        private void DisplayCOCExchListCount()
        {
            lblCountCOC.Text = mg.GetCOCExchangeHouseCount();
        }

        private void DisplayExchangeHouseList()
        {
            //DataTable dtAPIExch = mg.GetAPIBasedExchangeHouseInfoList();
            //DataTable dtFileExch = mg.GetFileBasedExchangeHouseInfoList();
            DataTable dtExchInfo = mg.GetExchangeHouseInfoList();
            dgridViewExhInfos.DataSource = null;
            dgridViewExhInfos.DataSource = dtExchInfo;
            dgridViewExhInfos.DataBind();
        }

        protected void dgridViewExhInfos_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridViewRow row = dgridViewExhInfos.SelectedRow;

          /* SL, PartyId,UserId, ExhType, ExHouseName, ExHCountry,CompanyType,NRTAccount,WalletAccount,USDAccount,AEDAccount, IsActive,ToAddress,CcAddress
           ,[ApiOrFile] ,[BEFTNBDTRate],[BKASHBDTRate],[CASHBDTRate],[ACCREDITBDTRate],[BEFTNUSDRate],[BKASHUSDRate],[CASHUSDRate],[ACCREDITUSDRate]*/

            txtSlNoUpd.Text = row.Cells[1].Text;
            txtPartyIdUpd.Text = row.Cells[2].Text;
            txtUserIdUpd.Text = row.Cells[3].Text;

            ddlExhTypeUpd.SelectedIndex = ddlExhTypeUpd.Items.IndexOf(ddlExhTypeUpd.Items.FindByText(row.Cells[4].Text));

            txtExHouseNameUpd.Text = row.Cells[5].Text;
            txtExHCountryUpd.Text = row.Cells[6].Text;
            txtCompanyTypeUpd.Text = row.Cells[7].Text;
            txtNRTAccountUpd.Text = row.Cells[8].Text;
            txtWalletAccountUpd.Text = row.Cells[9].Text;
            txtUSDAccountUpd.Text = row.Cells[10].Text;
            txtAEDAccountUpd.Text = row.Cells[11].Text;

            ddlExhActiveUpd.SelectedIndex = ddlExhActiveUpd.Items.IndexOf(ddlExhActiveUpd.Items.FindByText(row.Cells[12].Text));
            txtToAddressUpd.Text = row.Cells[13].Text;
            txtCcAddressUpd.Text = row.Cells[14].Text;
            ddlApiOrFileUpd.SelectedIndex = ddlApiOrFileUpd.Items.IndexOf(ddlApiOrFileUpd.Items.FindByText(row.Cells[15].Text));

            if (txtUSDAccountUpd.Text.Contains("&nbsp;"))
            {
                //txtUSDAccountUpd.Text.Replace("&nbsp;", "");
                txtUSDAccountUpd.Text = "";
            }
            if (txtWalletAccountUpd.Text.Contains("&nbsp;"))
            {
                //txtWalletAccountUpd.Text.Replace("&nbsp;", "");//
                txtWalletAccountUpd.Text = "";
            }
            if (txtAEDAccountUpd.Text.Contains("&nbsp;"))
            {
                //txtAEDAccountUpd.Text.Replace("&nbsp;", "");//
                txtAEDAccountUpd.Text = "";
            }
            if (txtToAddressUpd.Text.Contains("&nbsp;"))
            {
                //txtToAddressUpd.Text.Replace("&nbsp;", "");//
                txtToAddressUpd.Text = "";
            }
            if (txtCcAddressUpd.Text.Contains("&nbsp;"))
            {
                //txtCcAddressUpd.Text.Replace("&nbsp;", "");//
                txtCcAddressUpd.Text = "";
            }

            txtBEFTNBDTRateUpd.Text = row.Cells[16].Text;
            txtBKASHBDTRateUpd.Text = row.Cells[17].Text;
            txtCASHBDTRateUpd.Text = row.Cells[18].Text;
            txtACCREDITBDTRateUpd.Text = row.Cells[19].Text;

            txtBEFTNUSDRateUpd.Text = row.Cells[20].Text;
            txtBKASHUSDRateUpd.Text = row.Cells[21].Text;
            txtCASHUSDRateUpd.Text = row.Cells[22].Text;
            txtACCREDITUSDRateUpd.Text = row.Cells[23].Text;

            ddlCommCurrUpd.SelectedIndex = ddlCommCurrUpd.Items.IndexOf(ddlCommCurrUpd.Items.FindByText(row.Cells[24].Text));
            txtExhShortNameUpd.Text = row.Cells[25].Text;
            ddlHasCOCUpd.SelectedIndex = ddlHasCOCUpd.Items.IndexOf(ddlHasCOCUpd.Items.FindByText(row.Cells[26].Text));


            lblUpdateSuccMsg.Text = "";
            lblUpdateErrorMsg.Text = "";

            lblSaveSuccMsg.Text = "";
            lblSaveErrorMsg.Text = "";
        }

        protected void btnDownloadExhList_Click(object sender, EventArgs e)
        {
            DataTable dtExchInfo = mg.GetExchangeHouseInfoList();
            string fileName = "ExchangeHouseInfoList.xls";

            if (dtExchInfo.Rows.Count > 0)
            {
                StringWriter tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = dtExchInfo;
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
        }

        protected void btnUpdateExhInfo_Click(object sender, EventArgs e)
        {
            int exchangeHouseInfoId = -1;
            ExchangeHouseInfo ehi = new ExchangeHouseInfo();
            
            ehi.slNo = txtSlNoUpd.Text;
            ehi.partyId = txtPartyIdUpd.Text.Trim();
            ehi.userId = txtUserIdUpd.Text.Trim();
            ehi.exhType = ddlExhTypeUpd.SelectedValue;
            ehi.exhName = txtExHouseNameUpd.Text.Trim();
            ehi.exhCountry = txtExHCountryUpd.Text.Trim();
            ehi.companyType = txtCompanyTypeUpd.Text.Trim();
            ehi.nrtAccount = txtNRTAccountUpd.Text.Trim();
            ehi.walletAccount = txtWalletAccountUpd.Text.Trim();
            ehi.usdAccount = txtUSDAccountUpd.Text.Trim();
            ehi.aedAccount = txtAEDAccountUpd.Text.Trim();
            ehi.activeStat = ddlExhActiveUpd.SelectedValue;
            ehi.toMailAddr = txtToAddressUpd.Text.Trim();
            ehi.ccMailAddr = txtCcAddressUpd.Text.Trim();
            ehi.apiorfile = ddlApiOrFileUpd.SelectedValue;
            
            ehi.EftBdtRate = txtBEFTNBDTRateUpd.Text;
            ehi.BkashBdtRate = txtBKASHBDTRateUpd.Text;
            ehi.CashBdtRate = txtCASHBDTRateUpd.Text;
            ehi.AcBdtRate = txtACCREDITBDTRateUpd.Text;
            ehi.EftUsdRate = txtBEFTNUSDRateUpd.Text;
            ehi.BkashUsdRate = txtBKASHUSDRateUpd.Text;
            ehi.CashUsdRate = txtCASHUSDRateUpd.Text;
            ehi.AcUsdRate = txtACCREDITUSDRateUpd.Text;
            ehi.commissionCurrency = ddlCommCurrUpd.SelectedValue;
            ehi.exhShortName = txtExhShortNameUpd.Text.Trim();
            ehi.isCOC = ddlHasCOCUpd.SelectedValue;
            
            try
            {
                bool stats = mg.UpdateExchangeHouseInfo(ehi);
                if (stats)
                {
                    if (mg.IsExistThisEntryIntoExchangeHouseInfoTable(ehi.userId, ref exchangeHouseInfoId))
                    {
                        bool updateStatAtInfoTable = mg.UpdateExhInfoIntoRemittanceDbTable(ehi, exchangeHouseInfoId);
                    }

                    lblUpdateSuccMsg.Text = "Exchange House Updated Successfully...";
                    DisplayExchangeHouseList();
                    DisplayCOCExchListCount();
                    ClearUpdateInfoControl();
                }
                else
                {
                    lblUpdateErrorMsg.Text = "ERROR !! Exchange House Update Failed";
                    DisplayExchangeHouseList();
                }
            }
            catch(Exception ex)
            {
                lblUpdateErrorMsg.Text = ex.ToString();
            }

        }

        private void ClearUpdateInfoControl()
        {
            txtSlNoUpd.Text = "";
            txtPartyIdUpd.Text = "";
            txtUserIdUpd.Text = "";
            ddlExhTypeUpd.SelectedIndex = 0;
            txtExHouseNameUpd.Text = "";
            txtExHCountryUpd.Text = "";
            txtCompanyTypeUpd.Text = "";
            txtNRTAccountUpd.Text = "";
            txtWalletAccountUpd.Text = "";
            txtUSDAccountUpd.Text = "";
            txtAEDAccountUpd.Text = "";
            ddlExhActiveUpd.SelectedIndex = 0;
            txtToAddressUpd.Text = "";
            txtCcAddressUpd.Text = "";
            ddlApiOrFileUpd.SelectedIndex = 0;

            txtBEFTNBDTRateUpd.Text = "";
            txtBKASHBDTRateUpd.Text = "";
            txtCASHBDTRateUpd.Text = "";
            txtACCREDITBDTRateUpd.Text = "";
            txtBEFTNUSDRateUpd.Text = "";
            txtBKASHUSDRateUpd.Text = "";
            txtCASHUSDRateUpd.Text = "";
            txtACCREDITUSDRateUpd.Text = "";
            ddlCommCurrUpd.SelectedIndex = 0;
            txtExhShortNameUpd.Text = "";
            ddlHasCOCUpd.SelectedIndex = 0;
        }

        protected void btnSaveExhInfo_Click(object sender, EventArgs e)
        {
            ExchangeHouseInfo ehi = new ExchangeHouseInfo();

            ehi.partyId = txtPartyIdNew.Text;
            ehi.userId = txtUserIdNew.Text;
            ehi.exhType = ddlExhTypeNew.SelectedValue;
            ehi.exhName = txtExHouseNameNew.Text.Trim();
            ehi.exhCountry = txtExHCountryNew.Text.Trim();
            ehi.companyType = txtCompanyTypeNew.Text.Trim();
            ehi.nrtAccount = txtNRTAccountNew.Text.Trim();
            ehi.walletAccount = txtWalletAccountNew.Text.Trim();
            ehi.usdAccount = txtUSDAccountNew.Text.Trim();
            ehi.aedAccount = txtAEDAccountNew.Text.Trim();
            ehi.activeStat = "1";
            ehi.toMailAddr = txtToAddressNew.Text.Trim();
            ehi.ccMailAddr = txtCcAddressNew.Text.Trim();
            ehi.apiorfile = ddlApiOrFileNew.SelectedValue;

            ehi.EftBdtRate = txtBEFTNBDTRateNew.Text;
            ehi.BkashBdtRate = txtBKASHBDTRateNew.Text;
            ehi.CashBdtRate = txtCASHBDTRateNew.Text;
            ehi.AcBdtRate = txtACCREDITBDTRateNew.Text;
            ehi.EftUsdRate = txtBEFTNUSDRateNew.Text;
            ehi.BkashUsdRate = txtBKASHUSDRateNew.Text;
            ehi.CashUsdRate = txtCASHUSDRateNew.Text;
            ehi.AcUsdRate = txtACCREDITUSDRateNew.Text;
            ehi.commissionCurrency = ddlCommCurrNew.SelectedValue;
            ehi.exhShortName = txtExhShortNameNew.Text;
            ehi.isCOC = ddlHasCOCNew.SelectedValue;

            try
            {
                bool stats = mg.SaveExchangeHouseInfo(ehi);
                if (stats)
                {
                    lblSaveSuccMsg.Text = "Exchange House Saved Successfully...";
                    DisplayExchangeHouseList();
                    DisplayCOCExchListCount();
                    ClearNewInfoControl();
                }
                else
                {
                    lblSaveErrorMsg.Text = "ERROR !! Exchange House Save Failed";
                    DisplayExchangeHouseList();
                }
            }
            catch (Exception ex)
            {
                lblSaveErrorMsg.Text = ex.ToString();
            }

        }

        private void ClearNewInfoControl()
        {
            txtPartyIdNew.Text = "";
            txtUserIdNew.Text = "";
            ddlExhTypeNew.SelectedIndex = 0;
            txtExHouseNameNew.Text = "";
            txtExHCountryNew.Text = "";
            txtCompanyTypeNew.Text = "";
            txtNRTAccountNew.Text = "";
            txtWalletAccountNew.Text = "";
            txtUSDAccountNew.Text = "";
            txtAEDAccountNew.Text = "";
            txtToAddressNew.Text = "";
            txtCcAddressNew.Text = "";
            ddlApiOrFileNew.SelectedIndex = 0;
            txtBEFTNBDTRateNew.Text = "";
            txtBKASHBDTRateNew.Text = "";
            txtCASHBDTRateNew.Text = "";
            txtACCREDITBDTRateNew.Text = "";
            txtBEFTNUSDRateNew.Text = "";
            txtBKASHUSDRateNew.Text = "";
            txtCASHUSDRateNew.Text = "";
            txtACCREDITUSDRateNew.Text = "";
            ddlCommCurrNew.SelectedIndex = 0;
            txtExhShortNameNew.Text = "";
            ddlHasCOCNew.SelectedIndex = 0;
        }

    }
}