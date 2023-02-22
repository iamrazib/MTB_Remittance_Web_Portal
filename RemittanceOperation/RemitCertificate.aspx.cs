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
    public partial class RemitCertificate : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable dtRemitCertMergeData;

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
                dtPickerFromRemitCert.Text = DateTime.Now.ToString("yyyy-MM-dd");
                dtPickerToRemitCert.Text = DateTime.Now.ToString("yyyy-MM-dd");

                cmbRemitCertPayMode.Items.Clear();
                cmbRemitCertPayMode.Items.Add("BEFTN");
                cmbRemitCertPayMode.Items.Add("MTBAc");
            }
        }

        protected void btnSearchRemitCertificate_Click(object sender, EventArgs e)
        {
            if (!textBoxAccountNo.Text.Trim().Equals(""))
            {
                lblAccountMissingError.Text = "";
                string payMode = cmbRemitCertPayMode.Text;

                DateTime dateTime1, dateTime2;

                dateTime1 = DateTime.ParseExact(dtPickerFromRemitCert.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                dateTime2 = DateTime.ParseExact(dtPickerToRemitCert.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                string dtValueFrom = dateTime1.ToString("yyyy-MM-dd");
                string dtValueTo = dateTime2.ToString("yyyy-MM-dd");

                dtValueFrom += " 00:00:01";
                dtValueTo += " 23:59:59";

                //DataTable dtOldSys = mg.GetRemitCertificateDataFromSystem("OLD", textBoxAccountNo.Text.Trim(), dtValueFrom, dtValueTo, payMode);
                DataTable dtNewSys = mg.GetRemitCertificateDataFromSystem("NEW", textBoxAccountNo.Text.Trim(), dtValueFrom, dtValueTo, payMode);

                //int rowCntOld = dtOldSys.Rows.Count;
                int rowCntNew = dtNewSys.Rows.Count;

                DataRow drow;
                int serialNo = 0;
                dtRemitCertMergeData = CreateDataTableForRemitCertMerger();

                /*
                if (rowCntOld > 0)
                {
                    for (int rw = 0; rw < rowCntOld; rw++)
                    {
                        drow = dtRemitCertMergeData.NewRow();
                        serialNo++;

                        drow["SL"] = serialNo;// dtOldSys.Rows[rw]["SL"];
                        drow["RequestDate"] = dtOldSys.Rows[rw]["RequestDate"];
                        drow["CreditDate"] = dtOldSys.Rows[rw]["CreditDate"];
                        drow["ReferenceNo"] = dtOldSys.Rows[rw]["ReferenceNo"].ToString().Trim();
                        drow["ExchangeHouse"] = dtOldSys.Rows[rw]["ExchangeHouse"];
                        drow["SenderName"] = dtOldSys.Rows[rw]["SenderName"];
                        drow["Amount"] = dtOldSys.Rows[rw]["Amount"];
                        drow["Beneficiary"] = dtOldSys.Rows[rw]["Beneficiary"];
                        drow["AccountNo"] = dtOldSys.Rows[rw]["AccountNo"];
                        drow["Bank"] = dtOldSys.Rows[rw]["Bank"];
                        drow["Branch"] = dtOldSys.Rows[rw]["Branch"];
                        drow["RoutingNo"] = dtOldSys.Rows[rw]["RoutingNo"];

                        dtRemitCertMergeData.Rows.Add(drow);
                    }
                }
                */

                if (rowCntNew > 0)
                {
                    for (int rw = 0; rw < rowCntNew; rw++)
                    {
                        drow = dtRemitCertMergeData.NewRow();
                        serialNo++;

                        drow["SL"] = serialNo;// dtNewSys.Rows[rw]["SL"];
                        drow["RequestDate"] = dtNewSys.Rows[rw]["RequestDate"];
                        drow["CreditDate"] = dtNewSys.Rows[rw]["CreditDate"];
                        drow["ReferenceNo"] = dtNewSys.Rows[rw]["ReferenceNo"].ToString().Trim();
                        drow["ExchangeHouse"] = dtNewSys.Rows[rw]["ExchangeHouse"];
                        drow["SenderName"] = dtNewSys.Rows[rw]["SenderName"];
                        drow["Amount"] = dtNewSys.Rows[rw]["Amount"];
                        drow["Beneficiary"] = dtNewSys.Rows[rw]["Beneficiary"];
                        drow["AccountNo"] = dtNewSys.Rows[rw]["AccountNo"];
                        drow["Bank"] = dtNewSys.Rows[rw]["Bank"];
                        drow["Branch"] = dtNewSys.Rows[rw]["Branch"];
                        drow["RoutingNo"] = dtNewSys.Rows[rw]["RoutingNo"];

                        dtRemitCertMergeData.Rows.Add(drow);
                    }
                }

                dataGridViewRemitCertificate.DataSource = null;
                dataGridViewRemitCertificate.DataSource = dtRemitCertMergeData;
                dataGridViewRemitCertificate.DataBind();
            }
            else
            {
                lblAccountMissingError.Text = "Please Enter Account Number";
            }
        }

        private DataTable CreateDataTableForRemitCertMerger()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SL");
            dt.Columns.Add("RequestDate");
            dt.Columns.Add("CreditDate");
            dt.Columns.Add("ReferenceNo");
            dt.Columns.Add("ExchangeHouse");
            dt.Columns.Add("SenderName");
            dt.Columns.Add("Amount");
            dt.Columns.Add("Beneficiary");
            dt.Columns.Add("AccountNo");
            dt.Columns.Add("Bank");
            dt.Columns.Add("Branch");
            dt.Columns.Add("RoutingNo");
            return dt;
        }

        protected void btnRemitCertExcelDownload_Click(object sender, EventArgs e)
        {
            string fileName = "RemitCertificateData_" + textBoxAccountNo.Text.Trim() + ".xls";
            
            if (dtRemitCertMergeData.Rows.Count > 0)
            {
                StringWriter tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = dtRemitCertMergeData;
                dgGrid.DataBind();

                foreach (DataGridItem item in dgGrid.Items)
                {
                    for (int j = 0; j < item.Cells.Count; j++)
                    {
                        if (j == 6)
                        {
                            item.Cells[j].Attributes.Add("style", "mso-number-format:0\\.00");
                        }
                        else
                        {
                            item.Cells[j].Attributes.Add("style", "mso-number-format:\\@");
                        }
                    }
                }

                dgGrid.RenderControl(hw);
                Response.ContentType = "application/vnd.ms-excel";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName + "");
                this.EnableViewState = false;
                Response.Write(tw.ToString());
                Response.End();
            }
        }

        public override void VerifyRenderingInServerForm(Control control)
        {
            /* Verifies that the control is rendered */
        }


    }
}