using RemittanceOperation.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RemittanceOperation
{
    public partial class UATBEFTNTableData : System.Web.UI.Page
    {
        static Manager mg = new Manager();
        static DataTable dtEFTList = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {


            if (!IsPostBack)
            {
                dtpickerFrom.Text = DateTime.Now.ToString("yyyy-MM-dd");
                dtpickerTo.Text = DateTime.Now.ToString("yyyy-MM-dd");

                LoadActiveExhUserList();
             
            }
        }

        private void LoadActiveExhUserList()
        {
            DataTable dtUsers = mg.GetUATActiveExchangeHouseUserList();
            ddlExhList.Items.Clear();
            ddlExhList.Items.Add("0 - PLEASE SELECT ");

            for (int rows = 0; rows < dtUsers.Rows.Count; rows++)
            {
                ddlExhList.Items.Add(dtUsers.Rows[rows][0] + "");
            }
            ddlExhList.SelectedIndex = 0;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            if (ddlExhList.SelectedIndex != 0)
            {
                int partyId = Convert.ToInt32(ddlExhList.Text.Split('-')[0]);
                string userId = ddlExhList.Text.Split('-')[1].Trim();
                //int reqTypeId = Convert.ToInt32(ddlRequestType.Text.Split('-')[0]);

                //dtEFTList = mg.GetUATDataByPartyIdPaymentMode(partyId, userId, "BEFTN");

                dataGridViewBEFTNTxn.DataSource = null;
                dataGridViewBEFTNTxn.DataSource = dtEFTList;
                dataGridViewBEFTNTxn.DataBind();

                lblMessage.Text = "Total Rows:" + dtEFTList.Rows.Count;
            }
            else
            {
                lblMessage.Text = "Exchange House Selection Error !!!";
            }
        }

        protected void btnUATBEFTNDataDownload_Click(object sender, EventArgs e)
        {

        }
    }
}