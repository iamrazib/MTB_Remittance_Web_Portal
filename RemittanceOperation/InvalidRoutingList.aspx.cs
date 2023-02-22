using OfficeOpenXml;
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
    public partial class InvalidRoutingList : System.Web.UI.Page
    {
        static Manager mg = new Manager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
                //Panel1.Visible = false;

                SearchInvalidList();
                lblStatus.Text = "";
                lblBulkUpdateStats.Text = "";
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }

        private void SearchInvalidList()
        {
            DataTable dtInvalidList = mg.GetInvalidEFTRoutingList();
            dataGridViewBEFTNInvalidRoutingList.DataSource = null;
            dataGridViewBEFTNInvalidRoutingList.DataSource = dtInvalidList;
            dataGridViewBEFTNInvalidRoutingList.DataBind();
        }

        protected void btnInvalidRoutingSearchAgain_Click(object sender, EventArgs e)
        {
            SearchInvalidList();
            lblStatus.Text = "";
            lblBulkUpdateStats.Text = "";
        }

        protected void btnUpdateRoutingNo_Click(object sender, EventArgs e)
        {
            string pinNo = textBoxPinNumber.Text.Trim();
            string routingNo = textBoxNewRouting.Text.Trim();

            if (!pinNo.Equals("") && !routingNo.Equals("") && routingNo.Length == 9)
            {
                bool stat = mg.UpdateInvalidRoutingWithCorrectRoutingNumber(pinNo, routingNo);
                bool statnbl = mg.UpdateNBLInvalidRoutingWithCorrectRoutingNumber(pinNo, routingNo);

                if (stat || statnbl)
                {
                    SearchInvalidList();
                    lblStatus.Text = "Update Successfully...";
                    textBoxPinNumber.Text = "";
                    textBoxNewRouting.Text = "";
                }
            }
            else
            {
                lblStatus.Text = "Invalid PIN/Routing !!!";
            }
        }

        protected void btnInvalidRoutingDownload_Click(object sender, EventArgs e)
        {
            DataTable dtInvalidList = mg.GetInvalidEFTRoutingList();
            string fileName = "InvalidRoutingList_" + DateTime.Now.ToString("yyyyMMdd_HHmmsstt") + ".xls";

            if (dtInvalidList.Rows.Count > 0)
            {
                StringWriter tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = dtInvalidList;
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

                //DataGrid dgGrid = new DataGrid();
                //dgGrid.DataSource = dtInvalidList;
                //dgGrid.DataBind();

                //Response.Clear();
                //Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);
                //Response.Charset = "";
                //string style = @"<style> TD { mso-number-format:\@; } </style>";
                //Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                //Response.ContentType = "application/vnd.xls";

                //StringWriter stringWriter = new StringWriter();
                //HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);

                //System.Web.UI.HtmlControls.HtmlForm form = new System.Web.UI.HtmlControls.HtmlForm();
                //Controls.Add(form);
                //form.Controls.Add(dgGrid);
                //form.RenderControl(htmlWriter);
                //Response.Write(style);
                //Response.Write(stringWriter.ToString());
                //Response.End();
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Verifies that the control is rendered */
        }
               
        protected void btnUploadCorrectRoutingInfo_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile)
            {
                if (Path.GetExtension(FileUpload1.FileName) == ".xlsx" || Path.GetExtension(FileUpload1.FileName) == ".xls")
                {
                    ExcelPackage package = new ExcelPackage(FileUpload1.FileContent);
                    ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
                    DataTable table = new DataTable();

                    foreach (var firstRowCell in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
                    {
                        table.Columns.Add(firstRowCell.Text);
                    }

                    for (var rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
                    {
                        var row = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];
                        var newRow = table.NewRow();
                        foreach (var cell in row)
                        {
                            newRow[cell.Start.Column - 1] = cell.Text;
                        }
                        table.Rows.Add(newRow);
                    }

                    if (table.Rows.Count > 0)
                    {
                        object pinno = "", routingNo = "";
                        int updateSuccessCount = 0;

                        for (int rowCount = 0; rowCount < table.Rows.Count; rowCount++)
                        {
                            pinno = table.Rows[rowCount][1];
                            if (pinno != null)
                            {
                                routingNo = table.Rows[rowCount][8];
                                if (routingNo != null && routingNo.ToString().Length == 9)
                                {
                                    bool stat = mg.UpdateInvalidRoutingWithCorrectRoutingNumber(pinno.ToString().Trim(), routingNo.ToString());
                                    if (stat)
                                    {
                                        updateSuccessCount++;
                                    }
                                }
                            }
                        }

                        lblBulkUpdateStats.Text = "Update Record : " + updateSuccessCount;
                        SearchInvalidList();
                    }//if End
                }//if
            }//if

        }//method end

    }
}