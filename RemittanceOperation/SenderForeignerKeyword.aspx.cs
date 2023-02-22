using ClosedXML.Excel;
using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class SenderForeignerKeyword : System.Web.UI.Page
    {
        static Manager mg = new Manager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

            btnForeignerSenderSearch_Click(sender, e);
            lblForeignerSenderNameSearchResult.Text = "";
            lblForeignerSenderNameSaveResult.Text = "";
            lblRemoveStatus.Text = "";


            btnForeignerSenderAccountSearch_Click(sender, e);
            lblForeignerSenderAccountSaveResult.Text = "";
            lblForeignerSenderAccountNoSearchResult.Text = "";
            lblRemoveForeignerSenderAccountStatus.Text = "";

        }

        protected void btnForeignerSenderSearch_Click(object sender, EventArgs e)
        {
            DataTable dtSenderName = mg.GetBEFTNForeignerSenderName();

            dataGridViewForeignerSenderName.DataSource = null;
            dataGridViewForeignerSenderName.DataSource = dtSenderName;
            dataGridViewForeignerSenderName.DataBind();
        }

        protected void btnSearchForeignerSenderNameIsExist_Click(object sender, EventArgs e)
        {
            if (!txtForeignerSenderNameSearchInput.Text.Trim().Equals(""))
            {
                DataTable dtRes = mg.SearchForeignerSenderNameByInput(txtForeignerSenderNameSearchInput.Text.Trim());
                if (dtRes.Rows.Count > 0)
                {
                    string outputmsg = "";
                    for (int ii = 0; ii < dtRes.Rows.Count; ii++)
                    {
                        outputmsg += "Id: " + dtRes.Rows[ii]["ID"] + ", Val: " + dtRes.Rows[ii]["SENDERNAME"];
                        outputmsg += "\n";
                    }
                    lblForeignerSenderNameSearchResult.Text = outputmsg;
                }
                else
                {
                    lblForeignerSenderNameSearchResult.Text = "NOTHING FOUND";
                }
            }
        }

        protected void btnForeignerSenderNameSave_Click(object sender, EventArgs e)
        {
            string[] lines = Regex.Split(textBoxForeignerSenderName.Text, "\n");
            string senderName = "";

            for (int i = 0; i <= lines.GetUpperBound(0); i++)
            {
                senderName = lines[i].ToString().Trim();
                if (!senderName.Equals(""))
                {
                    mg.SaveNewForeignSenderName(senderName.ToUpper());                    
                }
            }

            lblForeignerSenderNameSaveResult.Text = "Database Updated...";
            //btnForeignerSenderSearch_Click(sender, e);

            textBoxForeignerSenderName.Text = "";
        }

        protected void dataGridViewForeignerSenderName_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            e.Row.Cells[1].Attributes["width"] = "350px"; //SENDERNAME
            e.Row.Cells[2].Attributes["width"] = "150px"; //CREATEDATE
        }

        protected void btnRemoveSenderName_Click(object sender, EventArgs e)
        {
            string txtId = txtSenderId.Text.Trim();
            if (!txtId.Equals(""))
            {
                try
                {
                    int senderId = Convert.ToInt32(txtId);
                    bool stats = mg.RemoveForeignerSenderById(senderId);
                    lblRemoveStatus.Text = "Database Updated...";
                    btnForeignerSenderSearch_Click(sender, e);
                    txtSenderId.Text = "";
                }
                catch (Exception ex)
                {
                    lblRemoveStatus.Text = "ERROR in Operation...";
                }
            }
        }



        protected void btnForeignerSenderAccountSearch_Click(object sender, EventArgs e)
        {
            DataTable dtFrSenderAccount = mg.GetBEFTNForeignerSenderAccount();

            dataGridViewForeignerSenderAccount.DataSource = null;
            dataGridViewForeignerSenderAccount.DataSource = dtFrSenderAccount;
            dataGridViewForeignerSenderAccount.DataBind();
        }

        protected void btnForeignerSenderAccountSave_Click(object sender, EventArgs e)
        {
            string[] lines = Regex.Split(textBoxForeignerSenderAccount.Text, "\n");
            string senderAcc = "";

            for (int i = 0; i <= lines.GetUpperBound(0); i++)
            {
                senderAcc = lines[i].ToString().Trim();
                if (!senderAcc.Equals(""))
                {
                    mg.SaveNewForeignSenderAccount(senderAcc.ToUpper());
                }
            }

            lblForeignerSenderAccountSaveResult.Text = "Database Updated...";
            //btnForeignerSenderSearch_Click(sender, e);

            textBoxForeignerSenderAccount.Text = "";
        }

        protected void btnSearchForeignerSenderAccountIsExist_Click(object sender, EventArgs e)
        {
            if (!txtForeignerSenderAccountSearchInput.Text.Trim().Equals(""))
            {
                DataTable dtRes = mg.SearchForeignerSenderAccountByInput(txtForeignerSenderAccountSearchInput.Text.Trim());
                if (dtRes.Rows.Count > 0)
                {
                    string outputmsg = "";
                    for (int ii = 0; ii < dtRes.Rows.Count; ii++)
                    {
                        outputmsg += "Id: " + dtRes.Rows[ii]["ID"] + ", Val: " + dtRes.Rows[ii]["ACCOUNT_NO"];
                        outputmsg += "<br>";
                    }
                    lblForeignerSenderAccountNoSearchResult.Text = outputmsg;
                }
                else
                {
                    lblForeignerSenderAccountNoSearchResult.Text = "NOTHING FOUND";
                }
            }
        }

        protected void btnRemoveForeignerSenderAccountNo_Click(object sender, EventArgs e)
        {
            string txtId = txtFrgnerSenderAccId.Text.Trim();
            if (!txtId.Equals(""))
            {
                try
                {
                    int senderId = Convert.ToInt32(txtId);
                    bool stats = mg.RemoveForeignerSenderAccountById(senderId);
                    lblRemoveForeignerSenderAccountStatus.Text = "Database Updated...";
                    //btnForeignerSenderSearch_Click(sender, e);
                    txtFrgnerSenderAccId.Text = "";
                }
                catch (Exception ex)
                {
                    lblRemoveForeignerSenderAccountStatus.Text = "ERROR in Operation...";
                }
            }
        }



        protected void LinkButtonDownloadSenderNameList_Click(object sender, EventArgs e)
        {
            DataTable dtSenderNameList = mg.GetBEFTNForeignerSenderName();
            string headerValue = "attachment;filename=SenderForeignerNameList.xlsx";

            using (XLWorkbook wb = new XLWorkbook())
            {
                dtSenderNameList.TableName = "SenderForeignerName";
                wb.Worksheets.Add(dtSenderNameList);  //Add DataTable as Worksheet.
                
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

        protected void LinkButtonDownloadSenderAccountList_Click(object sender, EventArgs e)
        {
            DataTable dtFrSenderAccount = mg.GetBEFTNForeignerSenderAccount();
            string headerValue = "attachment;filename=SenderForeignerAccountList.xlsx";

            using (XLWorkbook wb = new XLWorkbook())
            {
                dtFrSenderAccount.TableName = "SenderForeignerAccount";
                wb.Worksheets.Add(dtFrSenderAccount);  //Add DataTable as Worksheet.

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
    }
}