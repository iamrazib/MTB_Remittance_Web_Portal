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
    public partial class APIRequestLog : System.Web.UI.Page
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
        }

        protected void btnSearchRequestLog_Click(object sender, EventArgs e)
        {
            string val = txtSearchValReqLog.Text.Trim();
            if (!val.Equals(""))
            {
                string query = "SELECT [LogId],[UserId],[RequesterLocaltion],[RequestTime],[Authenticated] Auth,[RequestCode],[ResponseCode] "
                    + " FROM [RemittanceDB].[dbo].[RequestLog] Where RequestCode like '%" + val + "%' Order By LogId desc";

                DataTable aDataTable = mg.GetAPIRequestLogByReferenceNo(val);
                
                dataGridViewRequestLogResult.DataSource = null;
                dataGridViewRequestLogResult.DataSource = aDataTable;
                dataGridViewRequestLogResult.DataBind();

                dtLog = aDataTable;
                lblRecordCount.Text = "Row Count=" + aDataTable.Rows.Count;                
            }
        }

        protected void btnDownload_Click(object sender, EventArgs e)
        {
            string headerValue = "attachment;filename=APIRequestLog.xlsx";
            if (dtLog.Rows.Count > 0)
            {
                dtLog.TableName = "APIRequestLog";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtLog);

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