using ClosedXML.Excel;
using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class PaymentSuccessButCBSValueDateNull : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable dtAllData = new DataTable();

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
                dtpickerFrom.Text = DateTime.Now.ToString("yyyy-MM-dd");
                dtpickerTo.Text = DateTime.Now.ToString("yyyy-MM-dd");
                dtpickerValueDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                lblStatus.Text = "";
                LoadAPIBasedExchList();
            }
        }

        private void LoadAPIBasedExchList()
        {
            DataTable dtExchs = mg.GetAPIActiveExchList();
            cbExchWise.Items.Clear();
            cbExchWise.Items.Add("0-ALL");

            ddlParty.Items.Clear();
            ddlParty.Items.Add("--SELECT--");

            ddlPartyNRTA.Items.Clear();
            ddlPartyNRTA.Items.Add("0");

            for (int rows = 0; rows < dtExchs.Rows.Count; rows++)
            {
                if (!dtExchs.Rows[rows]["PartyId"].ToString().Equals("0"))
                {
                    cbExchWise.Items.Add(dtExchs.Rows[rows]["PartyId"] + "-" + dtExchs.Rows[rows]["ExShortName"]);

                    ddlParty.Items.Add(dtExchs.Rows[rows]["ExShortName"] + "");
                    ddlPartyNRTA.Items.Add(dtExchs.Rows[rows]["NRTAccount"] + "");
                }
            }
            cbExchWise.SelectedIndex = 0;
            ddlParty.SelectedIndex = 0;
        }

        protected void btnSearchTxn_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "";
            textBoxPinNumber.Text = "";

            DateTime dateTime1 = DateTime.ParseExact(dtpickerFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dateTime2 = DateTime.ParseExact(dtpickerTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string dtValue1 = dateTime1.ToString("yyyy-MM-dd");
            string dtValue2 = dateTime2.ToString("yyyy-MM-dd");

            int partyId = Convert.ToInt32(cbExchWise.Text.Split('-')[0]);

            dtAllData = new DataTable();
            dtAllData = mg.GetBEFTNPaymentSuccessButCBSValueDateNull(dtValue1, dtValue2, partyId);

            dataGridViewTxn.DataSource = null;
            dataGridViewTxn.DataSource = dtAllData;
            dataGridViewTxn.DataBind();

            lblTotalRec.Text = "Total: " + dtAllData.Rows.Count;
        }

        protected void btnDownloadTxnAsExcel_Click(object sender, EventArgs e)
        {
            string headerValue = "attachment;filename=BEFTNPaymentSuccessButCBSValueDateNull.xlsx";

            if (dtAllData.Rows.Count > 0)
            {
                dtAllData.TableName = "CBSValueDateEmpty";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtAllData);
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", headerValue);
                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            else
            {
                lblDownloadMsg.Text = "Nothing to Download...";
            }
        }

        protected void btnUpdateValueDate_Click(object sender, EventArgs e)
        {
            DateTime dateTime1 = DateTime.ParseExact(dtpickerValueDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            string dtValue1 = dateTime1.ToString("yyyy-MM-dd");

            if (!String.IsNullOrEmpty(textBoxPinNumber.Text.Trim()))
            {
                bool stat = mg.UpdateEmptyBEFTNValueDate(textBoxPinNumber.Text.Trim(), dtValue1);
                if (stat)
                {
                    lblStatus.Text = "Update Successful";
                    btnSearchTxn_Click(sender, e);
                }
                else
                {
                    lblStatus.Text = "";
                }
            }
        }

        protected void ddlParty_SelectedIndexChanged(object sender, EventArgs e)
        {
            int prtyIndx = ddlParty.SelectedIndex;
            ddlPartyNRTA.SelectedIndex = prtyIndx;
        }

        protected void btnRemitTranCheck_Click(object sender, EventArgs e)
        {

        }
    }
}