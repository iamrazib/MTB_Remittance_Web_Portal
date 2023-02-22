using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using Renci.SshNet;
using System;
using System.Collections.Generic;
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
    public partial class AlipaySftpFileUpload : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        string SERVER_UPLOAD_FOLDER = "~/UploadedFiles/AlipayFiles/";

        static string USERNAME = "openvise_fluxnet_mtb";
        static string PASSWORD = "WVS0DN";
        static string HOST = "isftp.alipay.com";
        static string DESTINATION = @"/upload/mtb/";
        static int PORT = 22;
        static string PROXY_HOST = "192.168.51.61";
        static int PROXY_PORT = 80;
        static string filePath = "";

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
                dateTimePickerRptDt.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                LoadPreviousUploadFileList();
            }
        }

        private void LoadPreviousUploadFileList()
        {
            DataTable dt = mg.LoadAlipayUploadedFileList();
            dgridViewAlipayUploadedFiles.DataSource = null;
            dgridViewAlipayUploadedFiles.DataSource = dt;
            dgridViewAlipayUploadedFiles.DataBind();
        }

        protected void btnUploadFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (uploadFile.PostedFile != null && uploadFile.PostedFile.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(uploadFile.PostedFile.FileName);
                    string folder = Server.MapPath(SERVER_UPLOAD_FOLDER);
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    filePath = Path.Combine(folder, fileName);
                    uploadFile.PostedFile.SaveAs(filePath);
                    lblFileUploadStatus.Text = "Upload Success !!!";
                    lblFileUploadStatus.ForeColor = Color.Green;
                    lblFileSendStatus.Text = "";
                }

            }
            catch (Exception ex)
            {
            }
        }

        protected void btnSendFileToSpftLocation_Click(object sender, EventArgs e)
        {
            if (!filePath.Trim().Equals(""))
            {
                lblFileUploadStatus.Text = "";

                string destinationpath = DESTINATION;
                string sftphost = HOST;
                string sftpusername = USERNAME;
                string sftppassword = PASSWORD;
                int sftpport = PORT;

                string sourcefile = filePath.Trim();

                string sourceFileFullPath = Path.GetFileName(sourcefile);
                string fileType = ddlFileType.SelectedValue;
                string rptDt = DateTime.ParseExact(dateTimePickerRptDt.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                


                Renci.SshNet.ConnectionInfo infoConnection = new Renci.SshNet.ConnectionInfo(sftphost, sftpport, sftpusername, ProxyTypes.Http, PROXY_HOST, PROXY_PORT,
                    "", "", new PasswordAuthenticationMethod(sftpusername, sftppassword));

                using (SftpClient client = new SftpClient(infoConnection))
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        lblSftpConnectStatus.ForeColor = Color.Green;
                        lblSftpConnectStatus.Text = "I AM CONNECTED to Alipay";
                    }

                    client.ChangeDirectory(destinationpath);

                    using (FileStream fs = new FileStream(sourcefile, FileMode.Open))
                    {
                        /*client.BufferSize = 4 * 1024;
                        client.UploadFile(fs, Path.GetFileName(sourcefile));
                        client.Disconnect();
                        client.Dispose();*/

                        lblFileSendStatus.ForeColor = Color.Green;
                        lblFileSendStatus.Text = "File Transfer Success...";

                        mg.InsertAlipayFileInfoIntoDB(sourceFileFullPath, fileType, rptDt);
                        LoadPreviousUploadFileList();
                    }
                }
            } //if
        }//

    }
}