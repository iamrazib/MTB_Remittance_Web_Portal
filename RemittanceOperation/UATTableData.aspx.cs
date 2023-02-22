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
    public partial class UATTableData : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable dtList = new DataTable();

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
                dTPickerFrom.Text = DateTime.Now.ToString("yyyy-MM-dd");
                dTPickerTo.Text = DateTime.Now.ToString("yyyy-MM-dd");

                LoadActiveExhUserList();
            }
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
            if (ddlExhList.SelectedIndex != 0)
            {
                DateTime dateTime1, dateTime2;

                int partyId = Convert.ToInt32(ddlExhList.Text.Split('-')[0]);
                string userId = ddlExhList.Text.Split('-')[1].Trim();
                string paymode = ddlPaymentMode.Text;
                                
                dateTime1 = DateTime.ParseExact(dTPickerFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                dateTime2 = DateTime.ParseExact(dTPickerTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                string dtValueFrom = dateTime1.ToString("yyyy-MM-dd");
                string dtValueTo = dateTime2.ToString("yyyy-MM-dd");

                dtList = mg.GetUATDataByPartyIdPaymentMode(partyId, userId, paymode, dtValueFrom, dtValueTo);

                dataGridViewUATTxn.DataSource = null;
                dataGridViewUATTxn.DataSource = dtList;
                dataGridViewUATTxn.DataBind();

                lblMessage.Text = "Total Rows:" + dtList.Rows.Count;
            }
            else
            {
                lblMessage.Text = "Exchange House Selection Error !!!";
            }
        }

        protected void btnUATTableDataDownload_Click(object sender, EventArgs e)
        {
            string userId = ddlExhList.Text.Split('-')[1].Trim();
            string paymode = ddlPaymentMode.Text;

            string headerValue = "attachment;filename=UATData_" + userId + "_" + paymode + ".xlsx";

            if (dtList.Rows.Count > 0)
            {
                dtList.TableName = paymode;

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtList);

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