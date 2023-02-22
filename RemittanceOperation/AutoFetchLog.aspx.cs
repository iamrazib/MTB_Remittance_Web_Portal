using RemittanceOperation.AppCode;
using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class AutoFetchLog : System.Web.UI.Page
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

            if (!IsPostBack)
            {
                LoadExchList();

                dTPickerFromChk.Text = DateTime.Now.ToString("yyyy-MM-dd");
                dTPickerToChk.Text = DateTime.Now.ToString("yyyy-MM-dd");

                //btnAPIDataSearch_Click(sender, e);
            }
        }

        private void LoadExchList()
        {
            comboBoxAPIExh.Items.Clear();
            comboBoxAPIExh.Items.Add("--ALL--");

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

            DataTable dtfetchLog = new DataTable();

            if (comboBoxAPIExh.SelectedIndex == 0)
            {
                dtfetchLog = mg.GetAPIDataAutoFetchLog(fromDate, toDate, "");
            }
            else
            {
                if(exh.Equals("NBL"))
                {
                    dtfetchLog = mg.GetAPIDataAutoFetchLog(fromDate, toDate, "NBL");
                }
                else if (exh.Equals("ICTC"))
                {
                    dtfetchLog = mg.GetAPIDataAutoFetchLog(fromDate, toDate, "InstantCash");
                }
                else if (exh.Equals("GCC"))
                {
                    dtfetchLog = mg.GetAPIDataAutoFetchLog(fromDate, toDate, "GCCRemit");
                }
                else
                {
                    dtfetchLog = new DataTable();
                }
            }

            dataGridViewFetchLog.DataSource = null;
            dataGridViewFetchLog.DataSource = dtfetchLog;
            dataGridViewFetchLog.DataBind();

            lblFetchLogRowCount.Text = "Rows: " + dtfetchLog.Rows.Count;
            lblFetchLogTime.Text = "Last Update Time: " + DateTime.Now;
        }

        protected void dataGridViewFetchLog_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if(e.Row.Cells[4].Text.ToUpper().Contains("ERROR"))
                {
                    e.Row.BackColor = Color.FromName("yellow");
                }
            }
        }
    }
}