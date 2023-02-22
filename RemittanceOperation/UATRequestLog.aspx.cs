using ClosedXML.Excel;
using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class UATRequestLog : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable dtLog = new DataTable();

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
                LoadActiveExhUserList();
                LoadRequestType();
                lblDownloadMsg.Text = "";
                lblMessage.Text = "";
            }
        }

        private void LoadRequestType()
        {
            ddlRequestType.Items.Clear();
            ddlRequestType.Items.Add("0 - ALL");
            ddlRequestType.Items.Add("1 - Account Enquiry");
            ddlRequestType.Items.Add("2 - Payment (MTB Ac)");
            ddlRequestType.Items.Add("3 - Status Enquiry");
            ddlRequestType.Items.Add("5 - BEFTN Payment");
            ddlRequestType.Items.Add("6 - OTC Payment");
            ddlRequestType.Items.Add("10 - MobileWalletBeneficiaryValidationRequest");
            ddlRequestType.Items.Add("11 - MobileWalletBeneficiaryValidationResponse");
            ddlRequestType.Items.Add("12 - MobileWalletPayment Response");
            ddlRequestType.Items.Add("16 - PartyAccountAvailableBalance");
            ddlRequestType.Items.Add("17 - Direct Mode");

            ddlRequestType.SelectedIndex = 0;
        }

        private void LoadActiveExhUserList()
        {
            DataTable dtUsers = mg.GetUATActiveExchangeHouseUserList();
            ddlExhList.Items.Clear();
            ddlExhList.Items.Add("0 - PLEASE SELECT "); 

            for (int rows = 0; rows < dtUsers.Rows.Count; rows++)
            {
                ddlExhList.Items.Add(dtUsers.Rows[rows][0] + "");                
            }            
            ddlExhList.SelectedIndex = 0;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            if (ddlExhList.SelectedIndex != 0)
            {
                int partyId = Convert.ToInt32(ddlExhList.Text.Split('-')[0]);
                string userId = ddlExhList.Text.Split('-')[1].Trim();
                int reqTypeId = Convert.ToInt32(ddlRequestType.Text.Split('-')[0]);

                dtLog = mg.GetUATRequestLogByPartyIdReqTypeId(partyId, reqTypeId, userId);
                
                dataGridViewReqLogTxn.DataSource = null;
                dataGridViewReqLogTxn.DataSource = dtLog;
                dataGridViewReqLogTxn.DataBind();

                lblMessage.Text = "Total Rows:" + dtLog.Rows.Count;
            }
            else
            {
                lblMessage.Text = "Exchange House Selection Error !!!";
            }
        }

        protected void btnUATRequestLogDownload_Click(object sender, EventArgs e)
        {
            string headerValue = "attachment;filename=UATRequestLog.xlsx";
            if (dtLog.Rows.Count > 0)
            {
                dtLog.TableName = "UATRequestLog";
                
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtLog);

                    //Export the Excel file.
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

    }
}