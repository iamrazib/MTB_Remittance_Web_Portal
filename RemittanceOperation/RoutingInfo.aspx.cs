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
    public partial class RoutingInfo : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        //int routingEditId = -1;
        int routingNoValue;

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
                LoadBankInfo();
                lblMessage.Text = "";
                lblUpdateSuccMsg.Text = "";
                lblUpdateErrorMsg.Text = "";
                lblSaveMsg.Text = "";
                
                LoadBankInfoAddRouting();
                LoadDistrictInfoAddRouting();

                LoadBankInfoUpdateRouting();
                LoadDistrictInfoUpdateRouting();
            }
        }        

        private void LoadBankInfoUpdateRouting()
        {
            cbBankNameUpdateRouting.Items.Clear();
            DataTable BdAllBanklist = mg.GetBdAllBanklist();
            cbBankNameUpdateRouting.Items.Add("---- Select ----");

            for (int rw = 0; rw < BdAllBanklist.Rows.Count; rw++)
            {
                cbBankNameUpdateRouting.Items.Add(BdAllBanklist.Rows[rw][0].ToString() + "=" + BdAllBanklist.Rows[rw][1].ToString());
            }
            cbBankNameUpdateRouting.SelectedIndex = 0;
        }

        private void LoadDistrictInfoUpdateRouting()
        {
            cbDistrictNameUpdateRouting.Items.Clear();
            DataTable BdAllDistrictlist = mg.GetBdAllDistrictlist();
            cbDistrictNameUpdateRouting.Items.Add("---- Select ----");

            for (int rw = 0; rw < BdAllDistrictlist.Rows.Count; rw++)
            {
                cbDistrictNameUpdateRouting.Items.Add(BdAllDistrictlist.Rows[rw][0].ToString());
            }
            cbDistrictNameUpdateRouting.SelectedIndex = 0;
        }

        private void LoadBankInfoAddRouting()
        {
            cbBankNameNewRouting.Items.Clear();
            DataTable BdAllBanklist = mg.GetBdAllBanklist();
            cbBankNameNewRouting.Items.Add("---- Select ----");
            cbBankCodeNewRouting.Items.Clear();
            cbBankCodeNewRouting.Items.Add("--Select--");

            for (int rw = 0; rw < BdAllBanklist.Rows.Count; rw++)
            {
                cbBankNameNewRouting.Items.Add(BdAllBanklist.Rows[rw][1].ToString());
                cbBankCodeNewRouting.Items.Add(BdAllBanklist.Rows[rw][0].ToString());
            }
            cbBankNameNewRouting.SelectedIndex = 0;
            cbBankCodeNewRouting.SelectedIndex = 0;
        }

        private void LoadDistrictInfoAddRouting()
        {
            cbDistrictNameNewRouting.Items.Clear();
            DataTable BdAllDistrictlist = mg.GetBdAllDistrictlist();
            cbDistrictNameNewRouting.Items.Add("---- Select ----");

            for (int rw = 0; rw < BdAllDistrictlist.Rows.Count; rw++)
            {
                cbDistrictNameNewRouting.Items.Add(BdAllDistrictlist.Rows[rw][0].ToString());
            }
            cbDistrictNameNewRouting.SelectedIndex = 0;
        }

        private void LoadBankInfo()
        {
            cbBankNameRoutingInfo.Items.Clear();
            DataTable BdAllBanklist = mg.GetBdAllBanklist();
            cbBankNameRoutingInfo.Items.Add("---- Select ----");

            cbBankCode.Items.Clear();
            cbBankCode.Items.Add("--Select--");

            for (int rw = 0; rw < BdAllBanklist.Rows.Count; rw++)
            {
                cbBankNameRoutingInfo.Items.Add(BdAllBanklist.Rows[rw][1].ToString());
                cbBankCode.Items.Add(BdAllBanklist.Rows[rw][0].ToString());
            }

            cbBankNameRoutingInfo.SelectedIndex = 0;
            cbBankCode.SelectedIndex = 0;
        }

        protected void cbBankNameRoutingInfo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string bankCode = "";
            cbBankCode.SelectedIndex = cbBankNameRoutingInfo.SelectedIndex;
            cbBranchNameRoutingInfo.Items.Clear();

            if (cbBankNameRoutingInfo.SelectedIndex != 0)
            {
                bankCode = cbBankCode.SelectedItem.ToString().Trim();
                LoadDistrictInfoByBankCode(bankCode);
                lblMessage.Text = "";
            }
            else
            {
                cbDistrictNameRoutingInfo.Items.Clear();
            }

            ClearAndRestoreFields();
        }

        private void ClearAndRestoreFields()
        {
            txtAutoId.Text = "";
            //txtBankName.Text = "";
            txtBranchName.Text = "";
            //txtDistrictName.Text = "";
            txtRoutingNoEdit.Text = "";
            lblUpdateSuccMsg.Text = "";
            lblUpdateErrorMsg.Text = "";

            LoadBankInfoAddRouting();
            LoadDistrictInfoAddRouting();
            txtBranchNameNewRouting.Text = "";
            txtRoutingNoNewRouting.Text = "";
            lblSaveMsg.Text = "";
        }

        private void LoadDistrictInfoByBankCode(string bankCode)
        {
            cbDistrictNameRoutingInfo.Items.Clear();
            DataTable BdAllDistrictlist = mg.GetDistrictListByBankCode(bankCode);
            cbDistrictNameRoutingInfo.Items.Add("---- Select ----");

            for (int rw = 0; rw < BdAllDistrictlist.Rows.Count; rw++)
            {
                cbDistrictNameRoutingInfo.Items.Add(BdAllDistrictlist.Rows[rw][0].ToString());
            }
            cbDistrictNameRoutingInfo.SelectedIndex = 0;
        }

        protected void cbDistrictNameRoutingInfo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbDistrictNameRoutingInfo.SelectedIndex != 0)
            {
                string bankCode = cbBankCode.SelectedItem.ToString().Trim();
                string distName = cbDistrictNameRoutingInfo.SelectedItem.ToString().Trim();
                DataTable Branchlist = mg.GetBranchListByBankCodeAndDistrictName(bankCode, distName);

                cbBranchNameRoutingInfo.Items.Clear();
                cbBranchNameRoutingInfo.Items.Add("---- Select ----");
                for (int rw = 0; rw < Branchlist.Rows.Count; rw++)
                {
                    cbBranchNameRoutingInfo.Items.Add(Branchlist.Rows[rw][0].ToString());
                }
                cbBranchNameRoutingInfo.SelectedIndex = 0;
            }
            else
            {
                cbBranchNameRoutingInfo.Items.Clear();
            }
        }

        protected void btnSearchRouting_Click(object sender, EventArgs e)
        {
            string bankCode = "", branchName = "", districtName = "", whereClause = "";
            //routingEditId = -1;

            if (cbBankNameRoutingInfo.SelectedIndex != 0)
            {
                lblMessage.Text = "";
                bankCode = cbBankCode.SelectedItem.ToString().Trim();
                whereClause = " WHERE [Bank Code]='" + bankCode + "' ";

                if (cbDistrictNameRoutingInfo.SelectedIndex != 0)
                {
                    districtName = cbDistrictNameRoutingInfo.SelectedItem.ToString().Trim().ToUpper();
                    whereClause += " AND upper([District])='" + districtName + "' ";
                }

                if (cbBranchNameRoutingInfo.SelectedIndex > 0)
                {
                    branchName = cbBranchNameRoutingInfo.SelectedItem.ToString().Trim().ToUpper();
                    whereClause += " AND upper([Branch Name])='" + branchName + "' ";
                }

                if (txtRoutingNo.Text.Trim().Length > 0)
                {
                    try
                    {
                        routingNoValue = Int32.Parse(txtRoutingNo.Text.Trim());
                        whereClause += " AND [Routing Number] like '" + txtRoutingNo.Text.Trim() + "%'";
                    }
                    catch (Exception ex)  // wrote branch name
                    {
                        whereClause += " AND UPPER([Branch Name]) like UPPER('" + txtRoutingNo.Text.Trim() + "%')";
                    }
                }

                DataTable dtRoutingInfos = mg.GetRoutingInfosByWhereClause(whereClause);
                LoadRoutingInfoDataIntoGridView(dtRoutingInfos);
            }
            else
            {
                dgridViewRoutingInfos.DataSource = null;
                dgridViewRoutingInfos.DataBind();

                lblMessage.Text = "Error in Bank Selection !!!";
                //MessageBox.Show("Error in Bank Selection !!!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoadRoutingInfoDataIntoGridView(DataTable dtRoutingInfos)
        {
            dgridViewRoutingInfos.DataSource = null;
            dgridViewRoutingInfos.DataSource = dtRoutingInfos;
            dgridViewRoutingInfos.DataBind();

            if (dtRoutingInfos.Rows.Count > 0)
            {
                //dgridViewRoutingInfos.Columns["Sl"].Width = 40;
                //dgridViewRoutingInfos.Columns["BankCode"].Width = 70;
                //dgridViewRoutingInfos.Columns["BankName"].Width = 230;
                //dgridViewRoutingInfos.Columns["BranchName"].Width = 150;
                //dgridViewRoutingInfos.Columns["Routing"].Width = 80;
                
            }
        }

        protected void dgridViewRoutingInfos_SelectedIndexChanged(object sender, EventArgs e)
        {
            //[MTB Sl No] Sl,[Bank Code] BankCode,[Agent Name] BankName,[Branch Name] BranchName,[District],[Routing Number] Routing,[IsActiveForBEFTNAP] IsActive

            txtAutoId.Text = dgridViewRoutingInfos.SelectedRow.Cells[1].Text;
            txtBranchName.Text = dgridViewRoutingInfos.SelectedRow.Cells[4].Text;
            txtRoutingNoEdit.Text = dgridViewRoutingInfos.SelectedRow.Cells[6].Text;

            cbBankNameUpdateRouting.SelectedValue = dgridViewRoutingInfos.SelectedRow.Cells[2].Text + "=" + dgridViewRoutingInfos.SelectedRow.Cells[3].Text;

            cbDistrictNameUpdateRouting.SelectedValue = dgridViewRoutingInfos.SelectedRow.Cells[5].Text;
        }

        protected void txtRoutingNo_TextChanged(object sender, EventArgs e)
        {
            if (txtRoutingNo.Text.Trim().Length > 0)
            {
                string bankCode = "", whereClause = "";

                try
                {
                    routingNoValue = Int32.Parse(txtRoutingNo.Text.Trim());

                    if (cbBankNameRoutingInfo.SelectedIndex != 0)
                    {
                        bankCode = cbBankCode.SelectedItem.ToString().Trim();
                        whereClause = " WHERE [Bank Code]='" + bankCode + "' ";

                        if (cbDistrictNameRoutingInfo.SelectedIndex != 0)
                        {
                            whereClause += " AND upper([District])='" + cbDistrictNameRoutingInfo.SelectedItem.ToString().Trim().ToUpper() + "' ";
                        }
                        if (cbBranchNameRoutingInfo.SelectedIndex > 0)
                        {
                            whereClause += " AND upper([BranchName])='" + cbBranchNameRoutingInfo.SelectedItem.ToString().Trim().ToUpper() + "' ";
                        }
                        if (txtRoutingNo.Text.Trim().Length > 0)
                        {
                            whereClause += " AND [Routing Number] like '" + txtRoutingNo.Text.Trim() + "%'";
                        }

                        DataTable dtRoutingInfos = mg.GetRoutingInfosByWhereClause(whereClause);
                        LoadRoutingInfoDataIntoGridView(dtRoutingInfos);
                    }
                    else  // bank not selected
                    {
                        whereClause = " WHERE [Routing Number] like '" + txtRoutingNo.Text.Trim() + "%'";
                        DataTable dtRoutingInfos = mg.GetRoutingInfosByWhereClause(whereClause);
                        LoadRoutingInfoDataIntoGridView(dtRoutingInfos);
                    }
                }
                catch (Exception ex)  // wrote branch name
                {
                    if (cbBankNameRoutingInfo.SelectedIndex != 0)
                    {
                        bankCode = cbBankCode.SelectedItem.ToString().Trim();
                        whereClause = " WHERE [Bank Code]='" + bankCode + "' ";

                        if (cbDistrictNameRoutingInfo.SelectedIndex != 0)
                        {
                            whereClause += " AND upper([District])='" + cbDistrictNameRoutingInfo.SelectedItem.ToString().Trim().ToUpper() + "' ";
                        }
                        if (cbBranchNameRoutingInfo.SelectedIndex > 0)
                        {
                            whereClause += " AND upper([Branch Name])='" + cbBranchNameRoutingInfo.SelectedItem.ToString().Trim().ToUpper() + "' ";
                        }

                        if (txtRoutingNo.Text.Trim().Length > 0)
                        {
                            whereClause += " AND UPPER([Branch Name]) like UPPER('" + txtRoutingNo.Text.Trim() + "%')";
                        }

                        DataTable dtRoutingInfos = mg.GetRoutingInfosByWhereClause(whereClause);
                        LoadRoutingInfoDataIntoGridView(dtRoutingInfos);
                    }
                    else  // bank not selected
                    {
                        whereClause = " WHERE UPPER([Branch Name]) like UPPER('%" + txtRoutingNo.Text.Trim() + "%')";
                        DataTable dtRoutingInfos = mg.GetRoutingInfosByWhereClause(whereClause);
                        LoadRoutingInfoDataIntoGridView(dtRoutingInfos);
                    }
                }
            }
            else  // display all list based 
            {
                string bankCode = "", whereClause = "";
                if (cbBankNameRoutingInfo.SelectedIndex != 0)
                {
                    bankCode = cbBankCode.SelectedItem.ToString().Trim();
                    whereClause = " WHERE [Bank Code]='" + bankCode + "' ";

                    if (cbDistrictNameRoutingInfo.SelectedIndex != 0)
                    {
                        whereClause += " AND upper([District])='" + cbDistrictNameRoutingInfo.SelectedItem.ToString().Trim().ToUpper() + "' ";
                    }
                    if (cbBranchNameRoutingInfo.SelectedIndex > 0)
                    {
                        whereClause += " AND upper([Branch Name])='" + cbBranchNameRoutingInfo.SelectedItem.ToString().Trim().ToUpper() + "' ";
                    }

                    DataTable dtRoutingInfos = mg.GetRoutingInfosByWhereClause(whereClause);
                    LoadRoutingInfoDataIntoGridView(dtRoutingInfos);
                }
                else
                {
                    dgridViewRoutingInfos.DataSource = null;
                }
            }
        }

        protected void btnUpdateRoutingInfo_Click(object sender, EventArgs e)
        {
            if (txtRoutingNoEdit.Text.Trim().Length != 9)
            {
                lblUpdateErrorMsg.Text = "Routing Number is not in proper length";
            }
            else
            {

                string rtNum = txtRoutingNoEdit.Text.Trim();
                string brName = txtBranchName.Text.Trim();
                int idVal = Convert.ToInt32(txtAutoId.Text);

                string bankCode = cbBankNameUpdateRouting.Text.Split('=')[0];
                string bankName = cbBankNameUpdateRouting.Text.Split('=')[1];
                string dist = cbDistrictNameUpdateRouting.Text;

                bool status = mg.UpdateRoutingInfo(idVal, rtNum, brName, bankCode, bankName, dist);
                if (status)
                {
                    lblUpdateSuccMsg.Text = "Update Successfully";
                }
                
            }
        }

        protected void btnSaveRoutingInfo_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtRoutingNoNewRouting.Text.Trim().Length != 9)
                {
                    lblMessage.Text ="Routing Number is not in proper length";
                }
                else if (txtBranchNameNewRouting.Text.Trim().Length == 0)
                {
                    lblMessage.Text ="Please Enter Branch Name";
                }
                else if (mg.IsRoutingNumberAlreadyExists(txtRoutingNoNewRouting.Text.Trim()))
                {
                    lblMessage.Text ="Routing Number Already Exists";
                }
                else if (cbDistrictNameNewRouting.SelectedIndex == 0)
                {
                    lblMessage.Text = "Please Select District";
                }
                else
                {
                    int lastSlNo = mg.GetLastRecordNumber();
                    if (lastSlNo != 0)
                    {
                        int slNo = lastSlNo + 1;
                        string bankCode = cbBankCodeNewRouting.SelectedItem.ToString();
                        string bankName = cbBankNameNewRouting.SelectedItem.ToString();
                        string brName = txtBranchNameNewRouting.Text.Trim().ToUpper();
                        string districtName = cbDistrictNameNewRouting.SelectedItem.ToString().Trim().ToUpper();
                        string rtNum = txtRoutingNoNewRouting.Text.Trim();

                        bool status = mg.SaveRoutingInfo(slNo, bankCode, bankName, brName, districtName, rtNum);
                        if (status)
                        {
                            lblSaveMsg.Text = "Saved Successfully";
                        }
                    }
                }

            }
            catch (Exception exc)
            {
                //MessageBox.Show("Error:" + exc.ToString());
            }
        }

        protected void cbBankNameNewRouting_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbBankCodeNewRouting.SelectedIndex = cbBankNameNewRouting.SelectedIndex;
        }

        protected void LinkButtonDownloadList_Click(object sender, EventArgs e)
        {
            DataTable dtRoutingInfos = mg.GetRoutingInfosByWhereClause("");
            string headerValue = "attachment;filename=RoutingList.xlsx";
            if (dtRoutingInfos.Rows.Count > 0)
            {
                dtRoutingInfos.TableName = "RoutingList";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dtRoutingInfos);

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
            else
            {                
            }
        }

        
    }
}