//using RemittanceOperation.AppCode.CSessionName;
using RemittanceOperation.DBUtility;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class AlipayStatementUpload : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable bkashTxnData = new DataTable();
        string FOLDER_PATH = ConfigurationManager.AppSettings["FolderPathAlipayFiles"];

        string DestinationPath = ConfigurationManager.AppSettings["AlipayDestinationPath"];
        string SftpHostAddr = ConfigurationManager.AppSettings["AlipayHostAddr"];
        string SftpUsername = ConfigurationManager.AppSettings["AlipaySftpUsername"];
        string SftpPassword = ConfigurationManager.AppSettings["AlipaySftpPassword"];
        int SftpPort = Convert.ToInt32(ConfigurationManager.AppSettings["AlipaySftpPort"]);

        string PROXY_HOST = ConfigurationManager.AppSettings["PROXY_HOST"];
        int PROXY_PORT = Convert.ToInt32(ConfigurationManager.AppSettings["PROXY_PORT"]);

        static string filePath = "";


        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[RemittanceOperation.AppCode.CSessionName.S_CURRENT_USER_RM] != null)
            {
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                dTPickerFrom.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                LoadPreviousUploadFileList();
            }
        }

        private void LoadPreviousUploadFileList()
        {
            DataTable dt = mg.GetAlipayPreviousUploadFileList();
            dataGridViewAlipayUploadedFiles.DataSource = null;
            dataGridViewAlipayUploadedFiles.DataSource = dt;
            dataGridViewAlipayUploadedFiles.DataBind();
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (FileUpload1.PostedFile != null && FileUpload1.PostedFile.ContentLength > 0)
            {
                string fileName = Path.GetFileName(FileUpload1.PostedFile.FileName);
                string fileFolderPath = Server.MapPath(FOLDER_PATH);
                Utility.DeleteOldDaysFiles(fileFolderPath);
                
                string fileTyp = ddlFileType.Text;
                string rptDt = dTPickerFrom.Text;

                filePath = Path.Combine(fileFolderPath, fileName);
                FileUpload1.PostedFile.SaveAs(filePath);

                UploadSFTPLocation(filePath, fileTyp, rptDt);
                LoadPreviousUploadFileList();
            }
        }

        private void UploadSFTPLocation(string filePath, string fileType, string rptDt)
        {
            try
            {
                ConnectionInfo infoConnection = new ConnectionInfo(SftpHostAddr, SftpPort, SftpUsername, ProxyTypes.Http, PROXY_HOST, PROXY_PORT,
                    "", "", new PasswordAuthenticationMethod(SftpUsername, SftpPassword));

                using (SftpClient client = new SftpClient(infoConnection))
                {
                    try
                    {
                        client.Connect();
                        if (client.IsConnected)
                        {
                            lblFileSendStatus.ForeColor = Color.Green;
                            lblFileSendStatus.Text = "I AM CONNECTED";
                        }

                        client.ChangeDirectory(DestinationPath);

                        using (FileStream fs = new FileStream(filePath, FileMode.Open))
                        {
                            client.BufferSize = 4 * 1024;
                            client.UploadFile(fs, Path.GetFileName(filePath));
                            client.Disconnect();
                            client.Dispose();

                            lblFileSendStatus.ForeColor = Color.Green;
                            lblFileSendStatus.Text = "File Transfer Success...";
                            
                            InsertIntoDB(Path.GetFileName(filePath), fileType, rptDt);
                        }
                    }
                    catch (Exception exc)
                    {
                        lblFileSendStatus.ForeColor = Color.Red;
                        lblFileSendStatus.Text = "Connection Error !!!" + exc;
                    }
                }
            }
            catch (Exception excp)
            { }
        }

        private void InsertIntoDB(string fileName, string fileType, string rptDt)
        {
            DateTime dateTime1 = DateTime.ParseExact(rptDt, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            mg.InsertALipayFileUploadInfoIntoLogTable(fileName, fileType, dateTime1);
        }
    }
}