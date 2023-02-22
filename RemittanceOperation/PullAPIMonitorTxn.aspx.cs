using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class PullAPIMonitorTxn : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable aDTbKashReg = new DataTable();
        static DataTable aDTableBEFTN = new DataTable();
        static DataTable aDataTableMTB = new DataTable();
        static DataTable aDataTableCASH = new DataTable();
        static DataTable aDataTableInvalid = new DataTable();

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
                LoadExchList();

                dTPickerFromChk.Text = DateTime.Now.ToString("yyyy-MM-dd");
                dTPickerToChk.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        private void LoadExchList()
        {
            comboBoxAPIExh.Items.Clear();
            comboBoxAPIExh.Items.Add("--Select--");

            comboBoxAPIExh.Items.Add("NBL");
            comboBoxAPIExh.Items.Add("ICTC");
            comboBoxAPIExh.Items.Add("GCC");  

            comboBoxAPIExh.SelectedIndex = 0;
        }

        protected void btnAPIDataSearch_Click(object sender, EventArgs e)
        {
            string exh = comboBoxAPIExh.Text;
            string fromDate = dTPickerFromChk.Text;
            string toDate = dTPickerToChk.Text;

            if (comboBoxAPIExh.SelectedIndex != 0)
            {
                aDTbKashReg = mg.GetPullAPIBkashTxnByExhPayMode(exh, "bkash", fromDate, toDate);
                dataGridViewBkashTxn.DataSource = null;
                dataGridViewBkashTxn.DataSource = aDTbKashReg;
                dataGridViewBkashTxn.DataBind();
                lblBkashTxnCount.Text = "Records: " + aDTbKashReg.Rows.Count;


                aDTableBEFTN = mg.GetPullAPIBkashTxnByExhPayMode(exh, "beftn", fromDate, toDate);
                dataGridViewBEFTNTxn.DataSource = null;
                dataGridViewBEFTNTxn.DataSource = aDTableBEFTN;
                dataGridViewBEFTNTxn.DataBind();
                lblBEFTNTxnCount.Text = "Records: " + aDTableBEFTN.Rows.Count;


                aDataTableMTB = mg.GetPullAPIBkashTxnByExhPayMode(exh, "mtbac", fromDate, toDate);
                dataGridViewMTBTxn.DataSource = null;
                dataGridViewMTBTxn.DataSource = aDataTableMTB;
                dataGridViewMTBTxn.DataBind();
                lblMTBAcTxnCount.Text = "Records: " + aDataTableMTB.Rows.Count;


                aDataTableCASH = mg.GetPullAPIBkashTxnByExhPayMode(exh, "cash", fromDate, toDate);
                dataGridViewCASHTxn.DataSource = null;
                dataGridViewCASHTxn.DataSource = aDataTableCASH;
                dataGridViewCASHTxn.DataBind();
                lblCASHTxnCount.Text = "Records: " + aDataTableCASH.Rows.Count;


                aDataTableInvalid = mg.GetPullAPIBkashTxnByExhPayMode(exh, "invalid", fromDate, toDate);
                dataGridViewINvalidTxn.DataSource = null;
                dataGridViewINvalidTxn.DataSource = aDataTableInvalid;
                dataGridViewINvalidTxn.DataBind();
                lblINVALIDxnCount.Text = "Records: " + aDataTableInvalid.Rows.Count;
            }
                        
        }

        protected void LinkButtonDownloadBKash_Click(object sender, EventArgs e)
        {
            if (aDTbKashReg.Rows.Count < 1)
            {
                //lblMsg.Text = "Nothing to Download !!!";
                //lblMsg.ForeColor = Color.Red;
            }
            else
            {               
                string fileName = comboBoxAPIExh.Text + "_bKash_" + dTPickerFromChk.Text + "_to_" + dTPickerToChk.Text + ".xls";
                DownloadExcelReport(fileName, aDTbKashReg);
                /*
                StringWriter tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = aDTbKashReg;
                dgGrid.DataBind();

                foreach (DataGridItem item in dgGrid.Items)
                {
                    for (int j = 0; j < item.Cells.Count; j++)
                    {
                        //if (j == 8)
                        //{
                        //    item.Cells[j].Attributes.Add("style", "mso-number-format:0\\.00");
                        //}
                        //else
                        //{
                            item.Cells[j].Attributes.Add("style", "mso-number-format:\\@");
                        //}
                    }
                }

                dgGrid.RenderControl(hw);
                Response.ContentType = "application/vnd.ms-excel";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName + "");
                this.EnableViewState = false;
                Response.Write(tw.ToString());
                Response.End();
                */
            }
        }

        private void DownloadExcelReport(string fileName, DataTable dataSource)
        {
            StringWriter tw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(tw);
            DataGrid dgGrid = new DataGrid();
            dgGrid.DataSource = dataSource;
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
        }

        protected void LinkButtonDownloadBEFTN_Click(object sender, EventArgs e)
        {
            if (aDTableBEFTN.Rows.Count < 1)
            {
                //lblMsg.Text = "Nothing to Download !!!";
                //lblMsg.ForeColor = Color.Red;
            }
            else
            {
                string fileName = comboBoxAPIExh.Text + "_BEFTN_" + dTPickerFromChk.Text + "_to_" + dTPickerToChk.Text + ".xls";
                DownloadExcelReport(fileName, aDTableBEFTN);
            }
        }

        protected void LinkButtonDownloadMTBAc_Click(object sender, EventArgs e)
        {
            if (aDataTableMTB.Rows.Count < 1)
            {
                //lblMsg.Text = "Nothing to Download !!!";
                //lblMsg.ForeColor = Color.Red;
            }
            else
            {
                string fileName = comboBoxAPIExh.Text + "_MTB_" + dTPickerFromChk.Text + "_to_" + dTPickerToChk.Text + ".xls";
                DownloadExcelReport(fileName, aDataTableMTB);
            }
        }

        protected void LinkButtonDownloadCASH_Click(object sender, EventArgs e)
        {
            if (aDataTableCASH.Rows.Count < 1)
            {
                //lblMsg.Text = "Nothing to Download !!!";
                //lblMsg.ForeColor = Color.Red;
            }
            else
            {
                string fileName = comboBoxAPIExh.Text + "_CASH_" + dTPickerFromChk.Text + "_to_" + dTPickerToChk.Text + ".xls";
                DownloadExcelReport(fileName, aDataTableCASH);
            }
        }

        protected void LinkButtonDownloadINVALID_Click(object sender, EventArgs e)
        {
            if (aDataTableInvalid.Rows.Count < 1)
            {
                //lblMsg.Text = "Nothing to Download !!!";
                //lblMsg.ForeColor = Color.Red;
            }
            else
            {
                string fileName = comboBoxAPIExh.Text + "_Invalid_" + dTPickerFromChk.Text + "_to_" + dTPickerToChk.Text + ".xls";
                DownloadExcelReport(fileName, aDataTableInvalid);
            }
        }
    }
}