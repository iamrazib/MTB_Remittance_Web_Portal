using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class BranchCashTxnDataUpload : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable branchCashTxnData = new DataTable();
        string FOLDER_PATH = ConfigurationManager.AppSettings["FolderPathBranchCashFiles"]; 

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
                DateTime thisMonthFirstDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                dtpickerFrom.Text = thisMonthFirstDay.ToString("yyyy-MM-dd");
                dtpickerTo.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

                lblFileUploadMsg.Text = "";
            }
        }

        protected void btnUploadFile_Click(object sender, EventArgs e)
        {
            if (FileUploadBranchCashFile.HasFile)
            {
                //string fileFolderPath = Server.MapPath("~/UploadedFiles/BranchCashTxnFiles/");
                string fileFolderPath = Server.MapPath(FOLDER_PATH);
                Utility.DeleteOldDaysFiles(fileFolderPath);
                FileUploadBranchCashFile.SaveAs(fileFolderPath + "\\" + FileUploadBranchCashFile.FileName.Split('\\')[FileUploadBranchCashFile.FileName.Split('\\').Length - 1]);

                string[] shtnm = Utility.GetExcelSheetNames(fileFolderPath + "\\" + FileUploadBranchCashFile.FileName);
                string sheetNm = shtnm[0];

                DataTable dt = Utility.GetExcelDataFromFirstSheet(fileFolderPath, FileUploadBranchCashFile.FileName, sheetNm);

                DataTable dtCashFile = CreateDataTable();
                DataRow drow;
                string refNo;

                for (int row = 4; row < dt.Rows.Count; row++)
                {
                    refNo = dt.Rows[row][0].ToString();
                   
                    if(!String.IsNullOrEmpty(refNo))
                    {
                        drow = dtCashFile.NewRow();
                        drow["RefNo"] = Convert.ToString(dt.Rows[row][0]);
                        drow["RemitterName"] = Convert.ToString(dt.Rows[row][1]);
                        drow["RemitterDistrict"] = Convert.ToString(dt.Rows[row][2]);
                        drow["SendingCountry"] = Convert.ToString(dt.Rows[row][4]);
                        drow["Amount"] = Convert.ToString(dt.Rows[row][5]);
                        drow["BeneficiaryName"] = Convert.ToString(dt.Rows[row][6]);
                        drow["ExchangeHouseName"] = Convert.ToString(dt.Rows[row][8]);
                        drow["PaymentDate"] = Convert.ToString(dt.Rows[row][9]);
                        drow["BranchCode"] = Convert.ToInt32(dt.Rows[row][11]);
                        drow["BranchName"] = Convert.ToString(dt.Rows[row][12]);
                        drow["BeneficiaryNID"] = Convert.ToString(dt.Rows[row][14]);
                        drow["BeneficiaryMobile"] = Convert.ToString(dt.Rows[row][16]);
                        drow["PaymentMode"] = Convert.ToString(dt.Rows[row][18]);
                        drow["ReportFromDate"] = dtpickerFrom.Text;
                        drow["ReportToDate"] = dtpickerTo.Text;

                        dtCashFile.Rows.Add(drow);
                    }
                }

                branchCashTxnData = dtCashFile;
                dataGridViewBranchCashData.DataSource = null;
                dataGridViewBranchCashData.DataSource = branchCashTxnData;
                dataGridViewBranchCashData.DataBind();
                lblTotalRecords.Text = branchCashTxnData.Rows.Count + "";

                if (branchCashTxnData.Rows.Count > 0)
                {
                    lblFileUploadMsg.Text = "File Uploaded Successfully...";
                }
            }
        }
                
        private DataTable CreateDataTable()
        {
            DataTable dtTemp = new DataTable();
            dtTemp.Columns.Add("RefNo");
            dtTemp.Columns.Add("RemitterName");
            dtTemp.Columns.Add("RemitterDistrict");
            dtTemp.Columns.Add("SendingCountry");
            dtTemp.Columns.Add("Amount");
            dtTemp.Columns.Add("BeneficiaryName");
            dtTemp.Columns.Add("ExchangeHouseName");
            dtTemp.Columns.Add("PaymentDate");
            dtTemp.Columns.Add("BranchCode");
            dtTemp.Columns.Add("BranchName");
            dtTemp.Columns.Add("BeneficiaryNID");
            dtTemp.Columns.Add("BeneficiaryMobile");
            dtTemp.Columns.Add("PaymentMode");
            dtTemp.Columns.Add("ReportFromDate");
            dtTemp.Columns.Add("ReportToDate");
            return dtTemp;
        }

        protected void btnDataUploadSystem_Click(object sender, EventArgs e)
        {
            if (branchCashTxnData.Rows.Count > 0)
            {
                lblFileUploadMsg.Text = "";

                DateTime dateTime1 = DateTime.ParseExact(dtpickerFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime dateTime2 = DateTime.ParseExact(dtpickerTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                string fromdt = dateTime1.ToString("yyyy-MM-dd");
                string todt = dateTime2.ToString("yyyy-MM-dd");

                mg.Delete_CashTxnPanBankReport_TableDataByDate(fromdt, todt);
                
                int CashTxnSaveCount = 0;                
                bool saveStatus = mg.SaveBranchCashTxnFileIntoDB(branchCashTxnData, ref CashTxnSaveCount);

                lblDataCountSaveIntoDB.Text = "Input File Count: " + branchCashTxnData.Rows.Count + " , Save Count Into DB: " + CashTxnSaveCount;
            }
            else
            {
                lblFileUploadMsg.Text = "NO DATA TO UPLOAD INTO SYSTEM !!!!";
            }
        }

        

        protected void dataGridViewBranchCashData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            if (e.NewPageIndex == -1)
                dataGridViewBranchCashData.PageIndex = Int32.MaxValue;
            else
                dataGridViewBranchCashData.PageIndex = e.NewPageIndex;
        }

        protected void dataGridViewBranchCashData_PageIndexChanged(object sender, EventArgs e)
        {
            dataGridViewBranchCashData.DataSource = branchCashTxnData;
            dataGridViewBranchCashData.DataBind();
        }

    }
}