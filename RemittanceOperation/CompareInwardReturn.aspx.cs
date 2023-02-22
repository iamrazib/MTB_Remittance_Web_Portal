using OfficeOpenXml;
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
    public partial class CompareInwardReturn : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnUploadEFTReturnFile_Click(object sender, EventArgs e)
        {
            DateTime dateTime1 = DateTime.ParseExact(dTPickerReturnDateCompare.Text, "dd-MMM-yyyy", CultureInfo.InvariantCulture);
            string dtValueFrom = dateTime1.ToString("yyyy-MM-dd");

            string dtCurrDay = DateTime.Now.ToString("yyyy-MM-dd");
            if (dtValueFrom.Equals(dtCurrDay))
            {
                lblErrorMsg.Text = "Please Select Correct RETURN Date !!!";
            }
            else
            {
                lblErrorMsg.Text = "";

                DataRow drow;
                DataTable dtFileInwardReturn = CreateDataTableEFTReturn();

                if (FileUpload1.HasFile)
                {
                    if (Path.GetExtension(FileUpload1.FileName) == ".xlsx" || Path.GetExtension(FileUpload1.FileName) == ".xls")
                    {
                        ExcelPackage package = new ExcelPackage(FileUpload1.FileContent);
                        ExcelWorksheet workSheet = package.Workbook.Worksheets.First();
                        DataTable table = new DataTable();


                    }
                }

            }



            
        }

        private DataTable CreateDataTableEFTReturn()
        {
            DataTable dt = new DataTable();
            //dt.Columns.Add("Sl");//0
            dt.Columns.Add("SettDate");//1
            dt.Columns.Add("BankFrom");//2
            dt.Columns.Add("ReturnReason");//3
            dt.Columns.Add("ExhAccount");//4
            dt.Columns.Add("ExhName");//5
            dt.Columns.Add("ReceiverAccountNo");//6
            dt.Columns.Add("ReceiverName");//7
            dt.Columns.Add("Amount");//8
            dt.Columns.Add("DrCr");//9
            return dt;
        }
    }
}