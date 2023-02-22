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
    public partial class MobileWalletFreelanceTransaction : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable dtAllFreelanceData = new DataTable();

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
                dtpickerFrom.Text = DateTime.Now.ToString("yyyy-MM-dd");
                dtpickerTo.Text = DateTime.Now.ToString("yyyy-MM-dd");
                btnSearchWalletFreelanceTxn_Click(sender, e);
            }
        }

        protected void btnSearchWalletFreelanceTxn_Click(object sender, EventArgs e)
        {
            DateTime dateTime1 = DateTime.ParseExact(dtpickerFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dateTime2 = DateTime.ParseExact(dtpickerTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string dtValue1 = dateTime1.ToString("yyyy-MM-dd");
            string dtValue2 = dateTime2.ToString("yyyy-MM-dd");

            dtAllFreelanceData = new DataTable();
            dtAllFreelanceData = mg.GetWalletFreelanceData(dtValue1, dtValue2);

            dataGridViewWalletFreelanceTxn.DataSource = null;
            dataGridViewWalletFreelanceTxn.DataSource = dtAllFreelanceData;
            dataGridViewWalletFreelanceTxn.DataBind();

            lblTotalRec.Text = "Total: " + dtAllFreelanceData.Rows.Count;
        }

        protected void btnDownloadWalletFreelanceTxnAsExcel_Click(object sender, EventArgs e)
        {
            DateTime dateTime1 = DateTime.ParseExact(dtpickerFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dateTime2 = DateTime.ParseExact(dtpickerTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            string dtValue1 = dateTime1.ToString("yyyy-MM-dd");
            string dtValue2 = dateTime2.ToString("yyyy-MM-dd");

            //string fileName = "WalletFreelance_Report_AsOn_" + dtValue1 + "_to_" + dtValue2 + ".xls";

            string headerValue = "attachment;filename=WalletFreelance_Report_AsOn_" + dtValue1 + "_to_" + dtValue2 + ".xlsx";

            if (dtAllFreelanceData.Rows.Count > 0)
            {
                dtAllFreelanceData.TableName = "WalletFreelanceTxn";
                
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtAllFreelanceData);

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

                //StringWriter tw = new StringWriter();
                //HtmlTextWriter hw = new HtmlTextWriter(tw);
                //DataGrid dgGrid = new DataGrid();
                //dgGrid.DataSource = dtAllFreelanceData;
                //dgGrid.DataBind();

                //foreach (DataGridItem item in dgGrid.Items)
                //{
                //    for (int j = 0; j < item.Cells.Count; j++)
                //    {
                //        if (j == 5)
                //        {
                //            item.Cells[j].Attributes.Add("style", "mso-number-format:0\\.00");
                //        }
                //        else
                //        {
                //            item.Cells[j].Attributes.Add("style", "mso-number-format:\\@");
                //        }
                //    }
                //}

                //dgGrid.RenderControl(hw);
                //Response.ContentType = "application/vnd.ms-excel";
                //Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName + "");
                //this.EnableViewState = false;
                //Response.Write(tw.ToString());
                //Response.End();

            }
        }


    }
}