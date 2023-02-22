using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
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
    public partial class BEFTNEligibleForIncentive : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable dtBeftnInctEligibleList = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[CSessionName.S_CURRENT_USER_RM] != null)
            {
                //Panel1.Visible = false;
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                dtBeftnInctEligibleList = new DataTable();
                dTPickerFrom.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                dTPickerTo.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            }
        }


        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DateTime dateTime1, dateTime2;
            dateTime1 = DateTime.ParseExact(dTPickerFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            dateTime2 = DateTime.ParseExact(dTPickerTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            string dtValueFrom = dateTime1.ToString("yyyy-MM-dd");
            string dtValueTo = dateTime2.ToString("yyyy-MM-dd");
            string incTyp = ddlIncentiveType.Text;

            dtBeftnInctEligibleList = mg.GetBEFTNIncentiveEligibleList(dtValueFrom, dtValueTo, incTyp);

            dataGridViewEFTEligibleInct.DataSource = null;
            dataGridViewEFTEligibleInct.DataSource = dtBeftnInctEligibleList;
            dataGridViewEFTEligibleInct.DataBind();

            lblRowCount.Text = "Total Rows:" + dtBeftnInctEligibleList.Rows.Count;
        }

        protected void btnDownloadIncentiveList_Click(object sender, EventArgs e)
        {
            DateTime dateTime1 = DateTime.ParseExact(dTPickerFrom.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            DateTime dateTime2 = DateTime.ParseExact(dTPickerTo.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            string dtValueFrom = dateTime1.ToString("yyyy-MM-dd");
            string dtValueTo = dateTime2.ToString("yyyy-MM-dd");

            string fileName = "IncentiveEligibleTxn_" + dtValueFrom + "_to_" + dtValueTo + ".xls";

            if (dtBeftnInctEligibleList.Rows.Count > 0)
            {
                StringWriter tw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = dtBeftnInctEligibleList;
                dgGrid.DataBind();

                foreach (DataGridItem item in dgGrid.Items)
                {
                    for (int j = 0; j < item.Cells.Count; j++)
                    {
                        //    item.Cells[j].Attributes.Add("style", "mso-number-format:\\@");

                        if (j == 7 || j == 8)
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
            else
            {
                lblDownloadMsg.Text = "Nothing to download !!!";
                lblDownloadMsg.ForeColor = Color.Red;
            }
        }
    }
}