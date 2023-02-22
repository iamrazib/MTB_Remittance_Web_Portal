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
    public partial class RoutingNumberUpdate : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable dtFileRows = CreateDataTable();
        static DataTable dtExistingRouting = new DataTable();
        static DataTable dtDifferRecords = CreateDataTable();


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
                LoadExistingRoutingNumbers();
                lblNewRoutingInsertStatus.Text = "";
                lblNewRoutingFileStats.Text = "";
                btnInsertNewRoutingNo.Visible = false;
            }
        }

        private void LoadExistingRoutingNumbers()
        {
            // SL, [BANK CODE], BANK, [BRANCH NAME], DISTRICT, [ROUTING NUMBER], ISACTIVEFORBEFTNAP
            dtExistingRouting = mg.GetExistingRoutingNumberList();
            dgridViewExistRoutingInfos.DataSource = null;
            dgridViewExistRoutingInfos.DataSource = dtExistingRouting;
            dgridViewExistRoutingInfos.DataBind();

            lblExistTotalRouting.Text = "Total Records:" + dtExistingRouting.Rows.Count;
        }

        protected void btnUploadNewRoutingFile_Click(object sender, EventArgs e)
        {
            if (FileUploadNewRoutingFile.HasFile)
            {
                if (Path.GetExtension(FileUploadNewRoutingFile.FileName) == ".xlsx" || Path.GetExtension(FileUploadNewRoutingFile.FileName) == ".xls")
                {
                    ExcelPackage package = new ExcelPackage(FileUploadNewRoutingFile.FileContent);
                    ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
                    DataTable table = new DataTable();                    
                    DataRow drow;
                    dtFileRows = CreateDataTable();

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
                        object bankcode = "", bankNm = "", distNm = "", brNm = "", routingCd = "";

                        for (int rowCount = 0; rowCount < table.Rows.Count; rowCount++)
                        {
                            bankcode = table.Rows[rowCount][0];
                            bankNm = table.Rows[rowCount][1];
                            distNm = table.Rows[rowCount][3];
                            brNm = table.Rows[rowCount][5];
                            routingCd = table.Rows[rowCount][6];

                            drow = dtFileRows.NewRow();

                            if (bankcode.ToString().Length < 3)
                            {
                                drow["BankCode"] = "0" + bankcode.ToString();
                            }
                            else
                            {
                                drow["BankCode"] = bankcode;
                            }
                                                        
                            drow["BankName"] = bankNm.ToString().Trim();
                            drow["BranchName"] = brNm.ToString().Trim();
                            drow["District"] = distNm.ToString().Trim();

                            if (routingCd.ToString().Length < 9)
                            {
                                drow["RoutingNo"] = "0" + routingCd.ToString();
                            }
                            else
                            {
                                drow["RoutingNo"] = routingCd.ToString().Trim();
                            }
                            
                            dtFileRows.Rows.Add(drow);
                        }//for end


                        FindDifferenceAndPopulateInGridView();
                        lblNewRoutingFileStats.Text = "File Upload Success...";

                    }// if (table.Rows.Count > 0)

                }//2nd if

            }//1st if
        }

        private void FindDifferenceAndPopulateInGridView()
        {
            dtDifferRecords = CreateDataTable();
            DataRow drow;
            object routingCd;
            bool matchFound = false;
            int ii = 0;

            for (int rowCount = 0; rowCount < dtFileRows.Rows.Count; rowCount++)
            {
                routingCd = dtFileRows.Rows[rowCount][4];
                matchFound = false;
                ii = 0;

                for (ii = 0; ii < dtExistingRouting.Rows.Count; ii++)
                {
                    if (dtExistingRouting.Rows[ii][5].ToString().Equals(routingCd.ToString()))
                    {
                        matchFound = true;
                        break;                        
                    }
                }

                if (!matchFound)
                {
                    drow = dtDifferRecords.NewRow();
                    drow["BankCode"] = dtFileRows.Rows[rowCount][0];
                    drow["BankName"] = dtFileRows.Rows[rowCount][1];
                    drow["BranchName"] = dtFileRows.Rows[rowCount][2];
                    drow["District"] = dtFileRows.Rows[rowCount][3];
                    drow["RoutingNo"] = dtFileRows.Rows[rowCount][4];
                    dtDifferRecords.Rows.Add(drow);
                }
            }

            if (dtDifferRecords.Rows.Count > 0)
            {
                dgridViewNotExistRoutingInfos.DataSource = null;
                dgridViewNotExistRoutingInfos.DataSource = dtDifferRecords;
                dgridViewNotExistRoutingInfos.DataBind();
                lblNotExistsRoutingNumbers.Text = "Not Exists Routing Numbers :: " + dtDifferRecords.Rows.Count;
                btnInsertNewRoutingNo.Visible = true;
            }
        }

        private static DataTable CreateDataTable()
        {
            DataTable dtTemp = new DataTable();
            dtTemp.Columns.Add("BankCode");
            dtTemp.Columns.Add("BankName");
            dtTemp.Columns.Add("BranchName");
            dtTemp.Columns.Add("District");
            dtTemp.Columns.Add("RoutingNo");
            return dtTemp;
        }

        protected void dgridViewExistRoutingInfos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            dgridViewExistRoutingInfos.PageIndex = e.NewPageIndex;
            LoadExistingRoutingNumbers();  
        }

        protected void btnInsertNewRoutingNo_Click(object sender, EventArgs e)
        {
            if (dtDifferRecords.Rows.Count > 0)
            {
                int lastSlNo = 0, slNo, saveCount = 0;
                string bankCode, bankName, brName, districtName;

                for (int rowCount = 0; rowCount < dtDifferRecords.Rows.Count; rowCount++)
                {
                    string rtNum = dtDifferRecords.Rows[rowCount]["RoutingNo"].ToString().Trim();

                    if (mg.IsRoutingNumberAlreadyExists(rtNum))
                    { }
                    else
                    {
                        lastSlNo = mg.GetLastRecordNumber();
                        if (lastSlNo != 0)
                        {
                            slNo = lastSlNo + 1;
                            bankCode = dtDifferRecords.Rows[rowCount]["BankCode"].ToString();
                            bankName = dtDifferRecords.Rows[rowCount]["BankName"].ToString().ToUpper();
                            brName = dtDifferRecords.Rows[rowCount]["BranchName"].ToString().ToUpper();
                            districtName = dtDifferRecords.Rows[rowCount]["District"].ToString().Trim().ToUpper();
                            
                            bool status = mg.SaveRoutingInfo(slNo, bankCode, bankName, brName, districtName, rtNum);
                            if (status)
                            {
                                saveCount++;
                            }
                        }
                    }
                }

                lblNewRoutingInsertStatus.Text = "Inserted " + saveCount + " Records";
            }
        }

        protected void LinkButtonRefreshExistingList_Click(object sender, EventArgs e)
        {
            LoadExistingRoutingNumbers();
        }


    }
}